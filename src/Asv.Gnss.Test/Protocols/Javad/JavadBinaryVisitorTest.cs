using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Asv.IO;
using Xunit;

namespace Asv.Gnss;

public class JavadBinaryVisitorTest
{
    [Fact]
    public void Accept_AllBinaryMessagesAndNestedRecords_ExposeEveryPayloadPropertyAndPreserveDefaults()
    {
        var factory = CreateFactory();
        var records = new HashSet<Type>();
        var visitor = new RootFieldsVisitor();
        Assert.Equal(271, factory.GetSupportedIds().Count());
        foreach (var id in factory.GetSupportedIds())
        {
            var message = factory.Create(id)!;
            var bytes = Serialize(message);
            var metadata = Metadata(message.GetType());
            Assert.Same(metadata, Metadata(message.GetType()));
            AssertMetadata(message.GetType(), metadata, records);
            visitor.Fields.Clear();

            message.Accept(visitor);

            Assert.Equal(metadata.Fields.Select(field => field.Name).Order(), visitor.Fields.Order());
            Assert.Equal(bytes, Serialize(message));
        }

        Assert.Equal(26, records.Count);
        foreach (var record in records.ToArray())
        {
            var value = Assert.IsAssignableFrom<IVisitable>(Activator.CreateInstance(record));
            var metadata = Metadata(record);
            AssertMetadata(record, metadata, records);
            visitor.Fields.Clear();
            value.Accept(visitor);
            Assert.Equal(metadata.Fields.Select(field => field.Name).Order(), visitor.Fields.Order());
        }
    }

    [Fact]
    public void Accept_ScalarNestedAndArrayEdits_AreVisibleDuringTheCallback()
    {
        var message = new JavadIqSamplesMessage
        {
            Satellite = new JavadSatelliteIdentifier { SystemId = 1, SatelliteId = 7 },
            SlotId = 2,
            IsQ = 1,
            Samples = [new JavadIqSample { I = 12, Q = -8 }],
        };
        var visitor = new DirectEditVisitor(message);

        message.Accept(visitor);

        Assert.Equal(4, visitor.Edits);
        var decoded = new JavadIqSamplesMessage();
        ReadOnlySpan<byte> input = Serialize(message);
        decoded.Deserialize(ref input);
        Assert.Equal(9, decoded.Satellite.SatelliteId);
        Assert.Equal(4, decoded.SlotId);
        Assert.Equal(42, Assert.Single(decoded.Samples).I);
        Assert.Equal((short?)-20, decoded.Samples[0].Q);
    }

    [Fact]
    public void Accept_ExtensionBytes_AreEditedByReferenceAndPreservedOnWire()
    {
        var message = new JavadReceiverTimeMessage { ExtensionData = [0xA1] };
        message.Accept(new DirectExtensionVisitor(message));
        Assert.Equal(new byte[] { 0xB2 }, message.ExtensionData);
        var decoded = new JavadReceiverTimeMessage();
        ReadOnlySpan<byte> input = Serialize(message);
        decoded.Deserialize(ref input);
        Assert.Equal(message.ExtensionData, decoded.ExtensionData);
    }

    [Theory]
    [InlineData("DP", "HasEdop", 4)]
    [InlineData("NU", "HasNt", 2)]
    [InlineData("WE", "HasFlags", 2)]
    [InlineData("EN", "HasDAf0", 4)]
    public void Accept_HistoricalTrailingFieldPresence_IsTypedAndEditedByReference(
        string id,
        string propertyName,
        int fieldSize
    )
    {
        var message = CreateFactory().Create(id)!;
        var original = Serialize(message);
        var metadata = Metadata(message.GetType());
        Assert.IsType<BoolType>(metadata[propertyName]!.DataType);
        var visitor = new EditPresenceVisitor(message, propertyName, false);

        message.Accept(visitor);

        Assert.Equal(1, visitor.Edits);
        var historical = Serialize(message);
        Assert.Equal(original.Length - fieldSize, historical.Length);
        var decoded = CreateFactory().Create(id)!;
        ReadOnlySpan<byte> input = historical;
        decoded.Deserialize(ref input);
        Assert.True(input.IsEmpty);
        Assert.Equal(historical, Serialize(decoded));
        message.Accept(new EditPresenceVisitor(message, propertyName, true));
        Assert.Equal(original, Serialize(message));
    }

