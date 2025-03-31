using System;
using System.Buffers;
using System.Diagnostics;
using Asv.IO;

namespace Asv.Gnss;
/// <summary>
/// Identifiers for communication and navigation systems.
/// </summary>
public enum NmeaTalkerClass
{
    /// <summary>
    /// Unknown identifier.
    /// </summary>
    Unknown,

    /// <summary>
    /// Autopilot - General
    /// </summary>
    AutopilotGeneral,

    /// <summary>
    /// Autopilot - Magnetic Compass
    /// </summary>
    AutopilotMagnetic,

    /// <summary>
    /// Communications - Digital Selective Calling (DSC)
    /// </summary>
    CommunicationsDigitalSelectiveCalling,

    /// <summary>
    /// Communications - Receiver or Beacon Receiver
    /// </summary>
    CommunicationsReceiver,

    /// <summary>
    /// Communications - Satellite
    /// </summary>
    CommunicationsSatellite,

    /// <summary>
    /// Communications - Radio Telephone (MF/HF)
    /// </summary>
    CommunicationsRadioTelephoneMfhf,

    /// <summary>
    /// Communications - Radio Telephone (VHF)
    /// </summary>
    CommunicationsRadioTelephoneVhf,

    /// <summary>
    /// Communications - Scanning Receiver
    /// </summary>
    CommunicationsScanningReceiver,

    /// <summary>
    /// Direction Finder
    /// </summary>
    DirectionFinder,

    /// <summary>
    /// Electronic Chart Display & Information System (ECDIS)
    /// </summary>
    ElectronicChartDisplayInformationSystem,

    /// <summary>
    /// Emergency Position Indicating Radio Beacon (EPIRB)
    /// </summary>
    EmergencyPositionIndicatingBeacon,

    /// <summary>
    /// Engine Room Monitoring Systems
    /// </summary>
    EngineRoomMonitoring,

    /// <summary>
    /// Global Positioning System (GPS)
    /// </summary>
    GlobalPositioningSystem,

    /// <summary>
    /// Heading - Magnetic Compass
    /// </summary>
    HeadingMagneticCompass,

    /// <summary>
    /// Heading - North Seeking Gyro
    /// </summary>
    HeadingNorthSeekingGyro,

    /// <summary>
    /// Heading - Non North Seeking Gyro
    /// </summary>
    HeadingNonNorthSeekingGyro,

    /// <summary>
    /// Integrated Instrumentation
    /// </summary>
    IntegratedInstrumentation,

    /// <summary>
    /// Integrated Navigation
    /// </summary>
    IntegratedNavigation,

    /// <summary>
    /// Loran C Navigation System
    /// </summary>
    LoranC,

    /// <summary>
    /// Proprietary Code
    /// </summary>
    ProprietaryCode,

    /// <summary>
    /// Radar or ARPA
    /// </summary>
    RadarArpa,

    /// <summary>
    /// Depth Sounder
    /// </summary>
    DepthSounder,

    /// <summary>
    /// Electronic Positioning System (Other or General)
    /// </summary>
    ElectronicPositioningSystemGeneral,

    /// <summary>
    /// Scanning Sounder
    /// </summary>
    ScanningSounder,

    /// <summary>
    /// Turn Rate Indicator
    /// </summary>
    TurnRateIndicator,

    /// <summary>
    /// Doppler Velocity Sensor (General)
    /// </summary>
    DopplerVelocitySensor,

    /// <summary>
    /// Speed Log (Water, Magnetic)
    /// </summary>
    SpeedLogWaterMagnetic,

    /// <summary>
    /// Speed Log (Water, Mechanical)
    /// </summary>
    SpeedLogWaterMechanical,

    /// <summary>
    /// Weather Instruments
    /// </summary>
    WeatherInstruments,

    /// <summary>
    /// Transducer
    /// </summary>
    Transducer,

    /// <summary>
    /// Timekeeper - Atomic Clock
    /// </summary>
    TimekeeperAtomicClock,

    /// <summary>
    /// Timekeeper - Chronometer
    /// </summary>
    TimekeeperChronometer,

    /// <summary>
    /// Timekeeper - Quartz
    /// </summary>
    TimekeeperQuartz,

    /// <summary>
    /// Timekeeper - Radio Update (WWV or WWVH)
    /// </summary>
    TimekeeperRadioUpdate
}


