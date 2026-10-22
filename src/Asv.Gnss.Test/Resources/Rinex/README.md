# Public RINEX regression fixtures

These small public files are checked in so tests are deterministic and require no internet connection. Files are unchanged from the pinned upstream revisions below; SHA-256 covers the exact checked-in bytes. Downloaded on 2026-10-22. Fixtures are format examples / receiver samples, not current navigation data.

## Sources and licenses

- GeoRinex revision `8d1210a0f1ada71ff7b8d0484cfaf22ff154a38e`: [repository](https://github.com/geospace-code/georinex/tree/8d1210a0f1ada71ff7b8d0484cfaf22ff154a38e/src/georinex/tests/data). MIT license is reproduced in `LICENSE.georinex.txt`. The gAGE/gLAB attribution in file comments is preserved.
- NAV Solutions data revision `d00db683b22c25d30d14ff2e085f76157ab76c6f`: [repository](https://github.com/nav-solutions/data/tree/d00db683b22c25d30d14ff2e085f76157ab76c6f). MPL-2.0 license is reproduced in `LICENSE.nav-solutions.txt`. The RINEX 4.02 file explicitly attributes examples from IGS/RTCM specification tables A12, A18, A32 and A41.

| Local file | Upstream path | Format | SHA-256 |
|---|---|---|---|
| `cari0010.07m` | `MET/V2/cari0010.07m` | 2.10 METEOROLOGICAL DATA | `353559bb2489a32a93533c3ef174fe4d7f293fb0de11ddf6324bdad9dc29b0cb` |
| `demo.10n` | `src/georinex/tests/data/demo.10n` | 2.11 N: GPS NAV. MESSAGE | `ff51b0908ea902206a94ccd0d04c1c8724a8965bac726852c37afabccd7d08f0` |
| `demo.10o` | `src/georinex/tests/data/demo.10o` | 2.11 OBSERVATION DATA | `b91455641e925e3a1884413fec3ccb83e0f99154815816d578b559f9af689013` |
| `demo_nav3.10n` | `src/georinex/tests/data/demo_nav3.10n` | 3.01 N: GNSS NAV DATA | `d837905150b7d178cd8a433959cf237d651ed213762d4d04f8ebba70109fc6fe` |
| `met4-example.txt` | `MET/V4/example1.txt` | 4.00 METEOROLOGICAL DATA | `5c95c1fe3d7f97cd568c804c4e64f9ea319c77f7205779e9603c004e39758792` |
| `obs3.01gage.10o` | `src/georinex/tests/data/obs3.01gage.10o` | 3.01 OBSERVATION DATA | `b3fe193593ee0584bdea9faf2b0c8406d181779fcaa39646b5684c9e2be31377` |
| `rinex402_examples_MN.rnx` | `NAV/V4/rinex402_examples_MN.rnx` | 4.02 N: GNSS NAV DATA | `ba4b549010bc97eaf61c6102a09052668d3b29c423688f6bc5b4b30f7e433442` |
| `demo_nav3.17n` | `src/georinex/tests/data/demo_nav3.17n` | 3.x navigation | `fe86139b9b9895e77ccae8058eb2d294670736e93299b4db01d2392c779c5ac9` |
| `BRDM00DLR_R_20130010000_01D_MN.rnx` | `src/georinex/tests/data/BRDM00DLR_R_20130010000_01D_MN.rnx` | 3.x navigation | `f70160c0bc3d26cf4878fe5aecc333befc8d2c7012372a1318aa21f1f1b3fe76` |
| `galileo3.15n` | `src/georinex/tests/data/galileo3.15n` | 3.x navigation | `e04043ebf1d64a6e69c0597308578c618dd27d5a8f34bac443848260f7ec0e77` |
| `brdc0320.16l.txt` | `src/georinex/tests/data/brdc0320.16l.txt` | 3.03 Galileo navigation | `db3ec78eb0e62fde36628610c3c86c5c0c30c99b019b627c985c6c310dd6967b` |

## Independent expected observations

- `demo.10o`: RINEX 2.11 mixed observations; 38 header lines, 2 epochs at 2010-03-05 00:00:00 / 00:00:30, respectively 14 / 8 satellites. First G13 L1 value 121367582.205, LLI 0, SSI 8; next value 94572134.492. Receiver clock offset -0.12345 s.
- `obs3.01gage.10o`: RINEX 3.01 mixed observations; 66 header lines, same 2 epochs and 14 / 8 satellites; G13 first two observations 121367582.205 / 94572134.492. Receiver clock offset -0.123456789012 s. Short GLONASS/SBAS rows and space-padded satellite `G 7` exercise nonuniform layouts.
- `demo.10n`: RINEX 2.11 GPS navigation; 11 header lines, 2 ephemerides, first G06 at 1999-09-02 19:00:00, clock bias -0.000839701388031 s, 31 numeric values per record.
- `demo_nav3.10n`: RINEX 3.01 SBAS navigation; 11 header lines, 2 ephemerides, first S22 at 2010-10-18 00:01:04, first clock coefficient -9.872019290924e-8, 15 numeric values per record.
- `cari0010.07m`: RINEX 2.10 meteorology; 10 header lines, 3 epochs at 1996-04-01 00:00:15 / 30 / 45. First PR/TD/HR values 987.1 / 10.6 / 89.5.
- `met4-example.txt`: RINEX 4.00 meteorology; 12 header lines, 5 epochs from 2021-01-07 00:00:00 at 30 s spacing. First PR/TD/HR values 993.3 / 23.0 / 90.0.
- `rinex402_examples_MN.rnx`: RINEX 4.02 mixed navigation; 6 header lines and 34 records: 9 EPH, 13 STO, 5 EOP, 7 ION. First EPH G04 LNAV at 2019-03-14 04:00:00 has clock bias 1.330170780420e-4. Includes GLONASS L1OC/L3OC, NavIC L1NV, KLOB and NEQN message subtypes.

Expected values are transcribed from the source text independently of the parser. Tests must not download fixtures or infer expected counts from the implementation under test.

Additional NAV3 files: `demo_nav3.17n` has 4 GPS ephemerides; `BRDM00DLR_R_20130010000_01D_MN.rnx` has 10 records (4 GPS, 4 GLONASS, 2 QZSS), first G01 at 2013-01-01 02:00:00 with bias 2.907030284405e-4, first R01 clock -1.750094816089e-4; `galileo3.15n` has 3 E12 records, first 2015-06-19 02:10:00 bias -0.00138392508961.

`galileo3.15n` is intentionally retained as an invalid public fixture: line 29 contains `. 000000000000D+00` with an embedded blank inside a numeric field. Strict reading must reject it. `brdc0320.16l.txt` is the valid Galileo positive fixture: 10 E02/E03/E04/E10/E11/E12/E18/E19/E26/E27 ephemerides at 2016-01-31 23:20:00, first Cuc -9.313225746155e-9 and eccentricity 8.999999961816e-3.
