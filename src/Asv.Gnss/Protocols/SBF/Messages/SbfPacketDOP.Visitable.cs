using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class SbfPacketDOP
{
    private static readonly Field PayloadVPLField = SbfComNavVisitor.Field(nameof(VPL), DoubleType.Default, "Vertical Protection Level (see the DO 229 standard).");
    private static readonly Field PayloadHPLField = SbfComNavVisitor.Field(nameof(HPL), DoubleType.Default, "Horizontal Protection Level (see the DO 229 standard)");
    private static readonly Field PayloadVDOPField = SbfComNavVisitor.Field(nameof(VDOP), DoubleType.Default, "VDOP");
    private static readonly Field PayloadHDOPField = SbfComNavVisitor.Field(nameof(HDOP), DoubleType.Default, "HDOP");
    private static readonly Field PayloadTDOPField = SbfComNavVisitor.Field(nameof(TDOP), DoubleType.Default, "TDOP");
    private static readonly Field PayloadPDOPField = SbfComNavVisitor.Field(nameof(PDOP), DoubleType.Default, "If 0, PDOP not available");
    private static readonly Field PayloadReservedField = SbfComNavVisitor.Field(nameof(Reserved), UInt8Type.Default, "Reserved");
    private static readonly Field PayloadNrSVField = SbfComNavVisitor.Field(nameof(NrSV), UInt8Type.Default, "Total number of satellites used in the DOP computation, or 0 if the DOP information is not available(in that case, the xDOP fields are all set to 0)");

    internal new static readonly StructType PayloadVisitorType = new([..SbfMessageBase.PayloadVisitorType.Fields, PayloadVPLField, PayloadHPLField, PayloadVDOPField, PayloadHDOPField, PayloadTDOPField, PayloadPDOPField, PayloadReservedField, PayloadNrSVField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        base.Accept(visitor);

        var valueVPL = VPL;
        DoubleType.Accept(visitor, PayloadVPLField, PayloadVPLField.DataType, ref valueVPL);
        if (!EqualityComparer<double>.Default.Equals(valueVPL, VPL))
        {
            VPL = valueVPL;
        }

        var valueHPL = HPL;
        DoubleType.Accept(visitor, PayloadHPLField, PayloadHPLField.DataType, ref valueHPL);
        if (!EqualityComparer<double>.Default.Equals(valueHPL, HPL))
        {
            HPL = valueHPL;
        }

        var valueVDOP = VDOP;
        DoubleType.Accept(visitor, PayloadVDOPField, PayloadVDOPField.DataType, ref valueVDOP);
        if (!EqualityComparer<double>.Default.Equals(valueVDOP, VDOP))
        {
            VDOP = valueVDOP;
        }

        var valueHDOP = HDOP;
        DoubleType.Accept(visitor, PayloadHDOPField, PayloadHDOPField.DataType, ref valueHDOP);
        if (!EqualityComparer<double>.Default.Equals(valueHDOP, HDOP))
        {
            HDOP = valueHDOP;
        }

        var valueTDOP = TDOP;
        DoubleType.Accept(visitor, PayloadTDOPField, PayloadTDOPField.DataType, ref valueTDOP);
        if (!EqualityComparer<double>.Default.Equals(valueTDOP, TDOP))
        {
            TDOP = valueTDOP;
        }

        var valuePDOP = PDOP;
        DoubleType.Accept(visitor, PayloadPDOPField, PayloadPDOPField.DataType, ref valuePDOP);
        if (!EqualityComparer<double>.Default.Equals(valuePDOP, PDOP))
        {
            PDOP = valuePDOP;
        }

        var valueReserved = Reserved;
        UInt8Type.Accept(visitor, PayloadReservedField, PayloadReservedField.DataType, ref valueReserved);
        if (!EqualityComparer<byte>.Default.Equals(valueReserved, Reserved))
        {
            Reserved = valueReserved;
        }

        var valueNrSV = NrSV;
        UInt8Type.Accept(visitor, PayloadNrSVField, PayloadNrSVField.DataType, ref valueNrSV);
        if (!EqualityComparer<byte>.Default.Equals(valueNrSV, NrSV))
        {
            NrSV = valueNrSV;
        }
    }
}