public readonly struct NmeaTalkerId : ISizedSpanSerializable, IEquatable<NmeaTalkerId>
{
    private const string Ag = "AG";
    private const string Ap = "AP";
    private const string Cd = "CD";
    private const string Cr = "CR";
    private const string Cs = "CS";
    private const string Ct = "CT";
    private const string Cv = "CV";
    private const string Cx = "CX";
    private const string Df = "DF";
    private const string Ec = "EC";
    private const string Ep = "EP";
    private const string Er = "ER";
    private const string Gp = "GP";
    private const string Hc = "HC";
    private const string He = "HE";
    private const string Hn = "HN";
    private const string Ii = "II";
    private const string In = "IN";
    private const string Lc = "LC";
    private const string P = "P";
    private const string Ra = "RA";
    private const string Sd = "SD";
    private const string Sn = "SN";
    private const string Ss = "SS";
    private const string Ti = "TI";
    private const string Vd = "VD";
    private const string Dm = "DM";
    private const string Vw = "VW";
    private const string Wi = "WI";
    private const string Yx = "YX";
    private const string Za = "ZA";
    private const string Zc = "ZC";
    private const string Zq = "ZQ";
    private const string Zv = "ZV";



    public NmeaTalkerId(NmeaTalkerClass type)
    {
        Type = type;

        Id = type switch
        {
            NmeaTalkerClass.AutopilotGeneral => Ag,
            NmeaTalkerClass.AutopilotMagnetic => Ap,
            NmeaTalkerClass.CommunicationsDigitalSelectiveCalling => Cd,
            NmeaTalkerClass.CommunicationsReceiver => Cr,
            NmeaTalkerClass.CommunicationsSatellite => Cs,
            NmeaTalkerClass.CommunicationsRadioTelephoneMfhf => Ct,
            NmeaTalkerClass.CommunicationsRadioTelephoneVhf => Cv,
            NmeaTalkerClass.CommunicationsScanningReceiver => Cx,
            NmeaTalkerClass.DirectionFinder => Df,
            NmeaTalkerClass.ElectronicChartDisplayInformationSystem => Ec,
            NmeaTalkerClass.EmergencyPositionIndicatingBeacon => Ep,
            NmeaTalkerClass.EngineRoomMonitoring => Er,
            NmeaTalkerClass.GlobalPositioningSystem => Gp,
            NmeaTalkerClass.HeadingMagneticCompass => Hc,
            NmeaTalkerClass.HeadingNorthSeekingGyro => He,
            NmeaTalkerClass.HeadingNonNorthSeekingGyro => Hn,
            NmeaTalkerClass.IntegratedInstrumentation => Ii,
            NmeaTalkerClass.IntegratedNavigation => In,
            NmeaTalkerClass.LoranC => Lc,
            NmeaTalkerClass.ProprietaryCode => P,
            NmeaTalkerClass.RadarArpa => Ra,
            NmeaTalkerClass.DepthSounder => Sd,
            NmeaTalkerClass.ElectronicPositioningSystemGeneral => Sn,
            NmeaTalkerClass.ScanningSounder => Ss,
            NmeaTalkerClass.TurnRateIndicator => Ti,
            NmeaTalkerClass.DopplerVelocitySensor => Vd,
            NmeaTalkerClass.SpeedLogWaterMagnetic => Dm,
            NmeaTalkerClass.SpeedLogWaterMechanical => Vw,
            NmeaTalkerClass.WeatherInstruments => Wi,
            NmeaTalkerClass.Transducer => Yx,
            NmeaTalkerClass.TimekeeperAtomicClock => Za,
            NmeaTalkerClass.TimekeeperChronometer => Zc,
            NmeaTalkerClass.TimekeeperQuartz => Zq,
            NmeaTalkerClass.TimekeeperRadioUpdate => Zv,
            NmeaTalkerClass.Unknown => throw new ArgumentOutOfRangeException(nameof(type), type,
                "Unknown talker type is not supported"),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Invalid talker type")
        };
    }

    public NmeaTalkerId(string talkerId)
        : this(talkerId.AsSpan())
    {
        
    }
    
    public NmeaTalkerId(ReadOnlySpan<char> talkerId)
    {
        if (talkerId.IsEmpty)
        {
            throw new ArgumentException("MessageId is empty", nameof(talkerId));
        }
        Id = string.Empty;
        if (talkerId[0] == NmeaProtocol.ProprietaryPrefix)
        {
            Id = P;
            Type = NmeaTalkerClass.ProprietaryCode;
            return;
        }
        
        if (talkerId.Length < 2)
        {
            throw new ArgumentException($"Message must be '{P}' for proprietary or two characters long", nameof(talkerId));
        }

        talkerId = talkerId[..2];
        switch (talkerId)
        {
            case Ag:
                Id = Ag;
                Type = NmeaTalkerClass.AutopilotGeneral;
                break;
            case Ap:
                Id = Ap;
                Type = NmeaTalkerClass.AutopilotMagnetic;
                break;
            case Cd:
                Id = Cd;
                Type = NmeaTalkerClass.CommunicationsDigitalSelectiveCalling;
                break;
            case Cr:
                Id = Cr;
                Type = NmeaTalkerClass.CommunicationsReceiver;
                break;
            case Cs:
                Id = Cs;
                Type = NmeaTalkerClass.CommunicationsSatellite;
                break;
            case Ct:
                Id = Ct;
                Type = NmeaTalkerClass.CommunicationsRadioTelephoneMfhf;
                break;
            case Cv:
                Id = Cv;
                Type = NmeaTalkerClass.CommunicationsRadioTelephoneVhf;
                break;
            case Cx:
                Id = Cx;
                Type = NmeaTalkerClass.CommunicationsScanningReceiver;
                break;
            case Df:
                Id = Df;
                Type = NmeaTalkerClass.DirectionFinder;
                break;
            case Ec:
                Id = Ec;
                Type = NmeaTalkerClass.ElectronicChartDisplayInformationSystem;
                break;
            case Ep:
                Id = Ep;
                Type = NmeaTalkerClass.EmergencyPositionIndicatingBeacon;
                break;
            case Er:
                Id = Er;
                Type = NmeaTalkerClass.EngineRoomMonitoring;
                break;
            case Gp:
                Id = Gp;
                Type = NmeaTalkerClass.GlobalPositioningSystem;
                break;
            case Hc:
                Id = Hc;
                Type = NmeaTalkerClass.HeadingMagneticCompass;
                break;
            case He:
                Id = He;
                Type = NmeaTalkerClass.HeadingNorthSeekingGyro;
                break;
            case Hn:
                Id = Hn;
                Type = NmeaTalkerClass.HeadingNonNorthSeekingGyro;
                break;
            case Ii:
                Id = Ii;
                Type = NmeaTalkerClass.IntegratedInstrumentation;
                break;
            case In:
                Id = In;
                Type = NmeaTalkerClass.IntegratedNavigation;
                break;
            case Lc:
                Id = Lc;
                Type = NmeaTalkerClass.LoranC;
                break;
            case Ra:
                Id = Ra;
                Type = NmeaTalkerClass.RadarArpa;
                break;
            case Sd:
                Id = Sd;
                Type = NmeaTalkerClass.DepthSounder;
                break;
            case Sn:
                Id = Sn;
                Type = NmeaTalkerClass.ElectronicPositioningSystemGeneral;
                break;
            case Ss:
                Id = Ss;
                Type = NmeaTalkerClass.ScanningSounder;
                break;
            case Ti:
                Id = Ti;
                Type = NmeaTalkerClass.TurnRateIndicator;
                break;
            case Vd:
                Id = Vd;
                Type = NmeaTalkerClass.DopplerVelocitySensor;
                break;
            case Dm:
                Id = Dm;
                Type = NmeaTalkerClass.SpeedLogWaterMagnetic;
                break;
            case Vw:
                Id = Vw;
                Type = NmeaTalkerClass.SpeedLogWaterMechanical;
                break;
            case Wi:
                Id = Wi;
                Type = NmeaTalkerClass.WeatherInstruments;
                break;
            case Yx:
                Id = Yx;
                Type = NmeaTalkerClass.Transducer;
                break;
            case Za:
                Id = Za;
                Type = NmeaTalkerClass.TimekeeperAtomicClock;
                break;
            case Zc:
                Id = Zc;
                Type = NmeaTalkerClass.TimekeeperChronometer;
                break;
            case Zq:
                Id = Zq;
                Type = NmeaTalkerClass.TimekeeperQuartz;
                break;
            case Zv:
                Id = Zv;
                Type = NmeaTalkerClass.TimekeeperRadioUpdate;
                break;
            case P:
                Debug.Assert(false, "We should not be here");
                break;
            default:
                Id = new string(talkerId);
                Type = NmeaTalkerClass.Unknown;
                break;
        }
    }

    public string Id { get; } = string.Empty;
    public NmeaTalkerClass Type { get; } = NmeaTalkerClass.Unknown;

    public override string ToString()
    {
        return Id;
    }

    public void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        throw new NotImplementedException($"We don't need this method for {nameof(NmeaTalkerId)} cause it's readonly struct");
    }

    public void Serialize(ref Span<byte> buffer)
    {
        var slice = NmeaProtocol.Encoding.GetBytes(Id, buffer);
        buffer = buffer[slice..];
    }

    public int GetByteSize()
    {
        return NmeaProtocol.Encoding.GetByteCount(Id);
    }

    public bool Equals(NmeaTalkerId other)
    {
        return string.Equals(Id, other.Id, StringComparison.InvariantCultureIgnoreCase);
    }

    public override bool Equals(object? obj)
    {
        return obj is NmeaTalkerId other && Equals(other);
    }

    public override int GetHashCode()
    {
        return StringComparer.InvariantCultureIgnoreCase.GetHashCode(Id);
    }

    public static bool operator ==(NmeaTalkerId left, NmeaTalkerId right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(NmeaTalkerId left, NmeaTalkerId right)
    {
        return !left.Equals(right);
    }
}