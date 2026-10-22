using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class NmeaMessageGga
{
    private static readonly Field NrLatitudeField = NmeaRinexVisitor.Field(nameof(Latitude), DoubleType.Default);
    private static readonly Field NrLongitudeField = NmeaRinexVisitor.Field(nameof(Longitude), DoubleType.Default);
    private static readonly Field NrReferenceStationIDField = NmeaRinexVisitor.Field(nameof(ReferenceStationID), Int32OptionalType.Default);
    private static readonly Field NrAgeOfDifferentialGPSDataField = NmeaRinexVisitor.Field(nameof(AgeOfDifferentialGPSData), DoubleType.Default);
    private static readonly Field NrGeoidalSeparationUnitsField = NmeaRinexVisitor.Field(nameof(GeoidalSeparationUnits), StringOptionalType.Utf8);
    private static readonly Field NrGeoidalSeparationField = NmeaRinexVisitor.Field(nameof(GeoidalSeparation), DoubleType.Default);
    private static readonly Field NrAntennaAltitudeUnitsField = NmeaRinexVisitor.Field(nameof(AntennaAltitudeUnits), StringOptionalType.Utf8);
    private static readonly Field NrAntennaAltitudeMslField = NmeaRinexVisitor.Field(nameof(AntennaAltitudeMsl), DoubleType.Default);
    private static readonly Field NrHorizontalDilutionPrecisionField = NmeaRinexVisitor.Field(nameof(HorizontalDilutionPrecision), DoubleType.Default);
    private static readonly Field NrNumberOfSatellitesField = NmeaRinexVisitor.Field(nameof(NumberOfSatellites), Int32OptionalType.Default);
    private static readonly Field NrGpsQualityField = NmeaRinexVisitor.Field(nameof(GpsQuality), Int32OptionalType.Default, sourceType: typeof(NmeaGpsQuality));
    private static readonly Field NrTimeField = NmeaRinexVisitor.Field(nameof(Time), TimeOnlyOptionalType.Default);

    internal new static readonly StructType NrVisitorType = new([..NmeaMessageBase.NrVisitorType.Fields, NrLatitudeField, NrLongitudeField, NrReferenceStationIDField, NrAgeOfDifferentialGPSDataField, NrGeoidalSeparationUnitsField, NrGeoidalSeparationField, NrAntennaAltitudeUnitsField, NrAntennaAltitudeMslField, NrHorizontalDilutionPrecisionField, NrNumberOfSatellitesField, NrGpsQualityField, NrTimeField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        base.Accept(visitor);

        var valueLatitude = Latitude;
        DoubleType.Accept(visitor, NrLatitudeField, NrLatitudeField.DataType, ref valueLatitude);
        if (!EqualityComparer<double>.Default.Equals(valueLatitude, Latitude))
        {
            Latitude = valueLatitude;
        }

        var valueLongitude = Longitude;
        DoubleType.Accept(visitor, NrLongitudeField, NrLongitudeField.DataType, ref valueLongitude);
        if (!EqualityComparer<double>.Default.Equals(valueLongitude, Longitude))
        {
            Longitude = valueLongitude;
        }

        var valueReferenceStationID = ReferenceStationID;
        Int32OptionalType.Accept(visitor, NrReferenceStationIDField, NrReferenceStationIDField.DataType, ref valueReferenceStationID);
        if (!EqualityComparer<int?>.Default.Equals(valueReferenceStationID, ReferenceStationID))
        {
            ReferenceStationID = valueReferenceStationID;
        }

        var valueAgeOfDifferentialGPSData = AgeOfDifferentialGPSData;
        DoubleType.Accept(visitor, NrAgeOfDifferentialGPSDataField, NrAgeOfDifferentialGPSDataField.DataType, ref valueAgeOfDifferentialGPSData);
        if (!EqualityComparer<double>.Default.Equals(valueAgeOfDifferentialGPSData, AgeOfDifferentialGPSData))
        {
            AgeOfDifferentialGPSData = valueAgeOfDifferentialGPSData;
        }

        string? valueGeoidalSeparationUnits = GeoidalSeparationUnits;
        StringOptionalType.Accept(visitor, NrGeoidalSeparationUnitsField, NrGeoidalSeparationUnitsField.DataType, ref valueGeoidalSeparationUnits);
        if (!EqualityComparer<string?>.Default.Equals(valueGeoidalSeparationUnits, GeoidalSeparationUnits))
        {
            GeoidalSeparationUnits = valueGeoidalSeparationUnits;
        }

        var valueGeoidalSeparation = GeoidalSeparation;
        DoubleType.Accept(visitor, NrGeoidalSeparationField, NrGeoidalSeparationField.DataType, ref valueGeoidalSeparation);
        if (!EqualityComparer<double>.Default.Equals(valueGeoidalSeparation, GeoidalSeparation))
        {
            GeoidalSeparation = valueGeoidalSeparation;
        }

        string? valueAntennaAltitudeUnits = AntennaAltitudeUnits;
        StringOptionalType.Accept(visitor, NrAntennaAltitudeUnitsField, NrAntennaAltitudeUnitsField.DataType, ref valueAntennaAltitudeUnits);
        if (!EqualityComparer<string?>.Default.Equals(valueAntennaAltitudeUnits, AntennaAltitudeUnits))
        {
            AntennaAltitudeUnits = valueAntennaAltitudeUnits;
        }

        var valueAntennaAltitudeMsl = AntennaAltitudeMsl;
        DoubleType.Accept(visitor, NrAntennaAltitudeMslField, NrAntennaAltitudeMslField.DataType, ref valueAntennaAltitudeMsl);
        if (!EqualityComparer<double>.Default.Equals(valueAntennaAltitudeMsl, AntennaAltitudeMsl))
        {
            AntennaAltitudeMsl = valueAntennaAltitudeMsl;
        }

        var valueHorizontalDilutionPrecision = HorizontalDilutionPrecision;
        DoubleType.Accept(visitor, NrHorizontalDilutionPrecisionField, NrHorizontalDilutionPrecisionField.DataType, ref valueHorizontalDilutionPrecision);
        if (!EqualityComparer<double>.Default.Equals(valueHorizontalDilutionPrecision, HorizontalDilutionPrecision))
        {
            HorizontalDilutionPrecision = valueHorizontalDilutionPrecision;
        }

        var valueNumberOfSatellites = NumberOfSatellites;
        Int32OptionalType.Accept(visitor, NrNumberOfSatellitesField, NrNumberOfSatellitesField.DataType, ref valueNumberOfSatellites);
        if (!EqualityComparer<int?>.Default.Equals(valueNumberOfSatellites, NumberOfSatellites))
        {
            NumberOfSatellites = valueNumberOfSatellites;
        }

        var valueGpsQuality = (int?)GpsQuality;
        Int32OptionalType.Accept(visitor, NrGpsQualityField, NrGpsQualityField.DataType, ref valueGpsQuality);
        if (!EqualityComparer<int?>.Default.Equals(valueGpsQuality, (int?)GpsQuality))
        {
            GpsQuality = (NmeaGpsQuality?)valueGpsQuality;
        }

        var valueTime = Time;
        TimeOnlyOptionalType.Accept(visitor, NrTimeField, NrTimeField.DataType, ref valueTime);
        if (!EqualityComparer<TimeOnly?>.Default.Equals(valueTime, Time))
        {
            Time = valueTime;
        }
    }
}
