using System;
using System.Collections.Generic;
using System.Linq;
using Asv.Common;
using Asv.IO;
using Geodesy;
using Xunit;

namespace Asv.Gnss;

public class UbxNavVisitorTest
{
    [Fact]
    public void Accept_NativeFieldReference_ShouldExposeMutationImmediatelyThroughPublicProperty()
    {
        var message = new UbxNavSat { Version = 1 };
        var visitor = new ImmediateVersionVisitor(message);

        message.Accept(visitor);

        Assert.True(visitor.Visited);
        Assert.Equal((byte)7, message.Version);
    }

    [Fact]
    public void Accept_SatelliteRecords_ShouldVisitNativeFieldsAndPreserveCollectionIdentity()
    {
        var first = new UbxNavSatelliteItem
        {
            GnssType = UbxNavSatGnssId.GLONASS,
            GnssId = 6,
            SvId = 11,
            CnobBHz = 45,
            ElevDeg = -15,
            AzimDeg = 270,
            PrResM = -1.25,
            QualityInd = UbxQualityIndEnum.CodeLockedAndTimeSynchronized,
            Health = UbxHealthInd.Healthy,
            OrbitSource = UbxOrbitSource.EphemerisIsUsed,
            SvUsed = true
        };
        var second = new UbxNavSatelliteItem { SvId = 22 };
        var items = new[] { first, second };
        var message = new UbxNavSat { iTOW = 123456, NumSvs = 2, Version = 1, Items = items };
        var visitor = new NavVisitor();

        message.Accept(visitor);

        Assert.Same(items, message.Items);
        Assert.Same(first, message.Items[0]);
        Assert.Same(second, message.Items[1]);
        Assert.Equal(new object?[] { (byte)11, (byte)22 }, visitor.Values(nameof(UbxNavSatelliteItem.SvId)));
        Assert.Equal((sbyte)-15, visitor.First(nameof(UbxNavSatelliteItem.ElevDeg)).Value);
        Assert.IsType<Int8Type>(visitor.First(nameof(UbxNavSatelliteItem.ElevDeg)).Field.DataType);
        Assert.Equal("deg", visitor.First(nameof(UbxNavSatelliteItem.ElevDeg)).Field.GetUnits());
        Assert.Equal(-1.25, visitor.First(nameof(UbxNavSatelliteItem.PrResM)).Value);
        Assert.Equal("dB-Hz", visitor.First(nameof(UbxNavSatelliteItem.CnobBHz)).Field.GetUnits());
        var constellation = visitor.First(nameof(UbxNavSatelliteItem.GnssType));
        Assert.Equal((byte)6, constellation.Value);
        Assert.Equal(nameof(UbxNavSatGnssId.GLONASS), constellation.Field.GetEnum<byte>()[6].Name);
        Assert.Equal((int)UbxQualityIndEnum.CodeLockedAndTimeSynchronized, visitor.First(nameof(UbxNavSatelliteItem.QualityInd)).Value);
        Assert.All(visitor.Visits, visit => Assert.False(string.IsNullOrWhiteSpace(visit.Field.GetDescription())));
        Assert.Equal(24, UbxNavSatelliteItem.StructType.Fields.Length);
    }

    [Fact]
    public void Accept_SbasRecords_ShouldVisitSignedCorrectionsAndNegativeEnumWithoutChangingState()
    {
        var satellite = new SvData { SvId = 9, Prc = -125, Ic = 350, IsIntegrity = true };
        var records = new[] { satellite };
        var message = new UbxNavSbas
        {
            iTOW = 123,
            Cnt = 1,
            Sys = UbxSbasSystem.Unknown,
            Mode = UbxSbasMode.EnabledIntegrity,
            SvDatas = records
        };
        var visitor = new NavVisitor();

        message.Accept(visitor);

        Assert.Same(records, message.SvDatas);
        Assert.Same(satellite, message.SvDatas[0]);
        Assert.Equal((short)-125, visitor.First(nameof(SvData.Prc)).Value);
        Assert.Equal("cm", visitor.First(nameof(SvData.Prc)).Field.GetUnits());
        Assert.IsType<Int16Type>(visitor.First(nameof(SvData.Prc)).Field.DataType);
        Assert.Equal((short)350, visitor.First(nameof(SvData.Ic)).Value);
        Assert.Equal(-1, visitor.First(nameof(UbxNavSbas.Sys)).Value);
        Assert.Equal(nameof(UbxSbasSystem.Unknown), visitor.First(nameof(UbxNavSbas.Sys)).Field.GetEnum<int>()[-1].Name);
        Assert.Equal(UbxSbasSystem.Unknown, message.Sys);
        Assert.Equal((short)-125, satellite.Prc);
        Assert.Equal(14, SvData.StructType.Fields.Length);
    }

