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

public class SbfComNavVisitorTest
{
    private static readonly string[] NestedModels = ["MeasEpochChannelType1", "QualityIndicator", "SbfReceiverStatusAGCState", "ComNavPortCfg", "LogMessage", "Observation"];
    public static IEnumerable<object[]> Models() => typeof(SbfMessageBase).Assembly.GetTypes()
        .Where(type => !type.IsAbstract && (typeof(SbfMessageBase).IsAssignableFrom(type)
            || typeof(ComNavBinaryMessageBase).IsAssignableFrom(type)
            || typeof(ComNavStringMessageBase).IsAssignableFrom(type) || NestedModels.Contains(type.Name)))
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
    public void Accept_ComNavAndSbf_CollectionsAndNumericEditsPreserveStorage()
    {
        var channels = new[] { new MeasEpochChannelType1 { PR = 123.5, SVID = 7 }, new MeasEpochChannelType1 { PR = 567.5, SVID = 9 } };
        var sbf = new SbfPacketMeasEpochRev1 { SubBlocks = channels, CumClkJumps = 2 };
        sbf.Accept(new ScalarEditor());
        Assert.Same(channels, sbf.SubBlocks);
        Assert.Equal(new[] { 124.5, 568.5 }, channels.Select(channel => channel.PR));
        Assert.Equal(3, sbf.CumClkJumps);
        var command = new ComNavAsciiLogCommand { Period = null, Trigger = null };
        command.Accept(new ScalarEditor());
        Assert.Equal((uint)1, command.Period);
        var settings = new ComNavAsciiSetCommand();
        settings.Params.Add("ON");
        var parameters = settings.Params;
        settings.Accept(new ResizeVisitor("Params", 2));
        Assert.Same(parameters, settings.Params);
        Assert.Equal(new[] { "ON", string.Empty }, settings.Params);
        var observation = new ComNavBinaryObservationInfo { Observations = [new Observation { Prn = 4, PseudoRange = 500 }] };
        observation.Accept(new ScalarEditor());
        Assert.Equal(5, observation.Observations[0].Prn);
        Assert.Equal(501, observation.Observations[0].PseudoRange);
        observation.Accept(new ResizeVisitor("Observations", 2));
        Assert.Equal(2, observation.Observations.Length);
        Assert.NotNull(observation.Observations[1]);
        var original = observation.Observations;
        Assert.Throws<ArgumentOutOfRangeException>(() => observation.Accept(new ResizeVisitor("Observations", uint.MaxValue)));
        Assert.Same(original, observation.Observations);
        var port = new ComNavPortCfg();
        port.OtherParams[0] = 123;
        var other = port.OtherParams;
        port.Accept(new ScalarEditor());
        Assert.Same(other, port.OtherParams);
        Assert.Equal((uint)124, other[0]);
        Assert.Throws<InvalidOperationException>(() => port.Accept(new ResizeVisitor("OtherParams", 10)));
    }

