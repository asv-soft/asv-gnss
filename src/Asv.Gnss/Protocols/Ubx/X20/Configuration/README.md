# X20 configuration values

The catalog contains all 826 configuration items and 38 groups in the pinned
official ZED-X20P X20 HPG 2.11 JSON, for protocol version 50.11. It retains native
types, scales, units, titles, descriptions and named constants. An empty constant
value in the upstream JSON remains an empty string; no numeric value is inferred.
`UbxX20ConfigurationCatalog.SourceUrl` identifies the pinned source revision.

## Typed configuration data

Use exact CLR native types. For example, `CFG-RATE-MEAS` is a `U2`, so its raw
value must be a `ushort`; the source scale `0.001 s` is metadata. The codec does
not apply that scale or validate receiver-specific accepted ranges and cross-key
dependencies.

```c#
UbxX20ConfigurationValue[] values =
[
    new(UbxX20ConfigurationKeys.Rate.Meas, (ushort)1000),
    new(UbxX20ConfigurationKeys.Signal.GpsEna, true),
];

var message = new UbxX20CfgValsetData0
{
    Layers = (byte)UbxX20ConfigurationLayers.Ram,
    CfgData = UbxX20ConfigurationCodec.EncodeValues(values),
};

// For a VALGET response, pass its CfgData property to DecodeValues.
IReadOnlyList<UbxX20ConfigurationValue> decoded =
    UbxX20ConfigurationCodec.DecodeValues(message.CfgData);
```

Native wire types map to these CLR types:

| Wire type | CLR type |
| --- | --- |
| `L` | `bool` |
| `U1`, `E1`, `X1` | `byte` |
| `U2`, `E2`, `X2` | `ushort` |
| `U4`, `E4`, `X4` | `uint` |
| `U8`, `X8` | `ulong` |
| `I1`, `I2`, `I4`, `I8` | `sbyte`, `short`, `int`, `long` |
| `R4`, `R8` | `float`, `double` |

Unknown concrete keys with a valid encoded size are retained as `byte[]` values.
Their signedness or floating-point interpretation cannot be inferred from the
key's size bits. All integer and floating-point values use little-endian wire
encoding. Logical values use the least significant bit of their storage byte.

## Key lists and complete payloads

`EncodeKeys` and `DecodeKeys` handle key arrays for VALGET polls and VALDEL.
Concrete keys must have storage code 1 through 5, reserved bits clear, a group
from `0x01` through `0xfe`, and an item from `0x001` through `0xffe`.

VALGET and VALDEL also support wildcard IDs as described in interface-description
sections 3.10.4 and 3.10.5:

- A valid group and item field `0xffff` select every item in that group.
- A group field of `0xfff` in bits 16 through 27 selects every group.
- Storage bits 28 through 30 are not interpreted for wildcard key lists because
  no value follows the key. Bit 31 remains reserved and must be clear.

The catalog's `UbxX20ConfigurationKeys.<Group>.All` constants and
`UbxX20ConfigurationCodec.AllKeys` provide canonical wildcard IDs. Wildcards
cannot appear in key-value data.

`CreateValSet`, `CreateValDelete`, `CreateValGetRequest` and
`CreateValGetResponse` return complete UBX **payloads**, including the four-byte
CFG-VAL header. They do not include the UBX frame header or checksum. Use
`EncodeValues` for a generated message's `CfgData` property; assigning a complete
payload there would repeat the CFG-VAL header.

```c#
byte[] pollPayload = UbxX20ConfigurationCodec.CreateValGetRequest(
    [UbxX20ConfigurationKeys.Rate.All],
    UbxX20ConfigurationLayer.Default,
    position: 0);

byte[] setPayload = UbxX20ConfigurationCodec.CreateValSet(
    values,
    UbxX20ConfigurationLayers.Ram | UbxX20ConfigurationLayers.Bbr,
    UbxX20ConfigurationTransaction.Begin);
```

One message accepts at most 64 entries. Duplicate keys retain their order. VALSET
supports RAM, BBR and Flash layer flags; VALDEL supports BBR and Flash only.
VALGET uses one numeric layer (`Ram = 0`, `Bbr = 1`, `Flash = 2`, `Default = 7`),
which differs from the write-layer bit flags.

A null transaction selects version 0 of VALSET or VALDEL. An explicit transaction
action selects version 1, including `None = 0`. Empty entry lists are accepted for
transaction management. These helpers validate one message's fields; receiver
transaction state, layer consistency between messages, supported hardware layers
and configuration validity remain receiver responsibilities. Reading a large
wildcard result requires successive VALGET requests with an updated position.

## Regeneration

From the repository root:

```shell
python3 src/Asv.Gnss/Protocols/Ubx/X20/Configuration/generate_x20_configuration.py
python3 src/Asv.Gnss/Protocols/Ubx/X20/Configuration/generate_x20_configuration.py --check
```

The generator reads the unchanged JSON under `X20/Schema`. Generated output
includes both the source-file SHA-256 and a SHA-256 of ordered key IDs, names and
native types. Tests independently pin the latter to detect missing or renamed
keys, changed IDs and changed native types.