    [Fact]
    public void Accept_PvtMutation_ShouldWriteBackNativeNumericEnumAndTimeValues()
    {
        var originalTime = new DateTime(2024, 1, 7, 12, 34, 56, DateTimeKind.Utc).AddTicks(1234);
        var expectedTime = originalTime.AddTicks(11);
        var message = new UbxNavPvt
        {
            iTOW = 123456,
            Latitude = -33.25,
            Longitude = 37.5,
            FixType = UbxGnssFixType.Fix3D,
            CarrierSolution = UbxCarrierSolutionStatus.FloatingAmbiguities,
            UtcTime = originalTime,
            GpsTime = originalTime.AddSeconds(18)
        };
        var visitor = new NavVisitor();
        visitor.Changes[nameof(UbxNavPvt.iTOW)] = ulong.MaxValue;
        visitor.Changes[nameof(UbxNavPvt.Latitude)] = 55.5;
        visitor.Changes[nameof(UbxNavPvt.FixType)] = (byte)255;
        visitor.Changes[nameof(UbxNavPvt.CarrierSolution)] = (int)UbxCarrierSolutionStatus.FixedAmbiguities;
        visitor.Changes[nameof(UbxNavPvt.GnssFixOK)] = true;
        visitor.Changes[nameof(UbxNavPvt.UtcTime)] = expectedTime;

        message.Accept(visitor);

        Assert.Equal(ulong.MaxValue, message.iTOW);
        Assert.Equal(55.5, message.Latitude);
        Assert.Equal(37.5, message.Longitude);
        Assert.Equal((UbxGnssFixType)255, message.FixType);
        Assert.Equal(UbxCarrierSolutionStatus.FixedAmbiguities, message.CarrierSolution);
        Assert.True(message.GnssFixOK);
        Assert.Equal(expectedTime, message.UtcTime);
        Assert.Equal(DateTimeKind.Utc, message.UtcTime.Kind);
        Assert.Equal(originalTime.AddSeconds(18), message.GpsTime);
        Assert.IsType<UInt64Type>(visitor.First(nameof(UbxNavPvt.iTOW)).Field.DataType);
        Assert.IsType<DateTimeType>(visitor.First(nameof(UbxNavPvt.UtcTime)).Field.DataType);
        Assert.Contains((byte)3, visitor.First(nameof(UbxNavPvt.FixType)).Field.GetEnum<byte>().Keys);
        Assert.Equal(default(GlobalPosition), message.MovingBaseLocation);
    }

    [Fact]
    public void Accept_StatusAndDop_ShouldExposeRawFlagsAndDerivedPropertiesIndependently()
    {
        var status = new UbxNavStatus { Flags = 0xFF, MapMatching = MapMatchingStatus.ValidButNotUsed, TTFF = 500 };
        var dop = new UbxNavDop { iTOW = 321, gDOP = 1.25, pDOP = 2.5, hDOP = 3.75 };
        var visitor = new NavVisitor();
        visitor.Changes[nameof(UbxNavStatus.MapMatching)] = (int)MapMatchingStatus.ValidAndUsedSensor;
        visitor.Changes[nameof(UbxNavDop.pDOP)] = 4.5;

        status.Accept(visitor);
        dop.Accept(visitor);

        Assert.Equal(MapMatchingStatus.ValidAndUsedSensor, status.MapMatching);
        Assert.Equal(0xFF, status.Flags);
        Assert.Equal(500U, status.TTFF);
        Assert.Equal("ms", visitor.First(nameof(UbxNavStatus.TTFF)).Field.GetUnits());
        Assert.Equal(4.5, dop.pDOP);
        Assert.Equal(1.25, dop.gDOP);
        Assert.Equal(3.75, dop.hDOP);
        Assert.Equal(321UL, dop.iTOW);
    }

    [Fact]
    public void Accept_SurveyPosition_ShouldTraverseAndMutateDerivedCoordinatesWithoutRecalculatingRawFields()
    {
        var message = new UbxNavSvin
        {
            Duration = 300,
            MeanX = 12345,
            MeanXhp = -7,
            Ecef = (123.4493, 456.25, -789.5),
            Location = new GeoPoint(55, 37, 100),
            Accuracy = 0.1234,
            Valid = true
        };
        var visitor = new NavVisitor();
        visitor.Changes["Ecef.X"] = -1000.25;
        visitor.Changes["Location.Altitude"] = 123.4;
        visitor.Changes[nameof(UbxNavSvin.MeanXhp)] = (sbyte)-99;

        message.Accept(visitor);

        Assert.Equal((-1000.25, 456.25, -789.5), message.Ecef);
        Assert.Equal(new GeoPoint(55, 37, 123.4), message.Location);
        Assert.Equal(12345, message.MeanX);
        Assert.Equal((sbyte)-99, message.MeanXhp);
        Assert.Equal("0.1 mm", visitor.First(nameof(UbxNavSvin.MeanXhp)).Field.GetUnits());
        Assert.Equal("m", visitor.First("X").Field.GetUnits());
        Assert.Equal(300U, message.Duration);
        Assert.True(message.Valid);
    }

