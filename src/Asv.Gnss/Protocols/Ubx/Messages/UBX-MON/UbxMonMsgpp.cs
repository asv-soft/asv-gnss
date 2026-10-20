using System;
using Asv.IO;

namespace Asv.Gnss;

public class UbxMonMsgppPool : UbxMessageBase
{
    public override string Name => "UBX-MON-MSGPP-POOL";
    public override byte Class => 0x0A;
    public override byte SubClass => 0x06;

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

/// <remarks>Payload fields follow u-blox document UBX-13003221 R23, section 32.16.6. Receiver and firmware support must be checked against the applicable interface description.</remarks>
public partial class UbxMonMsgpp : UbxMessageBase
{
    private readonly ushort[] _msg1 = new ushort[8];
    private readonly ushort[] _msg2 = new ushort[8];
    private readonly ushort[] _msg3 = new ushort[8];
    private readonly ushort[] _msg4 = new ushort[8];
    private readonly ushort[] _msg5 = new ushort[8];
    private readonly ushort[] _msg6 = new ushort[8];
    private readonly uint[] _skipped = new uint[6];

    public override string Name => "UBX-MON-MSGPP";
    public override byte Class => 0x0A;
    public override byte SubClass => 0x06;

    /// <summary>
    /// Number of successfully parsed messages
    /// for each protocol on port0
    /// </summary>
    public ushort[] Msg1 => _msg1;

    /// <summary>
    /// Number of successfully parsed messages
    /// for each protocol on port1
    /// </summary>
    public ushort[] Msg2 => _msg2;

    /// <summary>
    /// Number of successfully parsed messages
    /// for each protocol on port2
    /// </summary>
    public ushort[] Msg3 => _msg3;

    /// <summary>
    /// Number of successfully parsed messages
    /// for each protocol on port3
    /// </summary>
    public ushort[] Msg4 => _msg4;

    /// <summary>
    /// Number of successfully parsed messages
    /// for each protocol on port4
    /// </summary>
    public ushort[] Msg5 => _msg5;

    /// <summary>
    /// Number of successfully parsed messages
    /// for each protocol on port5
    /// </summary>
    public ushort[] Msg6 => _msg6;

    /// <summary>
    /// Number skipped bytes for each port
    /// </summary>
    public uint[] Skipped => _skipped;

    protected override void SerializeContent(ref Span<byte> buffer)
    {

    }

    protected override void DeserializeContent(ref ReadOnlySpan<byte> buffer)
    {
        for (int i = 0; i < Msg1.Length; i++)
        {
            Msg1[i] = BinSerialize.ReadUShort(ref buffer);
        }

        for (int i = 0; i < Msg2.Length; i++)
        {
            Msg2[i] = BinSerialize.ReadUShort(ref buffer);
        }

        for (int i = 0; i < Msg3.Length; i++)
        {
            Msg3[i] = BinSerialize.ReadUShort(ref buffer);
        }

        for (int i = 0; i < Msg4.Length; i++)
        {
            Msg4[i] = BinSerialize.ReadUShort(ref buffer);
        }

        for (int i = 0; i < Msg5.Length; i++)
        {
            Msg5[i] = BinSerialize.ReadUShort(ref buffer);
        }

        for (int i = 0; i < Msg6.Length; i++)
        {
            Msg6[i] = BinSerialize.ReadUShort(ref buffer);
        }

        for (int i = 0; i < Skipped.Length; i++)
        {
            Skipped[i] = BinSerialize.ReadUInt(ref buffer);
        }
    }


    protected override int GetContentByteSize() => 120;

    public override void Randomize(Random random)
    {

    }
}