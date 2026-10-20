using System;
using Asv.Common;
using Asv.IO;

namespace Asv.Gnss;

public class UbxCfgAntPool : UbxMessageBase
{
    public override string Name => "UBX-CFG-ANT-POOL";
    public override byte Class => 0x06;
    public override byte SubClass => 0x13;

    protected override void SerializeContent(ref Span<byte> buffer)
    {

    }

    protected override void DeserializeContent(ref ReadOnlySpan<byte> buffer)
    {

    }

    protected override int GetContentByteSize() => 0;

    public override void Randomize(Random random)
    {
        // do nothing
    }
}

/// <summary>
/// Antenna control settings
/// u-blox 8 / u-blox M8 protocol versions 15, 15.01, 16, 17, 18, 19, 19.1, 19.2, 20, 20.01,
/// 20.1, 20.2, 20.3, 22, 22.01, 23 and 23.01
/// This message allows the user to configure the antenna supervisor.
/// The antenna supervisor can be used to detect the status of an active antenna
/// and control it. It can be used to turn off the supply to the antenna in the event of
/// a short cirquit (for example) or to manage power consumption in power save
/// mode.
/// Refer to antenna supervisor configuration in the Integration manual for more
/// information regarding the behavior of the antenna supervisor.
/// Refer to UBX-MON-HW for a description of the fields in the message used to
/// obtain the status of the antenna.
/// Note that not all pins can be used for antenna supervisor operation, it is
/// recommended that you use the default pins, consult the Integration manual if
/// you need to use other pins.
/// </summary>
/// <remarks>Payload fields follow u-blox document UBX-13003221 R23, section 32.10.1. Receiver and firmware support must be checked against the applicable interface description.</remarks>
/// <remarks>ZED-F9P integration manual UBX-18010802 R08, section 3.1.6, describes limited legacy UBX-CFG compatibility and recommends the configuration-item interface.</remarks>
public partial class UbxCfgAnt : UbxMessageBase
{

    public override string Name => "UBX-CFG-ANT";
    public override byte Class => 0x06;
    public override byte SubClass => 0x13;

    protected override void SerializeContent(ref Span<byte> buffer)
    {
        BinSerialize.WriteUShort(ref buffer, (ushort)Flags);
        var bitfield = new UintBitArray(0, 16);
        bitfield.SetBitU(0, 5, PinSwitch);
        bitfield.SetBitU(5, 5, PinSCD);
        bitfield.SetBitU(10, 5, PinOCD);
        bitfield[15] = Reconfig;
        BinSerialize.WriteUShort(ref buffer, (ushort)bitfield.Value);
    }

    protected override void DeserializeContent(ref ReadOnlySpan<byte> buffer)
    {
        Flags = (UbxCfgAntFlags)BinSerialize.ReadUShort(ref buffer);
        var bitfield = new UintBitArray(BinSerialize.ReadUShort(ref buffer), 16);
        PinSwitch = (byte)bitfield.GetBitU(0, 5);
        PinSCD = (byte)bitfield.GetBitU(5, 5);
        PinOCD = (byte)bitfield.GetBitU(10, 5);
        Reconfig = bitfield[15];
    }

    private bool _reconfig;

    /// <summary>
    /// Whether to apply the antenna pin assignment in this message.
    /// </summary>
    public bool Reconfig
    {
        get => _reconfig;
        set => _reconfig = value;
    }
    private byte _pinOCD;

    /// <summary>
    /// PIO number used for antenna open-circuit detection.
    /// </summary>
    public byte PinOCD
    {
        get => _pinOCD;
        set => _pinOCD = value;
    }
    private byte _pinSCD;

    /// <summary>
    /// PIO number used for antenna short-circuit detection.
    /// </summary>
    public byte PinSCD
    {
        get => _pinSCD;
        set => _pinSCD = value;
    }
    private byte _pinSwitch;

    /// <summary>
    /// PIO number used to switch antenna power.
    /// </summary>
    public byte PinSwitch
    {
        get => _pinSwitch;
        set => _pinSwitch = value;
    }

    private UbxCfgAntFlags _flags;

    /// <summary>
    /// Antenna supervision options for power control, fault detection and recovery.
    /// </summary>
    public UbxCfgAntFlags Flags
    {
        get => _flags;
        set => _flags = value;
    }

    protected override int GetContentByteSize() => 4;

    public override void Randomize(Random random)
    {
        Reconfig = random.Next() % 2 == 0;
        PinOCD = (byte)random.Next(0, 10);
        PinSCD = (byte)random.Next(0, 10);
        PinSwitch = (byte)random.Next(0, 10);
    }
}


[Flags]
public enum UbxCfgAntFlags : ushort
{
    /// <summary>
    /// Enable antenna supply voltage control signal
    /// </summary>
    Svcs = 0b00000001,

    /// <summary>
    /// Enable short circuit detection
    /// </summary>
    Scd = 0b00000010,

    /// <summary>
    /// Enable open circuit detection
    /// </summary>
    Ocd = 0b00000100,

    /// <summary>
    /// Power down antenna supply if short circuit is detected. (only in combination with bit 1)
    /// </summary>
    PdwnOnScd = 0b00001000,

    /// <summary>
    /// Enable automatic recovery from short state
    /// </summary>
    Recovery = 0b00010000,

}