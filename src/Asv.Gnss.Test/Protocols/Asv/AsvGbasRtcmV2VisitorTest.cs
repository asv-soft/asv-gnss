using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Asv.IO;
using Xunit;

namespace Asv.Gnss;

public class AsvGbasRtcmV2VisitorTest
{
    private static readonly string[] ModelNames =
    [
        "AsvMessageGpsRawCa",
        "AsvMessageGpsObservations",
        "AsvGpsObservation",
        "AsvMessageGbasVdbSendV2",
        "AsvMessageGbasMonDevSendV2",
        "AsvMessageHeartBeat",
        "AsvMessageGbasVdbSend",
        "AsvMessagePvtGeo",
        "AsvMessageGloRawCa",
        "AsvMessageGloObservations",
        "AsvGloObservation",
        "GbasMessage",
        "FasDataSet",
        "MessageBlockType4",
        "MessageBlockType3",
        "MessageBlockType2",
        "AdditionalDataBlock3",
        "AdditionalDataBlock2",
        "AdditionalDataBlock1",
        "AdditionalDataBlock4",
        "ImpactedSource",
        "MessageBlockType5",
        "ObstructedApproaches",
        "MeasurementBlockMsg101",
        "MessageBlockType101",
        "MeasurementBlockMsg1",
        "MessageBlockType1",
        "MeasurementBlockD",
        "MessageBlockType11",
        "RtcmV2Message14",
        "DObservationItem",
        "RtcmV2Message1",
        "RtcmV2Message21",
        "RtcmV2Message15",
        "IonosphericDelayItem",
        "RtcmV2Message31",
        "RtcmV2Message9",
        "RtcmV2Message17"
    ];

    public static IEnumerable<object[]> Models() => typeof(AsvMessageBase).Assembly.GetTypes()
        .Where(type => !type.IsAbstract && ModelNames.Contains(type.Name))
        .OrderBy(type => type.Name).Select(type => new object[] { type });

