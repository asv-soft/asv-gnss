using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class SbfPacketGpsNav
{
    private static readonly Field PayloadPrnField = SbfComNavVisitor.Field(nameof(Prn), UInt8Type.Default, "Prn");
    private static readonly Field PayloadWeekField = SbfComNavVisitor.Field(nameof(Week), UInt16Type.Default, "Week");
    private static readonly Field PayloadToeSecField = SbfComNavVisitor.Field(nameof(ToeSec), UInt32Type.Default, "ToeSec");
    private static readonly Field PayloadiodeField = SbfComNavVisitor.Field(nameof(iode), UInt8Type.Default, "iode");
    private static readonly Field PayloadiodcField = SbfComNavVisitor.Field(nameof(iodc), UInt16Type.Default, "iodc");
    private static readonly Field PayloadHealthField = SbfComNavVisitor.Field(nameof(Health), UInt8Type.Default, "Health");
    private static readonly Field PayloadAccuracyField = SbfComNavVisitor.Field(nameof(Accuracy), UInt8Type.Default, "Accuracy");
    private static readonly Field PayloadAf0Field = SbfComNavVisitor.Field(nameof(Af0), FloatType.Default, "Af0");
    private static readonly Field PayloadAf1Field = SbfComNavVisitor.Field(nameof(Af1), FloatType.Default, "Af1");
    private static readonly Field PayloadAf2Field = SbfComNavVisitor.Field(nameof(Af2), FloatType.Default, "Af2");
    private static readonly Field PayloadToeField = SbfComNavVisitor.Field(nameof(Toe), DateTimeType.Default, "Toe");
    private static readonly Field PayloadTocField = SbfComNavVisitor.Field(nameof(Toc), DateTimeType.Default, "Toc");
    private static readonly Field PayloadTimeTransField = SbfComNavVisitor.Field(nameof(TimeTrans), DateTimeType.Default, "TimeTrans");
    private static readonly Field PayloadAField = SbfComNavVisitor.Field(nameof(A), DoubleType.Default, "A");
    private static readonly Field PayloadEField = SbfComNavVisitor.Field(nameof(E), DoubleType.Default, "E");
    private static readonly Field PayloadomegaField = SbfComNavVisitor.Field(nameof(omega), DoubleType.Default, "omega");
    private static readonly Field PayloadOMEGA0Field = SbfComNavVisitor.Field(nameof(OMEGA0), DoubleType.Default, "OMEGA0");
    private static readonly Field Payloadi0Field = SbfComNavVisitor.Field(nameof(i0), DoubleType.Default, "i0");
    private static readonly Field PayloadM0Field = SbfComNavVisitor.Field(nameof(M0), DoubleType.Default, "M0");
    private static readonly Field PayloadDeltaNField = SbfComNavVisitor.Field(nameof(DeltaN), FloatType.Default, "DeltaN");
    private static readonly Field PayloadOmegaDotField = SbfComNavVisitor.Field(nameof(OmegaDot), FloatType.Default, "OmegaDot");
    private static readonly Field PayloadidotField = SbfComNavVisitor.Field(nameof(idot), FloatType.Default, "idot");
    private static readonly Field PayloadCrcField = SbfComNavVisitor.Field(nameof(Crc), FloatType.Default, "Crc");
    private static readonly Field PayloadCrsField = SbfComNavVisitor.Field(nameof(Crs), FloatType.Default, "Crs");
    private static readonly Field PayloadCucField = SbfComNavVisitor.Field(nameof(Cuc), FloatType.Default, "Cuc");
    private static readonly Field PayloadCusField = SbfComNavVisitor.Field(nameof(Cus), FloatType.Default, "Cus");
    private static readonly Field PayloadCicField = SbfComNavVisitor.Field(nameof(Cic), FloatType.Default, "Cic");
    private static readonly Field PayloadCisField = SbfComNavVisitor.Field(nameof(Cis), FloatType.Default, "Cis");
    private static readonly Field PayloadTgdField = SbfComNavVisitor.Field(nameof(Tgd), FloatType.Default, "Tgd");

    internal new static readonly StructType PayloadVisitorType = new([..SbfMessageBase.PayloadVisitorType.Fields, PayloadPrnField, PayloadWeekField, PayloadToeSecField, PayloadiodeField, PayloadiodcField, PayloadHealthField, PayloadAccuracyField, PayloadAf0Field, PayloadAf1Field, PayloadAf2Field, PayloadToeField, PayloadTocField, PayloadTimeTransField, PayloadAField, PayloadEField, PayloadomegaField, PayloadOMEGA0Field, Payloadi0Field, PayloadM0Field, PayloadDeltaNField, PayloadOmegaDotField, PayloadidotField, PayloadCrcField, PayloadCrsField, PayloadCucField, PayloadCusField, PayloadCicField, PayloadCisField, PayloadTgdField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        base.Accept(visitor);

        var valuePrn = Prn;
        UInt8Type.Accept(visitor, PayloadPrnField, PayloadPrnField.DataType, ref valuePrn);
        if (!EqualityComparer<byte>.Default.Equals(valuePrn, Prn))
        {
            Prn = valuePrn;
        }

        var valueWeek = Week;
        UInt16Type.Accept(visitor, PayloadWeekField, PayloadWeekField.DataType, ref valueWeek);
        if (!EqualityComparer<ushort>.Default.Equals(valueWeek, Week))
        {
            Week = valueWeek;
        }

        var valueToeSec = ToeSec;
        UInt32Type.Accept(visitor, PayloadToeSecField, PayloadToeSecField.DataType, ref valueToeSec);
        if (!EqualityComparer<uint>.Default.Equals(valueToeSec, ToeSec))
        {
            ToeSec = valueToeSec;
        }

        var valueiode = iode;
        UInt8Type.Accept(visitor, PayloadiodeField, PayloadiodeField.DataType, ref valueiode);
        if (!EqualityComparer<byte>.Default.Equals(valueiode, iode))
        {
            iode = valueiode;
        }

        var valueiodc = iodc;
        UInt16Type.Accept(visitor, PayloadiodcField, PayloadiodcField.DataType, ref valueiodc);
        if (!EqualityComparer<ushort>.Default.Equals(valueiodc, iodc))
        {
            iodc = valueiodc;
        }

        var valueHealth = Health;
        UInt8Type.Accept(visitor, PayloadHealthField, PayloadHealthField.DataType, ref valueHealth);
        if (!EqualityComparer<byte>.Default.Equals(valueHealth, Health))
        {
            Health = valueHealth;
        }

        var valueAccuracy = Accuracy;
        UInt8Type.Accept(visitor, PayloadAccuracyField, PayloadAccuracyField.DataType, ref valueAccuracy);
        if (!EqualityComparer<byte>.Default.Equals(valueAccuracy, Accuracy))
        {
            Accuracy = valueAccuracy;
        }

        var valueAf0 = Af0;
        FloatType.Accept(visitor, PayloadAf0Field, PayloadAf0Field.DataType, ref valueAf0);
        if (!EqualityComparer<float>.Default.Equals(valueAf0, Af0))
        {
            Af0 = valueAf0;
        }

        var valueAf1 = Af1;
        FloatType.Accept(visitor, PayloadAf1Field, PayloadAf1Field.DataType, ref valueAf1);
        if (!EqualityComparer<float>.Default.Equals(valueAf1, Af1))
        {
            Af1 = valueAf1;
        }

        var valueAf2 = Af2;
        FloatType.Accept(visitor, PayloadAf2Field, PayloadAf2Field.DataType, ref valueAf2);
        if (!EqualityComparer<float>.Default.Equals(valueAf2, Af2))
        {
            Af2 = valueAf2;
        }

        var valueToe = Toe;
        DateTimeType.Accept(visitor, PayloadToeField, PayloadToeField.DataType, ref valueToe);
        if (!EqualityComparer<DateTime>.Default.Equals(valueToe, Toe))
        {
            Toe = valueToe;
        }

        var valueToc = Toc;
        DateTimeType.Accept(visitor, PayloadTocField, PayloadTocField.DataType, ref valueToc);
        if (!EqualityComparer<DateTime>.Default.Equals(valueToc, Toc))
        {
            Toc = valueToc;
        }

        var valueTimeTrans = TimeTrans;
        DateTimeType.Accept(visitor, PayloadTimeTransField, PayloadTimeTransField.DataType, ref valueTimeTrans);
        if (!EqualityComparer<DateTime>.Default.Equals(valueTimeTrans, TimeTrans))
        {
            TimeTrans = valueTimeTrans;
        }

        var valueA = A;
        DoubleType.Accept(visitor, PayloadAField, PayloadAField.DataType, ref valueA);
        if (!EqualityComparer<double>.Default.Equals(valueA, A))
        {
            A = valueA;
        }

        var valueE = E;
        DoubleType.Accept(visitor, PayloadEField, PayloadEField.DataType, ref valueE);
        if (!EqualityComparer<double>.Default.Equals(valueE, E))
        {
            E = valueE;
        }

        var valueomega = omega;
        DoubleType.Accept(visitor, PayloadomegaField, PayloadomegaField.DataType, ref valueomega);
        if (!EqualityComparer<double>.Default.Equals(valueomega, omega))
        {
            omega = valueomega;
        }

        var valueOMEGA0 = OMEGA0;
        DoubleType.Accept(visitor, PayloadOMEGA0Field, PayloadOMEGA0Field.DataType, ref valueOMEGA0);
        if (!EqualityComparer<double>.Default.Equals(valueOMEGA0, OMEGA0))
        {
            OMEGA0 = valueOMEGA0;
        }

        var valuei0 = i0;
        DoubleType.Accept(visitor, Payloadi0Field, Payloadi0Field.DataType, ref valuei0);
        if (!EqualityComparer<double>.Default.Equals(valuei0, i0))
        {
            i0 = valuei0;
        }

        var valueM0 = M0;
        DoubleType.Accept(visitor, PayloadM0Field, PayloadM0Field.DataType, ref valueM0);
        if (!EqualityComparer<double>.Default.Equals(valueM0, M0))
        {
            M0 = valueM0;
        }

        var valueDeltaN = DeltaN;
        FloatType.Accept(visitor, PayloadDeltaNField, PayloadDeltaNField.DataType, ref valueDeltaN);
        if (!EqualityComparer<float>.Default.Equals(valueDeltaN, DeltaN))
        {
            DeltaN = valueDeltaN;
        }

        var valueOmegaDot = OmegaDot;
        FloatType.Accept(visitor, PayloadOmegaDotField, PayloadOmegaDotField.DataType, ref valueOmegaDot);
        if (!EqualityComparer<float>.Default.Equals(valueOmegaDot, OmegaDot))
        {
            OmegaDot = valueOmegaDot;
        }

        var valueidot = idot;
        FloatType.Accept(visitor, PayloadidotField, PayloadidotField.DataType, ref valueidot);
        if (!EqualityComparer<float>.Default.Equals(valueidot, idot))
        {
            idot = valueidot;
        }

        var valueCrc = Crc;
        FloatType.Accept(visitor, PayloadCrcField, PayloadCrcField.DataType, ref valueCrc);
        if (!EqualityComparer<float>.Default.Equals(valueCrc, Crc))
        {
            Crc = valueCrc;
        }

        var valueCrs = Crs;
        FloatType.Accept(visitor, PayloadCrsField, PayloadCrsField.DataType, ref valueCrs);
        if (!EqualityComparer<float>.Default.Equals(valueCrs, Crs))
        {
            Crs = valueCrs;
        }

        var valueCuc = Cuc;
        FloatType.Accept(visitor, PayloadCucField, PayloadCucField.DataType, ref valueCuc);
        if (!EqualityComparer<float>.Default.Equals(valueCuc, Cuc))
        {
            Cuc = valueCuc;
        }

        var valueCus = Cus;
        FloatType.Accept(visitor, PayloadCusField, PayloadCusField.DataType, ref valueCus);
        if (!EqualityComparer<float>.Default.Equals(valueCus, Cus))
        {
            Cus = valueCus;
        }

        var valueCic = Cic;
        FloatType.Accept(visitor, PayloadCicField, PayloadCicField.DataType, ref valueCic);
        if (!EqualityComparer<float>.Default.Equals(valueCic, Cic))
        {
            Cic = valueCic;
        }

        var valueCis = Cis;
        FloatType.Accept(visitor, PayloadCisField, PayloadCisField.DataType, ref valueCis);
        if (!EqualityComparer<float>.Default.Equals(valueCis, Cis))
        {
            Cis = valueCis;
        }

        var valueTgd = Tgd;
        FloatType.Accept(visitor, PayloadTgdField, PayloadTgdField.DataType, ref valueTgd);
        if (!EqualityComparer<float>.Default.Equals(valueTgd, Tgd))
        {
            Tgd = valueTgd;
        }
    }
}
