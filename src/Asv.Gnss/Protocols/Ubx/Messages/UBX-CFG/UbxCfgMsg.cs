using System;
using Asv.IO;

namespace Asv.Gnss;

public enum UbxCfgMsgType
{
    Pool,
    Rate,
    ManyRate,
}


public partial class UbxCfgMsgPool : UbxMessageBase
{
    public override byte Class => 0x06;
    public override byte SubClass => 0x01;
    public override string Name => "UBX-CFG-MSG-POOL";

    private byte _msgClass;

    /// <summary>
    /// UBX class identifier of the message whose output rate is queried or configured.
    /// </summary>
    public byte MsgClass
    {
        get => _msgClass;
        set => _msgClass = value;
    }
    private byte _msgId;

    /// <summary>
    /// UBX message identifier within MsgClass.
    /// </summary>
    public byte MsgId
    {
        get => _msgId;
        set => _msgId = value;
    }

    protected override void SerializeContent(ref Span<byte> buffer)
    {
        BinSerialize.WriteByte(ref buffer, MsgClass);
        BinSerialize.WriteByte(ref buffer, MsgId);
    }

    protected override void DeserializeContent(ref ReadOnlySpan<byte> buffer)
    {
        MsgClass = BinSerialize.ReadByte(ref buffer);
        MsgId = BinSerialize.ReadByte(ref buffer);
    }

    protected override int GetContentByteSize() => 2;

    public override void Randomize(Random random)
    {
        MsgClass = (byte)random.Next(0, byte.MaxValue);
        MsgId = (byte)random.Next(0, byte.MaxValue);
    }


}

/// <summary>
/// 32.10.18 UBX-CFG-MSG (0x06 0x01)
/// Poll a message configuration
/// Supported on:
/// u-blox 8 / u-blox M8 protocol versions 15, 15.01, 16, 17, 18, 19, 19.1, 19.2, 20, 20.01,
/// 20.1, 20.2, 20.3, 22, 22.01, 23 and 23.01
/// Set message rate configuration for the current port.
/// </summary>
/// <remarks>Payload fields follow u-blox document UBX-13003221 R23, section 32.10.18. Receiver and firmware support must be checked against the applicable interface description.</remarks>
/// <remarks>ZED-F9P integration manual UBX-18010802 R08, section 3.1.6, describes limited legacy UBX-CFG compatibility and recommends the configuration-item interface.</remarks>
public partial class UbxCfgMsg : UbxMessageBase
{
    public override byte Class => 0x06;
    public override byte SubClass => 0x01;
    public override string Name => "UBX-CFG-MSG";

    private byte _msgClass;

    /// <summary>
    /// UBX class identifier of the message whose output rate is queried or configured.
    /// </summary>
    public byte MsgClass
    {
        get => _msgClass;
        set => _msgClass = value;
    }
    private byte _msgId;

    /// <summary>
    /// UBX message identifier within MsgClass.
    /// </summary>
    public byte MsgId
    {
        get => _msgId;
        set => _msgId = value;
    }
    private byte? _currentPortRate;

    /// <summary>
    /// Output interval for the current port in navigation epochs; 0 disables output. A non-null value selects the three-byte form.
    /// </summary>
    public byte? CurrentPortRate
    {
        get => _currentPortRate;
        set => _currentPortRate = value;
    }
    private byte[]? _ports;

    /// <summary>
    /// Per-port output intervals in navigation epochs for the eight-byte message form: DDC, UART1, UART2, USB, SPI, and one reserved byte. Zero disables output on a port.
    /// </summary>
    public byte[] Ports
    {
        get => _ports!;
        set => _ports = value;
    }

    protected override void SerializeContent(ref Span<byte> buffer)
    {
        BinSerialize.WriteByte(ref buffer, MsgClass);
        BinSerialize.WriteByte(ref buffer, MsgId);
        if (CurrentPortRate != null && Ports != null)
        {
            throw new Exception($"Must set only one field: {nameof(CurrentPortRate)} or {nameof(Ports)}");
        }

        if (CurrentPortRate == null && Ports == null)
        {
            throw new Exception($"Must set one field: {nameof(CurrentPortRate)}==null or {nameof(Ports)}==null");
        }

        if (Ports != null && Ports.Length != 6)
        {
            throw new Exception($"{nameof(Ports)} length must be 6 element");
        }

        if (Ports != null)
        {
            foreach (var port in Ports)
            {
                BinSerialize.WriteByte(ref buffer, port);
            }
        }

        if (CurrentPortRate != null)
        {
            BinSerialize.WriteByte(ref buffer, CurrentPortRate.Value);
        }

    }

    protected override void DeserializeContent(ref ReadOnlySpan<byte> buffer)
    {
        if (buffer.Length is not (3 or 8))
        {
            throw new ArgumentException("CFG-MSG payload must contain 3 or 8 bytes.");
        }

        MsgClass = BinSerialize.ReadByte(ref buffer);
        MsgId = BinSerialize.ReadByte(ref buffer);
        if (buffer.Length == 1)
        {
            Ports = null!;
            CurrentPortRate = BinSerialize.ReadByte(ref buffer);
        }
        else
        {
            CurrentPortRate = null;
            Ports = new byte[6];
            BinSerialize.ReadBlock(ref buffer, Ports);
        }
    }

    protected override int GetContentByteSize() => CurrentPortRate != null ? 3 : 8;

    public override void Randomize(Random random)
    {
        MsgClass = (byte)random.Next(0, byte.MaxValue);
        MsgId = (byte)random.Next(0, byte.MaxValue);
        CurrentPortRate = (byte)random.Next(0, byte.MaxValue);
    }


}
