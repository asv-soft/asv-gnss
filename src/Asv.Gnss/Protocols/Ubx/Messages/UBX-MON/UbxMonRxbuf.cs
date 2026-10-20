using System;
using Asv.IO;

namespace Asv.Gnss;

public class UbxMonRxbufPool : UbxMessageBase
{
    public override string Name => "UBX-MON-RXBUF-POOL";
    public override byte Class => 0x0A;
    public override byte SubClass => 0x07;

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

/// <remarks>Payload fields follow u-blox document UBX-13003221 R23, section 32.16.8. Receiver and firmware support must be checked against the applicable interface description.</remarks>
public partial class UbxMonRxbuf : UbxMessageBase
{
    private readonly ushort[] _pending = new ushort[6];
    private readonly byte[] _usage = new byte[6];
    private readonly byte[] _peakUsage = new byte[6];

    public override string Name => "UBX-MON-RXBUF";
    public override byte Class => 0x0A;
    public override byte SubClass => 0x07;

    /// <summary>
    /// Number of bytes pending in transmitter
    /// buffer for each target
    /// </summary>
    public ushort[] Pending => _pending;

    /// <summary>
    /// Maximum usage transmitter buffer during
    /// the last sysmon period for each target
    /// </summary>
    public byte[] Usage => _usage;

    /// <summary>
    /// Maximum usage transmitter buffer for each target
    /// </summary>
    public byte[] PeakUsage => _peakUsage;

    protected override void SerializeContent(ref Span<byte> buffer)
    {

    }

    protected override void DeserializeContent(ref ReadOnlySpan<byte> buffer)
    {
        for (int i = 0; i < Pending.Length; i++)
        {
            Pending[i] = BinSerialize.ReadUShort(ref buffer);
        }

        for (int i = 0; i < Usage.Length; i++)
        {
            Usage[i] = BinSerialize.ReadByte(ref buffer);
        }

        for (int i = 0; i < PeakUsage.Length; i++)
        {
            PeakUsage[i] = BinSerialize.ReadByte(ref buffer);
        }
    }

    protected override int GetContentByteSize() => 24;

    public override void Randomize(Random random)
    {

    }

}