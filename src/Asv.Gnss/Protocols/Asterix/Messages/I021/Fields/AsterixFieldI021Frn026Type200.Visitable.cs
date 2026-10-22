using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixFieldI021Frn026Type200
{
    private static readonly Field VisitorIcfField = new Field.Builder().Name(nameof(Icf)).Title(nameof(Icf)).Description("Icf").DataType(BoolType.Default).Build();
    private static readonly Field VisitorLnavField = new Field.Builder().Name(nameof(Lnav)).Title(nameof(Lnav)).Description("Lnav").DataType(BoolType.Default).Build();
    private static readonly Field VisitorPsField = new Field.Builder().Name(nameof(Ps)).Title(nameof(Ps)).Description("Ps").DataType(UInt8Type.Default).Build();
    private static readonly Field VisitorSsField = new Field.Builder().Name(nameof(Ss)).Title(nameof(Ss)).Description("Ss").DataType(UInt8Type.Default).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorIcfField, VisitorLnavField, VisitorPsField, VisitorSsField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueIcf = Icf;
        BoolType.Accept(visitor, VisitorIcfField, ref valueIcf);
        if (!EqualityComparer<bool>.Default.Equals(valueIcf, Icf)) Icf = valueIcf;
        var valueLnav = Lnav;
        BoolType.Accept(visitor, VisitorLnavField, ref valueLnav);
        if (!EqualityComparer<bool>.Default.Equals(valueLnav, Lnav)) Lnav = valueLnav;
        var valuePs = Ps;
        UInt8Type.Accept(visitor, VisitorPsField, ref valuePs);
        if (!EqualityComparer<byte>.Default.Equals(valuePs, Ps)) Ps = valuePs;
        var valueSs = Ss;
        UInt8Type.Accept(visitor, VisitorSsField, ref valueSs);
        if (!EqualityComparer<byte>.Default.Equals(valueSs, Ss)) Ss = valueSs;
    }
}
