using System;
using Asv.Common;

namespace Asv.Gnss;

/// <summary>
/// [GGA] Global Positioning System Fix Data. Time, Position and fix related data for a GPS receiver
/// https://receiverhelp.trimble.com/alloy-gnss/en-us/NMEA-0183messages_GGA.html
/// https://docs.novatel.com/OEM7/Content/Logs/GPGGA.htm?TocPath=Commands%20%26%20Logs%7CLogs%7CGNSS%20Logs%7C_____59
/// </summary>
public class NmeaMessageGga : NmeaMessageBase
{
    public const string MessageName = "GGA";
    public static readonly NmeaMessageId MessageId = new(MessageName);
    private int? _referenceStationId;
    private double _ageOfDifferentialGpsData;
    private string? _geoidalSeparationUnits;
    private double _geoidalSeparation;
    private string? _antennaAltitudeUnits;
    private double _antennaAltitudeMsl;
    private double _horizontalDilutionPrecision;
    private int? _numberOfSatellites;
    private NmeaGpsQuality? _gpsQuality;
    private TimeSpan? _time;
    private double _latitude;
    private double _longitude;

    public override string Name => MessageName;
    public override NmeaMessageId Id => MessageId;

    protected override void InternalDeserialize(ref ReadOnlySpan<char> buffer)
    {
        ReadTime(ref buffer, out _time);
        ReadLatitude(ref buffer, out _latitude);
        ReadLongitude(ref buffer, out _longitude);
        ReadGpsQuality(ref buffer, out _gpsQuality);
        ReadInt(ref buffer, out _numberOfSatellites);
        ReadDouble(ref buffer, out _horizontalDilutionPrecision);
        ReadDouble(ref buffer, out _antennaAltitudeMsl);
        ReadString(ref buffer, out _antennaAltitudeUnits);
        ReadDouble(ref buffer, out _geoidalSeparation);
        ReadString(ref buffer, out _geoidalSeparationUnits);
        ReadDouble(ref buffer, out _ageOfDifferentialGpsData, false);
        ReadInt(ref buffer, out _referenceStationId,false);
    }

    

    protected override void InternalSerialize(ref Span<byte> buffer)
    {
        WriteTime(ref buffer,in _time);
        WriteLatitude(ref buffer, in _latitude);
        WriteLongitude(ref buffer, in _longitude);
        WriteGpsQuality(ref buffer, _gpsQuality);
        WriteInt(ref buffer, _numberOfSatellites, in NmeaIntFormat.IntD1);
        WriteDouble(ref buffer, _horizontalDilutionPrecision, NmeaDoubleFormat.Double1X3);
        WriteDouble(ref buffer, _antennaAltitudeMsl, NmeaDoubleFormat.Double1X3);
        WriteString(ref buffer, _antennaAltitudeUnits);
        WriteDouble(ref buffer, _geoidalSeparation, NmeaDoubleFormat.Double1X3);
        WriteString(ref buffer, _geoidalSeparationUnits);
        WriteDouble(ref buffer, _ageOfDifferentialGpsData, NmeaDoubleFormat.Double1X3);
        WriteInt(ref buffer, _referenceStationId, in NmeaIntFormat.IntD2);
    }

    
    protected override int InternalGetByteSize()
    {
        return SizeOfTime(in _time)
            + SizeOfLatitude(_latitude)
            + SizeOfLongitude(_longitude)
            + SizeOfGpsQuality(_gpsQuality)
            + SizeOfInt(_numberOfSatellites, in NmeaIntFormat.IntD1)
            + SizeOfDouble(_horizontalDilutionPrecision, NmeaDoubleFormat.Double1X3)
            + SizeOfDouble(_antennaAltitudeMsl, NmeaDoubleFormat.Double1X3)
            + SizeOfString(_antennaAltitudeUnits)
            + SizeOfDouble(_geoidalSeparation, NmeaDoubleFormat.Double1X3)
            + SizeOfString(_geoidalSeparationUnits)
            + SizeOfDouble(_ageOfDifferentialGpsData, NmeaDoubleFormat.Double1X3)
            + SizeOfInt(_referenceStationId, in NmeaIntFormat.IntD2);
    }

   

    public double Latitude
    {
        get => _latitude;
        set => _latitude = value;
    }

    public double Longitude
    {
        get => _longitude;
        set => _longitude = value;
    }

    /// <summary>
    /// Gets or sets the reference station ID.
    /// </summary>
    /// <value>
    /// The reference station ID.
    /// </value>
    public int? ReferenceStationID
    {
        get => _referenceStationId;
        set => _referenceStationId = value;
    }

    /// <summary>
    /// Age of differential GPS data, time in seconds since last SC104
    /// type 1 or 9 update, null field when DGPS is not used
    /// </summary>
    public double AgeOfDifferentialGPSData
    {
        get => _ageOfDifferentialGpsData;
        set => _ageOfDifferentialGpsData = value;
    }

    public string? GeoidalSeparationUnits
    {
        get => _geoidalSeparationUnits;
        set => _geoidalSeparationUnits = value;
    }

    /// <summary>
    ///  Geoidal separation, the difference between the WGS-84 earth
    ///  ellipsoid and mean-sea-level(geoid), "-" means mean-sea-level below ellipsoid
    /// return MSL - Ellipsoid
    /// </summary>
    public double GeoidalSeparation
    {
        get => _geoidalSeparation;
        set => _geoidalSeparation = value;
    }

    /// <summary>
    ///  Units of antenna altitude, meters
    /// </summary>
    public string? AntennaAltitudeUnits
    {
        get => _antennaAltitudeUnits;
        set => _antennaAltitudeUnits = value;
    }

    /// <summary>
    ///  Antenna Altitude above/below mean-sea-level (geoid)
    /// </summary>
    public double AntennaAltitudeMsl
    {
        get => _antennaAltitudeMsl;
        set => _antennaAltitudeMsl = value;
    }

    /// <summary>
    ///  Horizontal Dilution of precision
    /// </summary>
    public double HorizontalDilutionPrecision
    {
        get => _horizontalDilutionPrecision;
        set => _horizontalDilutionPrecision = value;
    }

    /// <summary>
    /// Number of satellites in view, 00 - 12
    /// </summary>
    public int? NumberOfSatellites
    {
        get => _numberOfSatellites;
        set => _numberOfSatellites = value;
    }

    /// <summary>
    /// Gets or sets the GPS quality of the NMEA data.
    /// </summary>
    /// <value>
    /// The GPS quality. This property represents the quality of the GPS signal received, indicating the accuracy of the reported data.
    /// </value>
    public NmeaGpsQuality? GpsQuality
    {
        get => _gpsQuality;
        set => _gpsQuality = value;
    }


    /// <summary>
    /// Time (UTC)
    /// </summary>
    public TimeSpan? Time
    {
        get => _time;
        set => _time = value;
    }
}