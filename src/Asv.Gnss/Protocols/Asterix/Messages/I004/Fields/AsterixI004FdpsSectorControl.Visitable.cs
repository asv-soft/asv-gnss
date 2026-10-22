using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixI004FdpsSectorControl : IVisitable
{
    private static readonly Field VisitorCentreField = new Field.Builder().Name(nameof(Centre)).Title(nameof(Centre)).Description("Centre").DataType(UInt8Type.Default).Build();
    private static readonly Field VisitorPositionField = new Field.Builder().Name(nameof(Position)).Title(nameof(Position)).Description("Position").DataType(UInt8Type.Default).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorCentreField, VisitorPositionField]);

    /// <inheritdoc />
    public void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueCentre = Centre;
        UInt8Type.Accept(visitor, VisitorCentreField, ref valueCentre);
        if (!EqualityComparer<byte>.Default.Equals(valueCentre, Centre)) Centre = valueCentre;
        var valuePosition = Position;
        UInt8Type.Accept(visitor, VisitorPositionField, ref valuePosition);
        if (!EqualityComparer<byte>.Default.Equals(valuePosition, Position)) Position = valuePosition;
    }
}
