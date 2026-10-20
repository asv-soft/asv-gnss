# Javad / GREIS status

`src/Asv.Gnss/Parsers/Javad` contains a reference document, `Docs/GREIS-4.2.01.pdf`, but no C# parser, message models, or registration extension. Javad/GREIS decoding is not implemented in the current source tree.

The presence of a vendor specification does not make that protocol available through `Asv.IO`.

## Specification message catalog

Source: `Parsers/Javad/Docs/GREIS-4.2.01.pdf`, GREIS v4.2.01, Chapter 3.4. The table expands each native receiver-message identifier in the contents on PDF pp. 4-9, including measurement families with multiple IDs. The obsolete SI form is described in the compatibility section. IDs are case-sensitive. PDF page and printed page numbering coincide in this document. Foreign NMEA/RTCM/CMR messages are wrappers or separate wire protocols; consult their protocol pages for library support. None of the native GREIS messages below has a parser or payload model in this checkout.

**Status:** **Model present** means a concrete payload model exists in the current checkout; it does not certify complete field, revision, encoding, or firmware coverage. **Partial** identifies a known incomplete decoder. **Not implemented** means no matching concrete model was found. **Default** reports whether the standard registration adds that model. Models outside the default set require explicit registration. See the limitations above before transmitting messages.

Page references below are **PDF page numbers (1-based)**, including covers and front matter.