    [Fact]
    public void Accept_DecodedReferencePackets_ReportsValuesWithoutChangingDecodedStorage()
    {
        var bytes = File.ReadLines(Path.Combine(AppContext.BaseDirectory, "Resources", "ComNav", "novatel-bestposb.hex"))
            .Where(line => !line.StartsWith('#') && !string.IsNullOrWhiteSpace(line))
            .SelectMany(line => line.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            .Select(value => byte.Parse(value, NumberStyles.HexNumber, CultureInfo.InvariantCulture)).ToArray();
        ReadOnlySpan<byte> input = bytes;
        var best = new ComNavBinaryBestPosPacket();
        best.Deserialize(ref input);
        var before = Snapshot(best);
        var reader = new Reader();
        best.Accept(reader);
        Assert.Contains(reader.Values, item => item.Name == "Latitude" && Equals(item.Value, 32.81519645735));
        Assert.Contains(reader.Values, item => item.Name == "ReceiverSwVersion" && Equals(item.Value, (ushort)9603));
        Assert.Equal(before.Values, Snapshot(best).Values);
        byte[] qualityPacket = [0x24, 0x40, 0x01, 0x51, 0xF2, 0x0F, 0x1C, 0x00,
            0xB8, 0xDD, 0x53, 0x16, 0xB3, 0x08, 0x05, 0x00,
            0x0B, 0x0A, 0x01, 0x02, 0x15, 0x0A, 0x1F, 0x07,
            0x00, 0x02, 0x00, 0x00];
        input = qualityPacket;
        var quality = new SbfPacketQualityInd();
        quality.Deserialize(ref input);
        var indicators = quality.Indicators;
        reader = new Reader();
        quality.Accept(reader);
        Assert.Same(indicators, quality.Indicators);
        Assert.Equal(new object?[] { (byte)10, (byte)2, (byte)10, (byte)7, (byte)2 },
            reader.Values.Where(item => item.Name == "Value").Select(item => item.Value));
        var command = new ComNavFixCommand { FixType = ComNavFixType.Position, Lat = 55.5, Lon = 37.5, Alt = 100 };
        var original = Serialize(command);
        command.Accept(new Reader());
        Assert.Equal(original, Serialize(command));
    }

    [Fact]
    public void Accept_RawNavigation_TraversesExistingGpsAndGlonassModels()
    {
        var gps = new GpsSubframe1 { WeekNumber = 1023, Af0 = 0.125 };
        var glo = new GlonassWord1 { P1 = 2, VelocityX = -123.5 };
        var messages = new IVisitable[]
        {
            new SbfPacketGpsRawCa { GpsSubFrame = gps },
            new ComNavBinaryRawGpsSubFramePacket { GpsSubFrame = gps },
            new SbfPacketGloRawCa { GlonassWord = glo },
            new ComNavBinaryRawGloEphemPacket { GlonassWords = [glo] }
        };
        foreach (var message in messages)
        {
            var reader = new Reader();
            message.Accept(reader);
            if (message is SbfPacketGpsRawCa or ComNavBinaryRawGpsSubFramePacket)
                Assert.Contains(reader.Values, item => item.Name == "WeekNumber" && Equals(item.Value, 1023));
            else
                Assert.Contains(reader.Values, item => item.Name == "VelocityX" && Equals(item.Value, -123.5));
            Assert.Equal(0, reader.Depth);
        }
    }

    private static byte[] Serialize(ComNavAsciiCommandBase message)
    {
        var data = new byte[message.GetByteSize()];
        Span<byte> output = data;
        message.Serialize(ref output);
        Assert.True(output.IsEmpty);
        return data;
    }

    private static IEnumerable<MemberInfo> PayloadMembers(Type type)
    {
        var isMessage = typeof(IProtocolMessage).IsAssignableFrom(type);
        return type.GetMembers(BindingFlags.Public | BindingFlags.Instance)
            .Where(member => member is PropertyInfo { GetMethod.IsPublic: true } property && property.GetIndexParameters().Length == 0
                || member is FieldInfo { IsStatic: false })
            .Where(member => !isMessage || member.Name is not ("MessageId" or "Name" or "Protocol" or "Tags" or "ProtocolId" or "MessageType" or "MessageRevision") && (member.Name != "Id" || type == typeof(ComNavDgpsTxIdCommand)));
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
        if (type.IsAbstract) return null;
        if (type == typeof(TimeSpan)) return TimeSpan.FromSeconds(seed);
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
            else if (Get(member, value) is Array array)
            {
                for (var index = 0; index < array.Length; index++) array.SetValue(Sample(type.GetElementType()!, index + 3), index);
            }
            else if (Get(member, value) is IList<string> list) { list.Add("alpha"); list.Add("bravo"); }
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
            type = Nullable.GetUnderlyingType(type) ?? type;
            if (type.IsEnum)
            {
                yield return ValueKey((name, Convert.ChangeType(value, Enum.GetUnderlyingType(type), CultureInfo.InvariantCulture)));
                yield break;
            }
            if (value is string or DateTime or TimeSpan || type.IsPrimitive || Nullable.GetUnderlyingType(type) != null)
            {
                yield return ValueKey((name, value));
                yield break;
            }
            if (value is IList<string> list)
            {
                foreach (var item in list) yield return ValueKey((name, item));
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
            if (item is null || item is string || item.GetType().IsPrimitive || item.GetType().IsEnum || item is DateTime or TimeSpan)
            {
                values[path] = item;
                return;
            }
            if (!item.GetType().IsValueType) references[path] = item;
            if (item is IList<string> list)
            {
                values[path + ".Count"] = list.Count;
                for (var index = 0; index < list.Count; index++) Read(list[index], path + $"[{index}]");
                return;
            }
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
        DateTimeType.IVisitor, TimeSpanType.IVisitor, FloatType.IVisitor, Int8Type.IVisitor, Int32OptionalType.IVisitor, ListType.IVisitor, StructType.IVisitor
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
        public virtual void Visit(Field field, TimeSpanType type, ref TimeSpan value) => Scalar(field, value);
        public virtual void Visit(Field field, FloatType type, ref float value) => Scalar(field, value);
        public virtual void Visit(Field field, Int8Type type, ref sbyte value) => Scalar(field, value);
        public virtual void Visit(Field field, Int32OptionalType type, ref int? value) => Scalar(field, value);
        public virtual void BeginList(Field field, ListType type, ref uint size) { Seen(field); Depth++; }
        public void EndList() => Depth--;
        public void BeginStruct(Field field, StructType type) { Seen(field); Depth++; }
        public void EndStruct() => Depth--;
        public void VisitUnknown(Field field, IFieldType type) => Seen(field);
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
