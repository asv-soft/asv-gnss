using System;
using System.Collections.Generic;
using System.Linq;
using Asv.IO;

namespace Asv.Gnss;

/// <summary>Provides the UBX interface profile for u-blox X20 HPG 2.11.</summary>
public static class UbxX20Protocol
{
    /// <summary>The receiver firmware described by this profile.</summary>
    public const string Firmware = "u-blox X20 HPG 2.11";

    /// <summary>The UBX protocol version documented for this firmware.</summary>
    public const string ProtocolVersion = "50.11";

    /// <summary>The largest complete frame allowed by UBX's unsigned 16-bit payload length.</summary>
    public const int MaxPacketSize = ushort.MaxValue + UbxProtocol.HeaderOffset + 2;

    /// <summary>The shared UBX wire-protocol identity.</summary>
    public static ProtocolInfo Info => UbxProtocol.Info;

    /// <summary>All documented message variants, including input, output and polling layouts.</summary>
    public static IReadOnlyList<UbxX20MessageDescriptor> Messages => UbxX20GeneratedFactory.Messages;

    /// <summary>Registers the exact X20 layouts with payload-aware variant selection.</summary>
    /// <param name="builder">The protocol parser builder.</param>
    /// <remarks>
    /// Use this instead of RegisterUbxProtocol in the same builder: both use the UBX wire identity.
    /// RegisterUbxProtocol retains its legacy model types and also recognizes new X20 message IDs.
    /// </remarks>
    public static void RegisterUbxX20Protocol(this IProtocolParserBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        var factory = new UbxX20MessageFactory();
        builder.Register(Info, (context, statistics) => new UbxMessageParser(factory, context, statistics, MaxPacketSize));
    }

    /// <summary>Creates a specific documented variant for transmission or direct deserialization.</summary>
    /// <param name="id">Combined UBX class and message ID.</param>
    /// <param name="variant">The exact JSON variant name, such as DATA1, POLL0 or EPH.</param>
    /// <returns>A new typed model with its documented discriminator defaults.</returns>
    /// <exception cref="ArgumentException">The ID and variant pair is not documented by this profile.</exception>
    public static UbxX20MessageBase CreateMessage(ushort id, string variant)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(variant);
        var descriptor = Messages.FirstOrDefault(message => message.Id == id && message.Variant == variant);
        return descriptor?.Create() ?? throw new ArgumentException(
            $"No {Firmware} variant '{variant}' exists for UBX ID 0x{id:X4}.", nameof(variant));
    }
}