| GREIS ID | Message | Status | C# model | Default | PDF page |
| --- | --- | --- | --- | --- | --- |
| `JP` | File Identifier | Not implemented | - | No | 73 |
| `MF` | Messages Format | Not implemented | - | No | 74 |
| `~~` | Receiver Time | Not implemented | - | No | 74 |
| `::` | Epoch Time | Not implemented | - | No | 75 |
| `RD` | Receiver Date | Not implemented | - | No | 75 |
| `TO` | Reference Time to Receiver Time Offset | Not implemented | - | No | 75 |
| `DO` | Derivative of Receiver Time Offset | Not implemented | - | No | 75 |
| `BP` | Rough Accuracy of Time Approximation | Not implemented | - | No | 76 |
| `GT` | GPS Time | Not implemented | - | No | 76 |
| `GO` | GPS to Receiver Time Offset | Not implemented | - | No | 76 |
| `NT` | GLONASS Time | Not implemented | - | No | 76 |
| `NO` | GLONASS to Receiver Time Offset | Not implemented | - | No | 76 |
| `EO` | GALILEO to Receiver Time Offset | Not implemented | - | No | 76 |
| `WO` | SBAS to Receiver Time Offset | Not implemented | - | No | 77 |
| `QO` | QZSS to Receiver Time Offset | Not implemented | - | No | 77 |
| `CO` | BeiDou to Receiver Time Offset | Not implemented | - | No | 77 |
| `Io` | IRNSS to Receiver Time Offset | Not implemented | - | No | 77 |
| `UO` | GPS UTC Time Parameters | Not implemented | - | No | 77 |
| `WU` | SBAS UTC Time Parameters | Not implemented | - | No | 78 |
| `EU` | GALILEO UTC and GPS Time Parameters | Not implemented | - | No | 78 |
| `QU` | QZSS UTC Time Parameters | Not implemented | - | No | 78 |
| `CU` | BeiDou UTC Time Parameters | Not implemented | - | No | 79 |
| `IU` | IRNSS UTC Time Parameters | Not implemented | - | No | 79 |
| `NU` | GLONASS UTC and GPS Time Parameters | Not implemented | - | No | 79 |
| `ST` | Solution Time-Tag | Not implemented | - | No | 79 |
| `PO` | Cartesian Position | Not implemented | - | No | 80 |
| `Po` | (PoWgs,PoLoc) Cartesian Position in Specific System | Not implemented | - | No | 80 |
| `VE` | Cartesian Velocity | Not implemented | - | No | 80 |
| `PV` | Cartesian Position and Velocity | Not implemented | - | No | 80 |
| `PG` | Geodetic Position | Not implemented | - | No | 81 |
| `Pg` | (PgWgs,PgLoc) Geodetic Position in Specific System | Not implemented | - | No | 81 |
| `VG` | Geodetic Velocity | Not implemented | - | No | 81 |
| `SG` | Position and Velocity RMS Errors | Not implemented | - | No | 81 |
| `mp` | Position in Local Plane | Not implemented | - | No | 82 |
| `bp` | Reference Station Position in Local Plane | Not implemented | - | No | 82 |
| `DP` | Dilution of Precision (DOP) | Not implemented | - | No | 83 |
| `SP` | Position Covariance Matrix | Not implemented | - | No | 83 |
| `SV` | Velocity Covariance Matrix | Not implemented | - | No | 83 |
| `BL` | Baseline | Not implemented | - | No | 83 |
| `bL` | Attitude Baselines | Not implemented | - | No | 84 |
| `mR` | Attitude Full Rotation Matrix | Not implemented | - | No | 84 |
| `PS` | Position Statistics | Not implemented | - | No | 84 |
| `PT` | Time of Continuous Position Computation | Not implemented | - | No | 85 |
| `SX` | Extended Satellite Indices | Not implemented | - | No | 92 |
| `AN` | Antenna Names | Not implemented | - | No | 92 |
| `NN` | GLONASS Satellite System Numbers | Not implemented | - | No | 93 |
| `EL` | Satellite Elevations | Not implemented | - | No | 93 |
| `AZ` | Satellite Azimuths | Not implemented | - | No | 93 |
| `RX` | Pseudo-ranges | Not implemented | - | No | 93 |
| `RC` | Pseudo-ranges | Not implemented | - | No | 93 |
| `R1` | Pseudo-ranges | Not implemented | - | No | 93 |
| `R2` | Pseudo-ranges | Not implemented | - | No | 93 |
| `R3` | Pseudo-ranges | Not implemented | - | No | 93 |
| `R5` | Pseudo-ranges | Not implemented | - | No | 93 |
| `Rl` | Pseudo-ranges | Not implemented | - | No | 93 |
| `rx` | Integer Pseudo-ranges | Not implemented | - | No | 94 |
| `rc` | Integer Pseudo-ranges | Not implemented | - | No | 94 |
| `r1` | Integer Pseudo-ranges | Not implemented | - | No | 94 |
| `r2` | Integer Pseudo-ranges | Not implemented | - | No | 94 |
| `r3` | Integer Pseudo-ranges | Not implemented | - | No | 94 |
| `r5` | Integer Pseudo-ranges | Not implemented | - | No | 94 |
| `rl` | Integer Pseudo-ranges | Not implemented | - | No | 94 |
| `CR` | Relative Pseudo-ranges | Not implemented | - | No | 95 |
| `1R` | Relative Pseudo-ranges | Not implemented | - | No | 95 |
| `2R` | Relative Pseudo-ranges | Not implemented | - | No | 95 |
| `3R` | Relative Pseudo-ranges | Not implemented | - | No | 95 |
| `5R` | Relative Pseudo-ranges | Not implemented | - | No | 95 |
| `lR` | Relative Pseudo-ranges | Not implemented | - | No | 95 |
| `cr` | Integer Relative Pseudo-ranges | Not implemented | - | No | 95 |
| `1r` | Integer Relative Pseudo-ranges | Not implemented | - | No | 95 |
| `2r` | Integer Relative Pseudo-ranges | Not implemented | - | No | 95 |
| `3r` | Integer Relative Pseudo-ranges | Not implemented | - | No | 95 |
| `5r` | Integer Relative Pseudo-ranges | Not implemented | - | No | 95 |
| `lr` | Integer Relative Pseudo-ranges | Not implemented | - | No | 95 |
| `cm` | Pseudo-range Corrections | Not implemented | - | No | 96 |
| `1m` | Pseudo-range Corrections | Not implemented | - | No | 96 |
| `2m` | Pseudo-range Corrections | Not implemented | - | No | 96 |
| `3m` | Pseudo-range Corrections | Not implemented | - | No | 96 |
| `5m` | Pseudo-range Corrections | Not implemented | - | No | 96 |
| `lm` | Pseudo-range Corrections | Not implemented | - | No | 96 |
| `CC` | Smoothing Corrections | Not implemented | - | No | 96 |
| `C1` | Smoothing Corrections | Not implemented | - | No | 96 |
| `C2` | Smoothing Corrections | Not implemented | - | No | 96 |
| `C3` | Smoothing Corrections | Not implemented | - | No | 96 |
| `C5` | Smoothing Corrections | Not implemented | - | No | 96 |
| `Cl` | Smoothing Corrections | Not implemented | - | No | 96 |
| `cc` | Smoothing Corrections | Not implemented | - | No | 97 |
| `c1` | Smoothing Corrections | Not implemented | - | No | 97 |
| `c2` | Smoothing Corrections | Not implemented | - | No | 97 |
| `c3` | Smoothing Corrections | Not implemented | - | No | 97 |
| `c5` | Smoothing Corrections | Not implemented | - | No | 97 |
| `cl` | Smoothing Corrections | Not implemented | - | No | 97 |
| `PC` | Carrier Phases | Not implemented | - | No | 97 |
| `P1` | Carrier Phases | Not implemented | - | No | 97 |
| `P2` | Carrier Phases | Not implemented | - | No | 97 |
| `P3` | Carrier Phases | Not implemented | - | No | 97 |
| `P5` | Carrier Phases | Not implemented | - | No | 97 |
| `Pl` | Carrier Phases | Not implemented | - | No | 97 |
| `pc` | Integer Carrier Phases | Not implemented | - | No | 97 |
| `p1` | Integer Carrier Phases | Not implemented | - | No | 97 |
| `p2` | Integer Carrier Phases | Not implemented | - | No | 97 |
| `p3` | Integer Carrier Phases | Not implemented | - | No | 97 |
| `p5` | Integer Carrier Phases | Not implemented | - | No | 97 |
| `pl` | Integer Carrier Phases | Not implemented | - | No | 97 |
| `CP` | Relative Carrier Phases | Not implemented | - | No | 98 |
| `1P` | Relative Carrier Phases | Not implemented | - | No | 98 |
| `2P` | Relative Carrier Phases | Not implemented | - | No | 98 |
| `3P` | Relative Carrier Phases | Not implemented | - | No | 98 |
| `5P` | Relative Carrier Phases | Not implemented | - | No | 98 |
| `lP` | Relative Carrier Phases | Not implemented | - | No | 98 |
| `cp` | Integer Relative Carrier Phases | Not implemented | - | No | 98 |
| `1p` | Integer Relative Carrier Phases | Not implemented | - | No | 98 |
| `2p` | Integer Relative Carrier Phases | Not implemented | - | No | 98 |
| `3p` | Integer Relative Carrier Phases | Not implemented | - | No | 98 |
| `5p` | Integer Relative Carrier Phases | Not implemented | - | No | 98 |
| `lp` | Integer Relative Carrier Phases | Not implemented | - | No | 98 |
| `cf` | Phase Corrections | Not implemented | - | No | 99 |
| `1f` | Phase Corrections | Not implemented | - | No | 99 |
| `2f` | Phase Corrections | Not implemented | - | No | 99 |
| `3f` | Phase Corrections | Not implemented | - | No | 99 |
| `5f` | Phase Corrections | Not implemented | - | No | 99 |
| `lf` | Phase Corrections | Not implemented | - | No | 99 |
| `DX` | Doppler | Not implemented | - | No | 99 |
| `DC` | Doppler | Not implemented | - | No | 99 |
| `D1` | Doppler | Not implemented | - | No | 99 |
| `D2` | Doppler | Not implemented | - | No | 99 |
| `D3` | Doppler | Not implemented | - | No | 99 |
| `D5` | Doppler | Not implemented | - | No | 99 |
| `Dl` | Doppler | Not implemented | - | No | 99 |
| `0d` | Relative Doppler | Not implemented | - | No | 100 |
| `1d` | Relative Doppler | Not implemented | - | No | 100 |
| `2d` | Relative Doppler | Not implemented | - | No | 100 |
| `3d` | Relative Doppler | Not implemented | - | No | 100 |
| `5d` | Relative Doppler | Not implemented | - | No | 100 |
| `ld` | Relative Doppler | Not implemented | - | No | 100 |
| `EC` | SNR | Not implemented | - | No | 100 |
| `E1` | SNR | Not implemented | - | No | 100 |
| `E2` | SNR | Not implemented | - | No | 100 |
| `E3` | SNR | Not implemented | - | No | 100 |
| `E5` | SNR | Not implemented | - | No | 100 |
| `El` | SNR | Not implemented | - | No | 100 |
| `CE` | SNR x 4 | Not implemented | - | No | 101 |
| `1E` | SNR x 4 | Not implemented | - | No | 101 |
| `2E` | SNR x 4 | Not implemented | - | No | 101 |
| `3E` | SNR x 4 | Not implemented | - | No | 101 |
| `5E` | SNR x 4 | Not implemented | - | No | 101 |
| `lE` | SNR x 4 | Not implemented | - | No | 101 |
| `s0` | SNR x 256 | Not implemented | - | No | 101 |
| `s1` | SNR x 256 | Not implemented | - | No | 101 |
| `s2` | SNR x 256 | Not implemented | - | No | 101 |
| `s3` | SNR x 256 | Not implemented | - | No | 101 |
| `s5` | SNR x 256 | Not implemented | - | No | 101 |
| `sl` | SNR x 256 | Not implemented | - | No | 101 |
| `j0` | Data SNR x 256 | Not implemented | - | No | 101 |
| `j1` | Data SNR x 256 | Not implemented | - | No | 101 |
| `j2` | Data SNR x 256 | Not implemented | - | No | 101 |
| `j3` | Data SNR x 256 | Not implemented | - | No | 101 |
| `j5` | Data SNR x 256 | Not implemented | - | No | 101 |
| `jl` | Data SNR x 256 | Not implemented | - | No | 101 |
| `FC` | Signal Lock Loop Flags | Not implemented | - | No | 101 |
| `F1` | Signal Lock Loop Flags | Not implemented | - | No | 101 |
| `F2` | Signal Lock Loop Flags | Not implemented | - | No | 101 |
| `F3` | Signal Lock Loop Flags | Not implemented | - | No | 101 |
| `F5` | Signal Lock Loop Flags | Not implemented | - | No | 101 |
| `Fl` | Signal Lock Loop Flags | Not implemented | - | No | 101 |
| `ec` | Raw Inphases (I) | Not implemented | - | No | 103 |
| `e1` | Raw Inphases (I) | Not implemented | - | No | 103 |
| `e2` | Raw Inphases (I) | Not implemented | - | No | 103 |
| `e3` | Raw Inphases (I) | Not implemented | - | No | 103 |
| `e5` | Raw Inphases (I) | Not implemented | - | No | 103 |
| `qc` | Raw Quadratures (Q) | Not implemented | - | No | 103 |
| `q1` | Raw Quadratures (Q) | Not implemented | - | No | 103 |
| `q2` | Raw Quadratures (Q) | Not implemented | - | No | 103 |
| `q3` | Raw Quadratures (Q) | Not implemented | - | No | 103 |
| `q5` | Raw Quadratures (Q) | Not implemented | - | No | 103 |
| `x0` | S4 | Not implemented | - | No | 103 |
| `x1` | S4 | Not implemented | - | No | 103 |
| `x2` | S4 | Not implemented | - | No | 103 |
| `x3` | S4 | Not implemented | - | No | 103 |
| `x5` | S4 | Not implemented | - | No | 103 |
| `x4` | S4 | Not implemented | - | No | 103 |
| `y0` | SigmaPhi | Not implemented | - | No | 103 |
| `y1` | SigmaPhi | Not implemented | - | No | 103 |
| `y2` | SigmaPhi | Not implemented | - | No | 103 |
| `y3` | SigmaPhi | Not implemented | - | No | 103 |
| `y5` | SigmaPhi | Not implemented | - | No | 103 |
| `y4` | SigmaPhi | Not implemented | - | No | 103 |
| `IQ` | 1-millisecond I and Q Samples | Not implemented | - | No | 104 |
| `TC` | CA/L1 Continuous Tracking Time | Not implemented | - | No | 104 |
| `SS` | Satellite Navigation Status | Not implemented | - | No | 104 |
| `ID` | Ionospheric Delays | Not implemented | - | No | 105 |
| `rr` | Satellite Range Residuals | Not implemented | - | No | 105 |
| `vr` | Satellite Velocity Residuals | Not implemented | - | No | 105 |
| `GA` | GPS Almanac | Not implemented | - | No | 105 |
| `EA` | GALILEO Almanac | Not implemented | - | No | 106 |
| `QA` | QZSS Almanac | Not implemented | - | No | 106 |
| `CA` | BeiDou Almanac | Not implemented | - | No | 106 |
| `IA` | IRNSS Almanac | Not implemented | - | No | 106 |
| `NA` | GLONASS Almanac | Not implemented | - | No | 107 |
| `WA` | SBAS Almanac | Not implemented | - | No | 107 |
| `GE` | GPS Ephemeris | Not implemented | - | No | 108 |
| `EN` | GALILEO Ephemeris | Not implemented | - | No | 109 |
| `QE` | QZSS Ephemeris | Not implemented | - | No | 110 |
| `CN` | BeiDou Ephemeris | Not implemented | - | No | 110 |
| `NE` | GLONASS Ephemeris | Not implemented | - | No | 111 |
| `WE` | SBAS Ephemeris | Not implemented | - | No | 112 |
| `IE` | IRNSS Ephemeris | Not implemented | - | No | 112 |
| `gd` | GPS Raw Navigation Data | Not implemented | - | No | 113 |
| `qd` | QZSS Raw Navigation Data | Not implemented | - | No | 114 |
| `lD` | GLONASS Raw Navigation Data | Not implemented | - | No | 114 |
| `WD` | SBAS Raw Navigation Data | Not implemented | - | No | 114 |
| `ED` | GALILEO Raw Navigation Data | Not implemented | - | No | 115 |
| `cd` | BeiDou Raw Navigation Data | Not implemented | - | No | 116 |
| `id` | IRNSS Raw Navigation Data | Not implemented | - | No | 117 |
| `xd` | QZSS L6 Raw Navigation Data | Not implemented | - | No | 117 |
| `sp` | Spectrum | Not implemented | - | No | 118 |
| `sP` | Extended Spectrum | Not implemented | - | No | 118 |
| `Sp` | Single Spectrum | Not implemented | - | No | 118 |
| `ms` | Modem Spectrum | Not implemented | - | No | 119 |
| `gC` | GLONASS Delays | Not implemented | - | No | 119 |
| `g1` | GLONASS Delays | Not implemented | - | No | 119 |
| `g2` | GLONASS Delays | Not implemented | - | No | 119 |
| `g3` | GLONASS Delays | Not implemented | - | No | 119 |
| `gR` | Code Delays of Receiver RF Bands | Not implemented | - | No | 120 |
| `MR` | Rotation Matrix | Not implemented | - | No | 121 |
| `mr` | Rotation Matrix and Vectors | Not implemented | - | No | 121 |
| `AR` | Rotation Angles | Not implemented | - | No | 121 |
| `AV` | Angular Velocities | Not implemented | - | No | 121 |
| `IM` | Inertial Measurements | Not implemented | - | No | 122 |
| `MA` | Accelerometer and Magnetometer Measurements | Not implemented | - | No | 122 |
| `fA` | Accelerometer Raw Data | Not implemented | - | No | 122 |
| `lA` | Accelerometer Raw Data | Not implemented | - | No | 123 |
| `tA` | Accelerometer and Gyroscope calibrated Data | Not implemented | - | No | 123 |
| `aV` | Gyroscope Raw Data | Not implemented | - | No | 123 |
| `mA` | Magnetometer Raw Data | Not implemented | - | No | 123 |
| `dV` | Acceleration in ENU | Not implemented | - | No | 124 |
| `PE` | WGS84 Coordinates of the Pole End | Not implemented | - | No | 124 |
| `pV` | Local Coordinates of Antenna Phase Center | Not implemented | - | No | 124 |
| `pE` | Tilt-compensated Solution (full) | Not implemented | - | No | 124 |
| `pe` | Tilt-compensated Solution (short) | Not implemented | - | No | 126 |
| `XA` | External Event | Not implemented | - | No | 127 |
| `XB` | External Event | Not implemented | - | No | 127 |
| `ZA` | PPS Offset | Not implemented | - | No | 127 |
| `ZB` | PPS Offset | Not implemented | - | No | 127 |
| `YA` | Time Offset at PPS Generation Time | Not implemented | - | No | 128 |
| `YB` | Time Offset at PPS Generation Time | Not implemented | - | No | 128 |
| `ha` | Heading and Pitch | Not implemented | - | No | 128 |
| `RO` | Lever Arm Cartesian Position | Not implemented | - | No | 128 |
| `RG` | Lever Arm Geodetic Position | Not implemented | - | No | 128 |
| `RE` | Reply | Not implemented | - | No | 129 |
| `ER` | Error | Not implemented | - | No | 129 |
| `IO` | GPS Ionospheric Parameters | Not implemented | - | No | 129 |
| `QI` | QZSS Ionospheric Parameters | Not implemented | - | No | 130 |
| `CI` | BeiDou Ionospheric Parameters | Not implemented | - | No | 130 |
| `II` | IRNSS Ionospheric Parameters | Not implemented | - | No | 130 |
| `==` | Event | Not implemented | - | No | 131 |
| `LT` | Message Output Latency | Not implemented | - | No | 131 |
| `>>` | Wrapper | Not implemented | - | No | 131 |
| `PM` | Parameters | Not implemented | - | No | 132 |
| `LH` | Logging History | Not implemented | - | No | 133 |
| `AI` | Antenna Information | Not implemented | - | No | 134 |
| `BI` | Base Station Information | Not implemented | - | No | 134 |
| `SE` | Security | Not implemented | - | No | 134 |
| `SM` | Security for [rM] | Not implemented | - | No | 134 |
| `TT` | CA/L1 Overall Continuous Tracking Time | Not implemented | - | No | 134 |
| `OO` | Oscillator Offset | Not implemented | - | No | 135 |
| `&#124;&#124;` | Epoch End | Not implemented | - | No | 135 |
| `DL` | Data Link Status | Not implemented | - | No | 137 |
| `GS` | GPS SVs Status | Not implemented | - | No | 138 |
| `ES` | GALILEO SVs Status | Not implemented | - | No | 139 |
| `WS` | SBAS SVs Status | Not implemented | - | No | 140 |
| `NS` | GLONASS SVs Status | Not implemented | - | No | 140 |
| `QS` | QZSS SVs Status | Not implemented | - | No | 141 |
| `CS` | BeiDou SVs Status | Not implemented | - | No | 142 |
| `Is` | IRNSS SVs Status | Not implemented | - | No | 142 |
| `LS` | L-band SVs Status | Not implemented | - | No | 143 |
| `RS` | Reference Station Status | Not implemented | - | No | 144 |
| `TX` | RTCM/CMR Text Message | Not implemented | - | No | 144 |
| `RM` | Results of RAIM Processing | Not implemented | - | No | 145 |
| `NP` | Navigation Position | Not implemented | - | No | 146 |
| `MP` | Position in Map Projection | Not implemented | - | No | 148 |
| `NR` | Lever Arm Position | Not implemented | - | No | 150 |
| `SY` | Geographic and Quasi Geographic Position | Not implemented | - | No | 151 |
| `TR` | Time Residuals | Not implemented | - | No | 151 |
| `TM` | Clock Offsets and Time Derivatives | Not implemented | - | No | 153 |
| `RP` | Reference Station Parameters | Not implemented | - | No | 154 |
| `RK` | RTK Solution Parameters | Not implemented | - | No | 155 |
| `AP` | Position Covariance Matrix | Not implemented | - | No | 156 |
| `AB` | Baseline | Not implemented | - | No | 157 |
| `QQ` | IMU Attitude Angles | Not implemented | - | No | 157 |
| `WW` | IMU Measurements | Not implemented | - | No | 158 |
| `ZZ` | IMU Integrated Antenna Velocities | Not implemented | - | No | 159 |
| `TD` | Text Data | Not implemented | - | No | 159 |
| `SI` | Satellite indices (obsolete USI form; compatibility section) | Not implemented | - | No | 87 |


[All protocols](supported-protocols.md)
