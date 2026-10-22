using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AdditionalDataBlock3
{
    private static readonly Field PayloadKmd_e_D_GPSField = ProtocolPayloadVisitor.Field(nameof(Kmd_e_D_GPS), DoubleType.Default);
    private static readonly Field PayloadKmd_e_D_GlonassField = ProtocolPayloadVisitor.Field(nameof(Kmd_e_D_Glonass), DoubleType.Default);
    private static readonly Field PayloadSigma_vert_iono_gradient_DField = ProtocolPayloadVisitor.Field(nameof(Sigma_vert_iono_gradient_D), DoubleType.Default);
    private static readonly Field PayloadY_EIGField = ProtocolPayloadVisitor.Field(nameof(Y_EIG), DoubleType.Default);
    private static readonly Field PayloadM_EIGField = ProtocolPayloadVisitor.Field(nameof(M_EIG), DoubleType.Default);

    internal new static readonly StructType PayloadVisitorType = new([..AdditionalDataBlock.PayloadVisitorType.Fields, PayloadKmd_e_D_GPSField, PayloadKmd_e_D_GlonassField, PayloadSigma_vert_iono_gradient_DField, PayloadY_EIGField, PayloadM_EIGField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        base.Accept(visitor);

        double valueKmd_e_D_GPS = Kmd_e_D_GPS;
        DoubleType.Accept(visitor, PayloadKmd_e_D_GPSField, PayloadKmd_e_D_GPSField.DataType, ref valueKmd_e_D_GPS);
        if (!EqualityComparer<double>.Default.Equals(valueKmd_e_D_GPS, Kmd_e_D_GPS))
        {
            Kmd_e_D_GPS = valueKmd_e_D_GPS;
        }

        double valueKmd_e_D_Glonass = Kmd_e_D_Glonass;
        DoubleType.Accept(visitor, PayloadKmd_e_D_GlonassField, PayloadKmd_e_D_GlonassField.DataType, ref valueKmd_e_D_Glonass);
        if (!EqualityComparer<double>.Default.Equals(valueKmd_e_D_Glonass, Kmd_e_D_Glonass))
        {
            Kmd_e_D_Glonass = valueKmd_e_D_Glonass;
        }

        double valueSigma_vert_iono_gradient_D = Sigma_vert_iono_gradient_D;
        DoubleType.Accept(visitor, PayloadSigma_vert_iono_gradient_DField, PayloadSigma_vert_iono_gradient_DField.DataType, ref valueSigma_vert_iono_gradient_D);
        if (!EqualityComparer<double>.Default.Equals(valueSigma_vert_iono_gradient_D, Sigma_vert_iono_gradient_D))
        {
            Sigma_vert_iono_gradient_D = valueSigma_vert_iono_gradient_D;
        }

        double valueY_EIG = Y_EIG;
        DoubleType.Accept(visitor, PayloadY_EIGField, PayloadY_EIGField.DataType, ref valueY_EIG);
        if (!EqualityComparer<double>.Default.Equals(valueY_EIG, Y_EIG))
        {
            Y_EIG = valueY_EIG;
        }

        double valueM_EIG = M_EIG;
        DoubleType.Accept(visitor, PayloadM_EIGField, PayloadM_EIGField.DataType, ref valueM_EIG);
        if (!EqualityComparer<double>.Default.Equals(valueM_EIG, M_EIG))
        {
            M_EIG = valueM_EIG;
        }
    }
}