    [Fact]
    public void Accept_GlobalPosition_ShouldPreserveValueOnReadAndWriteCoordinates()
    {
        var position = new GlobalPosition(new GlobalCoordinates(55, 37), 100);
        var time = new DateTime(2024, 1, 7, 0, 0, 0, DateTimeKind.Utc).AddTicks(1234567);
        var message = new UbxNavPvt { MovingBaseLocation = position, UtcTime = time };
        var reader = new NavVisitor();

        message.Accept(reader);

        Assert.Equal(position, message.MovingBaseLocation);
        Assert.Equal(time, message.UtcTime);
        Assert.Equal(55.0, reader.At("MovingBaseLocation.Latitude").Value);
        Assert.Equal(100.0, reader.At("MovingBaseLocation.Altitude").Value);
        var mutator = new NavVisitor();
        mutator.Changes["MovingBaseLocation.Longitude"] = -70.25;

        message.Accept(mutator);

        Assert.NotEqual(position, message.MovingBaseLocation);
        Assert.Equal(55.0, message.MovingBaseLocation.Latitude.Degrees);
        Assert.Equal(-70.25, message.MovingBaseLocation.Longitude.Degrees);
        Assert.Equal(100.0, message.MovingBaseLocation.Elevation);
        Assert.Equal(37.0, position.Longitude.Degrees);
    }

    [Fact]
    public void Accept_ReadVisitor_ShouldPreserveNullAndEmptyCollectionsAndPositionValues()
    {
        var missingSat = new UbxNavSat { Items = null! };
        var missingSbas = new UbxNavSbas { SvDatas = null! };
        var survey = new UbxNavSvin { Location = null };
        var pvt = new UbxNavPvt { MovingBaseLocation = default };
        UbxNavSatelliteItem[] emptySatellites = [];
        SvData[] emptySbas = [];
        var emptySat = new UbxNavSat { Items = emptySatellites };
        var emptySbasMessage = new UbxNavSbas { SvDatas = emptySbas };
        var visitor = new NavVisitor();

        foreach (var message in new UbxMessageBase[] { missingSat, missingSbas, survey, pvt, emptySat, emptySbasMessage })
        {
            message.Accept(visitor);
        }

        Assert.Null(missingSat.Items);
        Assert.Null(missingSbas.SvDatas);
        Assert.Null(survey.Location);
        Assert.Equal(default(GlobalPosition), pvt.MovingBaseLocation);
        Assert.Same(emptySatellites, emptySat.Items);
        Assert.Same(emptySbas, emptySbasMessage.SvDatas);
        Assert.IsType<OptionalStructType>(visitor.First(nameof(UbxNavSvin.Location)).Field.DataType);
    }

    [Fact]
    public void Accept_MutatingNestedLists_ShouldCreateRecordsAndWriteBackNestedScalars()
    {
        var original = new UbxNavSatelliteItem { SvId = 1 };
        var message = new UbxNavSat { NumSvs = 1, Items = [original] };
        var sbas = new UbxNavSbas { Cnt = 0, SvDatas = null! };
        var visitor = new NavVisitor();
        visitor.ListSizes[nameof(UbxNavSat.Items)] = 2;
        visitor.ListSizes[nameof(UbxNavSbas.SvDatas)] = 1;
        visitor.Changes["Items.Items.SvId"] = (byte)42;
        visitor.Changes["SvDatas.SvDatas.Prc"] = (short)-250;

        message.Accept(visitor);
        sbas.Accept(visitor);

        Assert.Equal(2, message.Items.Length);
        Assert.Same(original, message.Items[0]);
        Assert.All(message.Items, item => Assert.Equal((byte)42, item.SvId));
        Assert.Equal((byte)1, message.NumSvs);
        Assert.Equal((short)-250, Assert.Single(sbas.SvDatas).Prc);
        Assert.Equal((byte)0, sbas.Cnt);
    }

    [Fact]
    public void Accept_MissingSurveyAndDefaultGlobalPosition_ShouldSupportCoordinateMutations()
    {
        var survey = new UbxNavSvin();
        var pvt = new UbxNavPvt();
        var visitor = new NavVisitor();
        visitor.CreateOptional.Add(nameof(UbxNavSvin.Location));
        visitor.Changes["Location.Latitude"] = 10.0;
        visitor.Changes["Location.Longitude"] = 20.0;
        visitor.Changes["Location.Altitude"] = 30.0;
        visitor.Changes["MovingBaseLocation.Latitude"] = -10.0;
        visitor.Changes["MovingBaseLocation.Longitude"] = -20.0;
        visitor.Changes["MovingBaseLocation.Altitude"] = -30.0;

        survey.Accept(visitor);
        pvt.Accept(visitor);

        Assert.Equal(new GeoPoint(10, 20, 30), survey.Location);
        Assert.Equal(-10.0, pvt.MovingBaseLocation.Latitude.Degrees);
        Assert.Equal(-20.0, pvt.MovingBaseLocation.Longitude.Degrees);
        Assert.Equal(-30.0, pvt.MovingBaseLocation.Elevation);
    }

