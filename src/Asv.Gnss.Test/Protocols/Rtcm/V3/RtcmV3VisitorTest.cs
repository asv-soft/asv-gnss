using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Asv.IO;
using R3;
using Xunit;

namespace Asv.Gnss;

public class RtcmV3VisitorTest
{
    private static readonly string[] NestedModels =
    [
        "SignalRaw", "MsmHeader", "SatelliteRtkResidual", "RtcmV3NetworkResidual", "EcefPoint",
        "SystemMessage", "GPSSatellite", "GLONASSSatellite", "Satellite", "Signal"
    ];

    public static IEnumerable<object[]> Models() => typeof(RtcmV3MessageBase).Assembly.GetTypes()
        .Where(type => !type.IsAbstract && (typeof(RtcmV3MessageBase).IsAssignableFrom(type) || NestedModels.Contains(type.Name)))
        .OrderBy(type => type.Name).Select(type => new object[] { type });

    [Theory]
    [MemberData(nameof(Models))]
    public void Accept_EveryModel_ExposesAllPayloadMembersAndCachedMetadata(Type type)
    {
        var model = Activator.CreateInstance(type, true)!;
        Populate(model);
        var visitable = Assert.IsAssignableFrom<IVisitable>(model);
        var before = Snapshot(model);
        var first = new Reader();
        var second = new Reader();

        visitable.Accept(first);
        visitable.Accept(second);

        var expected = PayloadMembers(type).Select(member => member.Name).Order().ToArray();
        Assert.NotEmpty(expected);
        Assert.Equal(expected, first.TopFields.Select(field => field.Name).Order());
        Assert.Equal(0, first.Depth);
        Assert.Equal(ExpectedValues(model).Order(), first.Values.Select(ValueKey).Order());
        Assert.Equal(first.Values, second.Values);
        Assert.Equal(first.Fields.Count, second.Fields.Count);
        for (var index = 0; index < first.Fields.Count; index++)
        {
            Assert.Same(first.Fields[index], second.Fields[index]);
            Assert.False(string.IsNullOrEmpty(first.Fields[index].GetDescription()));
        }
        var after = Snapshot(model);
        Assert.Equal(before.Values, after.Values);
        foreach (var (name, reference) in before.References)
            Assert.Same(reference, after.References[name]);
        Assert.Throws<ArgumentNullException>(() => visitable.Accept(null!));
    }

    [Theory]
    [MemberData(nameof(Models))]
    public void Accept_DefaultModelsAndUnsupportedVisitor_DoNotNormalizeStorage(Type type)
    {
        var model = Activator.CreateInstance(type, true)!;
        var visitable = Assert.IsAssignableFrom<IVisitable>(model);
        var before = Snapshot(model);
        var reader = new Reader();
        var unknown = new UnknownVisitor();

        visitable.Accept(reader);
        visitable.Accept(unknown);

        Assert.Equal(PayloadMembers(type).Select(member => member.Name).Order(), unknown.Names.Order());
        var after = Snapshot(model);
        Assert.Equal(before.Values, after.Values);
        foreach (var (name, reference) in before.References)
            Assert.Same(reference, after.References[name]);
    }

    [Fact]
    public void Accept_StationAndEphemeris_UpdatesTypedScalarsAndNestedCoordinates()
    {
        var station = new RtcmV3Message1006 { ReferenceStationID = 7, X = 123.25, Y = -22, Z = 50, Height = 4 };
        var reader = new Reader();
        station.Accept(reader);
        Assert.Equal(123.25, reader.Values.Single(value => value.Name == "X").Value);
        station.Accept(new ScalarEditor());
        Assert.Equal((uint)8, station.ReferenceStationID);
        Assert.Equal(124.25, station.X);
        Assert.Equal(-21, station.Y);
        Assert.Equal(51, station.Z);
        Assert.Equal(5, station.Height);
        var ephemeris = new RtcmV3Message1020();
        ephemeris.Position.X = 100;
        var position = ephemeris.Position;
        ephemeris.Accept(new ScalarEditor());
        Assert.Same(position, ephemeris.Position);
        Assert.Equal(101, ephemeris.Position.X);
    }

