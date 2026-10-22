using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class RtcmV2Message17
{
    private static readonly Field PayloadSatelliteField = ProtocolPayloadVisitor.Field(nameof(Satellite), Int32Type.Default);
    private static readonly Field PayloadWeekNumberRawField = ProtocolPayloadVisitor.Field(nameof(WeekNumberRaw), UInt32Type.Default);
    private static readonly Field PayloadWeekNumberField = ProtocolPayloadVisitor.Field(nameof(WeekNumber), Int32Type.Default);
    private static readonly Field PayloadIdotField = ProtocolPayloadVisitor.Field(nameof(Idot), DoubleType.Default);
    private static readonly Field PayloadIodeField = ProtocolPayloadVisitor.Field(nameof(Iode), UInt32Type.Default);
    private static readonly Field PayloadTocField = ProtocolPayloadVisitor.Field(nameof(Toc), DateTimeType.Default);
    private static readonly Field PayloadAF1Field = ProtocolPayloadVisitor.Field(nameof(AF1), DoubleType.Default);
    private static readonly Field PayloadAF2Field = ProtocolPayloadVisitor.Field(nameof(AF2), DoubleType.Default);
    private static readonly Field PayloadCrsField = ProtocolPayloadVisitor.Field(nameof(Crs), DoubleType.Default);
    private static readonly Field PayloadDeltaNField = ProtocolPayloadVisitor.Field(nameof(DeltaN), DoubleType.Default);
    private static readonly Field PayloadCucField = ProtocolPayloadVisitor.Field(nameof(Cuc), DoubleType.Default);
    private static readonly Field PayloadEField = ProtocolPayloadVisitor.Field(nameof(E), DoubleType.Default);
    private static readonly Field PayloadCusField = ProtocolPayloadVisitor.Field(nameof(Cus), Int16Type.Default);
    private static readonly Field PayloadAField = ProtocolPayloadVisitor.Field(nameof(A), DoubleType.Default);
    private static readonly Field PayloadToesField = ProtocolPayloadVisitor.Field(nameof(Toes), UInt16Type.Default);
    private static readonly Field PayloadToeField = ProtocolPayloadVisitor.Field(nameof(Toe), DateTimeType.Default);
    private static readonly Field PayloadOmega0Field = ProtocolPayloadVisitor.Field(nameof(Omega0), DoubleType.Default);
    private static readonly Field PayloadCicField = ProtocolPayloadVisitor.Field(nameof(Cic), DoubleType.Default);
    private static readonly Field PayloadI0Field = ProtocolPayloadVisitor.Field(nameof(I0), DoubleType.Default);
    private static readonly Field PayloadCisField = ProtocolPayloadVisitor.Field(nameof(Cis), DoubleType.Default);
    private static readonly Field PayloadOmegaField = ProtocolPayloadVisitor.Field(nameof(Omega), DoubleType.Default);
    private static readonly Field PayloadCrcField = ProtocolPayloadVisitor.Field(nameof(Crc), DoubleType.Default);
    private static readonly Field PayloadOmegaDotField = ProtocolPayloadVisitor.Field(nameof(OmegaDot), DoubleType.Default);
    private static readonly Field PayloadM0Field = ProtocolPayloadVisitor.Field(nameof(M0), DoubleType.Default);
    private static readonly Field PayloadIodcField = ProtocolPayloadVisitor.Field(nameof(Iodc), UInt16Type.Default);
    private static readonly Field PayloadAF0Field = ProtocolPayloadVisitor.Field(nameof(AF0), DoubleType.Default);
    private static readonly Field PayloadTgdField = ProtocolPayloadVisitor.Field(nameof(Tgd), DoubleType.Default);
    private static readonly Field PayloadCodeOnL2Field = ProtocolPayloadVisitor.Field(nameof(CodeOnL2), UInt8Type.Default);
    private static readonly Field PayloadSVAccuracyField = ProtocolPayloadVisitor.Field(nameof(SVAccuracy), UInt8Type.Default);
    private static readonly Field PayloadSVHealthField = ProtocolPayloadVisitor.Field(nameof(SVHealth), UInt8Type.Default);
    private static readonly Field PayloadL2PDataFlagField = ProtocolPayloadVisitor.Field(nameof(L2PDataFlag), UInt8Type.Default);

    internal new static readonly StructType PayloadVisitorType = new([..RtcmV2MessageBase.PayloadVisitorType.Fields, PayloadSatelliteField, PayloadWeekNumberRawField, PayloadWeekNumberField, PayloadIdotField, PayloadIodeField, PayloadTocField, PayloadAF1Field, PayloadAF2Field, PayloadCrsField, PayloadDeltaNField, PayloadCucField, PayloadEField, PayloadCusField, PayloadAField, PayloadToesField, PayloadToeField, PayloadOmega0Field, PayloadCicField, PayloadI0Field, PayloadCisField, PayloadOmegaField, PayloadCrcField, PayloadOmegaDotField, PayloadM0Field, PayloadIodcField, PayloadAF0Field, PayloadTgdField, PayloadCodeOnL2Field, PayloadSVAccuracyField, PayloadSVHealthField, PayloadL2PDataFlagField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        base.Accept(visitor);

        int valueSatellite = Satellite;
        Int32Type.Accept(visitor, PayloadSatelliteField, PayloadSatelliteField.DataType, ref valueSatellite);
        if (!EqualityComparer<int>.Default.Equals(valueSatellite, Satellite))
        {
            Satellite = valueSatellite;
        }

        uint valueWeekNumberRaw = WeekNumberRaw;
        UInt32Type.Accept(visitor, PayloadWeekNumberRawField, PayloadWeekNumberRawField.DataType, ref valueWeekNumberRaw);
        if (!EqualityComparer<uint>.Default.Equals(valueWeekNumberRaw, WeekNumberRaw))
        {
            WeekNumberRaw = valueWeekNumberRaw;
        }

        int valueWeekNumber = WeekNumber;
        Int32Type.Accept(visitor, PayloadWeekNumberField, PayloadWeekNumberField.DataType, ref valueWeekNumber);
        if (!EqualityComparer<int>.Default.Equals(valueWeekNumber, WeekNumber))
        {
            WeekNumber = valueWeekNumber;
        }

        double valueIdot = Idot;
        DoubleType.Accept(visitor, PayloadIdotField, PayloadIdotField.DataType, ref valueIdot);
        if (!EqualityComparer<double>.Default.Equals(valueIdot, Idot))
        {
            Idot = valueIdot;
        }

        uint valueIode = Iode;
        UInt32Type.Accept(visitor, PayloadIodeField, PayloadIodeField.DataType, ref valueIode);
        if (!EqualityComparer<uint>.Default.Equals(valueIode, Iode))
        {
            Iode = valueIode;
        }

        DateTime valueToc = Toc;
        DateTimeType.Accept(visitor, PayloadTocField, PayloadTocField.DataType, ref valueToc);
        if (!EqualityComparer<DateTime>.Default.Equals(valueToc, Toc))
        {
            Toc = valueToc;
        }

        double valueAF1 = AF1;
        DoubleType.Accept(visitor, PayloadAF1Field, PayloadAF1Field.DataType, ref valueAF1);
        if (!EqualityComparer<double>.Default.Equals(valueAF1, AF1))
        {
            AF1 = valueAF1;
        }

        double valueAF2 = AF2;
        DoubleType.Accept(visitor, PayloadAF2Field, PayloadAF2Field.DataType, ref valueAF2);
        if (!EqualityComparer<double>.Default.Equals(valueAF2, AF2))
        {
            AF2 = valueAF2;
        }

        double valueCrs = Crs;
        DoubleType.Accept(visitor, PayloadCrsField, PayloadCrsField.DataType, ref valueCrs);
        if (!EqualityComparer<double>.Default.Equals(valueCrs, Crs))
        {
            Crs = valueCrs;
        }

        double valueDeltaN = DeltaN;
        DoubleType.Accept(visitor, PayloadDeltaNField, PayloadDeltaNField.DataType, ref valueDeltaN);
        if (!EqualityComparer<double>.Default.Equals(valueDeltaN, DeltaN))
        {
            DeltaN = valueDeltaN;
        }

        double valueCuc = Cuc;
        DoubleType.Accept(visitor, PayloadCucField, PayloadCucField.DataType, ref valueCuc);
        if (!EqualityComparer<double>.Default.Equals(valueCuc, Cuc))
        {
            Cuc = valueCuc;
        }

        double valueE = E;
        DoubleType.Accept(visitor, PayloadEField, PayloadEField.DataType, ref valueE);
        if (!EqualityComparer<double>.Default.Equals(valueE, E))
        {
            E = valueE;
        }

        short valueCus = Cus;
        Int16Type.Accept(visitor, PayloadCusField, PayloadCusField.DataType, ref valueCus);
        if (!EqualityComparer<short>.Default.Equals(valueCus, Cus))
        {
            Cus = valueCus;
        }

        double valueA = A;
        DoubleType.Accept(visitor, PayloadAField, PayloadAField.DataType, ref valueA);
        if (!EqualityComparer<double>.Default.Equals(valueA, A))
        {
            A = valueA;
        }

        ushort valueToes = Toes;
        UInt16Type.Accept(visitor, PayloadToesField, PayloadToesField.DataType, ref valueToes);
        if (!EqualityComparer<ushort>.Default.Equals(valueToes, Toes))
        {
            Toes = valueToes;
        }

        DateTime valueToe = Toe;
        DateTimeType.Accept(visitor, PayloadToeField, PayloadToeField.DataType, ref valueToe);
        if (!EqualityComparer<DateTime>.Default.Equals(valueToe, Toe))
        {
            Toe = valueToe;
        }

        double valueOmega0 = Omega0;
        DoubleType.Accept(visitor, PayloadOmega0Field, PayloadOmega0Field.DataType, ref valueOmega0);
        if (!EqualityComparer<double>.Default.Equals(valueOmega0, Omega0))
        {
            Omega0 = valueOmega0;
        }

        double valueCic = Cic;
        DoubleType.Accept(visitor, PayloadCicField, PayloadCicField.DataType, ref valueCic);
        if (!EqualityComparer<double>.Default.Equals(valueCic, Cic))
        {
            Cic = valueCic;
        }

        double valueI0 = I0;
        DoubleType.Accept(visitor, PayloadI0Field, PayloadI0Field.DataType, ref valueI0);
        if (!EqualityComparer<double>.Default.Equals(valueI0, I0))
        {
            I0 = valueI0;
        }

        double valueCis = Cis;
        DoubleType.Accept(visitor, PayloadCisField, PayloadCisField.DataType, ref valueCis);
        if (!EqualityComparer<double>.Default.Equals(valueCis, Cis))
        {
            Cis = valueCis;
        }

        double valueOmega = Omega;
        DoubleType.Accept(visitor, PayloadOmegaField, PayloadOmegaField.DataType, ref valueOmega);
        if (!EqualityComparer<double>.Default.Equals(valueOmega, Omega))
        {
            Omega = valueOmega;
        }

        double valueCrc = Crc;
        DoubleType.Accept(visitor, PayloadCrcField, PayloadCrcField.DataType, ref valueCrc);
        if (!EqualityComparer<double>.Default.Equals(valueCrc, Crc))
        {
            Crc = valueCrc;
        }

        double valueOmegaDot = OmegaDot;
        DoubleType.Accept(visitor, PayloadOmegaDotField, PayloadOmegaDotField.DataType, ref valueOmegaDot);
        if (!EqualityComparer<double>.Default.Equals(valueOmegaDot, OmegaDot))
        {
            OmegaDot = valueOmegaDot;
        }

        double valueM0 = M0;
        DoubleType.Accept(visitor, PayloadM0Field, PayloadM0Field.DataType, ref valueM0);
        if (!EqualityComparer<double>.Default.Equals(valueM0, M0))
        {
            M0 = valueM0;
        }

        ushort valueIodc = Iodc;
        UInt16Type.Accept(visitor, PayloadIodcField, PayloadIodcField.DataType, ref valueIodc);
        if (!EqualityComparer<ushort>.Default.Equals(valueIodc, Iodc))
        {
            Iodc = valueIodc;
        }

        double valueAF0 = AF0;
        DoubleType.Accept(visitor, PayloadAF0Field, PayloadAF0Field.DataType, ref valueAF0);
        if (!EqualityComparer<double>.Default.Equals(valueAF0, AF0))
        {
            AF0 = valueAF0;
        }

        double valueTgd = Tgd;
        DoubleType.Accept(visitor, PayloadTgdField, PayloadTgdField.DataType, ref valueTgd);
        if (!EqualityComparer<double>.Default.Equals(valueTgd, Tgd))
        {
            Tgd = valueTgd;
        }

        byte valueCodeOnL2 = CodeOnL2;
        UInt8Type.Accept(visitor, PayloadCodeOnL2Field, PayloadCodeOnL2Field.DataType, ref valueCodeOnL2);
        if (!EqualityComparer<byte>.Default.Equals(valueCodeOnL2, CodeOnL2))
        {
            CodeOnL2 = valueCodeOnL2;
        }

        byte valueSVAccuracy = SVAccuracy;
        UInt8Type.Accept(visitor, PayloadSVAccuracyField, PayloadSVAccuracyField.DataType, ref valueSVAccuracy);
        if (!EqualityComparer<byte>.Default.Equals(valueSVAccuracy, SVAccuracy))
        {
            SVAccuracy = valueSVAccuracy;
        }

        byte valueSVHealth = SVHealth;
        UInt8Type.Accept(visitor, PayloadSVHealthField, PayloadSVHealthField.DataType, ref valueSVHealth);
        if (!EqualityComparer<byte>.Default.Equals(valueSVHealth, SVHealth))
        {
            SVHealth = valueSVHealth;
        }

        byte valueL2PDataFlag = L2PDataFlag;
        UInt8Type.Accept(visitor, PayloadL2PDataFlagField, PayloadL2PDataFlagField.DataType, ref valueL2PDataFlag);
        if (!EqualityComparer<byte>.Default.Equals(valueL2PDataFlag, L2PDataFlag))
        {
            L2PDataFlag = valueL2PDataFlag;
        }
    }
}
