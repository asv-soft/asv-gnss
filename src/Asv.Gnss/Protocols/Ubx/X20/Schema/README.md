# u-blox X20 HPG 2.11 source profile

`u-blox-X20-HPG-2.11.json` is the unchanged official interface-description JSON
for ZED-X20P X20 HPG 2.11, protocol version 50.11.

- Upstream revision: `02c3c9302cacf1b2fb17eeb10a6a4e0b7ef2379e`.
- SHA-256: `22bff43945a5622f6a2e44a84e781796456923095fa0c250e65a5b1b7ee47d46`.
- [Pinned JSON](https://github.com/u-blox/u-blox-X20-interface-description-json/blob/02c3c9302cacf1b2fb17eeb10a6a4e0b7ef2379e/HPG/ZED-X20P/u-blox-X20-HPG-2.11.json).
- [Official interface description, UBXDOC-304424225-21617 R01](https://content.u-blox.com/sites/default/files/documents/u-blox-X20-HPG-2.11_InterfaceDescription_UBXDOC-304424225-21617.pdf).
- [Pinned upstream usage notice](https://github.com/u-blox/u-blox-X20-interface-description-json/blob/02c3c9302cacf1b2fb17eeb10a6a4e0b7ef2379e/README.md), copied verbatim to [UPSTREAM-NOTICE.md](UPSTREAM-NOTICE.md).

The JSON explicitly marks messages and configuration as filtered for this
receiver/firmware. Its UBX section contains 163 variants, 101 class/ID pairs and
11 message classes. Its configuration section contains 826 items in 38 groups.
The JSON also contains NMEA, RTCM and SPARTN descriptions; the X20 generator here
generates its UBX section only. Other protocols keep their existing registrations.

## Generation and documented normalizations

The source JSON is not edited. The model generator applies these implementation
rules and narrowly documented classifier corrections:

- Class, message and variant identifiers become unique `UbxX20...` C# model
  names. Exact source labels, variant identifiers and delivery modes remain in
  `UbxX20Protocol.Messages`. A variant name such as `DATA0` is not interpreted as
  the value of a wire version byte; discriminator defaults come from field
  descriptions. In particular, MON-SYS `DATA0` has `MsgVer = 1` and CFG-VALGET
  `DATA0` has `Version = 1`, as stated by their source field descriptions. These
  defaults are not inferred from the `DATA0` suffix.
- Native signedness, integer widths, floating-point types and scales are
  preserved. Scale/unit text becomes XML and visitor metadata; values are not
  converted to physical units.
- Repeated homogeneous primitive groups become arrays. Multi-field groups
  become typed record arrays; optional groups become nullable typed records.
  Separate scalar counters are read-only properties derived from array lengths.
  Packed bit-field counters remain in their wire container and must match their
  corresponding arrays.
- `CH` fields remain raw byte arrays, preserving padding, NUL and bytes outside
  ASCII. Read-only `...Ascii` properties are display aliases. Bit-field accessors
  are aliases of their numeric wire containers. Visitors traverse canonical wire
  fields once, without duplicating those aliases.
- The published RXM-PMREQ classifier's eight-byte leaf omits its variant name.
  The generator maps that leaf to source variant `0`, as specified by the PDF's
  short power-management request layout.
- The published UPD-SOS classifier probes a command byte and omits the empty
  poll request. The generator explicitly recognizes an empty payload as source
  variant `POLL0`, as specified by the PDF.

The configuration generator preserves source key IDs, names, native types,
scale/unit expressions, descriptions and constants. Empty named-constant values
remain empty strings, rather than inferred zeroes. Generated ID constants use
PascalCase C# group/item names; metadata retains the official spelling. Known
configuration values use their exact CLR native types, while unknown concrete
keys with valid storage sizes retain raw bytes. See
[configuration helpers](../Configuration/README.md) for key-list wildcards,
layer values, transaction actions and payload boundaries.

The exposed NAV-STATUS `indoorDet` and `selectiveL5` bit descriptions retain the
PDF's protocol-version restriction of 52.00 or later. Their presence in the
source for protocol 50.11 does not make those features available on 50.11 firmware.

## Regenerate

Run both generators from the repository root. They use the vendored source and
Python's standard library; no network access or third-party Python packages are
needed.

```shell
python3 src/Asv.Gnss/Protocols/Ubx/X20/Schema/generate_ubx_x20.py
python3 src/Asv.Gnss/Protocols/Ubx/X20/Configuration/generate_x20_configuration.py
```

Verify that generated files match the source without changing files:

```shell
python3 src/Asv.Gnss/Protocols/Ubx/X20/Schema/generate_ubx_x20.py --check
python3 src/Asv.Gnss/Protocols/Ubx/X20/Configuration/generate_x20_configuration.py --check
```

The model output is under `X20/Generated`; configuration output is
`X20/Configuration/UbxX20ConfigurationCatalog.Generated.cs`. The configuration
catalog records both the source-file SHA-256 and a hash of ordered key IDs, names
and native types. Tests independently pin the latter, the model/variant count,
and a hash of every UBX ID, label, variant and direction from the official JSON.
Keep the documentation's variant table synchronized with the official source
and the generated `UbxX20GeneratedFactory.g.cs` manifest after a deliberate
source-profile update.
