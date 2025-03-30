using System;

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

public readonly struct NmeaTalkerId
{
    public const string Ag = "AG";
    public const string Ap = "AP";
    public const string Cd = "CD";
    public const string Cr = "CR";
    public const string Cs = "CS";
    public const string Ct = "CT";
    public const string Cv = "CV";
    public const string Cx = "CX";
    public const string Df = "DF";
    public const string Ec = "EC";
    public const string Ep = "EP";
    public const string Er = "ER";
    public const string Gp = "GP";
    public const string Hc = "HC";
    public const string He = "HE";
    public const string Hn = "HN";
    public const string Ii = "II";
    public const string In = "IN";
    public const string Lc = "LC";
    public const string P = "P";
    public const string Ra = "RA";
    public const string Sd = "SD";
    public const string Sn = "SN";
    public const string Ss = "SS";
    public const string Ti = "TI";
    public const string Vd = "VD";
    public const string Dm = "DM";
    public const string Vw = "VW";
    public const string Wi = "WI";
    public const string Yx = "YX";
    public const string Za = "ZA";
    public const string Zc = "ZC";
    public const string Zq = "ZQ";
    public const string Zv = "ZV";

    public NmeaTalkerId(string msgId)
        : this(msgId.AsSpan())
    {
    }

    public NmeaTalkerId(ReadOnlySpan<char> msgId)
    {
        if (msgId.IsEmpty)
        {
            throw new ArgumentException("MessageId is empty", nameof(msgId));
        }

        if (msgId[0] == NmeaProtocol.ProprietaryPrefix)
        {
            Type = NmeaTalkerClass.ProprietaryCode;
            Id = P;
        }
        else
        {
            if (msgId.Length < 2)
            {
                throw new ArgumentException("MessageId is too short", nameof(msgId));
            }

            var talkerId = msgId[0..2];
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
                default:
                    Id = new string(talkerId);
                    Type = NmeaTalkerClass.Unknown;
                    break;
            }
        }
    }

    public string Id { get; }
    public NmeaTalkerClass Type { get; }

    public override string ToString()
    {
        return Id;
    }
}