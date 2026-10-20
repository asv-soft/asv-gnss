# GBAS

GBAS support preserves the VDB burst and application message formats used by `asv-gbas-v4` (`Asv.Gbas.Core/GbasMessages`, source revision `c755fdf`). This is a codec migration from that implementation; it does not establish compliance with a particular ICAO amendment or validate navigation algorithms.

## Messages

| Type | Model | Default registration |
|------|-------|----------------------|
| 1 | `MessageBlockType1` | Yes |
| 2 | `MessageBlockType2` | Yes |
| 3 | `MessageBlockType3` | Yes |
| 4 | `MessageBlockType4` | Yes |
| 5 | `MessageBlockType5` | Yes |
| 11 | `MessageBlockType11` | Yes |
| 101 | `MessageBlockType101` | Yes |

The models include correction measurements, MT2 additional data blocks 1–4, MT4 FAS data sets and MT5 impacted sources and obstructed approaches. Public model names and binary field scales follow the source implementation.

## Stream decoding

```c#
using Asv.Gnss;
using Asv.IO;
using R3;

var protocol = Protocol.Create(builder => builder.Protocols.RegisterGbasProtocol());
using var parser = protocol.CreateParser(GbasProtocol.Info.Id);
using var messages = parser.OnMessage.Subscribe(message =>
{
    if (message is MessageBlockType1 corrections)
    {
        Console.WriteLine($"{corrections.GbasId}: {corrections.Measurements.Count} corrections");
    }
});

// vdbData is AsvMessageGbasVdbSendV2.Data, after ASV envelope decoding.
foreach (var value in vdbData)
{
    parser.Push(value);
}
```

`GbasMessageParser` accepts byte-aligned VDB bursts beginning with their six-byte synchronization sequence. It validates training data and transmission length, descrambles the burst, applies Reed–Solomon correction, checks application block boundaries and CRC-32Q, and publishes one typed message per application block. `Push` returns true once per decoded burst, which can contain multiple application messages. Factory customization follows the other protocols through the optional registration callback. Unknown message types are reported through the parser error stream; later blocks remain independently decodable.

The input must exclude the surrounding ASV envelope. After a transport discontinuity call `Reset` before feeding the next burst. Raw radio samples and packed, non-byte-aligned sequences of consecutive bursts are outside this stream parser's input format.

## Encoding and direct decoding

`GbasMessageBlockBase.Serialize` and `Deserialize` handle an individual application block, including its header and CRC. They do not add VDB framing.

```c#
var burst = new GbasMessage { SsId = SlotIdEnum.A };
burst.ApplicationData.Add(new MessageBlockType3 { GbasId = "TEST" });
var bytes = new byte[burst.GetByteSize()];
var output = bytes.AsSpan();
burst.Serialize(ref output);

var decoded = new GbasMessage();
ReadOnlySpan<byte> input = bytes;
decoded.Deserialize(ref input);
```

`GbasMessage` handles the containing burst, including synchronization, training protection, scrambling, application FEC and padding to a multiple of three bits. Its bit-position overloads support a burst embedded at an arbitrary bit offset. `GetFrameBitSize` reads the protected header; `DecodeApplicationData` returns the decoded application bytes and station slot for custom dispatch. Application data is limited to 222 bytes, matching the original implementation.

MT1 and MT101 use a nullable source-availability duration; MT1/MT101 B values use `double.NaN` for the encoded unavailable sentinel. FAS preserves the source's separate inner CRC and TCH unit scaling.

ASV transport messages `AsvMessageGbasVdbSendV2` and `AsvMessageGbasMonDevSendV2` remain part of [ASV](protocol-asv.md). Correction generation, receiver control, satellite orbit propagation, database storage and statistical monitoring are not part of this protocol codec.
