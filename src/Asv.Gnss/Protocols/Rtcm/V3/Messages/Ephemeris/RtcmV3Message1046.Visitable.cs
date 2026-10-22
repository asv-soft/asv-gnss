using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class RtcmV3Message1046
{
    private static readonly Field RtcmSatelliteNumberField = RtcmV3Visitor.Field(nameof(SatelliteNumber), Int32Type.Default, "Satellite number in all the constellations.");
    private static readonly Field RtcmSatellitePrnField = RtcmV3Visitor.Field(nameof(SatellitePrn), UInt32Type.Default, "A Galileo SVID parameter is coded with 6 bits. However the ma constellation which can be accomodated within the I/NAV and F/NAV frames is 36 satellites (3 planes of 12 satellites each)");
    private static readonly Field RtcmSatelliteCodeField = RtcmV3Visitor.Field(nameof(SatelliteCode), StringOptionalType.Utf8, "A BDS Satellite RINEX Code.", readOnly: true);
    private static readonly Field RtcmWeekRawField = RtcmV3Visitor.Field(nameof(WeekRaw), UInt32Type.Default, "Galileo Week number. Range 0 - 4095 weeks. Resolution - 1 week. Roll-over every 4096 (about 78 years). Galileo System Time (GST). The GST start epoch is defined as 13 seconds before midnight between 21st August and 22nd August 1999. i.e. GST was equal 13 seconds at 22 nd August 1999 00:00:00 UTC.");
    private static readonly Field RtcmIoDnavField = RtcmV3Visitor.Field(nameof(IoDnav), UInt32Type.Default, "Issue of Data for navigation data (IODnav). Each IoDNav has an associated reference time parameter disseminated within the batch. Note: the broadcast group delay validity status and signal health status are not identified by any Issue of Data value unitless.");
    private static readonly Field RtcmSvSisaField = RtcmV3Visitor.Field(nameof(SvSisa), UInt8Type.Default, "SIS Accuracy");
    private static readonly Field RtcmIdotRawField = RtcmV3Visitor.Field(nameof(IdotRaw), DoubleType.Default, "IDOT. Effective range attainable with the indicated bit allocation and scale factor. Resolution ^-43 Rate of inclination angle. Unit: semi-circles/s");
    private static readonly Field RtcmTocRawField = RtcmV3Visitor.Field(nameof(TocRaw), UInt32Type.Default, "The reference time of clock parameters. Unit: s. Range 0 - 604.792. Resolution: 60.");
    private static readonly Field RtcmAf2Field = RtcmV3Visitor.Field(nameof(Af2), DoubleType.Default, "Clock correcton parameter Unit: s/s^2. Effective range attainable with the indicated bit allocation and scale factor. Resolution: 2^-59.");
    private static readonly Field RtcmAf1Field = RtcmV3Visitor.Field(nameof(Af1), DoubleType.Default, "Clock correcton parameter Unit:s/s. Effective range attainable with the indicated bit allocation and scale factor. Resolution: 2^-46");
    private static readonly Field RtcmAf0Field = RtcmV3Visitor.Field(nameof(Af0), DoubleType.Default, "Clock correcton parameter Unit:s. Effective range attainable with the indicated bit allocation and scale factor. Resolution: 2^-34");
    private static readonly Field RtcmCrsField = RtcmV3Visitor.Field(nameof(Crs), DoubleType.Default, "Amplitude of sine harmonic correction term to the orbit radius. Unit: m. Effective range attainable with the indicated bit allocation and scale factor. Resolution: 2^-5 m.");
    private static readonly Field RtcmDeltaNField = RtcmV3Visitor.Field(nameof(DeltaN), DoubleType.Default, "Mean moution difference from computed value. Unit: pi/s. Effective range attainable with the indicated bit allocation and scale factor. Resolution 2^-43 pi/s");
    private static readonly Field RtcmM0Field = RtcmV3Visitor.Field(nameof(M0), DoubleType.Default, "Mean anomaly at reference time. Unit: pi. Effective range attainable with the indicated bit allocation and scale factor. Resolution 2^-31 pi");
    private static readonly Field RtcmCucField = RtcmV3Visitor.Field(nameof(Cuc), DoubleType.Default, "Amplitude of cusine harmonic correction term to the argument of latitude. Unit: rad. Effective range attainable with the indicated bit allocation and scale factor. Resolution: 2^-29 rad.");
    private static readonly Field RtcmeField = RtcmV3Visitor.Field(nameof(e), DoubleType.Default, "Eccentricity. Effective range attainable with the indicated bit allocation and scale factor. Resolution: 2^-33.");
    private static readonly Field RtcmCusField = RtcmV3Visitor.Field(nameof(Cus), DoubleType.Default, "Amplitude of sine harmonic correction term to the argument of latitude. Unit: rad. Effective range attainable with the indicated bit allocation and scale factor. Resolution: 2^-29 rad.");
    private static readonly Field RtcmApow1_2Field = RtcmV3Visitor.Field(nameof(Apow1_2), DoubleType.Default, "Square root of semi-major axis. Unit: m^1/2. Effective range attainable with the indicated bit allocation and scale factor. Resolution: 2^-19 m^1/2.");
    private static readonly Field RtcmToeRawField = RtcmV3Visitor.Field(nameof(ToeRaw), UInt32Type.Default, "Ephemeris reference time. Unit: s. Range 0 - 604.792. Resolution: 60.");
    private static readonly Field RtcmCicField = RtcmV3Visitor.Field(nameof(Cic), DoubleType.Default, "Amplitude of cusine harmonic correction term to the argument of angle of inclination. Unit: rad. Effective range attainable with the indicated bit allocation and scale factor. Resolution: 2^-29 rad.");
    private static readonly Field RtcmOmegaBig0Field = RtcmV3Visitor.Field(nameof(OmegaBig0), DoubleType.Default, "Longitude of ascending node of orbital plane at weekly epoch. Unit: pi. Effective range attainable with the indicated bit allocation and scale factor. Resolution 2^-31 pi");
    private static readonly Field RtcmCisField = RtcmV3Visitor.Field(nameof(Cis), DoubleType.Default, "Amplitude of sine harmonic correction term to the angle of inclination. Unit: rad. Effective range attainable with the indicated bit allocation and scale factor. Resolution: 2^-29 rad.");
    private static readonly Field RtcmI0Field = RtcmV3Visitor.Field(nameof(I0), DoubleType.Default, "Inclination angle at reference time. Unit: pi. Effective range attainable with the indicated bit allocation and scale factor. Resolution 2^-31 pi");
    private static readonly Field RtcmCrcField = RtcmV3Visitor.Field(nameof(Crc), DoubleType.Default, "Amplitude of cosine harmonic correction term to the orbit radius. Unit: m. Effective range attainable with the indicated bit allocation and scale factor. Resolution: 2^-5 m.");
    private static readonly Field RtcmOmegaField = RtcmV3Visitor.Field(nameof(Omega), DoubleType.Default, "Argument of Perigree. Unit: pi. Effective range attainable with the indicated bit allocation and scale factor. Resolution 2^-31 pi");
    private static readonly Field RtcmOmegaDotField = RtcmV3Visitor.Field(nameof(OmegaDot), DoubleType.Default, "Rate of right ascension. Unit: pi/s. Effective range attainable with the indicated bit allocation and scale factor. Resolution: 2^-43 pi/s.");
    private static readonly Field RtcmBGdE1E5aField = RtcmV3Visitor.Field(nameof(BGdE1E5a), DoubleType.Default, "E1/E5a Broadcast Group Delay. Effective range attainable with the indicated bit allocation and scale factor. Resolution: 2^-32.");
    private static readonly Field RtcmBGdE5bE1Field = RtcmV3Visitor.Field(nameof(BGdE5bE1), DoubleType.Default, "E5b/E1 Broadcast Group Delay (reserved). Effective range attainable with the indicated bit allocation and scale factor. Resolution: 2^-32.");
    private static readonly Field RtcmE5bSignalHealthFlagField = RtcmV3Visitor.Field(nameof(E5bSignalHealthFlag), UInt8Type.Default, "E5a Signal Health Status. Bit Values are: 0 - Signal OK 1 - Signal out of service 2 - Signal will be out of service 3 - Signal Component currently in Test");
    private static readonly Field RtcmE5bDataValidityField = RtcmV3Visitor.Field(nameof(E5bDataValidity), UInt8Type.Default, "The navigation data validity status transmitted on E5a.");
    private static readonly Field RtcmE1BSignalHealthFlagField = RtcmV3Visitor.Field(nameof(E1BSignalHealthFlag), UInt8Type.Default, "The E5b Signal Health Status. Bit Values are: 0 - Signal OK 1 - Signal out of service 2 - Signal will be out of service 3 - Signal Component currently in Test");
    private static readonly Field RtcmE1BDataValidityField = RtcmV3Visitor.Field(nameof(E1BDataValidity), UInt8Type.Default, "The navigation data validity status transmitted on E5b.");
    private static readonly Field RtcmReservedField = RtcmV3Visitor.Field(nameof(Reserved), UInt8Type.Default, "Reserved bit.");
    private static readonly Field RtcmIdotField = RtcmV3Visitor.Field(nameof(Idot), DoubleType.Default, "Idot", readOnly: true);
    private static readonly Field RtcmIsE5bSignalOkField = RtcmV3Visitor.Field(nameof(IsE5bSignalOk), BoolType.Default, "IsE5bSignalOk", readOnly: true);
    private static readonly Field RtcmIsE5bSignalOuOfServiceField = RtcmV3Visitor.Field(nameof(IsE5bSignalOuOfService), BoolType.Default, "IsE5bSignalOuOfService", readOnly: true);
    private static readonly Field RtcmIsE5bSignalWillOuOfServiceField = RtcmV3Visitor.Field(nameof(IsE5bSignalWillOuOfService), BoolType.Default, "IsE5bSignalWillOuOfService", readOnly: true);
    private static readonly Field RtcmIsE5bSignalInTestField = RtcmV3Visitor.Field(nameof(IsE5bSignalInTest), BoolType.Default, "IsE5bSignalInTest", readOnly: true);
    private static readonly Field RtcmIsE1BSignalOkField = RtcmV3Visitor.Field(nameof(IsE1BSignalOk), BoolType.Default, "IsE1BSignalOk", readOnly: true);
    private static readonly Field RtcmE1BSignalOuOfServiceField = RtcmV3Visitor.Field(nameof(E1BSignalOuOfService), BoolType.Default, "E1BSignalOuOfService", readOnly: true);
    private static readonly Field RtcmE1BSignalWillOuOfServiceField = RtcmV3Visitor.Field(nameof(E1BSignalWillOuOfService), BoolType.Default, "E1BSignalWillOuOfService", readOnly: true);
    private static readonly Field RtcmE1BSignalInTestField = RtcmV3Visitor.Field(nameof(E1BSignalInTest), BoolType.Default, "E1BSignalInTest", readOnly: true);

    internal new static readonly StructType RtcmVisitorType = new([..RtcmV3MessageBase.RtcmVisitorType.Fields, RtcmSatelliteNumberField, RtcmSatellitePrnField, RtcmSatelliteCodeField, RtcmWeekRawField, RtcmIoDnavField, RtcmSvSisaField, RtcmIdotRawField, RtcmTocRawField, RtcmAf2Field, RtcmAf1Field, RtcmAf0Field, RtcmCrsField, RtcmDeltaNField, RtcmM0Field, RtcmCucField, RtcmeField, RtcmCusField, RtcmApow1_2Field, RtcmToeRawField, RtcmCicField, RtcmOmegaBig0Field, RtcmCisField, RtcmI0Field, RtcmCrcField, RtcmOmegaField, RtcmOmegaDotField, RtcmBGdE1E5aField, RtcmBGdE5bE1Field, RtcmE5bSignalHealthFlagField, RtcmE5bDataValidityField, RtcmE1BSignalHealthFlagField, RtcmE1BDataValidityField, RtcmReservedField, RtcmIdotField, RtcmIsE5bSignalOkField, RtcmIsE5bSignalOuOfServiceField, RtcmIsE5bSignalWillOuOfServiceField, RtcmIsE5bSignalInTestField, RtcmIsE1BSignalOkField, RtcmE1BSignalOuOfServiceField, RtcmE1BSignalWillOuOfServiceField, RtcmE1BSignalInTestField]);

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

        var valueIoDnav = IoDnav;
        UInt32Type.Accept(visitor, RtcmIoDnavField, RtcmIoDnavField.DataType, ref valueIoDnav);
        if (!EqualityComparer<uint>.Default.Equals(valueIoDnav, IoDnav))
        {
            IoDnav = valueIoDnav;
        }

        var valueSvSisa = SvSisa;
        UInt8Type.Accept(visitor, RtcmSvSisaField, RtcmSvSisaField.DataType, ref valueSvSisa);
        if (!EqualityComparer<byte>.Default.Equals(valueSvSisa, SvSisa))
        {
            SvSisa = valueSvSisa;
        }

        var valueIdotRaw = IdotRaw;
        DoubleType.Accept(visitor, RtcmIdotRawField, RtcmIdotRawField.DataType, ref valueIdotRaw);
        if (!EqualityComparer<double>.Default.Equals(valueIdotRaw, IdotRaw))
        {
            IdotRaw = valueIdotRaw;
        }

        var valueTocRaw = TocRaw;
        UInt32Type.Accept(visitor, RtcmTocRawField, RtcmTocRawField.DataType, ref valueTocRaw);
        if (!EqualityComparer<uint>.Default.Equals(valueTocRaw, TocRaw))
        {
            TocRaw = valueTocRaw;
        }

        var valueAf2 = Af2;
        DoubleType.Accept(visitor, RtcmAf2Field, RtcmAf2Field.DataType, ref valueAf2);
        if (!EqualityComparer<double>.Default.Equals(valueAf2, Af2))
        {
            Af2 = valueAf2;
        }

        var valueAf1 = Af1;
        DoubleType.Accept(visitor, RtcmAf1Field, RtcmAf1Field.DataType, ref valueAf1);
        if (!EqualityComparer<double>.Default.Equals(valueAf1, Af1))
        {
            Af1 = valueAf1;
        }

        var valueAf0 = Af0;
        DoubleType.Accept(visitor, RtcmAf0Field, RtcmAf0Field.DataType, ref valueAf0);
        if (!EqualityComparer<double>.Default.Equals(valueAf0, Af0))
        {
            Af0 = valueAf0;
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

        var valueOmegaDot = OmegaDot;
        DoubleType.Accept(visitor, RtcmOmegaDotField, RtcmOmegaDotField.DataType, ref valueOmegaDot);
        if (!EqualityComparer<double>.Default.Equals(valueOmegaDot, OmegaDot))
        {
            OmegaDot = valueOmegaDot;
        }

        var valueBGdE1E5a = BGdE1E5a;
        DoubleType.Accept(visitor, RtcmBGdE1E5aField, RtcmBGdE1E5aField.DataType, ref valueBGdE1E5a);
        if (!EqualityComparer<double>.Default.Equals(valueBGdE1E5a, BGdE1E5a))
        {
            BGdE1E5a = valueBGdE1E5a;
        }

        var valueBGdE5bE1 = BGdE5bE1;
        DoubleType.Accept(visitor, RtcmBGdE5bE1Field, RtcmBGdE5bE1Field.DataType, ref valueBGdE5bE1);
        if (!EqualityComparer<double>.Default.Equals(valueBGdE5bE1, BGdE5bE1))
        {
            BGdE5bE1 = valueBGdE5bE1;
        }

        var valueE5bSignalHealthFlag = E5bSignalHealthFlag;
        UInt8Type.Accept(visitor, RtcmE5bSignalHealthFlagField, RtcmE5bSignalHealthFlagField.DataType, ref valueE5bSignalHealthFlag);
        if (!EqualityComparer<byte>.Default.Equals(valueE5bSignalHealthFlag, E5bSignalHealthFlag))
        {
            E5bSignalHealthFlag = valueE5bSignalHealthFlag;
        }

        var valueE5bDataValidity = E5bDataValidity;
        UInt8Type.Accept(visitor, RtcmE5bDataValidityField, RtcmE5bDataValidityField.DataType, ref valueE5bDataValidity);
        if (!EqualityComparer<byte>.Default.Equals(valueE5bDataValidity, E5bDataValidity))
        {
            E5bDataValidity = valueE5bDataValidity;
        }

        var valueE1BSignalHealthFlag = E1BSignalHealthFlag;
        UInt8Type.Accept(visitor, RtcmE1BSignalHealthFlagField, RtcmE1BSignalHealthFlagField.DataType, ref valueE1BSignalHealthFlag);
        if (!EqualityComparer<byte>.Default.Equals(valueE1BSignalHealthFlag, E1BSignalHealthFlag))
        {
            E1BSignalHealthFlag = valueE1BSignalHealthFlag;
        }

        var valueE1BDataValidity = E1BDataValidity;
        UInt8Type.Accept(visitor, RtcmE1BDataValidityField, RtcmE1BDataValidityField.DataType, ref valueE1BDataValidity);
        if (!EqualityComparer<byte>.Default.Equals(valueE1BDataValidity, E1BDataValidity))
        {
            E1BDataValidity = valueE1BDataValidity;
        }

        var valueReserved = Reserved;
        UInt8Type.Accept(visitor, RtcmReservedField, RtcmReservedField.DataType, ref valueReserved);
        if (!EqualityComparer<byte>.Default.Equals(valueReserved, Reserved))
        {
            Reserved = valueReserved;
        }

        var valueIdot = Idot;
        DoubleType.Accept(visitor, RtcmIdotField, RtcmIdotField.DataType, ref valueIdot);

        var valueIsE5bSignalOk = IsE5bSignalOk;
        BoolType.Accept(visitor, RtcmIsE5bSignalOkField, RtcmIsE5bSignalOkField.DataType, ref valueIsE5bSignalOk);

        var valueIsE5bSignalOuOfService = IsE5bSignalOuOfService;
        BoolType.Accept(visitor, RtcmIsE5bSignalOuOfServiceField, RtcmIsE5bSignalOuOfServiceField.DataType, ref valueIsE5bSignalOuOfService);

        var valueIsE5bSignalWillOuOfService = IsE5bSignalWillOuOfService;
        BoolType.Accept(visitor, RtcmIsE5bSignalWillOuOfServiceField, RtcmIsE5bSignalWillOuOfServiceField.DataType, ref valueIsE5bSignalWillOuOfService);

        var valueIsE5bSignalInTest = IsE5bSignalInTest;
        BoolType.Accept(visitor, RtcmIsE5bSignalInTestField, RtcmIsE5bSignalInTestField.DataType, ref valueIsE5bSignalInTest);

        var valueIsE1BSignalOk = IsE1BSignalOk;
        BoolType.Accept(visitor, RtcmIsE1BSignalOkField, RtcmIsE1BSignalOkField.DataType, ref valueIsE1BSignalOk);

        var valueE1BSignalOuOfService = E1BSignalOuOfService;
        BoolType.Accept(visitor, RtcmE1BSignalOuOfServiceField, RtcmE1BSignalOuOfServiceField.DataType, ref valueE1BSignalOuOfService);

        var valueE1BSignalWillOuOfService = E1BSignalWillOuOfService;
        BoolType.Accept(visitor, RtcmE1BSignalWillOuOfServiceField, RtcmE1BSignalWillOuOfServiceField.DataType, ref valueE1BSignalWillOuOfService);

        var valueE1BSignalInTest = E1BSignalInTest;
        BoolType.Accept(visitor, RtcmE1BSignalInTestField, RtcmE1BSignalInTestField.DataType, ref valueE1BSignalInTest);
    }
}
