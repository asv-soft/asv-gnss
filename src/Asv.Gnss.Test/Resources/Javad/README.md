# Public JAVAD GREIS recordings

These are public receiver recordings and upstream regression excerpts, downloaded on 2026-10-22. All file contents are unchanged. Tests read local resources and never download data. The project copies `*.jps` and `*.bin` to the test output with `CopyToOutputDirectory="PreserveNewest"`, following the UBX resources.

| Local file | Bytes | Complete frames | Distinct IDs | Source |
| --- | ---: | ---: | ---: | --- |
| `javad_20110115.jps` | 262144 | 5280 | 62 | [RTKLIB recording](https://github.com/tomojitakasu/RTKLIB/blob/71db0ffa0d9735697c6adfd06fdf766d0e5ce807/test/data/rcvraw/javad_20110115.jps) |
| `gpsd-greis-def.bin` | 16188 | 450 | 40 | [GPSD greis-def.log](https://github.com/ntpsec/gpsd/blob/fc2f63a0c5452565db211e706e8950ad8528ae7d/test/daemon/greis-def.log) |
| `TDCO00SHN_20260826_10epochs.jps` | 45775 | 491 | 48 | [EarthScope observations](https://gitlab.com/earthscope/gnsstools/-/blob/758cdd1ee14604d4c414814ddb6d43f1bc3227f8/codecs/javad/testdata/TDCO00SHN_20260826_10epochs.jps) |
| `TDCO00SHN_20260826_nav.jps` | 14407 | 150 | 11 | [EarthScope navigation](https://gitlab.com/earthscope/gnsstools/-/blob/758cdd1ee14604d4c414814ddb6d43f1bc3227f8/codecs/javad/testdata/TDCO00SHN_20260826_nav.jps) |
| `STEP_20260917_iono.jps` | 220 | 5 | 1 | [EarthScope ionosphere](https://gitlab.com/earthscope/gnsstools/-/blob/758cdd1ee14604d4c414814ddb6d43f1bc3227f8/codecs/javad/testdata/STEP_20260917_iono.jps) |

Total: 338734 bytes. Source URLs are pinned to repository commits. No files were regenerated with the library's serializer or locally trimmed to remove inconvenient frames.

## Provenance and notices

- **RTKLIB**, commit `71db0ffa0d9735697c6adfd06fdf766d0e5ce807`, `test/data/rcvraw/javad_20110115.jps`. The recording identifies a JAVAD DELTA, firmware `3.4.0a0_Q2 Dec,21,2010`, and date 2011-01-15. The upstream file already ends with an incomplete `1p055` frame: byte offset 262056, 88 available bytes, 90 required. This tail is retained. Historical `PM` frames use both `,@` and `@`; `DP`, `NU`, `WE`, and `EN` omit fields present in newer firmware. The package's license section is retained in [Licenses/RTKLIB-LICENSE.txt](Licenses/RTKLIB-LICENSE.txt), from its [readme.txt](https://github.com/tomojitakasu/RTKLIB/blob/71db0ffa0d9735697c6adfd06fdf766d0e5ce807/readme.txt). Copyright (c) 2007–2013, T. Takasu.
- **GPSD**, commit `fc2f63a0c5452565db211e706e8950ad8528ae7d`, `test/daemon/greis-def.log`. Renamed to `.bin` because repository-wide rules ignore `*.log`; all bytes, including the five leading comment lines, are preserved. The comments credit Gary E. Miller and describe a nonstandard `{RT,jps/def,ET}` configuration with SNR and IPR. The comment date is 2018-03-29; the receiver's `RD` messages contain 2018-03-30, which is the value asserted by tests. GPSD's BSD-2-Clause notice is retained in [Licenses/GPSD-COPYING.txt](Licenses/GPSD-COPYING.txt).
- **EarthScope gnsstools**, commit `758cdd1ee14604d4c414814ddb6d43f1bc3227f8`, `codecs/javad/testdata/`. The TDCO observation excerpt contains ten epochs starting at 2026-08-26 00:00:00, including modern `SX` satellite indices. The navigation excerpt is described in upstream [nav_test.go](https://gitlab.com/earthscope/gnsstools/-/blob/758cdd1ee14604d4c414814ddb6d43f1bc3227f8/codecs/javad/nav_test.go) as navigation frames from a GFZ TR_2S recording, plus its first `GT` frame. The STEP excerpt contains five unchanged CRC-valid `IO` frames, including out-of-range time-of-week values; the codec retains raw values without assigning invented timestamps. Its upstream provenance note is copied as [STEP_20260917_iono.md](STEP_20260917_iono.md). The repository's Apache-2.0 license is retained in [Licenses/EarthScope-LICENSE.txt](Licenses/EarthScope-LICENSE.txt). Copyright 2023 EarthScope; the upstream README also credits Henry Berglund, EarthScope Data Services. The upstream root has no `NOTICE`; its `THIRD_PARTY_LICENSES` applies to tidal-loading code, which is not included here.

## Integrity

`JavadRecordedStreamTest` checks these lengths and SHA-256 digests before replay. Downloaded bytes were also checked against the upstream Git blob IDs.

| File | SHA-256 |
| --- | --- |
| `javad_20110115.jps` | `2A500C9BAB93F506E4F90FAC3DCC4E9CA309E57B54F12F6EE494E0929DDBEE26` |
| `gpsd-greis-def.bin` | `77189E70AE922BC690F3B6D8E7D1859805AC9718382265803F057F325D734F02` |
| `TDCO00SHN_20260826_10epochs.jps` | `15072F226B749584DF21F71940F51DC3A8DF187CF8466794825995C761F16D03` |
| `TDCO00SHN_20260826_nav.jps` | `EE2BC9BDB7B07CACE75D4B5B467D9A50EAD1E83AEF2DECD8084F81F5281EF2F0` |
| `STEP_20260917_iono.jps` | `93513B9A695E02D506D647C15A41DDB189ABCE77A010DEF7B5EC5D97F0BBFF45` |

The tests inventory frame boundaries independently of the production parser, require every complete frame in source order, verify exact reserialization, pin dates/times/positions, and preserve the original truncated tail. These fixtures exercise recorded protocol traffic; they do not validate receiver control, hardware timing, or navigation solutions.