    [Fact]
    public void Accept_Msm_VisitsAndEditsSatellitesSignalsMasksAndRawArrays()
    {
        var signal = new Signal { RinexCode = "L1C", PseudoRange = 1234.5, Cnr = 45.25 };
        var message = new RtcmV3Msm7Msg1077
        {
            SatelliteIds = [3, 8], SignalIds = [1], CellMask = [[1], [1]],
            RoughRangeIntegerRaw = [7, 9],
            Satellites = [new Satellite { SatellitePrn = 3, Signals = [signal] }]
        };
        var raw = message.RoughRangeIntegerRaw;
        var masks = message.CellMask;
        var satellites = message.Satellites;
        var reader = new Reader();
        message.Accept(reader);
        Assert.Contains(reader.Values, value => value.Name == "PseudoRange" && Equals(value.Value, 1234.5));
        Assert.Contains(reader.Values, value => value.Name == "RinexCode" && Equals(value.Value, "L1C"));
        message.Accept(new ScalarEditor());
        Assert.Equal(1235.5, signal.PseudoRange);
        Assert.Equal(46.25, signal.Cnr);
        Assert.Equal(new[] { 8, 10 }, raw);
        Assert.Same(raw, message.RoughRangeIntegerRaw);
        Assert.Same(masks, message.CellMask);
        Assert.Same(satellites, message.Satellites);
        message.Accept(new ResizeVisitor("Satellites", 2));
        Assert.Equal(2, message.Satellites.Length);
        Assert.Same(signal, message.Satellites[0].Signals[0]);
        Assert.NotNull(message.Satellites[1]);
    }

    [Fact]
    public void Accept_Lists_CanGrowJaggedPrimitiveAndRecordStorageAndRejectOversize()
    {
        var message = new RtcmV3Msm7Msg1077 { CellMask = [[1, 0]], SatelliteIds = [5] };
        message.Accept(new ResizeVisitor("SatelliteIds", 3));
        Assert.Equal(new byte[] { 5, 0, 0 }, message.SatelliteIds);
        message.Accept(new ResizeVisitor("CellMask", 2));
        Assert.Equal(2, message.CellMask.Length);
        Assert.Equal(new byte[] { 1, 0 }, message.CellMask[0]);
        Assert.Equal(new byte[] { 0, 0 }, message.CellMask[1]);
        message.Accept(new ResizeVisitor("SatelliteIds", 1));
        Assert.Equal(new byte[] { 5 }, message.SatelliteIds);
        var before = message.SatelliteIds;
        Assert.Throws<ArgumentOutOfRangeException>(() => message.Accept(new ResizeVisitor("SatelliteIds", uint.MaxValue)));
        Assert.Same(before, message.SatelliteIds);
    }

    [Fact]
    public void Accept_AnnouncementsAndResiduals_TraversesEveryNestedRecord()
    {
        var announcements = new RtcmV3Message1013
        {
            SystemMessages = [new() { Id = 1005, TransmissionInterval = 1 }, new() { Id = 1077, TransmissionInterval = 2 }]
        };
        announcements.Accept(new ScalarEditor());
        Assert.Equal(new[] { 2d, 3d }, announcements.SystemMessages.Select(item => item.TransmissionInterval));
        var residuals = new RtcmV3Message1030
        {
            SatelliteResiduals = [new() { SatelliteId = 3, SOc = 2 }, new() { SatelliteId = 8, SOc = 4 }]
        };
        var reader = new Reader();
        var records = residuals.SatelliteResiduals;
        residuals.Accept(reader);
        Assert.Equal(new object?[] { 2d, 4d, 2d }, reader.Values.Where(value => value.Name == "SOc").Select(value => value.Value));
        Assert.Same(records, residuals.SatelliteResiduals);
    }

