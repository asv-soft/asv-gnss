using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixFieldI021Frn041Type008
{
    private static readonly Field VisitorRaField = new Field.Builder().Name(nameof(Ra)).Title(nameof(Ra)).Description("Ra").DataType(BoolType.Default).Build();
    private static readonly Field VisitorTcField = new Field.Builder().Name(nameof(Tc)).Title(nameof(Tc)).Description("Tc").DataType(UInt8Type.Default).Build();
    private static readonly Field VisitorTsField = new Field.Builder().Name(nameof(Ts)).Title(nameof(Ts)).Description("Ts").DataType(BoolType.Default).Build();
    private static readonly Field VisitorArvField = new Field.Builder().Name(nameof(Arv)).Title(nameof(Arv)).Description("Arv").DataType(BoolType.Default).Build();
    private static readonly Field VisitorCdtiAField = new Field.Builder().Name(nameof(CdtiA)).Title(nameof(CdtiA)).Description("CdtiA").DataType(BoolType.Default).Build();
    private static readonly Field VisitorNotTcasField = new Field.Builder().Name(nameof(NotTcas)).Title(nameof(NotTcas)).Description("NotTcas").DataType(BoolType.Default).Build();
    private static readonly Field VisitorSaField = new Field.Builder().Name(nameof(Sa)).Title(nameof(Sa)).Description("Sa").DataType(BoolType.Default).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorRaField, VisitorTcField, VisitorTsField, VisitorArvField, VisitorCdtiAField, VisitorNotTcasField, VisitorSaField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueRa = Ra;
        BoolType.Accept(visitor, VisitorRaField, ref valueRa);
        if (!EqualityComparer<bool>.Default.Equals(valueRa, Ra)) Ra = valueRa;
        var valueTc = Tc;
        UInt8Type.Accept(visitor, VisitorTcField, ref valueTc);
        if (!EqualityComparer<byte>.Default.Equals(valueTc, Tc)) Tc = valueTc;
        var valueTs = Ts;
        BoolType.Accept(visitor, VisitorTsField, ref valueTs);
        if (!EqualityComparer<bool>.Default.Equals(valueTs, Ts)) Ts = valueTs;
        var valueArv = Arv;
        BoolType.Accept(visitor, VisitorArvField, ref valueArv);
        if (!EqualityComparer<bool>.Default.Equals(valueArv, Arv)) Arv = valueArv;
        var valueCdtiA = CdtiA;
        BoolType.Accept(visitor, VisitorCdtiAField, ref valueCdtiA);
        if (!EqualityComparer<bool>.Default.Equals(valueCdtiA, CdtiA)) CdtiA = valueCdtiA;
        var valueNotTcas = NotTcas;
        BoolType.Accept(visitor, VisitorNotTcasField, ref valueNotTcas);
        if (!EqualityComparer<bool>.Default.Equals(valueNotTcas, NotTcas)) NotTcas = valueNotTcas;
        var valueSa = Sa;
        BoolType.Accept(visitor, VisitorSaField, ref valueSa);
        if (!EqualityComparer<bool>.Default.Equals(valueSa, Sa)) Sa = valueSa;
    }
}
