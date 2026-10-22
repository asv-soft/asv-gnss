using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixFieldI001Frn002Type020
{
    private static readonly Field VisitorTypField = new Field.Builder().Name(nameof(Typ)).Title(nameof(Typ)).Description("Typ").DataType(UInt8Type.Default).Build();
    private static readonly Field VisitorSimField = new Field.Builder().Name(nameof(Sim)).Title(nameof(Sim)).Description("Sim").DataType(BoolType.Default).Build();
    private static readonly Field VisitorSsrPsrField = new Field.Builder().Name(nameof(SsrPsr)).Title(nameof(SsrPsr)).Description("SsrPsr").DataType(UInt8Type.Default).Build();
    private static readonly Field VisitorAntField = new Field.Builder().Name(nameof(Ant)).Title(nameof(Ant)).Description("Ant").DataType(BoolType.Default).Build();
    private static readonly Field VisitorSpiField = new Field.Builder().Name(nameof(Spi)).Title(nameof(Spi)).Description("Spi").DataType(BoolType.Default).Build();
    private static readonly Field VisitorRabField = new Field.Builder().Name(nameof(Rab)).Title(nameof(Rab)).Description("Rab").DataType(BoolType.Default).Build();
    private static readonly Field VisitorFxField = new Field.Builder().Name(nameof(Fx)).Title(nameof(Fx)).Description("Fx").DataType(BoolType.Default).Metadata("readOnly", true).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorTypField, VisitorSimField, VisitorSsrPsrField, VisitorAntField, VisitorSpiField, VisitorRabField, VisitorFxField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueTyp = Typ;
        UInt8Type.Accept(visitor, VisitorTypField, ref valueTyp);
        if (!EqualityComparer<byte>.Default.Equals(valueTyp, Typ)) Typ = valueTyp;
        var valueSim = Sim;
        BoolType.Accept(visitor, VisitorSimField, ref valueSim);
        if (!EqualityComparer<bool>.Default.Equals(valueSim, Sim)) Sim = valueSim;
        var valueSsrPsr = SsrPsr;
        UInt8Type.Accept(visitor, VisitorSsrPsrField, ref valueSsrPsr);
        if (!EqualityComparer<byte>.Default.Equals(valueSsrPsr, SsrPsr)) SsrPsr = valueSsrPsr;
        var valueAnt = Ant;
        BoolType.Accept(visitor, VisitorAntField, ref valueAnt);
        if (!EqualityComparer<bool>.Default.Equals(valueAnt, Ant)) Ant = valueAnt;
        var valueSpi = Spi;
        BoolType.Accept(visitor, VisitorSpiField, ref valueSpi);
        if (!EqualityComparer<bool>.Default.Equals(valueSpi, Spi)) Spi = valueSpi;
        var valueRab = Rab;
        BoolType.Accept(visitor, VisitorRabField, ref valueRab);
        if (!EqualityComparer<bool>.Default.Equals(valueRab, Rab)) Rab = valueRab;
        var valueFx = Fx;
        BoolType.Accept(visitor, VisitorFxField, ref valueFx);
    }
}
