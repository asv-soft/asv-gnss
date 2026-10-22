using System;
using Asv.IO;

namespace Asv.Gnss;

/// <summary>A native 56/112-bit Mode S downlink with AVR or Beast receiver framing.</summary>
public partial class AdsbDfMessage : IProtocolMessage<ushort>, IVisitable
{
    private byte[] _rawData = new byte[7];
    private ProtocolTags _tags = [];
    public ProtocolInfo Protocol => AdsbProtocol.Info;
    public string Name => $"Mode S DF{DownlinkFormat}";
    public ushort Id => DownlinkFormat;
    public byte DownlinkFormat => (byte)(_rawData[0] >> 3);
    /// <summary>The three low header bits. Their meaning depends on the downlink format.</summary>
    public byte HeaderBits => (byte)(_rawData[0] & 7);
    public ref ProtocolTags Tags => ref _tags;
    public string GetIdAsString() => Id.ToString(System.Globalization.CultureInfo.InvariantCulture);
    /// <summary>Unmodified native Mode S bytes, owned by this message.</summary>
    public ReadOnlyMemory<byte> RawData => _rawData;
    public AdsbFrameFormat FrameFormat { get; set; }
    public char AvrMarker { get; set; } = '*';
    /// <summary>Raw 48-bit receiver clock value; this is not a UTC timestamp.</summary>
    public ulong? ReceiverTimestamp { get; set; }
    /// <summary>Unscaled receiver signal level (0..255), when supplied by the transport.</summary>
    public byte? SignalLevel { get; set; }
    /// <summary>Explicit ICAO address only. Address/data-parity recovery is not trusted without an aircraft cache.</summary>
    public uint? IcaoAddress => DownlinkFormat is 11 or 17 || DownlinkFormat == 18 && HeaderBits == 0
        ? Read24(_rawData.AsSpan(1, 3)) : null;
    public uint Parity => Read24(_rawData.AsSpan(_rawData.Length - 3));
    /// <summary>Payload CRC XOR transmitted parity; nonzero is expected for address/data parity.</summary>
    public uint ParitySyndrome => ComputeCrc(_rawData.AsSpan(0, _rawData.Length - 3)) ^ Parity;
    public AdsbParityStatus ParityStatus => DownlinkFormat switch
    {
        17 or 18 => ParitySyndrome == 0 ? AdsbParityStatus.Valid : AdsbParityStatus.Invalid,
        11 => ParitySyndrome <= 127 ? AdsbParityStatus.InterrogatorParity : AdsbParityStatus.Invalid,
        _ => AdsbParityStatus.AddressParityUnverified,
    };
    public AdsbExtendedSquitter? ExtendedSquitter { get; private set; }

    /// <summary>Copies and validates a native downlink, refreshing its decoded message element.</summary>
    public void SetRawData(ReadOnlySpan<byte> data)
    {
        ValidateRaw(data);
        var df = data[0] >> 3;
        var decoded = df == 17 || df == 18 && (data[0] & 7) is 0 or 1 or 2 or 5 or 6
            ? AdsbExtendedSquitter.Decode(data) : null;
        _rawData = data.ToArray();
        ExtendedSquitter = decoded;
    }

    /// <inheritdoc />
    public void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        Span<byte> raw = stackalloc byte[14];
        var header = AdsbFrameCodec.Read(buffer, raw);
        SetRawData(raw[..header.RawSize]);
        FrameFormat = header.Format;
        AvrMarker = header.Marker;
        ReceiverTimestamp = header.Timestamp;
        SignalLevel = header.Signal;
        buffer = buffer[header.Size..];
    }

    public void Serialize(ref Span<byte> buffer) => AdsbFrameCodec.Write(this, ref buffer);
    public int GetByteSize() => AdsbFrameCodec.Size(this);

    internal static bool ValidLength(ReadOnlySpan<byte> raw) => raw.Length is 7 or 14 && raw.Length == ((raw[0] & 0x80) == 0 ? 7 : 14);
    internal static bool ValidParity(ReadOnlySpan<byte> raw)
    {
        var df = raw[0] >> 3;
        var syndrome = ComputeCrc(raw[..^3]) ^ Read24(raw[^3..]);
        return df switch { 17 or 18 => syndrome == 0, 11 => syndrome <= 127, _ => true };
    }

    private static void ValidateRaw(ReadOnlySpan<byte> data)
    {
        if (!ValidLength(data)) throw new FormatException("Mode S downlink length does not match the DF length bit.");
        if (!ValidParity(data)) throw new ProtocolParserCrcException(AdsbProtocol.Info);
    }

    private static uint Read24(ReadOnlySpan<byte> data) => (uint)(data[0] << 16 | data[1] << 8 | data[2]);

    private static uint ComputeCrc(ReadOnlySpan<byte> data)
    {
        uint remainder = 0;
        foreach (var value in data)
        {
            remainder ^= (uint)value << 16;
            for (var bit = 0; bit < 8; bit++)
                remainder = ((remainder << 1) ^ ((remainder & 0x800000) != 0 ? 0xFFF409u : 0)) & 0xFFFFFF;
        }
        return remainder;
    }
}
