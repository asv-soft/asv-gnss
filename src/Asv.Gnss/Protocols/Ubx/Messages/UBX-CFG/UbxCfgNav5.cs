using System;
using Asv.IO;

namespace Asv.Gnss;

public class UbxCfgNav5Pool : UbxMessageBase
{
    public override string Name => "UBX-CFG-NAV5-POOL";
    public override byte Class => 0x06;
    public override byte SubClass => 0x24;

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

/// <summary>
/// Navigation engine settings
/// Supported on:
/// • u-blox 8 / u-blox M8 protocol versions 15, 15.01, 16, 17, 18, 19, 19.1, 19.2, 20, 20.01,
/// 20.1, 20.2, 20.3, 22, 22.01, 23 and 23.01
/// See the Navigation Configuration Settings Description for a detailed description
/// of how these settings affect receiver operation.
/// </summary>
/// <remarks>Payload fields follow u-blox document UBX-13003221 R23, section 32.10.19. Receiver and firmware support must be checked against the applicable interface description.</remarks>
/// <remarks>ZED-F9P integration manual UBX-18010802 R08, section 3.1.6, describes limited legacy UBX-CFG compatibility and recommends the configuration-item interface.</remarks>
public partial class UbxCfgNav5 : UbxMessageBase
{
    public override string Name => "UBX-CFG-NAV5";
    public override byte Class => 0x06;
    public override byte SubClass => 0x24;

    private bool _applyDynamicModel = true;

    /// <summary>
    /// Whether the NAV5 update mask applies the platform model.
    /// </summary>
    public bool ApplyDynamicModel
    {
        get => _applyDynamicModel;
        set => _applyDynamicModel = value;
    }
    private bool _applyMinimumElevation = true;

    /// <summary>
    /// Whether the NAV5 update mask applies the minimum elevation.
    /// </summary>
    public bool ApplyMinimumElevation
    {
        get => _applyMinimumElevation;
        set => _applyMinimumElevation = value;
    }
    private bool _applyFixMode = true;

    /// <summary>
    /// Whether the NAV5 update mask applies the 2D/3D fix policy.
    /// </summary>
    public bool ApplyFixMode
    {
        get => _applyFixMode;
        set => _applyFixMode = value;
    }
    private bool _applyDrLimit = true;

    /// <summary>
    /// Legacy update-mask bit for the dead-reckoning limit; reserved in the R23 NAV5 mask definition.
    /// </summary>
    public bool ApplyDrLimit
    {
        get => _applyDrLimit;
        set => _applyDrLimit = value;
    }
    private bool _applyPositionMask = true;

    /// <summary>
    /// Whether the NAV5 update mask applies the position DOP and accuracy masks.
    /// </summary>
    public bool ApplyPositionMask
    {
        get => _applyPositionMask;
        set => _applyPositionMask = value;
    }
    private bool _applyTimeMask = true;

    /// <summary>
    /// Whether the NAV5 update mask applies the time DOP and accuracy masks.
    /// </summary>
    public bool ApplyTimeMask
    {
        get => _applyTimeMask;
        set => _applyTimeMask = value;
    }
    private bool _applyStaticHold = true;

    /// <summary>
    /// Whether the NAV5 update mask applies the static-hold settings.
    /// </summary>
    public bool ApplyStaticHold
    {
        get => _applyStaticHold;
        set => _applyStaticHold = value;
    }
    private bool _applyDGPS = true;

    /// <summary>
    /// Whether the NAV5 update mask applies the differential correction timeout.
    /// </summary>
    public bool ApplyDGPS
    {
        get => _applyDGPS;
        set => _applyDGPS = value;
    }
    private bool _applyCnoThreshold = true;

    /// <summary>
    /// Whether the NAV5 update mask applies the carrier-to-noise thresholds.
    /// </summary>
    public bool ApplyCnoThreshold
    {
        get => _applyCnoThreshold;
        set => _applyCnoThreshold = value;
    }
    private bool _applyUTC = true;

    /// <summary>
    /// Whether the NAV5 update mask applies the UTC standard selector.
    /// </summary>
    public bool ApplyUTC
    {
        get => _applyUTC;
        set => _applyUTC = value;
    }

