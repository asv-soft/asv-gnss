using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixI034MessageCountValue : IVisitable
{
    private static readonly Field VisitorAerialField = new Field.Builder().Name(nameof(Aerial)).Title(nameof(Aerial)).Description("Aerial").DataType(BoolType.Default).Build();
    private static readonly Field VisitorTypeField = new Field.Builder().Name(nameof(Type)).Title(nameof(Type)).Description("Type").DataType(UInt8Type.Default).Build();
    private static readonly Field VisitorCounterField = new Field.Builder().Name(nameof(Counter)).Title(nameof(Counter)).Description("Counter").DataType(UInt16Type.Default).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorAerialField, VisitorTypeField, VisitorCounterField]);

    /// <inheritdoc />
    public void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueAerial = Aerial;
        BoolType.Accept(visitor, VisitorAerialField, ref valueAerial);
        if (!EqualityComparer<bool>.Default.Equals(valueAerial, Aerial)) Aerial = valueAerial;
        var valueType = Type;
        UInt8Type.Accept(visitor, VisitorTypeField, ref valueType);
        if (!EqualityComparer<byte>.Default.Equals(valueType, Type)) Type = valueType;
        var valueCounter = Counter;
        UInt16Type.Accept(visitor, VisitorCounterField, ref valueCounter);
        if (!EqualityComparer<ushort>.Default.Equals(valueCounter, Counter)) Counter = valueCounter;
    }
}
