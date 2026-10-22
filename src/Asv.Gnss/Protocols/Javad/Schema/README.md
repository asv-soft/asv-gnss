# GREIS binary payload schemas

`binary-messages.json` is the reviewed source for native binary message declarations in GREIS 4.2.01, sections 3.4.3–3.4.17 and Appendix A.4. It includes the `sj` message omitted from the specification's table of contents and the obsolete integrated/navigation formats. Text messages and command syntax have separate codecs.

Regenerate the checked-in models and factory with Python 3:

```shell
python3 src/Asv.Gnss/Protocols/Javad/Schema/generate_binary.py
python3 src/Asv.Gnss/Protocols/Javad/Schema/generate_binary.py --check
```

The generator does not read the PDF or require additional packages. Review schema edits against the page cited by each record. Generated source is checked in so consumers do not need Python. Do not edit or independently format the generated file.

The same generator produces `IVisitable.Accept(IVisitor)`, cached `Field`/`StructType` metadata, and direct backing-field access for each binary model and nested record. Fixed arrays use `ArrayType`; variable arrays use `ListType` with bounds checked before resizing. Optional records and nullable scalars retain their optional field types. Repeated read-only traversal does not copy arrays or allocate metadata.

Computed lengths and the legacy layout properties declared in `visitorFields` carry `readOnly=true` metadata. A visitor that changes one of these derived values receives `InvalidOperationException`; change the source array or flags instead. A read-only visit preserves absent, short, and oversized storage rather than repairing it. Serialization remains responsible for enforcing wire-layout validity after edits.

Each schema records case-sensitive identifiers, source pages, checksum form, wire types, arrays, shared records, and optional structures. Primitive field values retain the specification's wire units and missing-value sentinels. Length fields that exclusively count an array are derived from that array when serializing; variable measurement arrays derive their element count from the payload length and reject incomplete elements. The factory registers 271 binary identifiers, including seven obsolete formats in addition to `SI`.

The generated declarations for `custom` records and messages are implemented in `Messages/Binary/JavadVariableBinaryMessages.cs` and `JavadLegacyBinaryMessages.cs`. These explicit codecs handle signal-selected ephemeris unions, interleaved IQ samples, spectrum dimensions, logging-history padding, and legacy per-record lengths. They expose typed fields and preserve future record extensions. Top-level extension bytes belong to `JavadMessageBase.ExtensionData`; checksum bytes are excluded from payloads.

The schema's `errata` list records specification inconsistencies. Size checks follow the individual wire fields for S4/SigmaPhi and single-spectrum messages. Optional navigation records use the declared selectors and available payload length. Byte order applies to all multi-byte primitive fields and is retained by each message.

The `legacyOptionalTail` marker preserves confirmed historical layouts from RTKLIB's `test/data/rcvraw/javad_20110115.jps`: `DP` without `Edop` (14-byte body), `NU` without `Nt` (25), `WE` without `Flags` (71), and `EN` without `DAf0` (145). These body sizes include the checksum. GREIS section 3.4.1 permits adding fixed-message fields immediately before that checksum. The generator retains the existing scalar property types and emits a corresponding `HasEdop`, `HasNt`, `HasFlags`, or `HasDAf0` flag, exposed to visitors as `BoolType`. Flags default to true on new messages, preserving the current full layout. Reading an old frame resets its absent scalar to zero and its flag to false; writing the unchanged object preserves the original bytes. To add the trailing field to an old message, assign its value and set its flag to true. A partially present field remains a truncation error. Extension data requires the known trailing field to be present, since an unknown extension cannot be distinguished from that field otherwise.

Satellite measurements remain wire records. Assigning satellite identities and resolving relative measurements requires an epoch assembler using `~~`, `::`, `SI`/`SX`, and reference measurements. The precedence and historical firmware rules on pages 89–92 must be applied by that higher-level consumer. The codecs do not guess receiver firmware, apply historical scale changes, correct carrier phases, or convert these records to RTCM/RINEX.