    [Fact]
    public void Accept_OptionalStructAndPrimitive_CanBeCreatedAndEditedExplicitly()
    {
        var message = new JavadSingleSpectrumMessage();
        message.Accept(new CreateSpectrumInfoVisitor());
        Assert.NotNull(message.SpectrumInfo);
        Assert.Equal(123, message.SpectrumInfo.Step);
        Assert.IsType<OptionalStructType>(JavadSingleSpectrumMessage.StructType[nameof(message.SpectrumInfo)]!.DataType);

        var sample = new JavadIqSample { I = 5 };
        sample.Accept(new EditOptionalSampleVisitor(-123));
        Assert.Equal((short?)-123, sample.Q);
        sample.Accept(new EditOptionalSampleVisitor(null));
        Assert.Null(sample.Q);
        Assert.IsType<Int16OptionalType>(JavadIqSample.StructType[nameof(sample.Q)]!.DataType);

        var satellite = new JavadLegacyRawMeasurementSlot();
        Assert.IsType<UInt16OptionalType>(JavadLegacyRawMeasurementSlot.StructType[nameof(satellite.Flags)]!.DataType);
        Assert.IsType<UInt32OptionalType>(JavadLegacyRawMeasurementSlot.StructType[nameof(satellite.Word2)]!.DataType);
        Assert.IsType<FloatOptionalType>(JavadBeiDouModernEphemerisData.StructType[nameof(JavadBeiDouModernEphemerisData.Tgd_B2Bdata)]!.DataType);
    }

    [Fact]
    public void Accept_ArrayMetadataDistinguishesFixedAndVariableStorage_AndVariableResizeRetainsPrefix()
    {
        var fixedArray = Assert.IsType<ArrayType>(JavadLegacyGpsNavigationSatellite.StructType[nameof(JavadLegacyGpsNavigationSatellite.Data)]!.DataType);
        Assert.Equal(10, fixedArray.Size);
        Assert.IsType<UInt32Type>(fixedArray.ElementType);
        var fixedRecord = new JavadLegacyGpsNavigationSatellite();
        var fixedStorage = fixedRecord.Data;
        fixedRecord.Accept(new EditFixedWordsVisitor(fixedRecord));
        Assert.Same(fixedStorage, fixedRecord.Data);
        Assert.All(fixedStorage, value => Assert.Equal(1U, value));
        var list = Assert.IsType<ListType>(JavadExtendedSatelliteIndicesMessage.StructType[nameof(JavadExtendedSatelliteIndicesMessage.Satellites)]!.DataType);
        Assert.Same(JavadSatelliteIdentifier.StructType, list.ElementType);
        var first = new JavadSatelliteIdentifier { SystemId = 2, SatelliteId = 4 };
        var message = new JavadExtendedSatelliteIndicesMessage { Satellites = [first] };

        message.Accept(new ResizeSatellitesVisitor(3));

        Assert.Equal(3, message.Satellites.Length);
        Assert.Same(first, message.Satellites[0]);
        Assert.NotNull(message.Satellites[1]);
        Assert.NotSame(message.Satellites[1], message.Satellites[2]);
        Assert.Equal(0, message.Satellites[1].SystemId);
        var storage = message.Satellites;
        Assert.Throws<ArgumentOutOfRangeException>(() => message.Accept(new ResizeSatellitesVisitor(uint.MaxValue)));
        Assert.Same(storage, message.Satellites);
    }