    [Fact]
    public void Accept_OptionalFrequencyAndEnum_ExposeNumericValuesAndSupportEdits()
    {
        var satellite = new RtcmV3Message1019 { WeekRaw = null };
        satellite.Accept(new ScalarEditor());
        Assert.Equal((uint)1, satellite.WeekRaw);
        var message = new RtcmV3Message1230 { FdmaSignalMask = RtcmV3Message1230.FdmaSignalBitmask.CAL1Mask };
        message.Accept(new ScalarEditor());
        Assert.Equal((RtcmV3Message1230.FdmaSignalBitmask)2, message.FdmaSignalMask);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1e-9)]
    [InlineData(-1e-9)]
    public void Accept_GalileoAndBeiDouIdot_UsesRawRateWithoutRecursion(double raw)
    {
        var beidou = new RtcmV3Message1042 { IdotRaw = raw };
        var galileo = new RtcmV3Message1046 { IdotRaw = raw };
        Assert.Equal(raw * Math.PI, beidou.Idot, 1e-21);
        Assert.Equal(raw * Math.PI, galileo.Idot, 1e-21);
        foreach (IVisitable message in new IVisitable[] { beidou, galileo })
        {
            var reader = new Reader();
            message.Accept(reader);
            Assert.Equal(raw * Math.PI, Assert.IsType<double>(reader.Values.Single(value => value.Name == "Idot").Value), 1e-21);
            Assert.Equal(true, reader.Fields.Single(field => field.Name == "Idot").Metadata["readOnly"]);
        }
    }

    [Theory]
    [InlineData("testglo.rtcm3", 429)]
    [InlineData("fw206mrtk_rtcm", 4996)]
    [InlineData("imu_rtcm", 761)]
    [InlineData("GMSD7_20121014.rtcm3", 1143)]
    public void Accept_RecordedMessages_PreservesSerializedBytesAndNestedStorage(string resource, int expectedCount)
    {
        var data = File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "Resources", resource));
        var factory = Protocol.Create(builder => builder.Protocols.RegisterRtcmV3Protocol());
        using var parser = factory.CreateParser(RtcmV3Protocol.Info.Id);
        var messages = new List<RtcmV3MessageBase>();
        using var subscription = parser.OnMessage.Subscribe(packet => messages.Add((RtcmV3MessageBase)packet));
        foreach (var value in data)
            parser.Push(value);
        Assert.Equal(expectedCount, messages.Count);
        foreach (var message in messages)
        {
            var bytes = Serialize(message);
            var before = Snapshot(message);
            var visitor = new Reader();
            message.Accept(visitor);
            Assert.NotEmpty(visitor.Values);
            Assert.Equal(bytes, Serialize(message));
            var after = Snapshot(message);
            Assert.Equal(before.Values, after.Values);
            foreach (var (name, reference) in before.References)
                Assert.Same(reference, after.References[name]);
        }
    }

    private static byte[] Serialize(RtcmV3MessageBase message)
    {
        var bytes = new byte[message.GetByteSize()];
        Span<byte> output = bytes;
        message.Serialize(ref output);
        Assert.True(output.IsEmpty);
        return bytes;
    }

    private static IEnumerable<MemberInfo> PayloadMembers(Type type)
    {
        var isMessage = typeof(RtcmV3MessageBase).IsAssignableFrom(type);
        return type.GetMembers(BindingFlags.Public | BindingFlags.Instance)
            .Where(member => member is PropertyInfo { GetMethod.IsPublic: true } property && property.GetIndexParameters().Length == 0
                || member is FieldInfo { IsStatic: false })
            .Where(member => !isMessage || member.Name is not ("Id" or "MessageId" or "Name" or "Protocol" or "Tags"));
    }

    private static Type MemberType(MemberInfo member) => member is PropertyInfo property ? property.PropertyType : ((FieldInfo)member).FieldType;
    private static object? Get(MemberInfo member, object value) => member is PropertyInfo property ? property.GetValue(value) : ((FieldInfo)member).GetValue(value);
    private static void Set(MemberInfo member, object value, object? item)
    {
        if (member is PropertyInfo property) property.SetValue(value, item);
        else ((FieldInfo)member).SetValue(value, item);
    }

    private static object? Sample(Type type, int seed)
    {
        if (Nullable.GetUnderlyingType(type) is { } underlying) return Sample(underlying, seed);
        if (type == typeof(string)) return "sample-" + seed;
        if (type == typeof(DateTime)) return new DateTime(2026, 1, 2, 3, 4, 5, DateTimeKind.Utc).AddSeconds(seed);
        if (type == typeof(bool)) return seed % 2 == 0;
        if (type.IsEnum) return Enum.ToObject(type, 1);
        if (type.IsPrimitive) return Convert.ChangeType(seed, type, CultureInfo.InvariantCulture);
        if (type.IsArray)
        {
            var items = Array.CreateInstance(type.GetElementType()!, 2);
            for (var index = 0; index < 2; index++) items.SetValue(Sample(type.GetElementType()!, seed + index), index);
            return items;
        }
        var result = Activator.CreateInstance(type, true)!;
        Populate(result);
        return result;
    }

    private static void Populate(object value)
    {
        foreach (var member in PayloadMembers(value.GetType()))
        {
            var type = MemberType(member);
            if (member is FieldInfo || member is PropertyInfo { SetMethod: not null }) Set(member, value, Sample(type, 1 + member.Name.Sum(character => (int)character) % 90));
            else if (!type.IsValueType && type != typeof(string) && Get(member, value) is { } nested) Populate(nested);
        }
    }

    private static string ValueKey((string Name, object? Value) value)
    {
        var text = value.Value is IFormattable formatted
            ? formatted.ToString(null, CultureInfo.InvariantCulture) : value.Value?.ToString() ?? "<null>";
        return $"{value.Name}:{value.Value?.GetType().Name}:{text}";
    }

    private static IEnumerable<string> ExpectedValues(object model)
    {
        IEnumerable<string> Read(object? value, Type type, string name)
        {
            if (value == null)
            {
                if (type == typeof(string) || Nullable.GetUnderlyingType(type) != null)
                    yield return ValueKey((name, null));
                yield break;
            }
            if (type.IsEnum)
            {
                yield return ValueKey((name, Convert.ToInt32(value, CultureInfo.InvariantCulture)));
                yield break;
            }
            if (value is string or DateTime || type.IsPrimitive || Nullable.GetUnderlyingType(type) != null)
            {
                yield return ValueKey((name, value));
                yield break;
            }
            if (value is Array array)
            {
                foreach (var element in array)
                    foreach (var item in Read(element, type.GetElementType()!, name))
                        yield return item;
                yield break;
            }
            foreach (var member in PayloadMembers(type))
                foreach (var item in Read(Get(member, value), MemberType(member), member.Name))
                    yield return item;
        }
        return Read(model, model.GetType(), string.Empty);
    }

    private static (Dictionary<string, object?> Values, Dictionary<string, object> References) Snapshot(object value)
    {
        var values = new Dictionary<string, object?>();
        var references = new Dictionary<string, object>();
        void Read(object? item, string path)
        {
            if (item is null || item is string || item.GetType().IsPrimitive || item.GetType().IsEnum || item is DateTime)
            {
                values[path] = item;
                return;
            }
            if (!item.GetType().IsValueType) references[path] = item;
            if (item is Array array)
            {
                values[path + ".Length"] = array.Length;
                for (var index = 0; index < array.Length; index++) Read(array.GetValue(index), path + $"[{index}]");
                return;
            }
            foreach (var member in PayloadMembers(item.GetType())) Read(Get(member, item), path + "." + member.Name);
        }
        Read(value, "$root");
        return (values, references);
    }

    private class Reader : DoubleType.IVisitor, BoolType.IVisitor, UInt8Type.IVisitor, UInt16Type.IVisitor,
        UInt32Type.IVisitor, Int32Type.IVisitor, UInt32OptionalType.IVisitor, StringOptionalType.IVisitor,
        DateTimeType.IVisitor, ListType.IVisitor, StructType.IVisitor
    {
        public int Depth { get; private set; }
        public List<Field> Fields { get; } = [];
        public List<Field> TopFields { get; } = [];
        public List<(string Name, object? Value)> Values { get; } = [];
        private void Seen(Field field)
        {
            Fields.Add(field);
            if (Depth == 0) TopFields.Add(field);
        }
        private void Scalar(Field field, object? value) { Seen(field); Values.Add((field.Name, value)); }
        public virtual void Visit(Field field, DoubleType type, ref double value) => Scalar(field, value);
        public virtual void Visit(Field field, BoolType type, ref bool value) => Scalar(field, value);
        public virtual void Visit(Field field, UInt8Type type, ref byte value) => Scalar(field, value);
        public virtual void Visit(Field field, UInt16Type type, ref ushort value) => Scalar(field, value);
        public virtual void Visit(Field field, UInt32Type type, ref uint value) => Scalar(field, value);
        public virtual void Visit(Field field, Int32Type type, ref int value) => Scalar(field, value);
        public virtual void Visit(Field field, UInt32OptionalType type, ref uint? value) => Scalar(field, value);
        public virtual void Visit(Field field, StringOptionalType type, ref string? value) => Scalar(field, value);
        public virtual void Visit(Field field, DateTimeType type, ref DateTime value) => Scalar(field, value);
        public virtual void BeginList(Field field, ListType type, ref uint size) { Seen(field); Depth++; }
        public void EndList() => Depth--;
        public void BeginStruct(Field field, StructType type) { Seen(field); Depth++; }
        public void EndStruct() => Depth--;
        public void VisitUnknown(Field field, IFieldType type) => throw new InvalidOperationException($"Unsupported {field.Name}: {type.Name}");
    }

    private sealed class ScalarEditor : Reader
    {
        public override void Visit(Field field, DoubleType type, ref double value) { value++; base.Visit(field, type, ref value); }
        public override void Visit(Field field, UInt32Type type, ref uint value) { value++; base.Visit(field, type, ref value); }
        public override void Visit(Field field, Int32Type type, ref int value) { value++; base.Visit(field, type, ref value); }
        public override void Visit(Field field, UInt32OptionalType type, ref uint? value) { value = (value ?? 0) + 1; base.Visit(field, type, ref value); }
    }

    private sealed class ResizeVisitor(string name, uint size) : Reader
    {
        public override void BeginList(Field field, ListType type, ref uint count)
        {
            if (field.Name == name) count = size;
            base.BeginList(field, type, ref count);
        }
    }

    private sealed class UnknownVisitor : IVisitor
    {
        public List<string> Names { get; } = [];
        public void VisitUnknown(Field field, IFieldType type) => Names.Add(field.Name);
    }
}
