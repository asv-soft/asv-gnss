using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AdditionalDataBlock1
{
    private static readonly Field PayloadRSDSField = ProtocolPayloadVisitor.Field(nameof(RSDS), UInt8Type.Default);
    private static readonly Field PayloadDMaxField = ProtocolPayloadVisitor.Field(nameof(DMax), UInt32Type.Default);
    private static readonly Field PayloadKmd_e_POS_GPSField = ProtocolPayloadVisitor.Field(nameof(Kmd_e_POS_GPS), DoubleType.Default);
    private static readonly Field PayloadKmd_e_GPSField = ProtocolPayloadVisitor.Field(nameof(Kmd_e_GPS), DoubleType.Default);
    private static readonly Field PayloadKmd_e_POS_GLONASSField = ProtocolPayloadVisitor.Field(nameof(Kmd_e_POS_GLONASS), DoubleType.Default);
    private static readonly Field PayloadKmd_e_GLONASSField = ProtocolPayloadVisitor.Field(nameof(Kmd_e_GLONASS), DoubleType.Default);

    internal new static readonly StructType PayloadVisitorType = new([..AdditionalDataBlock.PayloadVisitorType.Fields, PayloadRSDSField, PayloadDMaxField, PayloadKmd_e_POS_GPSField, PayloadKmd_e_GPSField, PayloadKmd_e_POS_GLONASSField, PayloadKmd_e_GLONASSField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        base.Accept(visitor);

        byte valueRSDS = RSDS;
        UInt8Type.Accept(visitor, PayloadRSDSField, PayloadRSDSField.DataType, ref valueRSDS);
        if (!EqualityComparer<byte>.Default.Equals(valueRSDS, RSDS))
        {
            RSDS = valueRSDS;
        }

        uint valueDMax = DMax;
        UInt32Type.Accept(visitor, PayloadDMaxField, PayloadDMaxField.DataType, ref valueDMax);
        if (!EqualityComparer<uint>.Default.Equals(valueDMax, DMax))
        {
            DMax = valueDMax;
        }

        double valueKmd_e_POS_GPS = Kmd_e_POS_GPS;
        DoubleType.Accept(visitor, PayloadKmd_e_POS_GPSField, PayloadKmd_e_POS_GPSField.DataType, ref valueKmd_e_POS_GPS);
        if (!EqualityComparer<double>.Default.Equals(valueKmd_e_POS_GPS, Kmd_e_POS_GPS))
        {
            Kmd_e_POS_GPS = valueKmd_e_POS_GPS;
        }

        double valueKmd_e_GPS = Kmd_e_GPS;
        DoubleType.Accept(visitor, PayloadKmd_e_GPSField, PayloadKmd_e_GPSField.DataType, ref valueKmd_e_GPS);
        if (!EqualityComparer<double>.Default.Equals(valueKmd_e_GPS, Kmd_e_GPS))
        {
            Kmd_e_GPS = valueKmd_e_GPS;
        }

        double valueKmd_e_POS_GLONASS = Kmd_e_POS_GLONASS;
        DoubleType.Accept(visitor, PayloadKmd_e_POS_GLONASSField, PayloadKmd_e_POS_GLONASSField.DataType, ref valueKmd_e_POS_GLONASS);
        if (!EqualityComparer<double>.Default.Equals(valueKmd_e_POS_GLONASS, Kmd_e_POS_GLONASS))
        {
            Kmd_e_POS_GLONASS = valueKmd_e_POS_GLONASS;
        }

        double valueKmd_e_GLONASS = Kmd_e_GLONASS;
        DoubleType.Accept(visitor, PayloadKmd_e_GLONASSField, PayloadKmd_e_GLONASSField.DataType, ref valueKmd_e_GLONASS);
        if (!EqualityComparer<double>.Default.Equals(valueKmd_e_GLONASS, Kmd_e_GLONASS))
        {
            Kmd_e_GLONASS = valueKmd_e_GLONASS;
        }
    }
}
