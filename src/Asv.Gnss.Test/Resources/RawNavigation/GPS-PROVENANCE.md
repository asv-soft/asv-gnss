# GPS L1 C/A navigation bits recorded on 2025-01-01

The source is an actual GFZ ground-station recording, PRN 1, GPS week 2347.
The selected interval contains the first 125 consecutive six-second subframes of
the day: start tags 00:00:00 through 00:12:24 GPS time, ending at 00:12:30.
Each subframe was independently recorded by six to eight GFZ stations (the archive
stores multiplicity, not individual station identifiers). This covers one entire
25-frame page schedule, with 25 occurrences of every subframe ID 1 through 5.

## Source and attribution

- Original dataset: Rothacher, M. and Beyerle, G. (2008), *GPS C/A Navigation
  Message Data Bits*, GFZ Data Services,
  <https://doi.org/10.1594/GFZ.ISDC.GNSS/GNSS-GPS-1-NAVBIT>.
- Dataset license: Creative Commons Attribution 4.0 International (CC BY 4.0),
  as verified in official DataCite DOI metadata retrieved on 2026-10-22.
  <https://api.datacite.org/dois/10.1594/GFZ.ISDC.GNSS/GNSS-GPS-1-NAVBIT>
  returns `rightsList[0].rightsIdentifier = "cc-by-4.0"` and
  `rightsUri = "https://creativecommons.org/licenses/by/4.0/legalcode"`.
  The metadata was updated on 2026-08-30. The downloaded JSON is preserved as
  `Sources/gfz-datacite.json`.
- Frozen public mirror: Steven J. Murdoch, *The Empty Field That Wasn't: GPS,
  OTAD and Two Decades of Encrypted Broadcasts*,
  <https://github.com/sjmurdoch/gps-special-messages>, commit
  `f1e9e7f1fc3a33513203f995e6801e3a14ba1dd5`.
- Exact download:
  <https://raw.githubusercontent.com/sjmurdoch/gps-special-messages/f1e9e7f1fc3a33513203f995e6801e3a14ba1dd5/data/arrow_test/NAVBIT-GPS-L1CA-2025-001-01-14400.arrow>.
- Mirror repository license: Apache License 2.0, preserved in
  `Licenses/GPS-SPECIAL-MESSAGES-LICENSE.txt`. Retain the original GFZ data
  attribution and license; the mirror's code license does not replace them.
- Original NetCDF filename retained in Arrow metadata:
  `NAVBIT-GPS-L1CA-2025-001-01-14400.nc`.
- Original archive history: `NetCDF file created 02-Jan-2025 21:23:56 by
  navbit2archive_ntrip`.
- The original GFZ HTTPS server timed out from this environment. The downloaded
  file is the research author's lossless Arrow conversion, not a claimed direct
  copy of the NetCDF container.

The mirror conversion is inspectable at
<https://github.com/sjmurdoch/gps-special-messages/blob/f1e9e7f1fc3a33513203f995e6801e3a14ba1dd5/pipeline/03_convert_to_arrow.jl>.
It copies bits 29 down to 0 of each original 32-bit NetCDF word into the Arrow
`navbits` column. It neither decodes payload fields nor recomputes parity. Every
Arrow row contains 38 bytes: 300 native navigation bits, MSB first, followed by
four zero storage padding bits. Columns also retain seconds since midnight GPS
time and observation multiplicity. The original file contains 14,400 rows.

## Local extraction

`extract_gfz_recording.py` reads `Sources/NAVBIT-GPS-L1CA-2025-001-01-14400.arrow` and its metadata, selects
rows 0 through 124, and writes:

- `gfz-gps-l1ca-2025-001-prn01-000000-001224.frames.bin`: 125 unchanged 38-byte
  source rows (4,750 bytes), including each row's four trailing storage bits.
- `gfz-gps-l1ca-2025-001-prn01-000000-001224.bin`: the same 37,500 native bits
  concatenated without per-subframe padding (4,688 bytes, four final zero bits).
- `gfz-gps-l1ca-2025-001-prn01-000000-001224.json`: provenance, hashes, source
  metadata, row offsets, TOW/page IDs, and each original 38-byte row as hex.

It only removes container padding. It does **not** replace recorded parity,
normalize D30* complementing, or serialize model objects. Reproduction requires
Python and the maintenance-only `pyarrow` reader; .NET tests need no Arrow package
or network access.

The independent extraction validation uses the six parity equations of
IS-GPS-200N Table 20-XIV directly, rather than calling the library codec.
All 1,250 selected words pass, including 397 words retaining D30* data
complementing. All 14,400 source subframes were also checked during acquisition;
all passed native chained parity, and 13,945 failed if D30* un-complementing was
omitted. This distinguishes the capture from parity-stripped receiver payloads.

## Independent decoded anchors

- First frame: subframe 1, TOW of following subframe 259206 s, broadcast 10-bit
  week 299, TLM message 299, code 1, URA index 0, health 63, IODC 40,
  Toc 266400 s, integrity/alert false, anti-spoof true.
- Second frame: subframe 2, IODE 40, Toe 266400 s, AODO 31.
- Row 8: subframe 4 / SV ID 55, special-message bytes
  `5445585452414E2E494A273932462F55472B50384E4F`.
- Row 13: subframe 4 / SV ID 56, WNt 43, current leap offset 18,
  WN_LSF 137, DN 7, future leap offset 18.
- Row 113: subframe 4 / SV ID 52, NMCT availability 2, all 30 ERDs are -32
  (invalid/unavailable sentinel).
- The scheduled subframe-5 page for PRN 22 carries SV ID 0 in this recording.
  Preserve and test it as the actual transmitted dummy page; do not rewrite it to 22.
- First TOW count is 43201; the last is 43325. All increment by one; native
  subframe IDs cycle 1,2,3,4,5. Exact SV-ID sequences are in the manifest.

## SHA-256

- Original Arrow, 153250 bytes:
  `1f1a0798833f655c3d350b1cd3662640978a1ef4e767efefbf41f312e6131c01`
- Selected source rows, 4750 bytes:
  `bb376e8c5e58443d1dd8e29f695487ee47e8dfc6e80798e92f79d11a08f2e8fa`
- Continuous native bits, 4688 bytes:
  `8fb240904f8bd56c1bfc7606c80ed82101c26c389eb8df74c7cd8c079562c958`