    [Fact]
    public void Accept_DerivedCountsAndLegacyLayoutValues_AreExplicitlyReadOnly()
    {
        var message = new JavadGpsRawNavigationDataMessage { Data = [1, 2] };
        var field = JavadGpsRawNavigationDataMessage.StructType[nameof(message.Len)]!;
        Assert.Equal(true, field.Metadata["readOnly"]);
        Assert.Throws<InvalidOperationException>(() => message.Accept(new EditCountVisitor()));
        Assert.Equal(2, message.Len);
        Assert.Equal(new uint[] { 1, 2 }, message.Data);
        var legacy = new JavadLegacyRawMeasurementsMessage();
        foreach (var name in new[] { "Version", "HeaderSize", "SlotSize" })
        {
            Assert.Equal(true, JavadLegacyRawMeasurementsMessage.StructType[name]!.Metadata["readOnly"]);
            Assert.Throws<InvalidOperationException>(() => legacy.Accept(new EditLegacyLayoutVisitor(name)));
            Assert.Equal(4, legacy.Flags);
        }
        Assert.Equal("ms", JavadReceiverTimeMessage.StructType[nameof(JavadReceiverTimeMessage.TimeOfDay)]!.GetUnits());
        Assert.Equal("s", JavadUtcOffsetParameters.StructType[nameof(JavadUtcOffsetParameters.A0)]!.GetUnits());
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(2)]
    [InlineData(10)]
    [InlineData(11)]
    public void Accept_ReadOnlyTraversal_PreservesNullShortAndOversizedArrays(int length)
    {
        var storage = length < 0 ? null : Enumerable.Range(1, length).Select(value => (uint)value).ToArray();
        var record = new JavadLegacyGpsNavigationSatellite { Data = storage! };
        record.Accept(new NoOpVisitor());
        Assert.Same(storage, record.Data);
        if (storage is not null)
            Assert.Equal(Enumerable.Range(1, length).Select(value => (uint)value), storage);

        var message = new JavadGpsEphemerisMessage { RequiredData = null! };
        message.Accept(new NoOpVisitor());
        Assert.Null(message.RequiredData);
        Assert.Null(message.OptionalData);
        var measurements = new JavadIqSamplesMessage { Samples = null! };
        measurements.Accept(new NoOpVisitor());
        Assert.Null(measurements.Samples);
    }

    [Fact]
    public void Accept_RepeatedTraversalOfEveryBinaryModel_DoesNotAllocate()
    {
        var factory = CreateFactory();
        var messages = factory.GetSupportedIds().Select(id => factory.Create(id)!).ToList();
        messages.Add(new JavadIqSamplesMessage { IsQ = 1, Samples = [new() { I = 2, Q = 3 }] });
        messages.Add(new JavadGpsEphemerisMessage { OptionalData = new() { NavType = 1, Cnav = new() } });
        messages.Add(new JavadLegacyRawMeasurementsMessage
        {
            Satellites = [new() { Slots = [new() { Flags = 1, Lock = 2, Word2 = 3 }] }],
        });
        messages.Add(new JavadSingleSpectrumMessage { PointCount = 2, Values = [1, 2], SpectrumInfo = new() });
        var visitor = new NoOpVisitor();
        foreach (var message in messages)
        {
            for (var index = 0; index < 100; index++)
                message.Accept(visitor);
            var before = GC.GetAllocatedBytesForCurrentThread();
            for (var index = 0; index < 100; index++)
                message.Accept(visitor);
            var allocated = GC.GetAllocatedBytesForCurrentThread() - before;
            Assert.Equal(0, allocated);
        }
        GC.KeepAlive(messages);
    }

    private static IProtocolMessageFactory<JavadMessageBase, string> CreateFactory()
    {
        var builder = new ProtocolMessageFactoryBuilder<JavadMessageBase, string>(JavadProtocol.Info);
        JavadBinaryMessageFactory.Register(builder);
        return builder.Build();
    }

    private static StructType Metadata(Type type) => Assert.IsType<StructType>(
        type.GetProperty("StructType", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)!.GetValue(null));

