using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixFieldI021Frn042Type271
{
    private static readonly Field VisitorPoaField = new Field.Builder().Name(nameof(Poa)).Title(nameof(Poa)).Description("Poa").DataType(BoolType.Default).Build();
    private static readonly Field VisitorCdtiSField = new Field.Builder().Name(nameof(CdtiS)).Title(nameof(CdtiS)).Description("CdtiS").DataType(BoolType.Default).Build();
    private static readonly Field VisitorB2LowField = new Field.Builder().Name(nameof(B2Low)).Title(nameof(B2Low)).Description("B2Low").DataType(BoolType.Default).Build();
    private static readonly Field VisitorRasField = new Field.Builder().Name(nameof(Ras)).Title(nameof(Ras)).Description("Ras").DataType(BoolType.Default).Build();
    private static readonly Field VisitorIdentField = new Field.Builder().Name(nameof(Ident)).Title(nameof(Ident)).Description("Ident").DataType(BoolType.Default).Build();
    private static readonly Field VisitorLengthAndWidthField = new Field.Builder().Name(nameof(LengthAndWidth)).Title(nameof(LengthAndWidth)).Description("LengthAndWidth").DataType(UInt8Type.Default).Build();
    private static readonly Field VisitorHasExtensionField = new Field.Builder().Name(nameof(HasExtension)).Title(nameof(HasExtension)).Description("HasExtension").DataType(BoolType.Default).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorPoaField, VisitorCdtiSField, VisitorB2LowField, VisitorRasField, VisitorIdentField, VisitorLengthAndWidthField, VisitorHasExtensionField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valuePoa = Poa;
        BoolType.Accept(visitor, VisitorPoaField, ref valuePoa);
        if (!EqualityComparer<bool>.Default.Equals(valuePoa, Poa)) Poa = valuePoa;
        var valueCdtiS = CdtiS;
        BoolType.Accept(visitor, VisitorCdtiSField, ref valueCdtiS);
        if (!EqualityComparer<bool>.Default.Equals(valueCdtiS, CdtiS)) CdtiS = valueCdtiS;
        var valueB2Low = B2Low;
        BoolType.Accept(visitor, VisitorB2LowField, ref valueB2Low);
        if (!EqualityComparer<bool>.Default.Equals(valueB2Low, B2Low)) B2Low = valueB2Low;
        var valueRas = Ras;
        BoolType.Accept(visitor, VisitorRasField, ref valueRas);
        if (!EqualityComparer<bool>.Default.Equals(valueRas, Ras)) Ras = valueRas;
        var valueIdent = Ident;
        BoolType.Accept(visitor, VisitorIdentField, ref valueIdent);
        if (!EqualityComparer<bool>.Default.Equals(valueIdent, Ident)) Ident = valueIdent;
        var valueLengthAndWidth = LengthAndWidth;
        UInt8Type.Accept(visitor, VisitorLengthAndWidthField, ref valueLengthAndWidth);
        if (!EqualityComparer<byte>.Default.Equals(valueLengthAndWidth, LengthAndWidth)) LengthAndWidth = valueLengthAndWidth;
        var valueHasExtension = HasExtension;
        BoolType.Accept(visitor, VisitorHasExtensionField, ref valueHasExtension);
        if (!EqualityComparer<bool>.Default.Equals(valueHasExtension, HasExtension)) HasExtension = valueHasExtension;
    }
}
