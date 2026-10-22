# GPSD GLONASS native navigation recording

`Sources/gpsd-ublox-glo.bin` is an unchanged copy of `ublox-glo.log`, a public receiver recording from the GPSD project, submitted by Matwey V. Kornilov. Its header identifies a full superframe capture made in Moscow, Russia, on 2023-06-28 using u-blox firmware ROM SPG 5.10 (7b202e), hardware 000A0000, protocol 34.10.

- Pinned source: https://github.com/ntpsec/gpsd/blob/fc2f63a0c5452565db211e706e8950ad8528ae7d/test/daemon/ublox-glo.log
- Download: https://raw.githubusercontent.com/ntpsec/gpsd/fc2f63a0c5452565db211e706e8950ad8528ae7d/test/daemon/ublox-glo.log
- License: BSD-2-Clause, see the unchanged `Licenses/GPSD-COPYING.txt` from the same commit. The log itself carries `Copyright 2010 by the GPSD project`; retain this original notice even though the capture date is 2023.
- Source size: 168595 bytes.
- Source SHA-256: `767aa0fb33233c56d592bcb7ddce25d7ac88cbb768cb176296face092703f5b2`.

`gpsd-glonass-sv12-superframe2158.bin` contains one complete recorded GLONASS L1OF superframe from satellite slot 12, frequency channel -1: five frames, each with strings 1 through 15 in acquisition order. The 75 strings retain every received navigation and Hamming bit. They are concatenated MSB first without inter-string byte padding: 6375 meaningful bits, then one zero padding bit to complete the final byte (797 bytes total).

- Output SHA-256: `21d2822aa684c57c96b412691cf2286ec354d3da1acfb66b63decdf40adbcde3`.
- First UBX packet offset: 12766.
- Last UBX packet offset: 95590.
- Full extraction provenance, including all 75 packet offsets and original string bytes: `gpsd-glonass-sv12-superframe2158.json`.
- Offline reproduction: `python3 extract_glonass.py Sources/gpsd-ublox-glo.bin <output-directory>`.

Extraction removes only the UBX envelope, the receiver word padding, and unrelated receiver messages. It does not rebuild, repair, or substitute Hamming bits, and it does not use `Asv.Gnss` serialization. All 584 GLONASS strings in the original capture pass an independent implementation of GLONASS ICD edition 5.1 (2008), section 4.7, table 4.13; the extraction script verifies this before writing the selected stream. Source SHA-256, UBX checksums, satellite ID, ordered frame/string IDs, and output SHA-256 can all be checked offline.

The superframe number 2158 and frame numbers 1–5 come from the fourth UBX-RXM-SFRBX receiver word. They are receiver-inferred metadata, not extra bits transmitted by the satellite. The fixture stores only the 85 transmitted data/check bits per string; the context remains in the manifest. The u-blox integration manual documents the layout in section 3.13.1.3, including the 11 padding bits in word 3 and the metadata in word 4: https://content.u-blox.com/sites/default/files/documents/ZED-F9L_IntegrationManual_UBX-23006074.pdf.

This is a receiver-decoded navigation-bit recording, not RF/IQ samples. GLONASS bi-binary/relative encoding and the separate time mark have already been removed by the receiver and cannot be recovered from this source. The recorded 85-bit strings, including their original Hamming code, are the native navigation-message input accepted by `RawGlonassProtocol`.

The first frame's string 1 reports GLONASS local time 17:52:30 (64350 seconds). String 4 contains day number 1275, slot 12 and GLONASS-M indicator 1; string 5 contains day number 1275 and four-year interval 7, agreeing with 2023-06-28. Frame 5/string 14 reports B1 = -38 / 1024 s, B2 = 18 / 65536 s/day, KP = 0; frame 5/string 15 has ln = 0 and nonzero reserved bits, providing a real preservation case.