    private static void AssertMetadata(Type type, StructType metadata, HashSet<Type> records)
    {
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(property => property.DeclaringType != typeof(JavadMessageBase)
                && property.Name is not ("Id" or "Name" or "ChecksumKind"))
            .ToArray();
        var names = properties.Select(property => property.Name);
        if (typeof(JavadMessageBase).IsAssignableFrom(type))
            names = names.Append(nameof(JavadMessageBase.ExtensionData));
        Assert.Equal(names.Order(), metadata.Fields.Select(field => field.Name).Order());
        foreach (var property in properties)
        {
            var nested = property.PropertyType.IsArray ? property.PropertyType.GetElementType()! : property.PropertyType;
            if (typeof(IVisitable).IsAssignableFrom(nested) && records.Add(nested))
                AssertMetadata(nested, Metadata(nested), records);
            if (!property.CanWrite)
                Assert.Equal(true, metadata[property.Name]!.Metadata["readOnly"]);
        }
    }

    private static byte[] Serialize(JavadMessageBase message)
    {
        var result = new byte[message.GetByteSize()];
        var buffer = result.AsSpan();
        message.Serialize(ref buffer);
        Assert.True(buffer.IsEmpty);
        return result;
    }

    private class NoOpVisitor : UInt8Type.IVisitor, Int8Type.IVisitor, UInt16Type.IVisitor,
        Int16Type.IVisitor, UInt32Type.IVisitor, Int32Type.IVisitor, FloatType.IVisitor,
        DoubleType.IVisitor, CharType.IVisitor, StringType.IVisitor, Int16OptionalType.IVisitor,
        UInt16OptionalType.IVisitor, UInt32OptionalType.IVisitor, FloatOptionalType.IVisitor,
        StructType.IVisitor, OptionalStructType.IVisitor, ArrayType.IVisitor, ListType.IVisitor,
        BoolType.IVisitor
    {
        protected int Depth;
        protected virtual void Seen(Field field) { }
        public virtual void VisitUnknown(Field field, IFieldType type) => Seen(field);
        public virtual void Visit(Field field, BoolType type, ref bool value) => Seen(field);
        public virtual void Visit(Field field, UInt8Type type, ref byte value) => Seen(field);
        public virtual void Visit(Field field, Int8Type type, ref sbyte value) => Seen(field);
        public virtual void Visit(Field field, UInt16Type type, ref ushort value) => Seen(field);
        public virtual void Visit(Field field, Int16Type type, ref short value) => Seen(field);
        public virtual void Visit(Field field, UInt32Type type, ref uint value) => Seen(field);
        public virtual void Visit(Field field, Int32Type type, ref int value) => Seen(field);
        public virtual void Visit(Field field, FloatType type, ref float value) => Seen(field);
        public virtual void Visit(Field field, DoubleType type, ref double value) => Seen(field);
        public virtual void Visit(Field field, CharType type, ref char value) => Seen(field);
        public virtual void Visit(Field field, StringType type, ref string value) => Seen(field);
        public virtual void Visit(Field field, Int16OptionalType type, ref short? value) => Seen(field);
        public virtual void Visit(Field field, UInt16OptionalType type, ref ushort? value) => Seen(field);
        public virtual void Visit(Field field, UInt32OptionalType type, ref uint? value) => Seen(field);
        public virtual void Visit(Field field, FloatOptionalType type, ref float? value) => Seen(field);
        public virtual void BeginStruct(Field field, StructType type) { Seen(field); Depth++; }
        public virtual void EndStruct() => Depth--;
        public virtual void BeginOptionalStruct(Field field, OptionalStructType type, bool isPresent, out bool createNew)
        { Seen(field); Depth++; createNew = false; }
        public virtual void EndOptionalStruct(bool isPresent) => Depth--;
        public virtual void BeginArray(Field field, ArrayType type) { Seen(field); Depth++; }
        public virtual void EndArray() => Depth--;
        public virtual void BeginList(Field field, ListType type, ref uint size) { Seen(field); Depth++; }
        public virtual void EndList() => Depth--;
    }

