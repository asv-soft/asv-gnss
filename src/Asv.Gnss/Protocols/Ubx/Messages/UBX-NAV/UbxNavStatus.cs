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

/// <remarks>Payload fields follow u-blox document UBX-13003221 R23, section 32.17.24. Receiver and firmware support must be checked against the applicable interface description.</remarks>
public partial class UbxNavStatus : UbxMessageBase
{
    private uint _iTOW;
    private byte _gpsFix;
    private byte _flags;
    private bool _isGpsFixOk;
    private bool _isDiffSoln;
    private bool _isWknSet;
    private bool _isTowSet;
    private byte _fixStat;
    private bool _isDiffCorr;
    private bool _isCarrSolnValid;
    private MapMatchingStatus _mapMatching = MapMatchingStatus.None;
    private byte _flags2;
    private PowerSaveModeState _psmState = PowerSaveModeState.Acquisition;
    private SpoofingDetectionState _spoofDetState = SpoofingDetectionState.UnknownOrDeactivated;
    private CPRSolutionStatus _carrSoln = CPRSolutionStatus.NoCPRSolution;
    private uint _ttff;
    private uint _msss;

    public override string Name => "UBX-NAV-STATUS";
    public override byte Class => 0x01;
    public override byte SubClass => 0x03;

    /// <summary>
    /// GPS time of week of the navigation epoch.
    /// </summary>
    public uint iTOW
    {
        get => _iTOW;
        set => _iTOW = value;
    }

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
    public byte GpsFix
    {
        get => _gpsFix;
        set => _gpsFix = value;
    }

    /// <summary>
    /// Navigation Status Flags (see graphic below)
    /// </summary>
    public byte Flags
    {
        get => _flags;
        set => _flags = value;
    }

    #region Flags bits

    /// <summary>
    /// Position and velocity valid and within DOP and ACC Masks.
    /// </summary>
    public bool IsGpsFixOk
    {
        get => _isGpsFixOk;
        set => _isGpsFixOk = value;
    }

    /// <summary>
    /// Differential corrections were applied.
    /// </summary>
    public bool IsDiffSoln
    {
        get => _isDiffSoln;
        set => _isDiffSoln = value;
    }

    /// <summary>
    /// Week Number valid.
    /// </summary>
    public bool IsWknSet
    {
        get => _isWknSet;
        set => _isWknSet = value;
    }

    /// <summary>
    /// Time of Week valid.
    /// </summary>
    public bool IsTowSet
    {
        get => _isTowSet;
        set => _isTowSet = value;
    }

    #endregion

    /// <summary>
    /// Fix Status Information (see graphic below)
    /// </summary>
    public byte FixStat
    {
        get => _fixStat;
        set => _fixStat = value;
    }

    #region FixStat bits

    /// <summary>
    /// Differential corrections available.
    /// </summary>
    public bool IsDiffCorr
    {
        get => _isDiffCorr;
        set => _isDiffCorr = value;
    }

    /// <summary>
    /// Valid carrSoln.
    /// </summary>
    public bool IsCarrSolnValid
    {
        get => _isCarrSolnValid;
        set => _isCarrSolnValid = value;
    }

    /// <summary>
    /// Map matching status.
    /// </summary>
    public MapMatchingStatus MapMatching
    {
        get => _mapMatching;
        set => _mapMatching = value;
    }

    #endregion

    /// <summary>
    /// further information about navigation output (see graphic below)
    /// </summary>
    public byte Flags2
    {
        get => _flags2;
        set => _flags2 = value;
    }

    #region Flags2 bits

    /// <summary>
    /// Power-save operating state decoded from flags2 bits 0 and 1.
    /// </summary>
    public PowerSaveModeState PsmState
    {
        get => _psmState;
        set => _psmState = value;
    }

    /// <summary>
    /// Spoofing detector status decoded from flags2 bits 3 and 4; an unknown state does not establish absence of spoofing.
    /// </summary>
    public SpoofingDetectionState SpoofDetState
    {
        get => _spoofDetState;
        set => _spoofDetState = value;
    }

    /// <summary>
    /// Carrier-phase solution state decoded from flags2 bits 6 and 7; interpret together with the carrier-solution validity flag.
    /// </summary>
    public CPRSolutionStatus CarrSoln
    {
        get => _carrSoln;
        set => _carrSoln = value;
    }

    #endregion

    /// <summary>
    /// Time to first fix (millisecond time tag)
    /// </summary>
    public uint TTFF
    {
        get => _ttff;
        set => _ttff = value;
    }

    /// <summary>
    /// Milliseconds since Startup / Reset
    /// </summary>
    public uint MSSS
    {
        get => _msss;
        set => _msss = value;
    }

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

        MapMatching = (MapMatchingStatus)((FixStat >> 6) & 0b11);

        IsTowSet = (Flags & 0b00001000) != 0;
        IsWknSet = (Flags & 0b00000100) != 0;
        IsDiffSoln = (Flags & 0b00000010) != 0;
        IsGpsFixOk = (Flags & 0b00000001) != 0;

        Flags2 = BinSerialize.ReadByte(ref buffer);

        CarrSoln = (CPRSolutionStatus)((Flags2 >> 6) & 0b11);
        SpoofDetState = (SpoofingDetectionState)((Flags2 >> 3) & 0b11);
        PsmState = (PowerSaveModeState)(Flags2 & 0b11);

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
