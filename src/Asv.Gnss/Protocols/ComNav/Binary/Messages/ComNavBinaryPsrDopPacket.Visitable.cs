using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class ComNavBinaryPsrDopPacket
{
    private static readonly Field PayloadGdopField = SbfComNavVisitor.Field(nameof(Gdop), FloatType.Default, "Geometric dilution of precision");
    private static readonly Field PayloadPdopField = SbfComNavVisitor.Field(nameof(Pdop), FloatType.Default, "Position dilution of precision");
    private static readonly Field PayloadHdopField = SbfComNavVisitor.Field(nameof(Hdop), FloatType.Default, "Horizontal dilution of precision");
    private static readonly Field PayloadHtdopField = SbfComNavVisitor.Field(nameof(Htdop), FloatType.Default, "Horizontal position and time dilution of precision");
    private static readonly Field PayloadTdopField = SbfComNavVisitor.Field(nameof(Tdop), FloatType.Default, "Time dilution of precision");
    private static readonly Field PayloadCutoffField = SbfComNavVisitor.Field(nameof(Cutoff), FloatType.Default, "Elevation cut-off angle");
    private static readonly Field PayloadSatellitesField = SbfComNavVisitor.Field(nameof(Satellites), new ListType(UInt32Type.Default, 0, ushort.MaxValue), "PRNs of SV PRN tracking");

    internal new static readonly StructType PayloadVisitorType = new([..ComNavBinaryMessageBase.PayloadVisitorType.Fields, PayloadGdopField, PayloadPdopField, PayloadHdopField, PayloadHtdopField, PayloadTdopField, PayloadCutoffField, PayloadSatellitesField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        base.Accept(visitor);

        var valueGdop = Gdop;
        FloatType.Accept(visitor, PayloadGdopField, PayloadGdopField.DataType, ref valueGdop);
        if (!EqualityComparer<float>.Default.Equals(valueGdop, Gdop))
        {
            Gdop = valueGdop;
        }

        var valuePdop = Pdop;
        FloatType.Accept(visitor, PayloadPdopField, PayloadPdopField.DataType, ref valuePdop);
        if (!EqualityComparer<float>.Default.Equals(valuePdop, Pdop))
        {
            Pdop = valuePdop;
        }

        var valueHdop = Hdop;
        FloatType.Accept(visitor, PayloadHdopField, PayloadHdopField.DataType, ref valueHdop);
        if (!EqualityComparer<float>.Default.Equals(valueHdop, Hdop))
        {
            Hdop = valueHdop;
        }

        var valueHtdop = Htdop;
        FloatType.Accept(visitor, PayloadHtdopField, PayloadHtdopField.DataType, ref valueHtdop);
        if (!EqualityComparer<float>.Default.Equals(valueHtdop, Htdop))
        {
            Htdop = valueHtdop;
        }

        var valueTdop = Tdop;
        FloatType.Accept(visitor, PayloadTdopField, PayloadTdopField.DataType, ref valueTdop);
        if (!EqualityComparer<float>.Default.Equals(valueTdop, Tdop))
        {
            Tdop = valueTdop;
        }

        var valueCutoff = Cutoff;
        FloatType.Accept(visitor, PayloadCutoffField, PayloadCutoffField.DataType, ref valueCutoff);
        if (!EqualityComparer<float>.Default.Equals(valueCutoff, Cutoff))
        {
            Cutoff = valueCutoff;
        }

        var valueSatellites = Satellites;
        SbfComNavVisitor.VisitArray(visitor, PayloadSatellitesField, PayloadSatellitesField.DataType, ref valueSatellites, static () => default(uint),
            static (IVisitor v, Field f, IFieldType t, ref uint item) =>
            {
                UInt32Type.Accept(v, f, t, ref item);
            });
        if (!ReferenceEquals(valueSatellites, Satellites))
        {
            Satellites = valueSatellites!;
        }
    }
}
