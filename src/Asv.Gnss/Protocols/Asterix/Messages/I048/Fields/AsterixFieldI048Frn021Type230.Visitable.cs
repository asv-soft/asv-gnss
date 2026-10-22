using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixFieldI048Frn021Type230
{
    private static readonly Field VisitorComField = new Field.Builder().Name(nameof(Com)).Title(nameof(Com)).Description("Com").DataType(UInt8Type.Default).Build();
    private static readonly Field VisitorStatField = new Field.Builder().Name(nameof(Stat)).Title(nameof(Stat)).Description("Stat").DataType(UInt8Type.Default).Build();
    private static readonly Field VisitorMsscField = new Field.Builder().Name(nameof(Mssc)).Title(nameof(Mssc)).Description("Mssc").DataType(BoolType.Default).Build();
    private static readonly Field VisitorArcField = new Field.Builder().Name(nameof(Arc)).Title(nameof(Arc)).Description("Arc").DataType(BoolType.Default).Build();
    private static readonly Field VisitorAicField = new Field.Builder().Name(nameof(Aic)).Title(nameof(Aic)).Description("Aic").DataType(BoolType.Default).Build();
    private static readonly Field VisitorB1AField = new Field.Builder().Name(nameof(B1A)).Title(nameof(B1A)).Description("B1A").DataType(BoolType.Default).Build();
    private static readonly Field VisitorB1BField = new Field.Builder().Name(nameof(B1B)).Title(nameof(B1B)).Description("B1B").DataType(UInt8Type.Default).Build();
    private static readonly Field VisitorSiField = new Field.Builder().Name(nameof(Si)).Title(nameof(Si)).Description("Si").DataType(BoolType.Default).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorComField, VisitorStatField, VisitorMsscField, VisitorArcField, VisitorAicField, VisitorB1AField, VisitorB1BField, VisitorSiField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueCom = Com;
        UInt8Type.Accept(visitor, VisitorComField, ref valueCom);
        if (!EqualityComparer<byte>.Default.Equals(valueCom, Com)) Com = valueCom;
        var valueStat = Stat;
        UInt8Type.Accept(visitor, VisitorStatField, ref valueStat);
        if (!EqualityComparer<byte>.Default.Equals(valueStat, Stat)) Stat = valueStat;
        var valueMssc = Mssc;
        BoolType.Accept(visitor, VisitorMsscField, ref valueMssc);
        if (!EqualityComparer<bool>.Default.Equals(valueMssc, Mssc)) Mssc = valueMssc;
        var valueArc = Arc;
        BoolType.Accept(visitor, VisitorArcField, ref valueArc);
        if (!EqualityComparer<bool>.Default.Equals(valueArc, Arc)) Arc = valueArc;
        var valueAic = Aic;
        BoolType.Accept(visitor, VisitorAicField, ref valueAic);
        if (!EqualityComparer<bool>.Default.Equals(valueAic, Aic)) Aic = valueAic;
        var valueB1A = B1A;
        BoolType.Accept(visitor, VisitorB1AField, ref valueB1A);
        if (!EqualityComparer<bool>.Default.Equals(valueB1A, B1A)) B1A = valueB1A;
        var valueB1B = B1B;
        UInt8Type.Accept(visitor, VisitorB1BField, ref valueB1B);
        if (!EqualityComparer<byte>.Default.Equals(valueB1B, B1B)) B1B = valueB1B;
        var valueSi = Si;
        BoolType.Accept(visitor, VisitorSiField, ref valueSi);
        if (!EqualityComparer<bool>.Default.Equals(valueSi, Si)) Si = valueSi;
    }
}
