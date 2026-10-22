using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class MessageBlockType2
{
    private static readonly Field PayloadGbasReferenceReceiversField = ProtocolPayloadVisitor.Field(nameof(GbasReferenceReceivers), UInt8Type.Default);
    private static readonly Field PayloadGroundAccuracyDesignatorLetterField = ProtocolPayloadVisitor.Field(nameof(GroundAccuracyDesignatorLetter), UInt8Type.Default, enumType: typeof(GADLEnum));
    private static readonly Field PayloadGbasDesignatorField = ProtocolPayloadVisitor.Field(nameof(GbasDesignator), UInt8Type.Default, enumType: typeof(GCIDEnum));
    private static readonly Field PayloadLocalMagneticVariationField = ProtocolPayloadVisitor.Field(nameof(LocalMagneticVariation), DoubleType.Default);
    private static readonly Field PayloadSigma_vert_iono_gradientField = ProtocolPayloadVisitor.Field(nameof(Sigma_vert_iono_gradient), DoubleType.Default);
    private static readonly Field PayloadRefractivityIndexField = ProtocolPayloadVisitor.Field(nameof(RefractivityIndex), UInt16Type.Default);
    private static readonly Field PayloadScaleHeightField = ProtocolPayloadVisitor.Field(nameof(ScaleHeight), UInt16Type.Default);
    private static readonly Field PayloadRefractivityUncertaintyField = ProtocolPayloadVisitor.Field(nameof(RefractivityUncertainty), UInt8Type.Default);
    private static readonly Field PayloadLatitudeField = ProtocolPayloadVisitor.Field(nameof(Latitude), DoubleType.Default);
    private static readonly Field PayloadLongitudeField = ProtocolPayloadVisitor.Field(nameof(Longitude), DoubleType.Default);
    private static readonly Field PayloadEllipsoidHeightField = ProtocolPayloadVisitor.Field(nameof(EllipsoidHeight), DoubleType.Default);
    private static readonly Field PayloadAdditionalDataField = ProtocolPayloadVisitor.Field(nameof(AdditionalData), new ListType(AdditionalDataBlock.PayloadVisitorType, 0, int.MaxValue), readOnly: true);

    internal new static readonly StructType PayloadVisitorType = new([..GbasMessageBlockBase.PayloadVisitorType.Fields, PayloadGbasReferenceReceiversField, PayloadGroundAccuracyDesignatorLetterField, PayloadGbasDesignatorField, PayloadLocalMagneticVariationField, PayloadSigma_vert_iono_gradientField, PayloadRefractivityIndexField, PayloadScaleHeightField, PayloadRefractivityUncertaintyField, PayloadLatitudeField, PayloadLongitudeField, PayloadEllipsoidHeightField, PayloadAdditionalDataField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        base.Accept(visitor);

        byte valueGbasReferenceReceivers = GbasReferenceReceivers;
        UInt8Type.Accept(visitor, PayloadGbasReferenceReceiversField, PayloadGbasReferenceReceiversField.DataType, ref valueGbasReferenceReceivers);
        if (!EqualityComparer<byte>.Default.Equals(valueGbasReferenceReceivers, GbasReferenceReceivers))
        {
            GbasReferenceReceivers = valueGbasReferenceReceivers;
        }

        GADLEnum valueGroundAccuracyDesignatorLetter = GroundAccuracyDesignatorLetter;
        var numericGroundAccuracyDesignatorLetter = (byte)valueGroundAccuracyDesignatorLetter;
        UInt8Type.Accept(visitor, PayloadGroundAccuracyDesignatorLetterField, PayloadGroundAccuracyDesignatorLetterField.DataType, ref numericGroundAccuracyDesignatorLetter);
        valueGroundAccuracyDesignatorLetter = (GADLEnum)numericGroundAccuracyDesignatorLetter;
        if (!EqualityComparer<GADLEnum>.Default.Equals(valueGroundAccuracyDesignatorLetter, GroundAccuracyDesignatorLetter))
        {
            GroundAccuracyDesignatorLetter = valueGroundAccuracyDesignatorLetter;
        }

        GCIDEnum valueGbasDesignator = GbasDesignator;
        var numericGbasDesignator = (byte)valueGbasDesignator;
        UInt8Type.Accept(visitor, PayloadGbasDesignatorField, PayloadGbasDesignatorField.DataType, ref numericGbasDesignator);
        valueGbasDesignator = (GCIDEnum)numericGbasDesignator;
        if (!EqualityComparer<GCIDEnum>.Default.Equals(valueGbasDesignator, GbasDesignator))
        {
            GbasDesignator = valueGbasDesignator;
        }

        double valueLocalMagneticVariation = LocalMagneticVariation;
        DoubleType.Accept(visitor, PayloadLocalMagneticVariationField, PayloadLocalMagneticVariationField.DataType, ref valueLocalMagneticVariation);
        if (!EqualityComparer<double>.Default.Equals(valueLocalMagneticVariation, LocalMagneticVariation))
        {
            LocalMagneticVariation = valueLocalMagneticVariation;
        }

        double valueSigma_vert_iono_gradient = Sigma_vert_iono_gradient;
        DoubleType.Accept(visitor, PayloadSigma_vert_iono_gradientField, PayloadSigma_vert_iono_gradientField.DataType, ref valueSigma_vert_iono_gradient);
        if (!EqualityComparer<double>.Default.Equals(valueSigma_vert_iono_gradient, Sigma_vert_iono_gradient))
        {
            Sigma_vert_iono_gradient = valueSigma_vert_iono_gradient;
        }

        ushort valueRefractivityIndex = RefractivityIndex;
        UInt16Type.Accept(visitor, PayloadRefractivityIndexField, PayloadRefractivityIndexField.DataType, ref valueRefractivityIndex);
        if (!EqualityComparer<ushort>.Default.Equals(valueRefractivityIndex, RefractivityIndex))
        {
            RefractivityIndex = valueRefractivityIndex;
        }

        ushort valueScaleHeight = ScaleHeight;
        UInt16Type.Accept(visitor, PayloadScaleHeightField, PayloadScaleHeightField.DataType, ref valueScaleHeight);
        if (!EqualityComparer<ushort>.Default.Equals(valueScaleHeight, ScaleHeight))
        {
            ScaleHeight = valueScaleHeight;
        }

        byte valueRefractivityUncertainty = RefractivityUncertainty;
        UInt8Type.Accept(visitor, PayloadRefractivityUncertaintyField, PayloadRefractivityUncertaintyField.DataType, ref valueRefractivityUncertainty);
        if (!EqualityComparer<byte>.Default.Equals(valueRefractivityUncertainty, RefractivityUncertainty))
        {
            RefractivityUncertainty = valueRefractivityUncertainty;
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

        double valueEllipsoidHeight = EllipsoidHeight;
        DoubleType.Accept(visitor, PayloadEllipsoidHeightField, PayloadEllipsoidHeightField.DataType, ref valueEllipsoidHeight);
        if (!EqualityComparer<double>.Default.Equals(valueEllipsoidHeight, EllipsoidHeight))
        {
            EllipsoidHeight = valueEllipsoidHeight;
        }

        List<AdditionalDataBlock> valueAdditionalData = AdditionalData;
        ProtocolPayloadVisitor.VisitList(visitor, PayloadAdditionalDataField, PayloadAdditionalDataField.DataType, ref valueAdditionalData,
            static () => default(AdditionalDataBlock)!,
            static (IVisitor v, Field f, IFieldType t, ref AdditionalDataBlock item) =>
            {
                if (item is not null)
                {
                    StructType.Accept(v, f, t, item);
                }
                else
                {
                    v.VisitUnknown(f, t);
                }
            });
    }
}
