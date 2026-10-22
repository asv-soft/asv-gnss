using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixFieldI010Frn011Type170
{
    private static readonly Field VisitorCnfField = new Field.Builder().Name(nameof(Cnf)).Title(nameof(Cnf)).Description("Cnf").DataType(BoolType.Default).Build();
    private static readonly Field VisitorTreField = new Field.Builder().Name(nameof(Tre)).Title(nameof(Tre)).Description("Tre").DataType(BoolType.Default).Build();
    private static readonly Field VisitorCstField = new Field.Builder().Name(nameof(Cst)).Title(nameof(Cst)).Description("Cst").DataType(BoolType.Default).Build();
    private static readonly Field VisitorMahField = new Field.Builder().Name(nameof(Mah)).Title(nameof(Mah)).Description("Mah").DataType(BoolType.Default).Build();
    private static readonly Field VisitorTccField = new Field.Builder().Name(nameof(Tcc)).Title(nameof(Tcc)).Description("Tcc").DataType(BoolType.Default).Build();
    private static readonly Field VisitorSthField = new Field.Builder().Name(nameof(Sth)).Title(nameof(Sth)).Description("Sth").DataType(BoolType.Default).Build();
    private static readonly Field VisitorTomField = new Field.Builder().Name(nameof(Tom)).Title(nameof(Tom)).Description("Tom").DataType(UInt8Type.Default).Build();
    private static readonly Field VisitorDouField = new Field.Builder().Name(nameof(Dou)).Title(nameof(Dou)).Description("Dou").DataType(BoolType.Default).Build();
    private static readonly Field VisitorMrsField = new Field.Builder().Name(nameof(Mrs)).Title(nameof(Mrs)).Description("Mrs").DataType(UInt8Type.Default).Build();
    private static readonly Field VisitorGhoField = new Field.Builder().Name(nameof(Gho)).Title(nameof(Gho)).Description("Gho").DataType(BoolType.Default).Build();
    private static readonly Field VisitorHasFirstExtensionField = new Field.Builder().Name(nameof(HasFirstExtension)).Title(nameof(HasFirstExtension)).Description("HasFirstExtension").DataType(BoolType.Default).Build();
    private static readonly Field VisitorHasSecondExtensionField = new Field.Builder().Name(nameof(HasSecondExtension)).Title(nameof(HasSecondExtension)).Description("HasSecondExtension").DataType(BoolType.Default).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorCnfField, VisitorTreField, VisitorCstField, VisitorMahField, VisitorTccField, VisitorSthField, VisitorTomField, VisitorDouField, VisitorMrsField, VisitorGhoField, VisitorHasFirstExtensionField, VisitorHasSecondExtensionField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueCnf = Cnf;
        BoolType.Accept(visitor, VisitorCnfField, ref valueCnf);
        if (!EqualityComparer<bool>.Default.Equals(valueCnf, Cnf)) Cnf = valueCnf;
        var valueTre = Tre;
        BoolType.Accept(visitor, VisitorTreField, ref valueTre);
        if (!EqualityComparer<bool>.Default.Equals(valueTre, Tre)) Tre = valueTre;
        var valueCst = Cst;
        BoolType.Accept(visitor, VisitorCstField, ref valueCst);
        if (!EqualityComparer<bool>.Default.Equals(valueCst, Cst)) Cst = valueCst;
        var valueMah = Mah;
        BoolType.Accept(visitor, VisitorMahField, ref valueMah);
        if (!EqualityComparer<bool>.Default.Equals(valueMah, Mah)) Mah = valueMah;
        var valueTcc = Tcc;
        BoolType.Accept(visitor, VisitorTccField, ref valueTcc);
        if (!EqualityComparer<bool>.Default.Equals(valueTcc, Tcc)) Tcc = valueTcc;
        var valueSth = Sth;
        BoolType.Accept(visitor, VisitorSthField, ref valueSth);
        if (!EqualityComparer<bool>.Default.Equals(valueSth, Sth)) Sth = valueSth;
        var valueTom = Tom;
        UInt8Type.Accept(visitor, VisitorTomField, ref valueTom);
        if (!EqualityComparer<byte>.Default.Equals(valueTom, Tom)) Tom = valueTom;
        var valueDou = Dou;
        BoolType.Accept(visitor, VisitorDouField, ref valueDou);
        if (!EqualityComparer<bool>.Default.Equals(valueDou, Dou)) Dou = valueDou;
        var valueMrs = Mrs;
        UInt8Type.Accept(visitor, VisitorMrsField, ref valueMrs);
        if (!EqualityComparer<byte>.Default.Equals(valueMrs, Mrs)) Mrs = valueMrs;
        var valueGho = Gho;
        BoolType.Accept(visitor, VisitorGhoField, ref valueGho);
        if (!EqualityComparer<bool>.Default.Equals(valueGho, Gho)) Gho = valueGho;
        var valueHasFirstExtension = HasFirstExtension;
        BoolType.Accept(visitor, VisitorHasFirstExtensionField, ref valueHasFirstExtension);
        if (!EqualityComparer<bool>.Default.Equals(valueHasFirstExtension, HasFirstExtension)) HasFirstExtension = valueHasFirstExtension;
        var valueHasSecondExtension = HasSecondExtension;
        BoolType.Accept(visitor, VisitorHasSecondExtensionField, ref valueHasSecondExtension);
        if (!EqualityComparer<bool>.Default.Equals(valueHasSecondExtension, HasSecondExtension)) HasSecondExtension = valueHasSecondExtension;
    }
}
