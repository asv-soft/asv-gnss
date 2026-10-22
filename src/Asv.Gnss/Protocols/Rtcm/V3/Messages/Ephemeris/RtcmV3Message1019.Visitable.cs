using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class RtcmV3Message1019
{
    private static readonly Field RtcmSatelliteNumberField = RtcmV3Visitor.Field(nameof(SatelliteNumber), Int32Type.Default, "/* satellite number */");
    private static readonly Field RtcmSatellitePrnField = RtcmV3Visitor.Field(nameof(SatellitePrn), Int32Type.Default, "SatellitePrn");
    private static readonly Field RtcmSatelliteCodeField = RtcmV3Visitor.Field(nameof(SatelliteCode), StringOptionalType.Utf8, "SatelliteCode");
    private static readonly Field RtcmIodeField = RtcmV3Visitor.Field(nameof(Iode), Int32Type.Default, "IODE");
    private static readonly Field RtcmIodcField = RtcmV3Visitor.Field(nameof(Iodc), Int32Type.Default, "IODC");
    private static readonly Field RtcmSvAccuracyField = RtcmV3Visitor.Field(nameof(SvAccuracy), Int32Type.Default, "/* SV accuracy (URA index) */");
    private static readonly Field RtcmSvHealthField = RtcmV3Visitor.Field(nameof(SvHealth), Int32Type.Default, "/* SV health (0:ok) */");
    private static readonly Field RtcmWeekRawField = RtcmV3Visitor.Field(nameof(WeekRaw), UInt32OptionalType.Default, "/* GPS/QZS: gps week, GAL: galileo week */");
    private static readonly Field RtcmWeekField = RtcmV3Visitor.Field(nameof(Week), Int32Type.Default, "Week");
    private static readonly Field RtcmCodeL2Field = RtcmV3Visitor.Field(nameof(CodeL2), Int32Type.Default, "GPS/QZS: code on L2. GAL: data sourceName defined as rinex 3.03. BDS: data sourceName (0:unknown,1:B1I,2:B1Q,3:B2I,4:B2Q,5:B3I,6:B3Q)");
    private static readonly Field RtcmFlagL2PDataField = RtcmV3Visitor.Field(nameof(FlagL2PData), Int32Type.Default, "GPS/QZS: L2 P data flag BDS: nav type (0:unknown,1:IGSO/MEO,2:GEO)");
    private static readonly Field RtcmToeField = RtcmV3Visitor.Field(nameof(Toe), DateTimeType.Default, "Toe");
    private static readonly Field RtcmTocField = RtcmV3Visitor.Field(nameof(Toc), DateTimeType.Default, "Toc");
    private static readonly Field RtcmTTransField = RtcmV3Visitor.Field(nameof(TTrans), DateTimeType.Default, "T_trans");
    private static readonly Field RtcmAField = RtcmV3Visitor.Field(nameof(A), DoubleType.Default, "Root of the Semi-Major Axis (km)");
    private static readonly Field RtcmEField = RtcmV3Visitor.Field(nameof(E), DoubleType.Default, "Eccentricity (e)");
    private static readonly Field RtcmI0Field = RtcmV3Visitor.Field(nameof(I0), DoubleType.Default, "Inclination Angle at Reference Time (i)");
    private static readonly Field RtcmOmg0Field = RtcmV3Visitor.Field(nameof(Omg0), DoubleType.Default, "Longitude of Ascending Node of Orbit Plane at Weekly Epoc (LΩ)");
    private static readonly Field RtcmOmgField = RtcmV3Visitor.Field(nameof(Omg), DoubleType.Default, "Argument of Perigee (ω)");
    private static readonly Field RtcmM0Field = RtcmV3Visitor.Field(nameof(M0), DoubleType.Default, "Mean Anomaly at Reference Time (m)");
    private static readonly Field RtcmDelnField = RtcmV3Visitor.Field(nameof(Deln), DoubleType.Default, "Deln");
    private static readonly Field RtcmOmgDField = RtcmV3Visitor.Field(nameof(OmgD), DoubleType.Default, "Rate of Right Ascension (dΩ/dt)");
    private static readonly Field RtcmIdotField = RtcmV3Visitor.Field(nameof(Idot), DoubleType.Default, "Idot");
    private static readonly Field RtcmCrcField = RtcmV3Visitor.Field(nameof(Crc), DoubleType.Default, "Crc");
    private static readonly Field RtcmCrsField = RtcmV3Visitor.Field(nameof(Crs), DoubleType.Default, "Crs");
    private static readonly Field RtcmCucField = RtcmV3Visitor.Field(nameof(Cuc), DoubleType.Default, "Cuc");
    private static readonly Field RtcmCusField = RtcmV3Visitor.Field(nameof(Cus), DoubleType.Default, "Cus");
    private static readonly Field RtcmCicField = RtcmV3Visitor.Field(nameof(Cic), DoubleType.Default, "Cic");
    private static readonly Field RtcmCisField = RtcmV3Visitor.Field(nameof(Cis), DoubleType.Default, "Cis");
    private static readonly Field RtcmToesField = RtcmV3Visitor.Field(nameof(Toes), DoubleType.Default, "/* Toe (s) in week */");
    private static readonly Field RtcmFitField = RtcmV3Visitor.Field(nameof(Fit), DoubleType.Default, "/* fit interval (h) */");
    private static readonly Field RtcmAf0Field = RtcmV3Visitor.Field(nameof(Af0), DoubleType.Default, "SV clock parameters af0");
    private static readonly Field RtcmAf1Field = RtcmV3Visitor.Field(nameof(Af1), DoubleType.Default, "SV clock parameters af1");
    private static readonly Field RtcmAf2Field = RtcmV3Visitor.Field(nameof(Af2), DoubleType.Default, "SV clock parameters af2");
    private static readonly Field RtcmTgdField = RtcmV3Visitor.Field(nameof(Tgd), new ListType(DoubleType.Default, 0, RtcmV3Visitor.MaximumElements), "group delay parameters. GPS/QZS:tgd[0]=TGD. GAL:tgd[0]=BGD_E1E5a,tgd[1]=BGD_E1E5b. CMP:tgd[0]=TGD_B1I ,tgd[1]=TGD_B2I/B2b,tgd[2]=TGD_B1Cp. tgd[3]=TGD_B2ap,tgd[4]=ISC_B1Cd. tgd[5]=ISC_B2ad");
    private static readonly Field RtcmAdotField = RtcmV3Visitor.Field(nameof(Adot), DoubleType.Default, "Adot for CNAV");
    private static readonly Field RtcmNdotField = RtcmV3Visitor.Field(nameof(Ndot), DoubleType.Default, "ndot for CNAV");

    internal new static readonly StructType RtcmVisitorType = new([..RtcmV3MessageBase.RtcmVisitorType.Fields, RtcmSatelliteNumberField, RtcmSatellitePrnField, RtcmSatelliteCodeField, RtcmIodeField, RtcmIodcField, RtcmSvAccuracyField, RtcmSvHealthField, RtcmWeekRawField, RtcmWeekField, RtcmCodeL2Field, RtcmFlagL2PDataField, RtcmToeField, RtcmTocField, RtcmTTransField, RtcmAField, RtcmEField, RtcmI0Field, RtcmOmg0Field, RtcmOmgField, RtcmM0Field, RtcmDelnField, RtcmOmgDField, RtcmIdotField, RtcmCrcField, RtcmCrsField, RtcmCucField, RtcmCusField, RtcmCicField, RtcmCisField, RtcmToesField, RtcmFitField, RtcmAf0Field, RtcmAf1Field, RtcmAf2Field, RtcmTgdField, RtcmAdotField, RtcmNdotField]);

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
        Int32Type.Accept(visitor, RtcmSatellitePrnField, RtcmSatellitePrnField.DataType, ref valueSatellitePrn);
        if (!EqualityComparer<int>.Default.Equals(valueSatellitePrn, SatellitePrn))
        {
            SatellitePrn = valueSatellitePrn;
        }

        string? valueSatelliteCode = SatelliteCode;
        StringOptionalType.Accept(visitor, RtcmSatelliteCodeField, RtcmSatelliteCodeField.DataType, ref valueSatelliteCode);
        if (!EqualityComparer<string>.Default.Equals(valueSatelliteCode, SatelliteCode))
        {
            SatelliteCode = valueSatelliteCode!;
        }

        var valueIode = Iode;
        Int32Type.Accept(visitor, RtcmIodeField, RtcmIodeField.DataType, ref valueIode);
        if (!EqualityComparer<int>.Default.Equals(valueIode, Iode))
        {
            Iode = valueIode;
        }

        var valueIodc = Iodc;
        Int32Type.Accept(visitor, RtcmIodcField, RtcmIodcField.DataType, ref valueIodc);
        if (!EqualityComparer<int>.Default.Equals(valueIodc, Iodc))
        {
            Iodc = valueIodc;
        }

        var valueSvAccuracy = SvAccuracy;
        Int32Type.Accept(visitor, RtcmSvAccuracyField, RtcmSvAccuracyField.DataType, ref valueSvAccuracy);
        if (!EqualityComparer<int>.Default.Equals(valueSvAccuracy, SvAccuracy))
        {
            SvAccuracy = valueSvAccuracy;
        }

        var valueSvHealth = SvHealth;
        Int32Type.Accept(visitor, RtcmSvHealthField, RtcmSvHealthField.DataType, ref valueSvHealth);
        if (!EqualityComparer<int>.Default.Equals(valueSvHealth, SvHealth))
        {
            SvHealth = valueSvHealth;
        }

        var valueWeekRaw = WeekRaw;
        UInt32OptionalType.Accept(visitor, RtcmWeekRawField, RtcmWeekRawField.DataType, ref valueWeekRaw);
        if (!EqualityComparer<uint?>.Default.Equals(valueWeekRaw, WeekRaw))
        {
            WeekRaw = valueWeekRaw;
        }

        var valueWeek = Week;
        Int32Type.Accept(visitor, RtcmWeekField, RtcmWeekField.DataType, ref valueWeek);
        if (!EqualityComparer<int>.Default.Equals(valueWeek, Week))
        {
            Week = valueWeek;
        }

        var valueCodeL2 = CodeL2;
        Int32Type.Accept(visitor, RtcmCodeL2Field, RtcmCodeL2Field.DataType, ref valueCodeL2);
        if (!EqualityComparer<int>.Default.Equals(valueCodeL2, CodeL2))
        {
            CodeL2 = valueCodeL2;
        }

        var valueFlagL2PData = FlagL2PData;
        Int32Type.Accept(visitor, RtcmFlagL2PDataField, RtcmFlagL2PDataField.DataType, ref valueFlagL2PData);
        if (!EqualityComparer<int>.Default.Equals(valueFlagL2PData, FlagL2PData))
        {
            FlagL2PData = valueFlagL2PData;
        }

        var valueToe = Toe;
        DateTimeType.Accept(visitor, RtcmToeField, RtcmToeField.DataType, ref valueToe);
        if (!EqualityComparer<DateTime>.Default.Equals(valueToe, Toe))
        {
            Toe = valueToe;
        }

        var valueToc = Toc;
        DateTimeType.Accept(visitor, RtcmTocField, RtcmTocField.DataType, ref valueToc);
        if (!EqualityComparer<DateTime>.Default.Equals(valueToc, Toc))
        {
            Toc = valueToc;
        }

        var valueTTrans = TTrans;
        DateTimeType.Accept(visitor, RtcmTTransField, RtcmTTransField.DataType, ref valueTTrans);
        if (!EqualityComparer<DateTime>.Default.Equals(valueTTrans, TTrans))
        {
            TTrans = valueTTrans;
        }

        var valueA = A;
        DoubleType.Accept(visitor, RtcmAField, RtcmAField.DataType, ref valueA);
        if (!EqualityComparer<double>.Default.Equals(valueA, A))
        {
            A = valueA;
        }

        var valueE = E;
        DoubleType.Accept(visitor, RtcmEField, RtcmEField.DataType, ref valueE);
        if (!EqualityComparer<double>.Default.Equals(valueE, E))
        {
            E = valueE;
        }

        var valueI0 = I0;
        DoubleType.Accept(visitor, RtcmI0Field, RtcmI0Field.DataType, ref valueI0);
        if (!EqualityComparer<double>.Default.Equals(valueI0, I0))
        {
            I0 = valueI0;
        }

        var valueOmg0 = Omg0;
        DoubleType.Accept(visitor, RtcmOmg0Field, RtcmOmg0Field.DataType, ref valueOmg0);
        if (!EqualityComparer<double>.Default.Equals(valueOmg0, Omg0))
        {
            Omg0 = valueOmg0;
        }

        var valueOmg = Omg;
        DoubleType.Accept(visitor, RtcmOmgField, RtcmOmgField.DataType, ref valueOmg);
        if (!EqualityComparer<double>.Default.Equals(valueOmg, Omg))
        {
            Omg = valueOmg;
        }

        var valueM0 = M0;
        DoubleType.Accept(visitor, RtcmM0Field, RtcmM0Field.DataType, ref valueM0);
        if (!EqualityComparer<double>.Default.Equals(valueM0, M0))
        {
            M0 = valueM0;
        }

        var valueDeln = Deln;
        DoubleType.Accept(visitor, RtcmDelnField, RtcmDelnField.DataType, ref valueDeln);
        if (!EqualityComparer<double>.Default.Equals(valueDeln, Deln))
        {
            Deln = valueDeln;
        }

        var valueOmgD = OmgD;
        DoubleType.Accept(visitor, RtcmOmgDField, RtcmOmgDField.DataType, ref valueOmgD);
        if (!EqualityComparer<double>.Default.Equals(valueOmgD, OmgD))
        {
            OmgD = valueOmgD;
        }

        var valueIdot = Idot;
        DoubleType.Accept(visitor, RtcmIdotField, RtcmIdotField.DataType, ref valueIdot);
        if (!EqualityComparer<double>.Default.Equals(valueIdot, Idot))
        {
            Idot = valueIdot;
        }

        var valueCrc = Crc;
        DoubleType.Accept(visitor, RtcmCrcField, RtcmCrcField.DataType, ref valueCrc);
        if (!EqualityComparer<double>.Default.Equals(valueCrc, Crc))
        {
            Crc = valueCrc;
        }

        var valueCrs = Crs;
        DoubleType.Accept(visitor, RtcmCrsField, RtcmCrsField.DataType, ref valueCrs);
        if (!EqualityComparer<double>.Default.Equals(valueCrs, Crs))
        {
            Crs = valueCrs;
        }

        var valueCuc = Cuc;
        DoubleType.Accept(visitor, RtcmCucField, RtcmCucField.DataType, ref valueCuc);
        if (!EqualityComparer<double>.Default.Equals(valueCuc, Cuc))
        {
            Cuc = valueCuc;
        }

        var valueCus = Cus;
        DoubleType.Accept(visitor, RtcmCusField, RtcmCusField.DataType, ref valueCus);
        if (!EqualityComparer<double>.Default.Equals(valueCus, Cus))
        {
            Cus = valueCus;
        }

        var valueCic = Cic;
        DoubleType.Accept(visitor, RtcmCicField, RtcmCicField.DataType, ref valueCic);
        if (!EqualityComparer<double>.Default.Equals(valueCic, Cic))
        {
            Cic = valueCic;
        }

        var valueCis = Cis;
        DoubleType.Accept(visitor, RtcmCisField, RtcmCisField.DataType, ref valueCis);
        if (!EqualityComparer<double>.Default.Equals(valueCis, Cis))
        {
            Cis = valueCis;
        }

        var valueToes = Toes;
        DoubleType.Accept(visitor, RtcmToesField, RtcmToesField.DataType, ref valueToes);
        if (!EqualityComparer<double>.Default.Equals(valueToes, Toes))
        {
            Toes = valueToes;
        }

        var valueFit = Fit;
        DoubleType.Accept(visitor, RtcmFitField, RtcmFitField.DataType, ref valueFit);
        if (!EqualityComparer<double>.Default.Equals(valueFit, Fit))
        {
            Fit = valueFit;
        }

        var valueAf0 = Af0;
        DoubleType.Accept(visitor, RtcmAf0Field, RtcmAf0Field.DataType, ref valueAf0);
        if (!EqualityComparer<double>.Default.Equals(valueAf0, Af0))
        {
            Af0 = valueAf0;
        }

        var valueAf1 = Af1;
        DoubleType.Accept(visitor, RtcmAf1Field, RtcmAf1Field.DataType, ref valueAf1);
        if (!EqualityComparer<double>.Default.Equals(valueAf1, Af1))
        {
            Af1 = valueAf1;
        }

        var valueAf2 = Af2;
        DoubleType.Accept(visitor, RtcmAf2Field, RtcmAf2Field.DataType, ref valueAf2);
        if (!EqualityComparer<double>.Default.Equals(valueAf2, Af2))
        {
            Af2 = valueAf2;
        }

        var valueTgd = Tgd;
        RtcmV3Visitor.VisitArray(visitor, RtcmTgdField, RtcmTgdField.DataType, ref valueTgd,
            static () => default(double),
            static (IVisitor v, Field f, IFieldType t, ref double item) =>
            {
                DoubleType.Accept(v, f, t, ref item);
            });
        if (!ReferenceEquals(valueTgd, Tgd))
        {
            Tgd = valueTgd!;
        }

        var valueAdot = Adot;
        DoubleType.Accept(visitor, RtcmAdotField, RtcmAdotField.DataType, ref valueAdot);
        if (!EqualityComparer<double>.Default.Equals(valueAdot, Adot))
        {
            Adot = valueAdot;
        }

        var valueNdot = Ndot;
        DoubleType.Accept(visitor, RtcmNdotField, RtcmNdotField.DataType, ref valueNdot);
        if (!EqualityComparer<double>.Default.Equals(valueNdot, Ndot))
        {
            Ndot = valueNdot;
        }
    }
}
