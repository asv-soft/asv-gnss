using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class RtcmV3Message1042
{
    private static readonly Field RtcmSatelliteNumberField = RtcmV3Visitor.Field(nameof(SatelliteNumber), Int32Type.Default, "satellite number in all the constellations.");
    private static readonly Field RtcmSatellitePrnField = RtcmV3Visitor.Field(nameof(SatellitePrn), UInt32Type.Default, "A BDS Satellite ID number from 1 to 37 refers to the PRN code of the BDS satellite. Satelilite ID's heigher then 37 are reserved. Range 1-63");
    private static readonly Field RtcmSatelliteCodeField = RtcmV3Visitor.Field(nameof(SatelliteCode), StringOptionalType.Utf8, "A BDS Satellite RINEX Code.", readOnly: true);
    private static readonly Field RtcmWeekRawField = RtcmV3Visitor.Field(nameof(WeekRaw), UInt32Type.Default, "BDS Week number. Range 0 - 8191. Resolution - 1 week. Roll-over every 8192 weeks starting fromm 00:00:00UTC on Jan 1. 2006 of BDT.");
    private static readonly Field RtcmUraiField = RtcmV3Visitor.Field(nameof(Urai), UInt8Type.Default, "BDS URAI Range 1 - 15. User Range Accuracy Index.");
    private static readonly Field RtcmIdotRawField = RtcmV3Visitor.Field(nameof(IdotRaw), DoubleType.Default, "BDS IDOT Range +-9.31*10e-10. Resolution ^-43 Rate of inclination angle. Unit: pi/s");
    private static readonly Field RtcmAodeField = RtcmV3Visitor.Field(nameof(Aode), UInt32Type.Default, "BDS IDOT Range 0-31. Age of Data Ephemeris.");
    private static readonly Field RtcmTocRawField = RtcmV3Visitor.Field(nameof(TocRaw), UInt32Type.Default, "The reference time of clock parameters. Unit: s. Range 0 - 604.792. Resolution: 8s.");
    private static readonly Field RtcmA2Field = RtcmV3Visitor.Field(nameof(A2), DoubleType.Default, "Clock correcton parameter Unit:s. Range: +-1.38E-17 s/s^2. Resolution: 2^-66 s/s^2");
    private static readonly Field RtcmA1Field = RtcmV3Visitor.Field(nameof(A1), DoubleType.Default, "Clock correcton parameter Unit:s. Range: +-1.86E-9 s/s^2. Resolution: 2^-50 s/s^2");
    private static readonly Field RtcmA0Field = RtcmV3Visitor.Field(nameof(A0), DoubleType.Default, "Clock correcton parameter Unit:s. Range: +-9.77E-4 s/s^2. Resolution: 2^-33 s/s^2");
    private static readonly Field RtcmAdocField = RtcmV3Visitor.Field(nameof(Adoc), UInt32Type.Default, "Age of Data Clock. Range: 0-31.");
    private static readonly Field RtcmCrsField = RtcmV3Visitor.Field(nameof(Crs), DoubleType.Default, "Amplitude of sine harmonic correction term to the orbit radius. Unit: m. Range: +-2048. Resolution: 2^-6 m.");
    private static readonly Field RtcmDeltaNField = RtcmV3Visitor.Field(nameof(DeltaN), DoubleType.Default, "Mean moution difference from computed value. Unit: pi/s. Range: +-3.73E-9 pi/s. Resolution 2^-43 pi/s");
    private static readonly Field RtcmM0Field = RtcmV3Visitor.Field(nameof(M0), DoubleType.Default, "Mean anomaly at reference time. Unit: pi. Range: +-pi. Resolution 2^-31 pi");
    private static readonly Field RtcmCucField = RtcmV3Visitor.Field(nameof(Cuc), DoubleType.Default, "Amplitude of cusine harmonic correction term to the argument of latitude. Unit: rad. Range: +-6.10E-5. Resolution: 2^-31 rad.");
    private static readonly Field RtcmeField = RtcmV3Visitor.Field(nameof(e), DoubleType.Default, "Eccentricity. Range: 0 - 0.5. Resolution: 2^-33.");
    private static readonly Field RtcmCusField = RtcmV3Visitor.Field(nameof(Cus), DoubleType.Default, "Amplitude of sine harmonic correction term to the argument of latitude. Unit: rad. Range: +-6.10E-5. Resolution: 2^-31 rad.");
    private static readonly Field RtcmApow1_2Field = RtcmV3Visitor.Field(nameof(Apow1_2), DoubleType.Default, "Square root of semi-major axis. Unit: m^1/2. Range: 0 - 8192 m^1/2. Resolution: 2^-19 m^1/2.");
    private static readonly Field RtcmToeRawField = RtcmV3Visitor.Field(nameof(ToeRaw), UInt32Type.Default, "Ephemeris reference time. Unit: s. Range 0 - 604.792. Resolution: 8s.");
    private static readonly Field RtcmCicField = RtcmV3Visitor.Field(nameof(Cic), DoubleType.Default, "Amplitude of cusine harmonic correction term to the argument of angle of inclination. Unit: rad. Range: +-6.10E-5. Resolution: 2^-31 rad.");
    private static readonly Field RtcmOmegaBig0Field = RtcmV3Visitor.Field(nameof(OmegaBig0), DoubleType.Default, "Longitude of ascending node of orbital of phase computed according to reference time. Unit: pi. Range: +-pi. Resolution 2^-31 pi");
    private static readonly Field RtcmCisField = RtcmV3Visitor.Field(nameof(Cis), DoubleType.Default, "Amplitude of sine harmonic correction term to the argument of angle of inclination. Unit: rad. Range: +-6.10E-5. Resolution: 2^-31 rad.");
    private static readonly Field RtcmI0Field = RtcmV3Visitor.Field(nameof(I0), DoubleType.Default, "Inclination angle at reference time. Unit: pi. Range: +-pi. Resolution 2^-31 pi");
    private static readonly Field RtcmCrcField = RtcmV3Visitor.Field(nameof(Crc), DoubleType.Default, "Amplitude of cosine harmonic correction term to the orbit radius. Unit: m. Range: +-2048. Resolution: 2^-6 m.");
    private static readonly Field RtcmOmegaField = RtcmV3Visitor.Field(nameof(Omega), DoubleType.Default, "Argument of perigree. Unit: pi. Range: +-pi. Resolution 2^-31 pi");
    private static readonly Field RtcmOmegaBigField = RtcmV3Visitor.Field(nameof(OmegaBig), DoubleType.Default, "Rate of right ascension. Unit: pi/s. Range: +-9.54E-7 pi/s. Resolution: 2^-43 pi/s.");
    private static readonly Field RtcmTGd1Field = RtcmV3Visitor.Field(nameof(TGd1), DoubleType.Default, "Equipment Group Delay Differential. Unit: ns. Range: +-51.2 ns. Resolution: 0.1 ns.");
    private static readonly Field RtcmTGd2Field = RtcmV3Visitor.Field(nameof(TGd2), DoubleType.Default, "Equipment Group Delay Differential. Unit: ns. Range: +-51.2 ns. Resolution: 0.1 ns.");
    private static readonly Field RtcmSvHealthField = RtcmV3Visitor.Field(nameof(SvHealth), UInt8Type.Default, "Autonomous satelite health flag. Range: 0 - 1. Resolution: 1.");
    private static readonly Field RtcmIdotField = RtcmV3Visitor.Field(nameof(Idot), DoubleType.Default, "Idot", readOnly: true);

    internal new static readonly StructType RtcmVisitorType = new([..RtcmV3MessageBase.RtcmVisitorType.Fields, RtcmSatelliteNumberField, RtcmSatellitePrnField, RtcmSatelliteCodeField, RtcmWeekRawField, RtcmUraiField, RtcmIdotRawField, RtcmAodeField, RtcmTocRawField, RtcmA2Field, RtcmA1Field, RtcmA0Field, RtcmAdocField, RtcmCrsField, RtcmDeltaNField, RtcmM0Field, RtcmCucField, RtcmeField, RtcmCusField, RtcmApow1_2Field, RtcmToeRawField, RtcmCicField, RtcmOmegaBig0Field, RtcmCisField, RtcmI0Field, RtcmCrcField, RtcmOmegaField, RtcmOmegaBigField, RtcmTGd1Field, RtcmTGd2Field, RtcmSvHealthField, RtcmIdotField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        base.Accept(visitor);

        var valueSatelliteNumber = SatelliteNumber;
        Int32Type.Accept(visitor, RtcmSatelliteNumberField, RtcmSatelliteNumberField.DataType, ref valueSatelliteNumber);
        if (!EqualityComparer<int>.Default.Equals(valueSatelliteNumber, SatelliteNumber))
        {
            SatelliteNumber = valueSatelliteNumber;
        }

        var valueSatellitePrn = SatellitePrn;
        UInt32Type.Accept(visitor, RtcmSatellitePrnField, RtcmSatellitePrnField.DataType, ref valueSatellitePrn);
        if (!EqualityComparer<uint>.Default.Equals(valueSatellitePrn, SatellitePrn))
        {
            SatellitePrn = valueSatellitePrn;
        }

        string? valueSatelliteCode = SatelliteCode;
        StringOptionalType.Accept(visitor, RtcmSatelliteCodeField, RtcmSatelliteCodeField.DataType, ref valueSatelliteCode);

        var valueWeekRaw = WeekRaw;
        UInt32Type.Accept(visitor, RtcmWeekRawField, RtcmWeekRawField.DataType, ref valueWeekRaw);
        if (!EqualityComparer<uint>.Default.Equals(valueWeekRaw, WeekRaw))
        {
            WeekRaw = valueWeekRaw;
        }

        var valueUrai = Urai;
        UInt8Type.Accept(visitor, RtcmUraiField, RtcmUraiField.DataType, ref valueUrai);
        if (!EqualityComparer<byte>.Default.Equals(valueUrai, Urai))
        {
            Urai = valueUrai;
        }

        var valueIdotRaw = IdotRaw;
        DoubleType.Accept(visitor, RtcmIdotRawField, RtcmIdotRawField.DataType, ref valueIdotRaw);
        if (!EqualityComparer<double>.Default.Equals(valueIdotRaw, IdotRaw))
        {
            IdotRaw = valueIdotRaw;
        }

        var valueAode = Aode;
        UInt32Type.Accept(visitor, RtcmAodeField, RtcmAodeField.DataType, ref valueAode);
        if (!EqualityComparer<uint>.Default.Equals(valueAode, Aode))
        {
            Aode = valueAode;
        }

        var valueTocRaw = TocRaw;
        UInt32Type.Accept(visitor, RtcmTocRawField, RtcmTocRawField.DataType, ref valueTocRaw);
        if (!EqualityComparer<uint>.Default.Equals(valueTocRaw, TocRaw))
        {
            TocRaw = valueTocRaw;
        }

        var valueA2 = A2;
        DoubleType.Accept(visitor, RtcmA2Field, RtcmA2Field.DataType, ref valueA2);
        if (!EqualityComparer<double>.Default.Equals(valueA2, A2))
        {
            A2 = valueA2;
        }

        var valueA1 = A1;
        DoubleType.Accept(visitor, RtcmA1Field, RtcmA1Field.DataType, ref valueA1);
        if (!EqualityComparer<double>.Default.Equals(valueA1, A1))
        {
            A1 = valueA1;
        }

        var valueA0 = A0;
        DoubleType.Accept(visitor, RtcmA0Field, RtcmA0Field.DataType, ref valueA0);
        if (!EqualityComparer<double>.Default.Equals(valueA0, A0))
        {
            A0 = valueA0;
        }

        var valueAdoc = Adoc;
        UInt32Type.Accept(visitor, RtcmAdocField, RtcmAdocField.DataType, ref valueAdoc);
        if (!EqualityComparer<uint>.Default.Equals(valueAdoc, Adoc))
        {
            Adoc = valueAdoc;
        }

        var valueCrs = Crs;
        DoubleType.Accept(visitor, RtcmCrsField, RtcmCrsField.DataType, ref valueCrs);
        if (!EqualityComparer<double>.Default.Equals(valueCrs, Crs))
        {
            Crs = valueCrs;
        }

        var valueDeltaN = DeltaN;
        DoubleType.Accept(visitor, RtcmDeltaNField, RtcmDeltaNField.DataType, ref valueDeltaN);
        if (!EqualityComparer<double>.Default.Equals(valueDeltaN, DeltaN))
        {
            DeltaN = valueDeltaN;
        }

        var valueM0 = M0;
        DoubleType.Accept(visitor, RtcmM0Field, RtcmM0Field.DataType, ref valueM0);
        if (!EqualityComparer<double>.Default.Equals(valueM0, M0))
        {
            M0 = valueM0;
        }

        var valueCuc = Cuc;
        DoubleType.Accept(visitor, RtcmCucField, RtcmCucField.DataType, ref valueCuc);
        if (!EqualityComparer<double>.Default.Equals(valueCuc, Cuc))
        {
            Cuc = valueCuc;
        }

        var valuee = e;
        DoubleType.Accept(visitor, RtcmeField, RtcmeField.DataType, ref valuee);
        if (!EqualityComparer<double>.Default.Equals(valuee, e))
        {
            e = valuee;
        }

        var valueCus = Cus;
        DoubleType.Accept(visitor, RtcmCusField, RtcmCusField.DataType, ref valueCus);
        if (!EqualityComparer<double>.Default.Equals(valueCus, Cus))
        {
            Cus = valueCus;
        }

        var valueApow1_2 = Apow1_2;
        DoubleType.Accept(visitor, RtcmApow1_2Field, RtcmApow1_2Field.DataType, ref valueApow1_2);
        if (!EqualityComparer<double>.Default.Equals(valueApow1_2, Apow1_2))
        {
            Apow1_2 = valueApow1_2;
        }

        var valueToeRaw = ToeRaw;
        UInt32Type.Accept(visitor, RtcmToeRawField, RtcmToeRawField.DataType, ref valueToeRaw);
        if (!EqualityComparer<uint>.Default.Equals(valueToeRaw, ToeRaw))
        {
            ToeRaw = valueToeRaw;
        }

        var valueCic = Cic;
        DoubleType.Accept(visitor, RtcmCicField, RtcmCicField.DataType, ref valueCic);
        if (!EqualityComparer<double>.Default.Equals(valueCic, Cic))
        {
            Cic = valueCic;
        }

        var valueOmegaBig0 = OmegaBig0;
        DoubleType.Accept(visitor, RtcmOmegaBig0Field, RtcmOmegaBig0Field.DataType, ref valueOmegaBig0);
        if (!EqualityComparer<double>.Default.Equals(valueOmegaBig0, OmegaBig0))
        {
            OmegaBig0 = valueOmegaBig0;
        }

        var valueCis = Cis;
        DoubleType.Accept(visitor, RtcmCisField, RtcmCisField.DataType, ref valueCis);
        if (!EqualityComparer<double>.Default.Equals(valueCis, Cis))
        {
            Cis = valueCis;
        }

        var valueI0 = I0;
        DoubleType.Accept(visitor, RtcmI0Field, RtcmI0Field.DataType, ref valueI0);
        if (!EqualityComparer<double>.Default.Equals(valueI0, I0))
        {
            I0 = valueI0;
        }

        var valueCrc = Crc;
        DoubleType.Accept(visitor, RtcmCrcField, RtcmCrcField.DataType, ref valueCrc);
        if (!EqualityComparer<double>.Default.Equals(valueCrc, Crc))
        {
            Crc = valueCrc;
        }

        var valueOmega = Omega;
        DoubleType.Accept(visitor, RtcmOmegaField, RtcmOmegaField.DataType, ref valueOmega);
        if (!EqualityComparer<double>.Default.Equals(valueOmega, Omega))
        {
            Omega = valueOmega;
        }

        var valueOmegaBig = OmegaBig;
        DoubleType.Accept(visitor, RtcmOmegaBigField, RtcmOmegaBigField.DataType, ref valueOmegaBig);
        if (!EqualityComparer<double>.Default.Equals(valueOmegaBig, OmegaBig))
        {
            OmegaBig = valueOmegaBig;
        }

        var valueTGd1 = TGd1;
        DoubleType.Accept(visitor, RtcmTGd1Field, RtcmTGd1Field.DataType, ref valueTGd1);
        if (!EqualityComparer<double>.Default.Equals(valueTGd1, TGd1))
        {
            TGd1 = valueTGd1;
        }

        var valueTGd2 = TGd2;
        DoubleType.Accept(visitor, RtcmTGd2Field, RtcmTGd2Field.DataType, ref valueTGd2);
        if (!EqualityComparer<double>.Default.Equals(valueTGd2, TGd2))
        {
            TGd2 = valueTGd2;
        }

        var valueSvHealth = SvHealth;
        UInt8Type.Accept(visitor, RtcmSvHealthField, RtcmSvHealthField.DataType, ref valueSvHealth);
        if (!EqualityComparer<byte>.Default.Equals(valueSvHealth, SvHealth))
        {
            SvHealth = valueSvHealth;
        }

        var valueIdot = Idot;
        DoubleType.Accept(visitor, RtcmIdotField, RtcmIdotField.DataType, ref valueIdot);
    }
}
