using System;
using System.Collections.Generic;
using System.Text;
using Asv.IO;

namespace Asv.Gnss;

public class UbxMonVerPool : UbxMessageBase
{
    public override string Name => "UBX-MON-VER-POOL";
    public override byte Class => 0x0A;
    public override byte SubClass => 0x04;

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

// [SerializationNotSupported]
/// <remarks>Payload fields follow u-blox document UBX-13003221 R23, section 32.16.13. Receiver and firmware support must be checked against the applicable interface description.</remarks>
public partial class UbxMonVer : UbxMessageBase
{
    private string? _software;
    private string? _hardware;
    private List<string>? _extensions;

    public override string Name => "UBX-MON-VER";
    public override byte Class => 0x0A;
    public override byte SubClass => 0x04;

    /// <summary>
    /// Nul-terminated software version string.
    /// </summary>
    public string Software
    {
        get => _software!;
        set => _software = value;
    }

    /// <summary>
    /// Nul-terminated hardware version string
    /// </summary>
    public string Hardware
    {
        get => _hardware!;
        set => _hardware = value;
    }

    /// <summary>
    /// Extended software information strings. A series of nul-terminated strings.Each
    /// extension field is 30 characters long and contains varying software information.
    /// Not all extension fields may appear. Examples of reported information: the
    /// software version string of the underlying ROM (when the receiver's firmware is
    /// running from flash), the firmware version, the supported protocol version, the
    /// module identifier, the flash information structure(FIS) file information, the
    /// supported major GNSS, the supported augmentation systems. See Firmware and protocol versions for
    /// details.
    /// </summary>
    public List<string> Extensions
    {
        get => _extensions!;
        set => _extensions = value;
    }

    protected override void SerializeContent(ref Span<byte> buffer)
    {
        throw new NotImplementedException();
    }

    protected override void DeserializeContent(ref ReadOnlySpan<byte> buffer)
    {
        // 40 + 30*N = PayloadLength
        var extLength = (buffer.Length - 40) / 30;
        Software = ReadNullTerminatedString(buffer.Slice(0, 30));
        buffer = buffer.Slice(30);
        Hardware = ReadNullTerminatedString(buffer.Slice(0, 10));
        buffer = buffer.Slice(10);
        Extensions = new List<string>();
        for (var i = 0; i < extLength; i++)
        {
            Extensions.Add(ReadNullTerminatedString(buffer.Slice(0, 30)));
            buffer = buffer.Slice(30);
        }
    }

    private static string ReadNullTerminatedString(ReadOnlySpan<byte> buffer)
    {
        var terminator = buffer.IndexOf((byte)0);
        return Encoding.ASCII.GetString(terminator < 0 ? buffer : buffer[..terminator]);
    }

    protected override int GetContentByteSize() => 40 + (Extensions?.Count ?? 0) * 30;


    public override void Randomize(Random random)
    {

    }
}