    [Fact]
    public void Accept_VisitorRequestsExistingLocationRecreation_ShouldStartFromDefaultCoordinates()
    {
        var message = new UbxNavSvin { Location = new GeoPoint(50, 60, 70) };
        var visitor = new NavVisitor();
        visitor.CreateOptional.Add(nameof(UbxNavSvin.Location));
        visitor.Changes["Location.Altitude"] = 42.0;

        message.Accept(visitor);

        Assert.Equal(new GeoPoint(0, 0, 42), message.Location);
    }

    private sealed record Visit(string Path, Field Field, object? Value);

    private sealed class ImmediateVersionVisitor(UbxNavSat message) : UInt8Type.IVisitor
    {
        public bool Visited { get; private set; }

        public void Visit(Field field, UInt8Type type, ref byte value)
        {
            if (field.Name != nameof(UbxNavSat.Version))
            {
                return;
            }

            value = 7;
            Visited = true;
            Assert.Equal((byte)7, message.Version);
        }

        public void VisitUnknown(Field field, IFieldType type) { }
    }

    private sealed class NavVisitor : UInt8Type.IVisitor, UInt16Type.IVisitor, UInt32Type.IVisitor,
        UInt64Type.IVisitor, Int8Type.IVisitor, Int16Type.IVisitor, Int32Type.IVisitor,
        DoubleType.IVisitor, BoolType.IVisitor, DateTimeType.IVisitor, ListType.IVisitor,
        StructType.IVisitor, OptionalStructType.IVisitor
    {
        private readonly List<string> _path = [];
        public List<Visit> Visits { get; } = [];
        public Dictionary<string, object> Changes { get; } = [];
        public Dictionary<string, uint> ListSizes { get; } = [];
        public HashSet<string> CreateOptional { get; } = [];

        public Visit First(string name) => Visits.First(visit => visit.Field.Name == name);
        public Visit At(string path) => Assert.Single(Visits, visit => visit.Path == path);
        public object?[] Values(string name) => Visits.Where(visit => visit.Field.Name == name).Select(visit => visit.Value).ToArray();
        private string Path(Field field) => string.Join(".", _path.Append(field.Name));

        private void Record<T>(Field field, ref T value)
        {
            var path = Path(field);
            Visits.Add(new Visit(path, field, value));
            if (Changes.TryGetValue(path, out var replacement))
            {
                value = (T)replacement;
            }
        }

        public void Visit(Field field, UInt8Type type, ref byte value) => Record(field, ref value);
        public void Visit(Field field, UInt16Type type, ref ushort value) => Record(field, ref value);
        public void Visit(Field field, UInt32Type type, ref uint value) => Record(field, ref value);
        public void Visit(Field field, UInt64Type type, ref ulong value) => Record(field, ref value);
        public void Visit(Field field, Int8Type type, ref sbyte value) => Record(field, ref value);
        public void Visit(Field field, Int16Type type, ref short value) => Record(field, ref value);
        public void Visit(Field field, Int32Type type, ref int value) => Record(field, ref value);
        public void Visit(Field field, DoubleType type, ref double value) => Record(field, ref value);
        public void Visit(Field field, BoolType type, ref bool value) => Record(field, ref value);
        public void Visit(Field field, DateTimeType type, ref DateTime value) => Record(field, ref value);

        public void BeginList(Field field, ListType type, ref uint size)
        {
            if (ListSizes.TryGetValue(field.Name, out var requestedSize))
            {
                size = requestedSize;
            }
            _path.Add(field.Name);
        }
        public void EndList() => _path.RemoveAt(_path.Count - 1);
        public void BeginStruct(Field field, StructType type) => _path.Add(field.Name);
        public void EndStruct() => _path.RemoveAt(_path.Count - 1);
        public void BeginOptionalStruct(Field field, OptionalStructType type, bool isPresent, out bool createNew)
        {
            Visits.Add(new Visit(Path(field), field, isPresent));
            createNew = CreateOptional.Contains(field.Name);
            _path.Add(field.Name);
        }
        public void EndOptionalStruct(bool isPresent) => _path.RemoveAt(_path.Count - 1);
        public void VisitUnknown(Field field, IFieldType type) => Assert.Fail($"Unexpected field type: {field.Name} ({type.Name}).");
    }
}
