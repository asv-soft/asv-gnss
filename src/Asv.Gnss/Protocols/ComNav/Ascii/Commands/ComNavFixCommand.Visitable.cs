using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class ComNavFixCommand
{
    private static readonly Field PayloadFixTypeField = SbfComNavVisitor.Field(nameof(FixType), Int32Type.Default, "Gets or sets the fix type of the ComNav device.", enumType: typeof(ComNavFixType));
    private static readonly Field PayloadLatField = SbfComNavVisitor.Field(nameof(Lat), DoubleType.Default, "Gets or sets the latitude coordinate.");
    private static readonly Field PayloadLonField = SbfComNavVisitor.Field(nameof(Lon), DoubleType.Default, "Gets or sets the longitude value.");
    private static readonly Field PayloadAltField = SbfComNavVisitor.Field(nameof(Alt), DoubleType.Default, "Gets or sets the altitude.");

    internal new static readonly StructType PayloadVisitorType = new([..ComNavAsciiCommandBase.PayloadVisitorType.Fields, PayloadFixTypeField, PayloadLatField, PayloadLonField, PayloadAltField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        base.Accept(visitor);

        var valueFixType = FixType;
        var numericvalueFixType = (int)valueFixType;
        Int32Type.Accept(visitor, PayloadFixTypeField, PayloadFixTypeField.DataType, ref numericvalueFixType);
        valueFixType = (ComNavFixType)numericvalueFixType;
        if (!EqualityComparer<ComNavFixType>.Default.Equals(valueFixType, FixType))
        {
            FixType = valueFixType;
        }

        var valueLat = Lat;
        DoubleType.Accept(visitor, PayloadLatField, PayloadLatField.DataType, ref valueLat);
        if (!EqualityComparer<double>.Default.Equals(valueLat, Lat))
        {
            Lat = valueLat;
        }

        var valueLon = Lon;
        DoubleType.Accept(visitor, PayloadLonField, PayloadLonField.DataType, ref valueLon);
        if (!EqualityComparer<double>.Default.Equals(valueLon, Lon))
        {
            Lon = valueLon;
        }

        var valueAlt = Alt;
        DoubleType.Accept(visitor, PayloadAltField, PayloadAltField.DataType, ref valueAlt);
        if (!EqualityComparer<double>.Default.Equals(valueAlt, Alt))
        {
            Alt = valueAlt;
        }
    }
}
