using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixFieldI021Frn020Type210
{
    private static readonly Field VisitorVnsField = new Field.Builder().Name(nameof(Vns)).Title(nameof(Vns)).Description("Vns").DataType(BoolType.Default).Build();
    private static readonly Field VisitorVnField = new Field.Builder().Name(nameof(Vn)).Title(nameof(Vn)).Description("Vn").DataType(UInt8Type.Default).Build();
    private static readonly Field VisitorLttField = new Field.Builder().Name(nameof(Ltt)).Title(nameof(Ltt)).Description("Ltt").DataType(UInt8Type.Default).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorVnsField, VisitorVnField, VisitorLttField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueVns = Vns;
        BoolType.Accept(visitor, VisitorVnsField, ref valueVns);
        if (!EqualityComparer<bool>.Default.Equals(valueVns, Vns)) Vns = valueVns;
        var valueVn = Vn;
        UInt8Type.Accept(visitor, VisitorVnField, ref valueVn);
        if (!EqualityComparer<byte>.Default.Equals(valueVn, Vn)) Vn = valueVn;
        var valueLtt = Ltt;
        UInt8Type.Accept(visitor, VisitorLttField, ref valueLtt);
        if (!EqualityComparer<byte>.Default.Equals(valueLtt, Ltt)) Ltt = valueLtt;
    }
}