    private ModelEnum _platformModel = ModelEnum.Stationary;

    /// <summary>
    /// Dynamic platform model used by the navigation engine; supported models depend on firmware.
    /// </summary>
    public ModelEnum PlatformModel
    {
        get => _platformModel;
        set => _platformModel = value;
    }
    private PositionModeEnum _positionMode = PositionModeEnum.Auto;

    /// <summary>
    /// Position solution policy: 2D only, 3D only, or automatic 2D/3D.
    /// </summary>
    public PositionModeEnum PositionMode
    {
        get => _positionMode;
        set => _positionMode = value;
    }

    private double _fixedAltitude;

    /// <summary>
    /// Altitude constraint for 2D fixes in meters above mean sea level; wire resolution is 0.01 m.
    /// </summary>
    public double FixedAltitude
    {
        get => _fixedAltitude;
        set => _fixedAltitude = value;
    }
    private double _fixedAltitudeVariance = 1.0;

    /// <summary>
    /// Variance of the fixed-altitude constraint, in square meters; wire resolution is 0.0001 square meters.
    /// </summary>
    public double FixedAltitudeVariance
    {
        get => _fixedAltitudeVariance;
        set => _fixedAltitudeVariance = value;
    }
    private byte _drLimit;

    /// <summary>
    /// Legacy dead-reckoning limit byte; reserved in the R23 NAV5 field table.
    /// </summary>
    public byte DrLimit
    {
        get => _drLimit;
        set => _drLimit = value;
    }
    private sbyte _minimumElevation = 15;

    /// <summary>
    /// Minimum satellite elevation for navigation use, in degrees.
    /// </summary>
    public sbyte MinimumElevation
    {
        get => _minimumElevation;
        set => _minimumElevation = value;
    }
    private double _positionDOP = 25.0;

    /// <summary>
    /// Position dilution-of-precision mask, dimensionless; wire resolution is 0.1.
    /// </summary>
    public double PositionDOP
    {
        get => _positionDOP;
        set => _positionDOP = value;
    }
    private double _timeDOP = 25.0;

    /// <summary>
    /// Time dilution-of-precision mask, dimensionless; wire resolution is 0.1.
    /// </summary>
    public double TimeDOP
    {
        get => _timeDOP;
        set => _timeDOP = value;
    }
    private ushort _positionAccuracy = 100;

    /// <summary>
    /// Position accuracy mask in meters.
    /// </summary>
    public ushort PositionAccuracy
    {
        get => _positionAccuracy;
        set => _positionAccuracy = value;
    }
    private ushort _timeAccuracy = 300;

    /// <summary>
    /// Time accuracy mask expressed as a distance in meters, as specified for the NAV5 tAcc field.
    /// </summary>
    public ushort TimeAccuracy
    {
        get => _timeAccuracy;
        set => _timeAccuracy = value;
    }
    private double _staticHoldThreshold;

    /// <summary>
    /// Speed threshold for static hold, in meters per second; wire resolution is 0.01 m/s.
    /// </summary>
    public double StaticHoldThreshold
    {
        get => _staticHoldThreshold;
        set => _staticHoldThreshold = value;
    }
    private byte _dgnssTimeout;

    /// <summary>
    /// Maximum age of differential GNSS corrections, in seconds.
    /// </summary>
    public byte DGNSSTimeout
    {
        get => _dgnssTimeout;
        set => _dgnssTimeout = value;
    }
    private byte _cnoThreshNumSVs;

    /// <summary>
    /// Minimum number of satellites that must meet the configured carrier-to-noise threshold.
    /// </summary>
    public byte CnoThreshNumSVs
    {
        get => _cnoThreshNumSVs;
        set => _cnoThreshNumSVs = value;
    }
    private byte _cnoThreshold = 35;

    /// <summary>
    /// Carrier-to-noise density threshold in dB-Hz.
    /// </summary>
    public byte CnoThreshold
    {
        get => _cnoThreshold;
        set => _cnoThreshold = value;
    }
    private ushort _staticHoldMaxDistance;

    /// <summary>
    /// Maximum displacement from the static-hold position, in meters.
    /// </summary>
    public ushort StaticHoldMaxDistance
    {
        get => _staticHoldMaxDistance;
        set => _staticHoldMaxDistance = value;
    }
    private byte _utcStandard;