    [Theory]
    [MemberData(nameof(Models))]
    public void Accept_EveryModel_ExposesAllPayloadMembersAndCachedMetadata(Type type)
    {
        var model = CreateModel(type);
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
        var model = CreateModel(type);
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
    public void Accept_AsvObservationsAndHeader_VisitsAndEditsNumericFieldsWithoutReplacingStorage()
    {
        var observation = new AsvGpsObservation { Prn = 3, L1PseudoRange = 1234.5, L1CarrierPhase = -74.25 };
        var message = new AsvMessageGpsObservations { Sequence = 17, SenderId = 9, TargetId = 2, Observations = [observation] };
        var storage = message.Observations;
        var reader = new Reader();
        message.Accept(reader);
        Assert.Contains(reader.Values, item => item.Name == "Sequence" && Equals(item.Value, (ushort)17));
        Assert.Contains(reader.Values, item => item.Name == "L1PseudoRange" && Equals(item.Value, 1234.5));
        message.Accept(new ScalarEditor());
        Assert.Equal(4, observation.Prn);
        Assert.Equal(1235.5, observation.L1PseudoRange);
        Assert.Equal(-73.25, observation.L1CarrierPhase);
        Assert.Same(storage, message.Observations);
        Assert.Same(observation, message.Observations[0]);
    }

    [Fact]
    public void Accept_GbasPolymorphicLists_TraversesEveryConcreteAdditionalBlockAndMeasurements()
    {
        var block1 = new AdditionalDataBlock1 { DMax = 1200 };
        var block2 = new AdditionalDataBlock2 { ChannelNumber = 300, DeltaLatitude = -4.5 };
        var block3 = new AdditionalDataBlock3 { Y_EIG = 2.25 };
        var block4 = new AdditionalDataBlock4();
        block4.SlotGroupDefinition.Add((SlotIdEnum)3);
        var message = new MessageBlockType2 { GbasId = "ABCD", Latitude = 42.5 };
        message.AdditionalData.AddRange([block1, block2, block3, block4]);
        var reader = new Reader();
        message.Accept(reader);
        Assert.Contains(reader.Values, item => item.Name == "DMax" && Equals(item.Value, (uint)1200));
        Assert.Contains(reader.Values, item => item.Name == "ChannelNumber" && Equals(item.Value, (ushort)300));
        Assert.Contains(reader.Values, item => item.Name == "Y_EIG" && Equals(item.Value, 2.25));
        Assert.Contains(reader.Values, item => item.Name == "SlotGroupDefinition" && Equals(item.Value, (byte)3));
        message.Accept(new ScalarEditor());
        Assert.Equal((uint)1201, block1.DMax);
        Assert.Equal(-3.5, block2.DeltaLatitude);
        Assert.Equal(3.25, block3.Y_EIG);
        Assert.Same(block1, message.AdditionalData[0]);
    }

    [Fact]
    public void Accept_GbasOptionalScalarsAndNestedMeasurements_PreservesNullsAndSupportsEdits()
    {
        var measurement = new MeasurementBlockMsg101 { RangingSourceId = 5, Prc = -21.5 };
        var message = new MessageBlockType101 { SourceAvailabilityDuration = null };
        message.Measurements.Add(measurement);
        var reader = new Reader();
        message.Accept(reader);
        Assert.Null(measurement.B);
        Assert.Null(message.SourceAvailabilityDuration);
        Assert.Contains(reader.Values, item => item.Name == "B" && item.Value is null);
        message.Accept(new OptionalEditor());
        Assert.Equal(12.5, measurement.B);
        Assert.Equal((ushort)42, message.SourceAvailabilityDuration);
    }

    [Fact]
    public void Accept_RtcmV2ObservationsAndDelays_UpdatesNestedFieldsAndKeepsArrayIdentity()
    {
        var observation = new DObservationItem(NavigationSystemEnum.SYS_GPS) { Prn = 4, Prc = 14.25, Rrc = -2.5 };
        var message = new RtcmV2Message9 { ReferenceStationId = 23, ObservationItems = [observation] };
        var storage = message.ObservationItems;
        message.Accept(new ScalarEditor());
        Assert.Equal(15.25, observation.Prc);
        Assert.Equal(-1.5, observation.Rrc);
        Assert.Same(storage, message.ObservationItems);
        var delay = new IonosphericDelayItem { Prn = 7, IonosphericDelay = 2.5, IonoRateOfChange = -0.25 };
        new RtcmV2Message15 { Delays = [delay] }.Accept(new ScalarEditor());
        Assert.Equal(3.5, delay.IonosphericDelay);
        Assert.Equal(0.75, delay.IonoRateOfChange);
    }

    [Fact]
    public void Accept_Collections_ResizeArraysAndListsAndRejectUnrepresentableCounts()
    {
        var message = new AsvMessageGpsObservations { Observations = [new AsvGpsObservation { Prn = 8 }] };
        var first = message.Observations[0];
        message.Accept(new ResizeVisitor("Observations", 2));
        Assert.Equal(2, message.Observations.Length);
        Assert.Same(first, message.Observations[0]);
        Assert.NotNull(message.Observations[1]);
        var block = new MessageBlockType3();
        block.Filter.Add(8);
        var list = block.Filter;
        block.Accept(new ResizeVisitor("Filter", 3));
        Assert.Equal(new byte[] { 8, 0, 0 }, block.Filter);
        Assert.Same(list, block.Filter);
        block.Accept(new ResizeVisitor("Filter", 1));
        Assert.Equal(new byte[] { 8 }, block.Filter);
        Assert.Throws<ArgumentOutOfRangeException>(() => block.Accept(new ResizeVisitor("Filter", uint.MaxValue)));
        Assert.Equal(new byte[] { 8 }, block.Filter);
    }

    [Theory]
    [InlineData((byte)1)]
    [InlineData((byte)2)]
    [InlineData((byte)3)]
    [InlineData((byte)4)]
    [InlineData((byte)5)]
    [InlineData((byte)11)]
    [InlineData((byte)101)]
    public void Accept_GbasFrozenFixtures_DoesNotChangeSerializedBytesOrNestedStorage(byte type)
    {
        var expected = Convert.FromHexString(GbasLegacyFixtures.Block(type));
        var message = GbasMessageBlockFactory.CreateGbasMessageBlock(type);
        var offset = 0;
        message.Deserialize(expected, ref offset);
        var before = Snapshot(message);
        message.Accept(new Reader());
        Assert.Equal(expected, GbasTestHelper.Serialize(message));
        var after = Snapshot(message);
        Assert.Equal(before.Values, after.Values);
        foreach (var (path, reference) in before.References) Assert.Same(reference, after.References[path]);
    }

    [Fact]
    public void Accept_AsvAllMessages_InspectionPreservesEncodedBytes()
    {
        AsvMessageBase[] messages =
        [
            new AsvMessageHeartBeat(), new AsvMessageGbasVdbSend(), new AsvMessageGbasVdbSendV2(),
            new AsvMessageGbasMonDevSendV2(), new AsvMessageGpsObservations(), new AsvMessageGloObservations(),
            new AsvMessageGpsRawCa(), new AsvMessageGloRawCa(), new AsvMessagePvtGeo()
        ];
        foreach (var message in messages)
        {
            message.Randomize(new Random(163));
            var expected = Serialize(message);
            message.Accept(new Reader());
            Assert.Equal(expected, Serialize(message));
        }
    }

    [Fact]
    public void Accept_RtcmV2DecodedCorrections_PreservesWireSentinelsAndFillBits()
    {
        byte[] expected = [0x66, 0x04, 0, 0, 0, 0x10, 1, 0, 1, 2, 3, 0xA5];
        ReadOnlySpan<byte> buffer = expected;
        var message = new RtcmV2Message1();
        message.Deserialize(ref buffer);
        var storage = message.ObservationItems;
        message.Accept(new Reader());
        Assert.Equal(expected, Serialize(message));
        Assert.Same(storage, message.ObservationItems);
    }

    private static byte[] Serialize(IProtocolMessage message)
    {
        var bytes = new byte[message.GetByteSize()];
        var output = bytes.AsSpan();
        message.Serialize(ref output);
        Assert.True(output.IsEmpty);
        return bytes;
    }

    [Fact]
    public void Accept_GbasFixedIntegrityArray_UpdatesFourValuesWithoutReplacingArray()
    {
        var measurement = new MeasurementBlockMsg1(1.25, -2.5, double.NaN, 4.75);
        var storage = measurement.B;
        var reader = new Reader();
        measurement.Accept(reader);
        Assert.Equal(4, reader.Values.Count(item => item.Name == "B"));
        measurement.Accept(new ScalarEditor());
        Assert.Same(storage, measurement.B);
        Assert.Equal(2.25, storage[0]);
        Assert.Equal(-1.5, storage[1]);
        Assert.True(double.IsNaN(storage[2]));
        Assert.Equal(5.75, storage[3]);
    }

    private sealed class OptionalEditor : Reader
    {
        public override void Visit(Field field, DoubleOptionalType type, ref double? value) => value = 12.5;
        public override void Visit(Field field, UInt16OptionalType type, ref ushort? value) => value = 42;
    }

    private static IEnumerable<MemberInfo> PayloadMembers(Type type)
    {
        var isMessage = typeof(IProtocolMessage).IsAssignableFrom(type);
        return type.GetMembers(BindingFlags.Public | BindingFlags.Instance)
            .Where(member => member is PropertyInfo { GetMethod.IsPublic: true } property && property.GetIndexParameters().Length == 0
                || member is FieldInfo { IsStatic: false })
            .Where(member => !isMessage || member.Name is not ("Id" or "MessageId" or "MessageTypeId" or "Name" or "Protocol" or "ProtocolId" or "Tags"));
    }

    private static Type MemberType(MemberInfo member) => member is PropertyInfo property ? property.PropertyType : ((FieldInfo)member).FieldType;
    private static object? Get(MemberInfo member, object value) => member is PropertyInfo property ? property.GetValue(value) : ((FieldInfo)member).GetValue(value);
    private static void Set(MemberInfo member, object value, object? item)
    {
        if (member is PropertyInfo property) property.SetValue(value, item);
        else ((FieldInfo)member).SetValue(value, item);
    }

    private static object CreateModel(Type type) => type == typeof(DObservationItem)
        ? new DObservationItem(NavigationSystemEnum.SYS_GPS)
        : type == typeof(MeasurementBlockMsg1) ? new MeasurementBlockMsg1() : Activator.CreateInstance(type, true)!;

    private static object? Sample(Type type, int seed)
    {
        if (Nullable.GetUnderlyingType(type) is { } underlying) return Sample(underlying, seed);
        if (type == typeof(string)) return "sample-" + seed;
        if (type == typeof(DateTime)) return new DateTime(2026, 1, 2, 3, 4, 5, DateTimeKind.Utc).AddSeconds(seed);
        if (type == typeof(bool)) return seed % 2 == 0;
        if (type.IsEnum) return Enum.ToObject(type, 1);
        if (type.IsPrimitive) return Convert.ChangeType(seed, type, CultureInfo.InvariantCulture);
        if (type.IsAbstract) return null;
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
        {
            var list = (IList)Activator.CreateInstance(type)!;
            var elementType = type.GetGenericArguments()[0];
            if (!elementType.IsAbstract)
                for (var i = 0; i < 2; i++) list.Add(Sample(elementType, seed + i));
            return list;
        }
        if (type.IsArray)
        {
            var items = Array.CreateInstance(type.GetElementType()!, 2);
            for (var index = 0; index < 2; index++) items.SetValue(Sample(type.GetElementType()!, seed + index), index);
            return items;
        }
        var result = CreateModel(type);
        Populate(result);
        return result;
    }

    private static void Populate(object value)
    {
        foreach (var member in PayloadMembers(value.GetType()))
        {
            var type = MemberType(member);
            if (member is FieldInfo || member is PropertyInfo { SetMethod: not null }) Set(member, value, Sample(type, 1 + member.Name.Sum(character => (int)character) % 90));
            else if (Get(member, value) is IList list)
            {
                var elementType = type.IsArray ? type.GetElementType()! : type.GetGenericArguments()[0];
                if (type.IsArray)
                {
                    for (var i = 0; i < list.Count; i++) list[i] = Sample(elementType, i + 13);
                }
                else if (!elementType.IsAbstract)
                    for (var i = 0; i < 2; i++) list.Add(Sample(elementType, i + 13));
            }
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
                yield return ValueKey((name, Convert.ChangeType(value, Enum.GetUnderlyingType(type), CultureInfo.InvariantCulture)));
                yield break;
            }
            if (value is string or DateTime || type.IsPrimitive || Nullable.GetUnderlyingType(type) != null)
            {
                yield return ValueKey((name, value));
                yield break;
            }
            if (value is IList array)
            {
                var elementType = type.IsArray ? type.GetElementType()! : type.GetGenericArguments()[0];
                foreach (var element in array)
                    foreach (var item in Read(element, elementType, name))
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
            if (item is IList array)
            {
                values[path + ".Length"] = array.Count;
                for (var index = 0; index < array.Count; index++) Read(array[index], path + $"[{index}]");
                return;
            }
            foreach (var member in PayloadMembers(item.GetType())) Read(Get(member, item), path + "." + member.Name);
        }
        Read(value, "$root");
        return (values, references);
    }

    private class Reader : DoubleType.IVisitor, BoolType.IVisitor, UInt8Type.IVisitor, UInt16Type.IVisitor,
        UInt32Type.IVisitor, Int32Type.IVisitor, UInt32OptionalType.IVisitor, StringOptionalType.IVisitor,
        Int16Type.IVisitor, Int64Type.IVisitor, UInt64Type.IVisitor, DoubleOptionalType.IVisitor, UInt16OptionalType.IVisitor,
        DateTimeType.IVisitor, ListType.IVisitor, StructType.IVisitor, ArrayType.IVisitor, OptionalStructType.IVisitor
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
        public virtual void Visit(Field field, Int16Type type, ref short value) => Scalar(field, value);
        public virtual void Visit(Field field, Int64Type type, ref long value) => Scalar(field, value);
        public virtual void Visit(Field field, UInt64Type type, ref ulong value) => Scalar(field, value);
        public virtual void Visit(Field field, DoubleOptionalType type, ref double? value) => Scalar(field, value);
        public virtual void Visit(Field field, UInt16OptionalType type, ref ushort? value) => Scalar(field, value);
        public virtual void BeginList(Field field, ListType type, ref uint size) { Seen(field); Depth++; }
        public void EndList() => Depth--;
        public void BeginArray(Field field, ArrayType type) { Seen(field); Depth++; }
        public void EndArray() => Depth--;
        public void BeginOptionalStruct(Field field, OptionalStructType type, bool isPresent, out bool createNew)
        { Seen(field); Depth++; createNew = false; }
        public void EndOptionalStruct(bool isPresent) => Depth--;
        public void BeginStruct(Field field, StructType type) { Seen(field); Depth++; }
        public void EndStruct() => Depth--;
        public void VisitUnknown(Field field, IFieldType type) { if (type is StructType) Seen(field); else throw new InvalidOperationException($"Unsupported {field.Name}: {type.Name}"); }
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
