using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixFieldI048Frn003Type020
{
    private static readonly Field VisitorTypField = new Field.Builder().Name(nameof(Typ)).Title(nameof(Typ)).Description("Typ").DataType(UInt8Type.Default).Build();
    private static readonly Field VisitorSimField = new Field.Builder().Name(nameof(Sim)).Title(nameof(Sim)).Description("Sim").DataType(BoolType.Default).Build();
    private static readonly Field VisitorRdpField = new Field.Builder().Name(nameof(Rdp)).Title(nameof(Rdp)).Description("Rdp").DataType(UInt8Type.Default).Build();
    private static readonly Field VisitorSpiField = new Field.Builder().Name(nameof(Spi)).Title(nameof(Spi)).Description("Spi").DataType(BoolType.Default).Build();
    private static readonly Field VisitorRabField = new Field.Builder().Name(nameof(Rab)).Title(nameof(Rab)).Description("Rab").DataType(BoolType.Default).Build();
    private static readonly Field VisitorTstField = new Field.Builder().Name(nameof(Tst)).Title(nameof(Tst)).Description("Tst").DataType(BoolType.Default).Build();
    private static readonly Field VisitorErrField = new Field.Builder().Name(nameof(Err)).Title(nameof(Err)).Description("Err").DataType(BoolType.Default).Build();
    private static readonly Field VisitorXppField = new Field.Builder().Name(nameof(Xpp)).Title(nameof(Xpp)).Description("Xpp").DataType(BoolType.Default).Build();
    private static readonly Field VisitorMeField = new Field.Builder().Name(nameof(Me)).Title(nameof(Me)).Description("Me").DataType(BoolType.Default).Build();
    private static readonly Field VisitorMiField = new Field.Builder().Name(nameof(Mi)).Title(nameof(Mi)).Description("Mi").DataType(BoolType.Default).Build();
    private static readonly Field VisitorFoeFriField = new Field.Builder().Name(nameof(FoeFri)).Title(nameof(FoeFri)).Description("FoeFri").DataType(UInt8Type.Default).Build();
    private static readonly Field VisitorHasFirstExtensionField = new Field.Builder().Name(nameof(HasFirstExtension)).Title(nameof(HasFirstExtension)).Description("HasFirstExtension").DataType(BoolType.Default).Build();
    private static readonly Field VisitorHasSecondExtensionField = new Field.Builder().Name(nameof(HasSecondExtension)).Title(nameof(HasSecondExtension)).Description("HasSecondExtension").DataType(BoolType.Default).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorTypField, VisitorSimField, VisitorRdpField, VisitorSpiField, VisitorRabField, VisitorTstField, VisitorErrField, VisitorXppField, VisitorMeField, VisitorMiField, VisitorFoeFriField, VisitorHasFirstExtensionField, VisitorHasSecondExtensionField]);

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
        var valueRdp = Rdp;
        UInt8Type.Accept(visitor, VisitorRdpField, ref valueRdp);
        if (!EqualityComparer<byte>.Default.Equals(valueRdp, Rdp)) Rdp = valueRdp;
        var valueSpi = Spi;
        BoolType.Accept(visitor, VisitorSpiField, ref valueSpi);
        if (!EqualityComparer<bool>.Default.Equals(valueSpi, Spi)) Spi = valueSpi;
        var valueRab = Rab;
        BoolType.Accept(visitor, VisitorRabField, ref valueRab);
        if (!EqualityComparer<bool>.Default.Equals(valueRab, Rab)) Rab = valueRab;
        var valueTst = Tst;
        BoolType.Accept(visitor, VisitorTstField, ref valueTst);
        if (!EqualityComparer<bool>.Default.Equals(valueTst, Tst)) Tst = valueTst;
        var valueErr = Err;
        BoolType.Accept(visitor, VisitorErrField, ref valueErr);
        if (!EqualityComparer<bool>.Default.Equals(valueErr, Err)) Err = valueErr;
        var valueXpp = Xpp;
        BoolType.Accept(visitor, VisitorXppField, ref valueXpp);
        if (!EqualityComparer<bool>.Default.Equals(valueXpp, Xpp)) Xpp = valueXpp;
        var valueMe = Me;
        BoolType.Accept(visitor, VisitorMeField, ref valueMe);
        if (!EqualityComparer<bool>.Default.Equals(valueMe, Me)) Me = valueMe;
        var valueMi = Mi;
        BoolType.Accept(visitor, VisitorMiField, ref valueMi);
        if (!EqualityComparer<bool>.Default.Equals(valueMi, Mi)) Mi = valueMi;
        var valueFoeFri = FoeFri;
        UInt8Type.Accept(visitor, VisitorFoeFriField, ref valueFoeFri);
        if (!EqualityComparer<byte>.Default.Equals(valueFoeFri, FoeFri)) FoeFri = valueFoeFri;
        var valueHasFirstExtension = HasFirstExtension;
        BoolType.Accept(visitor, VisitorHasFirstExtensionField, ref valueHasFirstExtension);
        if (!EqualityComparer<bool>.Default.Equals(valueHasFirstExtension, HasFirstExtension)) HasFirstExtension = valueHasFirstExtension;
        var valueHasSecondExtension = HasSecondExtension;
        BoolType.Accept(visitor, VisitorHasSecondExtensionField, ref valueHasSecondExtension);
        if (!EqualityComparer<bool>.Default.Equals(valueHasSecondExtension, HasSecondExtension)) HasSecondExtension = valueHasSecondExtension;
    }
}
