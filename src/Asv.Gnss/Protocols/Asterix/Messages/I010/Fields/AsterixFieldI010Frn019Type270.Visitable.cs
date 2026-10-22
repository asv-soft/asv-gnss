using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixFieldI010Frn019Type270
{
    private static readonly Field VisitorLengthField = new Field.Builder().Name(nameof(Length)).Title(nameof(Length)).Description("Length").DataType(DoubleType.Default).Build();
    private static readonly Field VisitorOrientationField = new Field.Builder().Name(nameof(Orientation)).Title(nameof(Orientation)).Description("Orientation").DataType(DoubleType.Default).Build();
    private static readonly Field VisitorWidthField = new Field.Builder().Name(nameof(Width)).Title(nameof(Width)).Description("Width").DataType(DoubleType.Default).Build();
    private static readonly Field VisitorHasOrientationField = new Field.Builder().Name(nameof(HasOrientation)).Title(nameof(HasOrientation)).Description("HasOrientation").DataType(BoolType.Default).Build();
    private static readonly Field VisitorHasWidthField = new Field.Builder().Name(nameof(HasWidth)).Title(nameof(HasWidth)).Description("HasWidth").DataType(BoolType.Default).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorLengthField, VisitorOrientationField, VisitorWidthField, VisitorHasOrientationField, VisitorHasWidthField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueLength = Length;
        DoubleType.Accept(visitor, VisitorLengthField, ref valueLength);
        if (!EqualityComparer<double>.Default.Equals(valueLength, Length)) Length = valueLength;
        var valueOrientation = Orientation;
        DoubleType.Accept(visitor, VisitorOrientationField, ref valueOrientation);
        if (!EqualityComparer<double>.Default.Equals(valueOrientation, Orientation)) Orientation = valueOrientation;
        var valueWidth = Width;
        DoubleType.Accept(visitor, VisitorWidthField, ref valueWidth);
        if (!EqualityComparer<double>.Default.Equals(valueWidth, Width)) Width = valueWidth;
        var valueHasOrientation = HasOrientation;
        BoolType.Accept(visitor, VisitorHasOrientationField, ref valueHasOrientation);
        if (!EqualityComparer<bool>.Default.Equals(valueHasOrientation, HasOrientation)) HasOrientation = valueHasOrientation;
        var valueHasWidth = HasWidth;
        BoolType.Accept(visitor, VisitorHasWidthField, ref valueHasWidth);
        if (!EqualityComparer<bool>.Default.Equals(valueHasWidth, HasWidth)) HasWidth = valueHasWidth;
    }
}
