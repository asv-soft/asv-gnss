using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsvMessagePvtGeo
{
    private static readonly Field PayloadTowField = ProtocolPayloadVisitor.Field(nameof(Tow), DateTimeType.Default);
    private static readonly Field PayloadPosTypeField = ProtocolPayloadVisitor.Field(nameof(PosType), Int32Type.Default, enumType: typeof(AsvPosTypeEnum));
    private static readonly Field PayloadErrorField = ProtocolPayloadVisitor.Field(nameof(Error), UInt32Type.Default);
    private static readonly Field PayloadLatitudeField = ProtocolPayloadVisitor.Field(nameof(Latitude), DoubleType.Default);
    private static readonly Field PayloadLongitudeField = ProtocolPayloadVisitor.Field(nameof(Longitude), DoubleType.Default);
    private static readonly Field PayloadHeightField = ProtocolPayloadVisitor.Field(nameof(Height), DoubleType.Default);
    private static readonly Field PayloadUndulationField = ProtocolPayloadVisitor.Field(nameof(Undulation), DoubleType.Default);
    private static readonly Field PayloadRxClkBiasField = ProtocolPayloadVisitor.Field(nameof(RxClkBias), DoubleType.Default);
    private static readonly Field PayloadRxClkDriftField = ProtocolPayloadVisitor.Field(nameof(RxClkDrift), DoubleType.Default);
    private static readonly Field PayloadTimeSystemField = ProtocolPayloadVisitor.Field(nameof(TimeSystem), Int32Type.Default, enumType: typeof(AsvTimeSystemEnum));
    private static readonly Field PayloadDatumField = ProtocolPayloadVisitor.Field(nameof(Datum), Int32Type.Default, enumType: typeof(AsvDatumEnum));
    private static readonly Field PayloadNrSvField = ProtocolPayloadVisitor.Field(nameof(NrSv), UInt32Type.Default);
    private static readonly Field PayloadMeanCorrAgeField = ProtocolPayloadVisitor.Field(nameof(MeanCorrAge), DoubleType.Default);
    private static readonly Field PayloadHAccuracyField = ProtocolPayloadVisitor.Field(nameof(HAccuracy), DoubleType.Default);
    private static readonly Field PayloadVAccuracyField = ProtocolPayloadVisitor.Field(nameof(VAccuracy), DoubleType.Default);

    internal new static readonly StructType PayloadVisitorType = new([..AsvMessageBase.PayloadVisitorType.Fields, PayloadTowField, PayloadPosTypeField, PayloadErrorField, PayloadLatitudeField, PayloadLongitudeField, PayloadHeightField, PayloadUndulationField, PayloadRxClkBiasField, PayloadRxClkDriftField, PayloadTimeSystemField, PayloadDatumField, PayloadNrSvField, PayloadMeanCorrAgeField, PayloadHAccuracyField, PayloadVAccuracyField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        base.Accept(visitor);

        DateTime valueTow = Tow;
        DateTimeType.Accept(visitor, PayloadTowField, PayloadTowField.DataType, ref valueTow);
        if (!EqualityComparer<DateTime>.Default.Equals(valueTow, Tow))
        {
            Tow = valueTow;
        }

        AsvPosTypeEnum valuePosType = PosType;
        var numericPosType = (int)valuePosType;
        Int32Type.Accept(visitor, PayloadPosTypeField, PayloadPosTypeField.DataType, ref numericPosType);
        valuePosType = (AsvPosTypeEnum)numericPosType;
        if (!EqualityComparer<AsvPosTypeEnum>.Default.Equals(valuePosType, PosType))
        {
            PosType = valuePosType;
        }

        uint valueError = Error;
        UInt32Type.Accept(visitor, PayloadErrorField, PayloadErrorField.DataType, ref valueError);
        if (!EqualityComparer<uint>.Default.Equals(valueError, Error))
        {
            Error = valueError;
        }

        double valueLatitude = Latitude;
        DoubleType.Accept(visitor, PayloadLatitudeField, PayloadLatitudeField.DataType, ref valueLatitude);
        if (!EqualityComparer<double>.Default.Equals(valueLatitude, Latitude))
        {
            Latitude = valueLatitude;
        }

        double valueLongitude = Longitude;
        DoubleType.Accept(visitor, PayloadLongitudeField, PayloadLongitudeField.DataType, ref valueLongitude);
        if (!EqualityComparer<double>.Default.Equals(valueLongitude, Longitude))
        {
            Longitude = valueLongitude;
        }

        double valueHeight = Height;
        DoubleType.Accept(visitor, PayloadHeightField, PayloadHeightField.DataType, ref valueHeight);
        if (!EqualityComparer<double>.Default.Equals(valueHeight, Height))
        {
            Height = valueHeight;
        }

        double valueUndulation = Undulation;
        DoubleType.Accept(visitor, PayloadUndulationField, PayloadUndulationField.DataType, ref valueUndulation);
        if (!EqualityComparer<double>.Default.Equals(valueUndulation, Undulation))
        {
            Undulation = valueUndulation;
        }

        double valueRxClkBias = RxClkBias;
        DoubleType.Accept(visitor, PayloadRxClkBiasField, PayloadRxClkBiasField.DataType, ref valueRxClkBias);
        if (!EqualityComparer<double>.Default.Equals(valueRxClkBias, RxClkBias))
        {
            RxClkBias = valueRxClkBias;
        }

        double valueRxClkDrift = RxClkDrift;
        DoubleType.Accept(visitor, PayloadRxClkDriftField, PayloadRxClkDriftField.DataType, ref valueRxClkDrift);
        if (!EqualityComparer<double>.Default.Equals(valueRxClkDrift, RxClkDrift))
        {
            RxClkDrift = valueRxClkDrift;
        }

        AsvTimeSystemEnum valueTimeSystem = TimeSystem;
        var numericTimeSystem = (int)valueTimeSystem;
        Int32Type.Accept(visitor, PayloadTimeSystemField, PayloadTimeSystemField.DataType, ref numericTimeSystem);
        valueTimeSystem = (AsvTimeSystemEnum)numericTimeSystem;
        if (!EqualityComparer<AsvTimeSystemEnum>.Default.Equals(valueTimeSystem, TimeSystem))
        {
            TimeSystem = valueTimeSystem;
        }

        AsvDatumEnum valueDatum = Datum;
        var numericDatum = (int)valueDatum;
        Int32Type.Accept(visitor, PayloadDatumField, PayloadDatumField.DataType, ref numericDatum);
        valueDatum = (AsvDatumEnum)numericDatum;
        if (!EqualityComparer<AsvDatumEnum>.Default.Equals(valueDatum, Datum))
        {
            Datum = valueDatum;
        }

        uint valueNrSv = NrSv;
        UInt32Type.Accept(visitor, PayloadNrSvField, PayloadNrSvField.DataType, ref valueNrSv);
        if (!EqualityComparer<uint>.Default.Equals(valueNrSv, NrSv))
        {
            NrSv = valueNrSv;
        }

        double valueMeanCorrAge = MeanCorrAge;
        DoubleType.Accept(visitor, PayloadMeanCorrAgeField, PayloadMeanCorrAgeField.DataType, ref valueMeanCorrAge);
        if (!EqualityComparer<double>.Default.Equals(valueMeanCorrAge, MeanCorrAge))
        {
            MeanCorrAge = valueMeanCorrAge;
        }

        double valueHAccuracy = HAccuracy;
        DoubleType.Accept(visitor, PayloadHAccuracyField, PayloadHAccuracyField.DataType, ref valueHAccuracy);
        if (!EqualityComparer<double>.Default.Equals(valueHAccuracy, HAccuracy))
        {
            HAccuracy = valueHAccuracy;
        }

        double valueVAccuracy = VAccuracy;
        DoubleType.Accept(visitor, PayloadVAccuracyField, PayloadVAccuracyField.DataType, ref valueVAccuracy);
        if (!EqualityComparer<double>.Default.Equals(valueVAccuracy, VAccuracy))
        {
            VAccuracy = valueVAccuracy;
        }
    }
}
