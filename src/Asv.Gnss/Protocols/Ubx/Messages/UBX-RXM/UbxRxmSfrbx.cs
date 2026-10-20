using System;
using Asv.IO;

namespace Asv.Gnss;

public enum UbxGnssTypeEnum
    {
        GPS = 0,
        SBAS = 1,
        Galileo = 2,
        BeiDou = 3,
        IMES = 4,
        QZSS = 5,
        GLONASS = 6,
    }

public class UbxRxmSfrbxPool : UbxMessageBase
{
    public override string Name => "UBX-RXM-SFRBX-POOL";
    public override byte Class => 0x02;
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

    }
}

/// <remarks>Payload fields follow u-blox document UBX-13003221 R23, section 32.18.7. Receiver and firmware support must be checked against the applicable interface description.</remarks>
public partial class UbxRxmSfrbx : UbxMessageBase
{
    private UbxGnssTypeEnum _ubxGnssId;
    private byte _svId;
    private byte _reserved1;
    private sbyte _freqId;
    private byte _numWords;
    private byte _chn;
    private byte _version;
    private byte _reserved2;
    private uint[]? _rawData;

    public override string Name => "UBX-RXM-SFRBX";
    public override byte Class => 0x02;
    public override byte SubClass => 0x13;

    /// <summary>
    /// GNSS identifier
    /// </summary>
    public UbxGnssTypeEnum UbxGnssId
    {
        get => _ubxGnssId;
        set => _ubxGnssId = value;
    }

    /// <summary>
    /// Satellite identifier
    /// </summary>
    public byte SvId
    {
        get => _svId;
        set => _svId = value;
    }

    /// <summary>
    /// Reserved in legacy layouts; carries the signal identifier (sigId) in newer interfaces,
    /// including F9 HPG 1.50. The original property name is retained for API compatibility.
    /// </summary>
    public byte Reserved1
    {
        get => _reserved1;
        set => _reserved1 = value;
    }

    /// <summary>
    /// GLONASS frequency slot from -7 to 6; this library subtracts 7 from the encoded freqId value. Meaningful only for GLONASS.
    /// </summary>
    public sbyte FreqId
    {
        get => _freqId;
        set => _freqId = value;
    }

    /// <summary>
    /// Number of 32-bit navigation words: version 1 permits 0 to 16; the R23 version-2 description lists up to 10 for supported signals.
    /// </summary>
    public byte NumWords
    {
        get => _numWords;
        set => _numWords = value;
    }

    /// <summary>
    /// Tracking channel number for payload version 2; reserved in version 1 and exposed as zero by this deserializer.
    /// </summary>
    public byte Chn
    {
        get => _chn;
        set => _chn = value;
    }

    /// <summary>
    /// Payload version: 1 for the protocol-17 layout, or 2 for the protocol-18-and-later layout described in R23.
    /// </summary>
    public byte Version
    {
        get => _version;
        set => _version = value;
    }

    /// <summary>
    /// Reserved
    /// </summary>
    public byte Reserved2
    {
        get => _reserved2;
        set => _reserved2 = value;
    }

    /// <summary>
    /// Broadcast navigation words in constellation-specific layout; includes NumWords unsigned 32-bit values.
    /// </summary>
    public uint[] RawData
    {
        get => _rawData!;
        set => _rawData = value;
    }

    protected override void SerializeContent(ref Span<byte> buffer)
    {

    }

    protected override void DeserializeContent(ref ReadOnlySpan<byte> buffer)
    {
        UbxGnssId = (UbxGnssTypeEnum)BinSerialize.ReadByte(ref buffer);
        SvId = BinSerialize.ReadByte(ref buffer);
        Reserved1 = BinSerialize.ReadByte(ref buffer);
        FreqId = (sbyte)(BinSerialize.ReadSByte(ref buffer) - 7);
        NumWords = BinSerialize.ReadByte(ref buffer);
        Chn = BinSerialize.ReadByte(ref buffer);
        Version = BinSerialize.ReadByte(ref buffer);
        Reserved2 = BinSerialize.ReadByte(ref buffer);

        if (Version == 0x01)
        {
            Chn = 0;
        }

        RawData = new uint[NumWords];

        for (int i = 0; i < NumWords; i++)
        {
            RawData[i] = BinSerialize.ReadUInt(ref buffer);
        }

        GpsSubFrame = null!;
        GlonassWord = null!;

        // UBX-18010802 R08, section 3.13.1.2: GPS L2C uses CNAV, not the L1C/A subframe layout.
        if (UbxGnssId == UbxGnssTypeEnum.GPS && (Version < 2 || Reserved1 == 0))
        {
            GpsSubFrame = GpsSubFrameFactory.Create(RawData);
        }
        else if (UbxGnssId == UbxGnssTypeEnum.GLONASS)
        {
            // Section 3.13.1.3: the fourth word is receiver frame metadata, outside the 85-bit string.
            GlonassWord = GlonassWordFactory.Create(RawData.Length == 4 ? RawData[..3] : RawData);
        }
    }

    protected override int GetContentByteSize() => 8 + (4 * RawData.Length);

    public override void Randomize(Random random)
    {

    }

    /// <summary>
    /// Library-decoded GLONASS navigation string. The optional fourth receiver-metadata word
    /// remains in RawData and is not passed to the 85-bit string decoder.
    /// </summary>
    public GlonassWordBase GlonassWord { get; set; }

    /// <summary>
    /// Library-decoded GPS L1C/A navigation subframe; remains null for other GPS signals.
    /// </summary>
    public GpsSubframeBase GpsSubFrame { get; set; }
}