    private sealed class RootFieldsVisitor : NoOpVisitor
    {
        public HashSet<string> Fields { get; } = [];
        protected override void Seen(Field field)
        {
            if (Depth == 0)
                Assert.True(Fields.Add(field.Name), $"Duplicate visit of {field.Name}.");
        }
    }

    private sealed class EditPresenceVisitor(
        JavadMessageBase message,
        string propertyName,
        bool present
    ) : NoOpVisitor
    {
        public int Edits { get; private set; }

        public override void Visit(Field field, BoolType type, ref bool value)
        {
            if (field.Name != propertyName)
                return;
            value = present;
            Assert.Equal(present, message.GetType().GetProperty(propertyName)!.GetValue(message));
            Edits++;
        }
    }

    private sealed class DirectEditVisitor(JavadIqSamplesMessage message) : NoOpVisitor
    {
        public int Edits { get; private set; }
        public override void Visit(Field field, UInt8Type type, ref byte value)
        {
            switch (field.Name)
            {
                case nameof(JavadSatelliteIdentifier.SatelliteId):
                    value = 9;
                    Assert.Equal(9, message.Satellite.SatelliteId);
                    break;
                case nameof(JavadIqSamplesMessage.SlotId):
                    value = 4;
                    Assert.Equal(4, message.SlotId);
                    break;
                default: return;
            }
            Edits++;
        }
        public override void Visit(Field field, Int16Type type, ref short value)
        {
            value = 42;
            Assert.Equal(42, message.Samples[0].I);
            Edits++;
        }
        public override void Visit(Field field, Int16OptionalType type, ref short? value)
        {
            value = -20;
            Assert.Equal((short?)-20, message.Samples[0].Q);
            Edits++;
        }
    }

    private sealed class CreateSpectrumInfoVisitor : NoOpVisitor
    {
        public override void BeginOptionalStruct(Field field, OptionalStructType type, bool isPresent, out bool createNew)
        {
            base.BeginOptionalStruct(field, type, isPresent, out createNew);
            createNew = field.Name == nameof(JavadSingleSpectrumMessage.SpectrumInfo) && !isPresent;
        }
        public override void Visit(Field field, FloatType type, ref float value)
        {
            if (field.Name == nameof(JavadSpectrumExtension.Step)) value = 123;
        }
    }

    private sealed class DirectExtensionVisitor(JavadReceiverTimeMessage message) : NoOpVisitor
    {
        public override void Visit(Field field, UInt8Type type, ref byte value)
        {
            value = 0xB2;
            Assert.Equal(0xB2, message.ExtensionData[0]);
        }
    }

    private sealed class EditOptionalSampleVisitor(short? replacement) : NoOpVisitor
    {
        public override void Visit(Field field, Int16OptionalType type, ref short? value) => value = replacement;
    }

    private sealed class ResizeSatellitesVisitor(uint count) : NoOpVisitor
    {
        public override void BeginList(Field field, ListType type, ref uint size)
        {
            base.BeginList(field, type, ref size);
            if (field.Name == nameof(JavadExtendedSatelliteIndicesMessage.Satellites)) size = count;
        }
    }

    private sealed class EditCountVisitor : NoOpVisitor
    {
        public override void Visit(Field field, UInt8Type type, ref byte value)
        {
            if (field.Name == nameof(JavadGpsRawNavigationDataMessage.Len)) value++;
        }
    }

    private sealed class EditLegacyLayoutVisitor(string name) : NoOpVisitor
    {
        public override void Visit(Field field, Int32Type type, ref int value)
        {
            if (field.Name == name) value++;
        }
    }

    private sealed class EditFixedWordsVisitor(JavadLegacyGpsNavigationSatellite record) : NoOpVisitor
    {
        private int _index;
        public override void Visit(Field field, UInt32Type type, ref uint value)
        {
            value++;
            Assert.Equal(value, record.Data[_index++]);
        }
    }
}