    /// <summary>
    /// UTC realization selector: 0 automatic, 3 USNO, 5 European, 6 SU, or 7 NTSC; support depends on firmware.
    /// </summary>
    public byte UtcStandard
    {
        get => _utcStandard;
        set => _utcStandard = value;
    }

    public enum PositionModeEnum
    {
        Only2D = 1,
        Only3D = 2,
        Auto = 3
    }

    public enum ModelEnum
    {
        Portable = 0,
        Stationary = 2,
        Pedestrian = 3,
        Automotive = 4,
        Sea = 5,
        AirborneWithLess1gAcceleration = 6,
        AirborneWithLess2gAcceleration = 7,
        AirborneWithLess4gAcceleration = 8,
        WristWornWatch = 9,
        Bike = 10
    }

    protected override void SerializeContent(ref Span<byte> buffer)
    {
        ushort appliedBitMask = 0;
        if (ApplyDynamicModel) appliedBitMask |= 0x01;
        if (ApplyMinimumElevation) appliedBitMask |= 0x02;
        if (ApplyFixMode) appliedBitMask |= 0x04;
        if (ApplyDrLimit) appliedBitMask |= 0x08;
        if (ApplyPositionMask) appliedBitMask |= 0x10;
        if (ApplyTimeMask) appliedBitMask |= 0x20;
        if (ApplyStaticHold) appliedBitMask |= 0x40;
        if (ApplyDGPS) appliedBitMask |= 0x80;
        if (ApplyCnoThreshold) appliedBitMask |= 0x100;
        if (ApplyUTC) appliedBitMask |= 0x400;
        BinSerialize.WriteUShort(ref buffer, appliedBitMask);

        BinSerialize.WriteByte(ref buffer, (byte)PlatformModel);
        BinSerialize.WriteByte(ref buffer, (byte)PositionMode);

        BinSerialize.WriteInt(ref buffer, (int)Math.Round(FixedAltitude * 100));
        BinSerialize.WriteUInt(ref buffer, (uint)Math.Round(FixedAltitudeVariance * 10000));
        BinSerialize.WriteSByte(ref buffer, (sbyte)MinimumElevation);
        BinSerialize.WriteByte(ref buffer, (byte)DrLimit);
        BinSerialize.WriteUShort(ref buffer, (ushort)Math.Round(PositionDOP * 10));
        BinSerialize.WriteUShort(ref buffer, (ushort)Math.Round(TimeDOP * 10));
        BinSerialize.WriteUShort(ref buffer, PositionAccuracy);
        BinSerialize.WriteUShort(ref buffer, TimeAccuracy);

        BinSerialize.WriteByte(ref buffer, (byte)Math.Round(StaticHoldThreshold * 100));
        BinSerialize.WriteByte(ref buffer, DGNSSTimeout);
        BinSerialize.WriteByte(ref buffer, CnoThreshNumSVs);
        BinSerialize.WriteByte(ref buffer, CnoThreshold);

        BinSerialize.WriteByte(ref buffer, 0); // reserved
        BinSerialize.WriteByte(ref buffer, 0); // reserved

        BinSerialize.WriteUShort(ref buffer, StaticHoldMaxDistance);
        BinSerialize.WriteByte(ref buffer, UtcStandard);

        BinSerialize.WriteByte(ref buffer, 0); // reserved
        BinSerialize.WriteByte(ref buffer, 0); // reserved
        BinSerialize.WriteByte(ref buffer, 0); // reserved
        BinSerialize.WriteByte(ref buffer, 0); // reserved
        BinSerialize.WriteByte(ref buffer, 0); // reserved
    }

