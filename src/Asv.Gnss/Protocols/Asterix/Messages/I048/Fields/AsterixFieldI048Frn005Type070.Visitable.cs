using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixFieldI048Frn005Type070
{
    private static readonly Field VisitorVField = new Field.Builder().Name(nameof(V)).Title(nameof(V)).Description("V").DataType(BoolType.Default).Build();
    private static readonly Field VisitorGField = new Field.Builder().Name(nameof(G)).Title(nameof(G)).Description("G").DataType(BoolType.Default).Build();
    private static readonly Field VisitorLField = new Field.Builder().Name(nameof(L)).Title(nameof(L)).Description("L").DataType(BoolType.Default).Build();
    private static readonly Field VisitorMode3AReplyField = new Field.Builder().Name(nameof(Mode3AReply)).Title(nameof(Mode3AReply)).Description("Mode3AReply").DataType(UInt16Type.Default).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorVField, VisitorGField, VisitorLField, VisitorMode3AReplyField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueV = V;
        BoolType.Accept(visitor, VisitorVField, ref valueV);
        if (!EqualityComparer<bool>.Default.Equals(valueV, V)) V = valueV;
        var valueG = G;
        BoolType.Accept(visitor, VisitorGField, ref valueG);
        if (!EqualityComparer<bool>.Default.Equals(valueG, G)) G = valueG;
        var valueL = L;
        BoolType.Accept(visitor, VisitorLField, ref valueL);
        if (!EqualityComparer<bool>.Default.Equals(valueL, L)) L = valueL;
        var valueMode3AReply = Mode3AReply;
        UInt16Type.Accept(visitor, VisitorMode3AReplyField, ref valueMode3AReply);
        if (!EqualityComparer<ushort>.Default.Equals(valueMode3AReply, Mode3AReply)) Mode3AReply = valueMode3AReply;
    }
}
