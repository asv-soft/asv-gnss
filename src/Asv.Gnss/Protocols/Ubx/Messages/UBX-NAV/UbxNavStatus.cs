using System;
using Asv.IO;

namespace Asv.Gnss;

public class UbxNavStatusPool : UbxMessageBase
{
    public override string Name => "UBX-NAV-STATUS-POOL";
    public override byte Class => 0x01;
    public override byte SubClass => 0x03;

    protected override void SerializeContent(ref Span<byte> buffer)
    {

    }

    protected override void DeserializeContent(ref ReadOnlySpan<byte> buffer)
    {

    }

    protected override int GetContentByteSize() => 0;

    public override void Randomize(Random random)
    {

    }
}

public class UbxNavStatus : UbxMessageBase
{
    public override string Name => "UBX-NAV-STATUS";
    public override byte Class => 0x01;
    public override byte SubClass => 0x03;

    /// <summary>
    /// GPS time of week of the navigation epoch.
    /// </summary>
    public uint iTOW { get; set; }

    /// <summary>
    /// GPSfix Type, this value does not qualify a
    /// fix as valid and within the limits. See note
    /// on flag gpsFixOk below.
    /// 0x00 = no fix
    /// 0x01 = dead reckoning only
    /// 0x02 = 2D-fix
    /// 0x03 = 3D-fix
    /// 0x04 = GPS + dead reckoning combined
    /// 0x05 = Time only fix
    /// 0x06..0xff = reserved
    /// </summary>
    public byte GpsFix { get; set; }

    /// <summary>
    /// Navigation Status Flags (see graphic below)
    /// </summary>
    public byte Flags { get; set; }

    #region Flags bits

    /// <summary>
    /// Position and velocity valid and within DOP and ACC Masks.
    /// </summary>
    public bool IsGpsFixOk { get; set; }

    /// <summary>
    /// Differential corrections were applied.
    /// </summary>
    public bool IsDiffSoln { get; set; }

    /// <summary>
    /// Week Number valid.
    /// </summary>
    public bool IsWknSet { get; set; }

    /// <summary>
    /// Time of Week valid.
    /// </summary>
    public bool IsTowSet { get; set; }

    #endregion

    /// <summary>
    /// Fix Status Information (see graphic below)
    /// </summary>
    public byte FixStat { get; set; }

    #region FixStat bits

    /// <summary>
    /// Differential corrections available.
    /// </summary>
    public bool IsDiffCorr { get; set; }

    /// <summary>
    /// Valid carrSoln.
    /// </summary>
    public bool IsCarrSolnValid { get; set; }

    /// <summary>
    /// Map matching status.
    /// </summary>
    public MapMatchingStatus MapMatching { get; set; } =
        MapMatchingStatus.None;

    #endregion

    /// <summary>
    /// further information about navigation output (see graphic below)
    /// </summary>
    public byte Flags2 { get; set; }

    #region Flags2 bits

    public PowerSaveModeState PsmState { get; set; } =
        PowerSaveModeState.Acquisition;

    public SpoofingDetectionState SpoofDetState { get; set; } =
        SpoofingDetectionState.UnknownOrDeactivated;

    public CPRSolutionStatus CarrSoln { get; set; } =
        CPRSolutionStatus.NoCPRSolution;

    #endregion

    /// <summary>
    /// Time to first fix (millisecond time tag)
    /// </summary>
    public uint TTFF { get; set; }

    /// <summary>
    /// Milliseconds since Startup / Reset
    /// </summary>
    public uint MSSS { get; set; }

    protected override void SerializeContent(ref Span<byte> buffer)
    {

    }

