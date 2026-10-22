using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Asv.IO;
using Xunit;

namespace Asv.Gnss;

public class NmeaRinexVisitorTest
{
    public static IEnumerable<object[]> Messages() => typeof(NmeaMessageBase).Assembly.GetTypes()
        .Where(t => !t.IsAbstract && (typeof(NmeaMessageBase).IsAssignableFrom(t) || typeof(RinexMessageBase).IsAssignableFrom(t)))
        .Select(t => new object[] { t });

    [Theory]
    [MemberData(nameof(Messages))]
    public void Accept_AllMessages_ExposeEveryPublicPayloadPropertyAndCacheFields(Type type)
    {
        var model = Assert.IsAssignableFrom<IVisitable>(Activator.CreateInstance(type));
        var first = new Reader();
        var second = new Reader();
        model.Accept(first);
        model.Accept(second);
        var expected = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.Name is not ("Name" or "Id" or "Protocol" or "Tags"))
            .Select(p => p.Name).Order();
        Assert.Equal(expected, first.TopFields.Select(f => f.Name).Order());
        Assert.Equal(first.Fields.Count, second.Fields.Count);
        for (var i = 0; i < first.Fields.Count; i++) Assert.Same(first.Fields[i], second.Fields[i]);
        Assert.Equal(0, first.Depth);
        Assert.Throws<ArgumentNullException>(() => model.Accept(null!));
    }

    [Fact]
    public void Accept_NmeaNestedSatellites_PreservesOptionalTypesAndStorage()
    {
        var satellite = new SatelliteInfo(new NmeaTalkerId("GP"), 12, 34, 125, null);
        var message = new NmeaMessageGsv { TotalMessages = 2, MessageNumber = 1, SatellitesInView = 5 };
        message.Satellites.Add(satellite);
        var reader = new Reader();
        message.Accept(reader);
        Assert.Contains(("NmeaPrn", (object?)12), reader.Values);
        Assert.Contains(("Elevation", (object?)34), reader.Values);
        Assert.Contains(("Azimuth", (object?)125), reader.Values);
        Assert.Contains(("Snr", (object?)null), reader.Values);
        Assert.Same(satellite, Assert.Single(message.Satellites));
        Assert.Equal(0, reader.Depth);
    }

    [Fact]
    public void Accept_NmeaScalarEditor_UpdatesWritableFieldsWithoutChangingImmutableSatellite()
    {
        var message = new NmeaMessageGga { Latitude = 51.25, Longitude = 60.75, NumberOfSatellites = 9, GpsQuality = NmeaGpsQuality.RtkFixed };
        message.Accept(new Editor());
        Assert.Equal(52.25, message.Latitude);
        Assert.Equal(61.75, message.Longitude);
        Assert.Equal(10, message.NumberOfSatellites);
        Assert.Equal((NmeaGpsQuality)5, message.GpsQuality);
        var satellite = new SatelliteInfo(new NmeaTalkerId("GP"), 12, 34, 125, 40);
        satellite.Accept(new Editor());
        Assert.Equal(12, satellite.NmeaPrn);
        Assert.Equal(34, satellite.Elevation);
    }

    [Fact]
    public void Accept_RinexNestedObservations_VisitsHeaderDictionaryEpochAndImmutableRecords()
    {
        var message = new RinexObservationEpochMessage
        {
            Epoch = new RinexEpoch(2026, 10, 23, 12, 34, 60.123456789012m),
            ReceiverClockOffset = 0.00000125,
            EpochFlag = 1
        };
        message.Header.ObservationTypes.Add('G', ["C1C", "L1C"]);
        var satellite = new RinexSatelliteObservation { Satellite = "G12" };
        var value = new RinexObservationValue("C1C", 123456.75, 2, 7);
        satellite.Observations.Add(value);
        message.Satellites.Add(satellite);
        var reader = new Reader();
        message.Accept(reader);
        Assert.Contains(("System", (object?)'G'), reader.Values);
        Assert.Contains(("Codes", (object?)"L1C"), reader.Values);
        Assert.Contains(("Seconds", (object?)(double)60.123456789012m), reader.Values);
        Assert.Contains(("Value", (object?)123456.75), reader.Values);
        Assert.Contains(("LossOfLockIndicator", (object?)(byte)2), reader.Values);
        Assert.Contains(("SignalStrengthIndicator", (object?)(byte)7), reader.Values);
        Assert.Equal(60.123456789012m, message.Epoch.Seconds);
        Assert.Same(value, Assert.Single(satellite.Observations));
        Assert.Equal(0, reader.Depth);
        message.Accept(new Editor());
        Assert.Equal(2, message.EpochFlag);
        Assert.Equal(60.123456789012m, message.Epoch.Seconds);
        Assert.Equal(123456.75, value.Value);
    }

    [Fact]
    public void Accept_RinexNavigationAndMeteorologicalLists_ExposeNumericValuesWithoutStringConversion()
    {
        var navigation = new RinexNavigationMessage();
        navigation.Values.AddRange([null, -1.25, 1.2e-12]);
        navigation.ValueFieldCounts.AddRange([3, 4]);
        var reader = new Reader();
        navigation.Accept(reader);
        Assert.Contains(("Values", (object?)null), reader.Values);
        Assert.Contains(("Values", (object?)(-1.25)), reader.Values);
        Assert.Contains(("Values", (object?)1.2e-12), reader.Values);
        Assert.Contains(("ValueFieldCounts", (object?)4), reader.Values);
        var met = new RinexMeteorologicalMessage();
        met.Observations.Add(new RinexMeteorologicalValue("PR", 1013.25));
        reader = new Reader();
        met.Accept(reader);
        Assert.Contains(("Value", (object?)1013.25), reader.Values);
    }

    [Theory]
    [InlineData("demo.10o")]
    [InlineData("obs3.01gage.10o")]
    [InlineData("demo.10n")]
    [InlineData("rinex402_examples_MN.rnx")]
    [InlineData("cari0010.07m")]
    public void Accept_RecordedRinexFiles_PreservesSerializedRecords(string file)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Resources", "Rinex", file);
        var messages = RinexFile.Read(path);
        Assert.NotEmpty(messages);
        foreach (var message in messages)
        {
            var before = new byte[message.GetByteSize()];
            var beforeBuffer = before.AsSpan();
            message.Serialize(ref beforeBuffer);
            var header = message.Header;
            var observationTypes = header.ObservationTypes;
            message.Accept(new Reader());
            var after = new byte[message.GetByteSize()];
            var afterBuffer = after.AsSpan();
            message.Serialize(ref afterBuffer);
            Assert.Equal(before, after);
            Assert.Same(header, message.Header);
            Assert.Same(observationTypes, message.Header.ObservationTypes);
        }
    }

    private class Reader : DoubleType.IVisitor, DoubleOptionalType.IVisitor, Int32Type.IVisitor,
        Int32OptionalType.IVisitor, UInt8Type.IVisitor, UInt8OptionalType.IVisitor, BoolType.IVisitor,
        StringOptionalType.IVisitor, CharType.IVisitor, DateOnlyOptionalType.IVisitor,
        TimeOnlyOptionalType.IVisitor, DateTimeOptionalType.IVisitor, StructType.IVisitor, ListType.IVisitor
    {
        public int Depth { get; private set; }
        public List<Field> Fields { get; } = [];
        public List<Field> TopFields { get; } = [];
        public List<(string Name, object? Value)> Values { get; } = [];
        private void Seen(Field field) { Fields.Add(field); if (Depth == 0) TopFields.Add(field); }
        private void Scalar(Field field, object? value) { Seen(field); Values.Add((field.Name, value)); }
        public void VisitUnknown(Field field, IFieldType type) => throw new InvalidOperationException($"Unsupported {field.Name}: {type.Name}");
        public virtual void Visit(Field f, DoubleType t, ref double v) => Scalar(f, v);
        public void Visit(Field f, DoubleOptionalType t, ref double? v) => Scalar(f, v);
        public virtual void Visit(Field f, Int32Type t, ref int v) => Scalar(f, v);
        public virtual void Visit(Field f, Int32OptionalType t, ref int? v) => Scalar(f, v);
        public void Visit(Field f, UInt8Type t, ref byte v) => Scalar(f, v);
        public void Visit(Field f, UInt8OptionalType t, ref byte? v) => Scalar(f, v);
        public void Visit(Field f, BoolType t, ref bool v) => Scalar(f, v);
        public void Visit(Field f, StringOptionalType t, ref string? v) => Scalar(f, v);
        public void Visit(Field f, CharType t, ref char v) => Scalar(f, v);
        public void Visit(Field f, DateOnlyOptionalType t, ref DateOnly? v) => Scalar(f, v);
        public void Visit(Field f, TimeOnlyOptionalType t, ref TimeOnly? v) => Scalar(f, v);
        public void Visit(Field f, DateTimeOptionalType t, ref DateTime? v) => Scalar(f, v);
        public void BeginStruct(Field f, StructType t) { Seen(f); Depth++; }
        public void EndStruct() => Depth--;
        public void BeginList(Field f, ListType t, ref uint size) { Seen(f); Depth++; }
        public void EndList() => Depth--;
    }

    private sealed class Editor : Reader
    {
        public override void Visit(Field f, DoubleType t, ref double v) { v++; base.Visit(f, t, ref v); }
        public override void Visit(Field f, Int32Type t, ref int v) { v++; base.Visit(f, t, ref v); }
        public override void Visit(Field f, Int32OptionalType t, ref int? v) { v = (v ?? 0) + 1; base.Visit(f, t, ref v); }
    }
}