    protected override void DeserializeContent(ref ReadOnlySpan<byte> buffer)
    {
        var appliedBitMask = BinSerialize.ReadUShort(ref buffer);
        ApplyDynamicModel = (appliedBitMask & 0x01) != 0;
        ApplyMinimumElevation = (appliedBitMask & 0x02) != 0;
        ApplyFixMode = (appliedBitMask & 0x04) != 0;
        ApplyDrLimit = (appliedBitMask & 0x08) != 0;
        ApplyPositionMask = (appliedBitMask & 0x10) != 0;
        ApplyTimeMask = (appliedBitMask & 0x20) != 0;
        ApplyStaticHold = (appliedBitMask & 0x40) != 0;
        ApplyDGPS = (appliedBitMask & 0x80) != 0;
        ApplyCnoThreshold = (appliedBitMask & 0x100) != 0;
        ApplyUTC = (appliedBitMask & 0x400) != 0;

        PlatformModel = (ModelEnum)BinSerialize.ReadByte(ref buffer);
        PositionMode = (PositionModeEnum)BinSerialize.ReadByte(ref buffer);
        FixedAltitude = BinSerialize.ReadInt(ref buffer) * 0.01;
        FixedAltitudeVariance = BinSerialize.ReadUInt(ref buffer) * 0.0001;
        MinimumElevation = BinSerialize.ReadSByte(ref buffer);
        DrLimit = BinSerialize.ReadByte(ref buffer);
        PositionDOP = BinSerialize.ReadUShort(ref buffer) * 0.1;
        TimeDOP = BinSerialize.ReadUShort(ref buffer) * 0.1;
        PositionAccuracy = BinSerialize.ReadUShort(ref buffer);
        TimeAccuracy = BinSerialize.ReadUShort(ref buffer);
        StaticHoldThreshold = BinSerialize.ReadByte(ref buffer) * 0.01;
        DGNSSTimeout = BinSerialize.ReadByte(ref buffer);
        CnoThreshNumSVs = BinSerialize.ReadByte(ref buffer);
        CnoThreshold = BinSerialize.ReadByte(ref buffer);

        BinSerialize.ReadByte(ref buffer); // reserved
        BinSerialize.ReadByte(ref buffer); // reserved

        StaticHoldMaxDistance = BinSerialize.ReadUShort(ref buffer);
        UtcStandard = BinSerialize.ReadByte(ref buffer);
        BinSerialize.ReadByte(ref buffer); // reserved
        BinSerialize.ReadByte(ref buffer); // reserved
        BinSerialize.ReadByte(ref buffer); // reserved
        BinSerialize.ReadByte(ref buffer); // reserved
        BinSerialize.ReadByte(ref buffer); // reserved
    }

    protected override int GetContentByteSize() => 36;


    public override void Randomize(Random random)
    {
        ApplyDynamicModel = random.NextDouble() > 0.5;
        ApplyMinimumElevation = random.NextDouble() > 0.5;
        ;
        ApplyFixMode = random.NextDouble() > 0.5;
        ;
        ApplyDrLimit = random.NextDouble() > 0.5;
        ;
        ApplyPositionMask = random.NextDouble() > 0.5;
        ;
        ApplyTimeMask = random.NextDouble() > 0.5;
        ;
        ApplyStaticHold = random.NextDouble() > 0.5;
        ;
        ApplyDGPS = random.NextDouble() > 0.5;
        ;
        ApplyCnoThreshold = random.NextDouble() > 0.5;
        ;
        ApplyUTC = random.NextDouble() > 0.5;
        ;
        PlatformModel = (ModelEnum)random.Next(0, 10);
        PositionMode = (PositionModeEnum)random.Next(1, 3);
        FixedAltitude = random.NextDouble() * 60;
        FixedAltitudeVariance = random.NextDouble() * 6;
        DrLimit = (byte)random.Next(0, byte.MaxValue);
        MinimumElevation = (sbyte)(random.Next(0, byte.MaxValue) - 127);
        PositionDOP = random.NextDouble() * 60;
        TimeDOP = random.NextDouble() * 60;
        PositionAccuracy = (ushort)random.Next(0, ushort.MaxValue);
        TimeAccuracy = (ushort)random.Next(0, ushort.MaxValue);
        StaticHoldThreshold = 0.1;
        DGNSSTimeout = (byte)random.Next(0, byte.MaxValue);
        CnoThreshNumSVs = (byte)random.Next(0, byte.MaxValue);
        CnoThreshold = (byte)random.Next(0, byte.MaxValue);
        StaticHoldMaxDistance = (ushort)random.Next(0, ushort.MaxValue);
        UtcStandard = (byte)random.Next(0, byte.MaxValue);
    }
}