    protected override void DeserializeContent(ref ReadOnlySpan<byte> buffer)
    {
        iTOW = BinSerialize.ReadUInt(ref buffer);
        GpsFix = BinSerialize.ReadByte(ref buffer);
        Flags = BinSerialize.ReadByte(ref buffer);

        IsTowSet = (Flags & 0b00001000) != 0;
        IsWknSet = (Flags & 0b00000100) != 0;
        IsDiffSoln = (Flags & 0b00000010) != 0;
        IsGpsFixOk = (Flags & 0b00000001) != 0;

        FixStat = BinSerialize.ReadByte(ref buffer);

        IsCarrSolnValid = (FixStat & 0b0000_0010) != 0;

        IsDiffCorr = (FixStat & 0b0000_0001) != 0;

        if ((FixStat & 0b1100_0000) != 0)
        {
            MapMatching = MapMatchingStatus.ValidAndUsedSensor;
        }
        else if ((FixStat & 0b1000_0000) != 0)
        {
            MapMatching = MapMatchingStatus.ValidAndUsed;
        }
        else if ((FixStat & 0b0100_0000) != 0)
        {
            MapMatching = MapMatchingStatus.ValidButNotUsed;
        }

        IsTowSet = (Flags & 0b00001000) != 0;
        IsWknSet = (Flags & 0b00000100) != 0;
        IsDiffSoln = (Flags & 0b00000010) != 0;
        IsGpsFixOk = (Flags & 0b00000001) != 0;

        Flags2 = BinSerialize.ReadByte(ref buffer);

        if ((Flags2 & 0b0100_0000) != 0)
        {
            CarrSoln = CPRSolutionStatus.CPRWithFloatingAmbiguities;
        }
        else if ((Flags2 & 0b1000_0000) != 0)
        {
            CarrSoln = CPRSolutionStatus.CPRWithFixedAmbiguities;
        }

        if ((Flags2 & 0b0001_1000) != 0)
        {
            SpoofDetState = SpoofingDetectionState.MultipleSpoofingIndications;
        }
        else if ((Flags2 & 0b0000_1000) != 0)
        {
            SpoofDetState = SpoofingDetectionState.NoSpoofingIndicated;
        }
        else if ((Flags2 & 0b0001_0000) != 0)
        {
            SpoofDetState = SpoofingDetectionState.SpoofingIndicated;
        }

        if ((Flags2 & 0b0000_0011) != 0)
        {
            PsmState = PowerSaveModeState.Inactive;
        }
        else if ((Flags2 & 0b0000_0001) != 0)
        {
            PsmState = PowerSaveModeState.Tracking;
        }
        else if ((Flags2 & 0b0000_0010) != 0)
        {
            PsmState = PowerSaveModeState.PowerOptimizedTracking;
        }

        TTFF = BinSerialize.ReadUInt(ref buffer);
        MSSS = BinSerialize.ReadUInt(ref buffer);
    }

    protected override int GetContentByteSize() => 16;

    public override void Randomize(Random random)
    {

    }
}

/// <summary>
/// Carrier phase range solution status.
/// </summary>
public enum CPRSolutionStatus
{
    NoCPRSolution = 0,
    CPRWithFloatingAmbiguities = 1,
    CPRWithFixedAmbiguities = 2
}

/// <summary>
/// Note that the spoofing state value only reflects the detector state for the current navigation epoch.
/// As spoofing can be detected most easily at the transition from real signal to spoofing signal, this is
/// also where the detector is triggered the most. I.e. a value of 1 - No spoofing indicated does not mean
/// that the receiver is not spoofed, it simply states that the detector was not triggered in this epoch.
/// </summary>
public enum SpoofingDetectionState
{
    UnknownOrDeactivated = 0,
    NoSpoofingIndicated = 1,
    SpoofingIndicated = 2,
    MultipleSpoofingIndications = 3
}

public enum PowerSaveModeState
{
    Acquisition = 0,
    Tracking = 1,
    PowerOptimizedTracking = 2,
    Inactive = 3
}

public enum MapMatchingStatus
{
    /// <summary>
    /// None.
    /// </summary>
    None = 0,

    /// <summary>
    /// Valid but not used, i.e. map matching data was received, but was too old.
    /// </summary>
    ValidButNotUsed = 1,

    /// <summary>
    /// Valid and used, map matching data was applied.
    /// </summary>
    ValidAndUsed = 2,

    /// <summary>
    /// Valid and used, map matching data was applied. In case of sensor unavailability map matching
    /// data enables dead reckoning. This requires map matched latitude/longitude or heading data.
    /// </summary>
    ValidAndUsedSensor = 3,
}
    