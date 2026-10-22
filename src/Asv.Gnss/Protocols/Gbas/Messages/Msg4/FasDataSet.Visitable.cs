using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class FasDataSet : IVisitable
{
    private static readonly Field PayloadOperationTypeField = ProtocolPayloadVisitor.Field(nameof(OperationType), UInt8Type.Default);
    private static readonly Field PayloadSbasServiceProviderField = ProtocolPayloadVisitor.Field(nameof(SbasServiceProvider), UInt8Type.Default, enumType: typeof(SBASServiceProviderEnum));
    private static readonly Field PayloadAirportIdField = ProtocolPayloadVisitor.Field(nameof(AirportId), StringOptionalType.Utf8);
    private static readonly Field PayloadRunwayNumberField = ProtocolPayloadVisitor.Field(nameof(RunwayNumber), UInt8Type.Default);
    private static readonly Field PayloadRunwayLetterField = ProtocolPayloadVisitor.Field(nameof(RunwayLetter), UInt8Type.Default, enumType: typeof(RunwayLetterEnum));
    private static readonly Field PayloadApproachPerformanceDesignatorField = ProtocolPayloadVisitor.Field(nameof(ApproachPerformanceDesignator), UInt8Type.Default, enumType: typeof(ApproachPerformanceDesignatorEnum));
    private static readonly Field PayloadRouteIndicatorField = ProtocolPayloadVisitor.Field(nameof(RouteIndicator), Int32Type.Default, enumType: typeof(RouteIndicatorEnum));
    private static readonly Field PayloadRPDSField = ProtocolPayloadVisitor.Field(nameof(RPDS), UInt8Type.Default);
    private static readonly Field PayloadReferencePathIdentifierField = ProtocolPayloadVisitor.Field(nameof(ReferencePathIdentifier), StringOptionalType.Utf8);
    private static readonly Field PayloadLtpFtpLatitudeField = ProtocolPayloadVisitor.Field(nameof(LtpFtpLatitude), DoubleType.Default);
    private static readonly Field PayloadLtpFtpLongitudeField = ProtocolPayloadVisitor.Field(nameof(LtpFtpLongitude), DoubleType.Default);
    private static readonly Field PayloadLtpFtpHeightField = ProtocolPayloadVisitor.Field(nameof(LtpFtpHeight), DoubleType.Default);
    private static readonly Field PayloadDeltaFPapLatitudeField = ProtocolPayloadVisitor.Field(nameof(DeltaFPapLatitude), DoubleType.Default);
    private static readonly Field PayloadDeltaFPapLongitudeField = ProtocolPayloadVisitor.Field(nameof(DeltaFPapLongitude), DoubleType.Default);
    private static readonly Field PayloadTCHField = ProtocolPayloadVisitor.Field(nameof(TCH), DoubleType.Default);
    private static readonly Field PayloadTCHUnitsField = ProtocolPayloadVisitor.Field(nameof(TCHUnits), UInt8Type.Default, enumType: typeof(TCHUnitsEnum));
    private static readonly Field PayloadGlidePathAngleField = ProtocolPayloadVisitor.Field(nameof(GlidePathAngle), DoubleType.Default);
    private static readonly Field PayloadCourseWidthField = ProtocolPayloadVisitor.Field(nameof(CourseWidth), DoubleType.Default);
    private static readonly Field PayloadDeltaLengthOffsetField = ProtocolPayloadVisitor.Field(nameof(DeltaLengthOffset), DoubleType.Default);
    private static readonly Field PayloadFASVALField = ProtocolPayloadVisitor.Field(nameof(FASVAL), DoubleType.Default);
    private static readonly Field PayloadFASLALField = ProtocolPayloadVisitor.Field(nameof(FASLAL), DoubleType.Default);

    internal static readonly StructType PayloadVisitorType = new([PayloadOperationTypeField, PayloadSbasServiceProviderField, PayloadAirportIdField, PayloadRunwayNumberField, PayloadRunwayLetterField, PayloadApproachPerformanceDesignatorField, PayloadRouteIndicatorField, PayloadRPDSField, PayloadReferencePathIdentifierField, PayloadLtpFtpLatitudeField, PayloadLtpFtpLongitudeField, PayloadLtpFtpHeightField, PayloadDeltaFPapLatitudeField, PayloadDeltaFPapLongitudeField, PayloadTCHField, PayloadTCHUnitsField, PayloadGlidePathAngleField, PayloadCourseWidthField, PayloadDeltaLengthOffsetField, PayloadFASVALField, PayloadFASLALField]);

    /// <inheritdoc />
    public virtual void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);

        byte valueOperationType = OperationType;
        UInt8Type.Accept(visitor, PayloadOperationTypeField, PayloadOperationTypeField.DataType, ref valueOperationType);
        if (!EqualityComparer<byte>.Default.Equals(valueOperationType, OperationType))
        {
            OperationType = valueOperationType;
        }

        SBASServiceProviderEnum valueSbasServiceProvider = SbasServiceProvider;
        var numericSbasServiceProvider = (byte)valueSbasServiceProvider;
        UInt8Type.Accept(visitor, PayloadSbasServiceProviderField, PayloadSbasServiceProviderField.DataType, ref numericSbasServiceProvider);
        valueSbasServiceProvider = (SBASServiceProviderEnum)numericSbasServiceProvider;
        if (!EqualityComparer<SBASServiceProviderEnum>.Default.Equals(valueSbasServiceProvider, SbasServiceProvider))
        {
            SbasServiceProvider = valueSbasServiceProvider;
        }

        string? valueAirportId = AirportId;
        StringOptionalType.Accept(visitor, PayloadAirportIdField, PayloadAirportIdField.DataType, ref valueAirportId);
        if (!EqualityComparer<string>.Default.Equals(valueAirportId, AirportId))
        {
            AirportId = valueAirportId!;
        }

        byte valueRunwayNumber = RunwayNumber;
        UInt8Type.Accept(visitor, PayloadRunwayNumberField, PayloadRunwayNumberField.DataType, ref valueRunwayNumber);
        if (!EqualityComparer<byte>.Default.Equals(valueRunwayNumber, RunwayNumber))
        {
            RunwayNumber = valueRunwayNumber;
        }

        RunwayLetterEnum valueRunwayLetter = RunwayLetter;
        var numericRunwayLetter = (byte)valueRunwayLetter;
        UInt8Type.Accept(visitor, PayloadRunwayLetterField, PayloadRunwayLetterField.DataType, ref numericRunwayLetter);
        valueRunwayLetter = (RunwayLetterEnum)numericRunwayLetter;
        if (!EqualityComparer<RunwayLetterEnum>.Default.Equals(valueRunwayLetter, RunwayLetter))
        {
            RunwayLetter = valueRunwayLetter;
        }

        ApproachPerformanceDesignatorEnum valueApproachPerformanceDesignator = ApproachPerformanceDesignator;
        var numericApproachPerformanceDesignator = (byte)valueApproachPerformanceDesignator;
        UInt8Type.Accept(visitor, PayloadApproachPerformanceDesignatorField, PayloadApproachPerformanceDesignatorField.DataType, ref numericApproachPerformanceDesignator);
        valueApproachPerformanceDesignator = (ApproachPerformanceDesignatorEnum)numericApproachPerformanceDesignator;
        if (!EqualityComparer<ApproachPerformanceDesignatorEnum>.Default.Equals(valueApproachPerformanceDesignator, ApproachPerformanceDesignator))
        {
            ApproachPerformanceDesignator = valueApproachPerformanceDesignator;
        }

        RouteIndicatorEnum valueRouteIndicator = RouteIndicator;
        var numericRouteIndicator = (int)valueRouteIndicator;
        Int32Type.Accept(visitor, PayloadRouteIndicatorField, PayloadRouteIndicatorField.DataType, ref numericRouteIndicator);
        valueRouteIndicator = (RouteIndicatorEnum)numericRouteIndicator;
        if (!EqualityComparer<RouteIndicatorEnum>.Default.Equals(valueRouteIndicator, RouteIndicator))
        {
            RouteIndicator = valueRouteIndicator;
        }

        byte valueRPDS = RPDS;
        UInt8Type.Accept(visitor, PayloadRPDSField, PayloadRPDSField.DataType, ref valueRPDS);
        if (!EqualityComparer<byte>.Default.Equals(valueRPDS, RPDS))
        {
            RPDS = valueRPDS;
        }

        string? valueReferencePathIdentifier = ReferencePathIdentifier;
        StringOptionalType.Accept(visitor, PayloadReferencePathIdentifierField, PayloadReferencePathIdentifierField.DataType, ref valueReferencePathIdentifier);
        if (!EqualityComparer<string>.Default.Equals(valueReferencePathIdentifier, ReferencePathIdentifier))
        {
            ReferencePathIdentifier = valueReferencePathIdentifier!;
        }

        double valueLtpFtpLatitude = LtpFtpLatitude;
        DoubleType.Accept(visitor, PayloadLtpFtpLatitudeField, PayloadLtpFtpLatitudeField.DataType, ref valueLtpFtpLatitude);
        if (!EqualityComparer<double>.Default.Equals(valueLtpFtpLatitude, LtpFtpLatitude))
        {
            LtpFtpLatitude = valueLtpFtpLatitude;
        }

        double valueLtpFtpLongitude = LtpFtpLongitude;
        DoubleType.Accept(visitor, PayloadLtpFtpLongitudeField, PayloadLtpFtpLongitudeField.DataType, ref valueLtpFtpLongitude);
        if (!EqualityComparer<double>.Default.Equals(valueLtpFtpLongitude, LtpFtpLongitude))
        {
            LtpFtpLongitude = valueLtpFtpLongitude;
        }

        double valueLtpFtpHeight = LtpFtpHeight;
        DoubleType.Accept(visitor, PayloadLtpFtpHeightField, PayloadLtpFtpHeightField.DataType, ref valueLtpFtpHeight);
        if (!EqualityComparer<double>.Default.Equals(valueLtpFtpHeight, LtpFtpHeight))
        {
            LtpFtpHeight = valueLtpFtpHeight;
        }

        double valueDeltaFPapLatitude = DeltaFPapLatitude;
        DoubleType.Accept(visitor, PayloadDeltaFPapLatitudeField, PayloadDeltaFPapLatitudeField.DataType, ref valueDeltaFPapLatitude);
        if (!EqualityComparer<double>.Default.Equals(valueDeltaFPapLatitude, DeltaFPapLatitude))
        {
            DeltaFPapLatitude = valueDeltaFPapLatitude;
        }

        double valueDeltaFPapLongitude = DeltaFPapLongitude;
        DoubleType.Accept(visitor, PayloadDeltaFPapLongitudeField, PayloadDeltaFPapLongitudeField.DataType, ref valueDeltaFPapLongitude);
        if (!EqualityComparer<double>.Default.Equals(valueDeltaFPapLongitude, DeltaFPapLongitude))
        {
            DeltaFPapLongitude = valueDeltaFPapLongitude;
        }

        double valueTCH = TCH;
        DoubleType.Accept(visitor, PayloadTCHField, PayloadTCHField.DataType, ref valueTCH);
        if (!EqualityComparer<double>.Default.Equals(valueTCH, TCH))
        {
            TCH = valueTCH;
        }

        TCHUnitsEnum valueTCHUnits = TCHUnits;
        var numericTCHUnits = (byte)valueTCHUnits;
        UInt8Type.Accept(visitor, PayloadTCHUnitsField, PayloadTCHUnitsField.DataType, ref numericTCHUnits);
        valueTCHUnits = (TCHUnitsEnum)numericTCHUnits;
        if (!EqualityComparer<TCHUnitsEnum>.Default.Equals(valueTCHUnits, TCHUnits))
        {
            TCHUnits = valueTCHUnits;
        }

        double valueGlidePathAngle = GlidePathAngle;
        DoubleType.Accept(visitor, PayloadGlidePathAngleField, PayloadGlidePathAngleField.DataType, ref valueGlidePathAngle);
        if (!EqualityComparer<double>.Default.Equals(valueGlidePathAngle, GlidePathAngle))
        {
            GlidePathAngle = valueGlidePathAngle;
        }

        double valueCourseWidth = CourseWidth;
        DoubleType.Accept(visitor, PayloadCourseWidthField, PayloadCourseWidthField.DataType, ref valueCourseWidth);
        if (!EqualityComparer<double>.Default.Equals(valueCourseWidth, CourseWidth))
        {
            CourseWidth = valueCourseWidth;
        }

        double valueDeltaLengthOffset = DeltaLengthOffset;
        DoubleType.Accept(visitor, PayloadDeltaLengthOffsetField, PayloadDeltaLengthOffsetField.DataType, ref valueDeltaLengthOffset);
        if (!EqualityComparer<double>.Default.Equals(valueDeltaLengthOffset, DeltaLengthOffset))
        {
            DeltaLengthOffset = valueDeltaLengthOffset;
        }

        double valueFASVAL = FASVAL;
        DoubleType.Accept(visitor, PayloadFASVALField, PayloadFASVALField.DataType, ref valueFASVAL);
        if (!EqualityComparer<double>.Default.Equals(valueFASVAL, FASVAL))
        {
            FASVAL = valueFASVAL;
        }

        double valueFASLAL = FASLAL;
        DoubleType.Accept(visitor, PayloadFASLALField, PayloadFASLALField.DataType, ref valueFASLAL);
        if (!EqualityComparer<double>.Default.Equals(valueFASLAL, FASLAL))
        {
            FASLAL = valueFASLAL;
        }
    }
}
