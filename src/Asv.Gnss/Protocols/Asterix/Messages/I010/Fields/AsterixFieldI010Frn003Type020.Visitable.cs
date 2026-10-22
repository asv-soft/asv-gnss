using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixFieldI010Frn003Type020
{
    private static readonly Field VisitorTypField = new Field.Builder().Name(nameof(Typ)).Title(nameof(Typ)).Description("Typ").DataType(UInt8Type.Default).Build();
    private static readonly Field VisitorDcrField = new Field.Builder().Name(nameof(Dcr)).Title(nameof(Dcr)).Description("Dcr").DataType(BoolType.Default).Build();
    private static readonly Field VisitorChnField = new Field.Builder().Name(nameof(Chn)).Title(nameof(Chn)).Description("Chn").DataType(BoolType.Default).Build();
    private static readonly Field VisitorGbsField = new Field.Builder().Name(nameof(Gbs)).Title(nameof(Gbs)).Description("Gbs").DataType(BoolType.Default).Build();
    private static readonly Field VisitorCrtField = new Field.Builder().Name(nameof(Crt)).Title(nameof(Crt)).Description("Crt").DataType(BoolType.Default).Build();
    private static readonly Field VisitorSimField = new Field.Builder().Name(nameof(Sim)).Title(nameof(Sim)).Description("Sim").DataType(BoolType.Default).Build();
    private static readonly Field VisitorTstField = new Field.Builder().Name(nameof(Tst)).Title(nameof(Tst)).Description("Tst").DataType(BoolType.Default).Build();
    private static readonly Field VisitorRabField = new Field.Builder().Name(nameof(Rab)).Title(nameof(Rab)).Description("Rab").DataType(BoolType.Default).Build();
    private static readonly Field VisitorLopField = new Field.Builder().Name(nameof(Lop)).Title(nameof(Lop)).Description("Lop").DataType(UInt8Type.Default).Build();
    private static readonly Field VisitorTotField = new Field.Builder().Name(nameof(Tot)).Title(nameof(Tot)).Description("Tot").DataType(UInt8Type.Default).Build();
    private static readonly Field VisitorSpiField = new Field.Builder().Name(nameof(Spi)).Title(nameof(Spi)).Description("Spi").DataType(BoolType.Default).Build();
    private static readonly Field VisitorHasFirstExtensionField = new Field.Builder().Name(nameof(HasFirstExtension)).Title(nameof(HasFirstExtension)).Description("HasFirstExtension").DataType(BoolType.Default).Build();
    private static readonly Field VisitorHasSecondExtensionField = new Field.Builder().Name(nameof(HasSecondExtension)).Title(nameof(HasSecondExtension)).Description("HasSecondExtension").DataType(BoolType.Default).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorTypField, VisitorDcrField, VisitorChnField, VisitorGbsField, VisitorCrtField, VisitorSimField, VisitorTstField, VisitorRabField, VisitorLopField, VisitorTotField, VisitorSpiField, VisitorHasFirstExtensionField, VisitorHasSecondExtensionField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueTyp = Typ;
        UInt8Type.Accept(visitor, VisitorTypField, ref valueTyp);
        if (!EqualityComparer<byte>.Default.Equals(valueTyp, Typ)) Typ = valueTyp;
        var valueDcr = Dcr;
        BoolType.Accept(visitor, VisitorDcrField, ref valueDcr);
        if (!EqualityComparer<bool>.Default.Equals(valueDcr, Dcr)) Dcr = valueDcr;
        var valueChn = Chn;
        BoolType.Accept(visitor, VisitorChnField, ref valueChn);
        if (!EqualityComparer<bool>.Default.Equals(valueChn, Chn)) Chn = valueChn;
        var valueGbs = Gbs;
        BoolType.Accept(visitor, VisitorGbsField, ref valueGbs);
        if (!EqualityComparer<bool>.Default.Equals(valueGbs, Gbs)) Gbs = valueGbs;
        var valueCrt = Crt;
        BoolType.Accept(visitor, VisitorCrtField, ref valueCrt);
        if (!EqualityComparer<bool>.Default.Equals(valueCrt, Crt)) Crt = valueCrt;
        var valueSim = Sim;
        BoolType.Accept(visitor, VisitorSimField, ref valueSim);
        if (!EqualityComparer<bool>.Default.Equals(valueSim, Sim)) Sim = valueSim;
        var valueTst = Tst;
        BoolType.Accept(visitor, VisitorTstField, ref valueTst);
        if (!EqualityComparer<bool>.Default.Equals(valueTst, Tst)) Tst = valueTst;
        var valueRab = Rab;
        BoolType.Accept(visitor, VisitorRabField, ref valueRab);
        if (!EqualityComparer<bool>.Default.Equals(valueRab, Rab)) Rab = valueRab;
        var valueLop = Lop;
        UInt8Type.Accept(visitor, VisitorLopField, ref valueLop);
        if (!EqualityComparer<byte>.Default.Equals(valueLop, Lop)) Lop = valueLop;
        var valueTot = Tot;
        UInt8Type.Accept(visitor, VisitorTotField, ref valueTot);
        if (!EqualityComparer<byte>.Default.Equals(valueTot, Tot)) Tot = valueTot;
        var valueSpi = Spi;
        BoolType.Accept(visitor, VisitorSpiField, ref valueSpi);
        if (!EqualityComparer<bool>.Default.Equals(valueSpi, Spi)) Spi = valueSpi;
        var valueHasFirstExtension = HasFirstExtension;
        BoolType.Accept(visitor, VisitorHasFirstExtensionField, ref valueHasFirstExtension);
        if (!EqualityComparer<bool>.Default.Equals(valueHasFirstExtension, HasFirstExtension)) HasFirstExtension = valueHasFirstExtension;
        var valueHasSecondExtension = HasSecondExtension;
        BoolType.Accept(visitor, VisitorHasSecondExtensionField, ref valueHasSecondExtension);
        if (!EqualityComparer<bool>.Default.Equals(valueHasSecondExtension, HasSecondExtension)) HasSecondExtension = valueHasSecondExtension;
    }
}
