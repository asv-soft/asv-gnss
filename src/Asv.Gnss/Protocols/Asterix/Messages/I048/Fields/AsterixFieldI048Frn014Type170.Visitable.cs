using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixFieldI048Frn014Type170
{
    private static readonly Field VisitorCnfField = new Field.Builder().Name(nameof(Cnf)).Title(nameof(Cnf)).Description("Cnf").DataType(BoolType.Default).Build();
    private static readonly Field VisitorRadField = new Field.Builder().Name(nameof(Rad)).Title(nameof(Rad)).Description("Rad").DataType(UInt8Type.Default).Build();
    private static readonly Field VisitorDouField = new Field.Builder().Name(nameof(Dou)).Title(nameof(Dou)).Description("Dou").DataType(BoolType.Default).Build();
    private static readonly Field VisitorMahField = new Field.Builder().Name(nameof(Mah)).Title(nameof(Mah)).Description("Mah").DataType(BoolType.Default).Build();
    private static readonly Field VisitorCdmField = new Field.Builder().Name(nameof(Cdm)).Title(nameof(Cdm)).Description("Cdm").DataType(UInt8Type.Default).Build();
    private static readonly Field VisitorHasExtensionField = new Field.Builder().Name(nameof(HasExtension)).Title(nameof(HasExtension)).Description("HasExtension").DataType(BoolType.Default).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorCnfField, VisitorRadField, VisitorDouField, VisitorMahField, VisitorCdmField, VisitorHasExtensionField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueCnf = Cnf;
        BoolType.Accept(visitor, VisitorCnfField, ref valueCnf);
        if (!EqualityComparer<bool>.Default.Equals(valueCnf, Cnf)) Cnf = valueCnf;
        var valueRad = Rad;
        UInt8Type.Accept(visitor, VisitorRadField, ref valueRad);
        if (!EqualityComparer<byte>.Default.Equals(valueRad, Rad)) Rad = valueRad;
        var valueDou = Dou;
        BoolType.Accept(visitor, VisitorDouField, ref valueDou);
        if (!EqualityComparer<bool>.Default.Equals(valueDou, Dou)) Dou = valueDou;
        var valueMah = Mah;
        BoolType.Accept(visitor, VisitorMahField, ref valueMah);
        if (!EqualityComparer<bool>.Default.Equals(valueMah, Mah)) Mah = valueMah;
        var valueCdm = Cdm;
        UInt8Type.Accept(visitor, VisitorCdmField, ref valueCdm);
        if (!EqualityComparer<byte>.Default.Equals(valueCdm, Cdm)) Cdm = valueCdm;
        var valueHasExtension = HasExtension;
        BoolType.Accept(visitor, VisitorHasExtensionField, ref valueHasExtension);
        if (!EqualityComparer<bool>.Default.Equals(valueHasExtension, HasExtension)) HasExtension = valueHasExtension;
    }
}
