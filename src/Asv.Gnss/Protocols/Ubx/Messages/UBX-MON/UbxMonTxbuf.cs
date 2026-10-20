using System;
using Asv.IO;

namespace Asv.Gnss;

public class UbxMonTxbufPool : UbxMessageBase
{
    public override string Name => "UBX-MON-TXBUF-POOL";
    public override byte Class => 0x0A;
    public override byte SubClass => 0x08;

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

/// <remarks>Payload fields follow u-blox document UBX-13003221 R23, section 32.16.12. Receiver and firmware support must be checked against the applicable interface description.</remarks>
public partial class UbxMonTxbuf : UbxMessageBase
{
    private readonly ushort[] _pending = new ushort[6];
    private readonly byte[] _usage = new byte[6];
    private readonly byte[] _peakUsage = new byte[6];
    private byte _tUsage;
    private byte _tPeakUsage;
    private byte _errors;
    private bool _isAlloc;
    private bool _isMem;
    private bool _isLimit;
    private byte _reserved1;

    public override string Name => "UBX-MON-TXBUF";
    public override byte Class => 0x0A;
    public override byte SubClass => 0x08;

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

    /// <summary>
    /// Maximum usage of transmitter buffer
    /// during the last sysmon period for all targets
    /// </summary>
    public byte TUsage
    {
        get => _tUsage;
        set => _tUsage = value;
    }

    /// <summary>
    /// Maximum usage of transmitter buffer for
    /// all targets
    /// </summary>
    public byte TPeakUsage
    {
        get => _tPeakUsage;
        set => _tPeakUsage = value;
    }

    /// <summary>
    /// Error bitmask (error bits in region below)
    /// </summary>
    public byte Errors
    {
        get => _errors;
        set => _errors = value;
    }

    #region Errors bits

    /// <summary>
    /// Allocation error (TX buffer full)
    /// </summary>
    public bool IsAlloc
    {
        get => _isAlloc;
        set => _isAlloc = value;
    }

    /// <summary>
    /// Memory Allocation error
    /// </summary>
    public bool IsMem
    {
        get => _isMem;
        set => _isMem = value;
    }

    /// <summary>
    /// Buffer limit of corresponding target reached
    /// </summary>
    public bool IsLimit
    {
        get => _isLimit;
        set => _isLimit = value;
    }

    #endregion

    /// <summary>
    /// Reserved
    /// </summary>
    public byte Reserved1
    {
        get => _reserved1;
        set => _reserved1 = value;
    }


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

        TUsage = BinSerialize.ReadByte(ref buffer);

        TPeakUsage = BinSerialize.ReadByte(ref buffer);

        Errors = BinSerialize.ReadByte(ref buffer);

        IsAlloc = (Errors & 0b1000_0000) != 0;

        IsMem = (Errors & 0b0100_0000) != 0;

        IsLimit = (Errors & 0b0010_0000) != 0;

        Reserved1 = BinSerialize.ReadByte(ref buffer);
    }

    protected override int GetContentByteSize() => 28;

    public override void Randomize(Random random)
    {

    }
}