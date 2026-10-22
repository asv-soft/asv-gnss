using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixFieldI004Frn007Type060
{
    private static readonly Field VisitorMrvaField = new Field.Builder().Name(nameof(Mrva)).Title(nameof(Mrva)).Description("Mrva").DataType(BoolType.Default).Build();
    private static readonly Field VisitorRamldField = new Field.Builder().Name(nameof(Ramld)).Title(nameof(Ramld)).Description("Ramld").DataType(BoolType.Default).Build();
    private static readonly Field VisitorRamhdField = new Field.Builder().Name(nameof(Ramhd)).Title(nameof(Ramhd)).Description("Ramhd").DataType(BoolType.Default).Build();
    private static readonly Field VisitorMsawField = new Field.Builder().Name(nameof(Msaw)).Title(nameof(Msaw)).Description("Msaw").DataType(BoolType.Default).Build();
    private static readonly Field VisitorApwField = new Field.Builder().Name(nameof(Apw)).Title(nameof(Apw)).Description("Apw").DataType(BoolType.Default).Build();
    private static readonly Field VisitorClamField = new Field.Builder().Name(nameof(Clam)).Title(nameof(Clam)).Description("Clam").DataType(BoolType.Default).Build();
    private static readonly Field VisitorStcaField = new Field.Builder().Name(nameof(Stca)).Title(nameof(Stca)).Description("Stca").DataType(BoolType.Default).Build();
    private static readonly Field VisitorAfdaField = new Field.Builder().Name(nameof(Afda)).Title(nameof(Afda)).Description("Afda").DataType(BoolType.Default).Build();
    private static readonly Field VisitorRimcaField = new Field.Builder().Name(nameof(Rimca)).Title(nameof(Rimca)).Description("Rimca").DataType(BoolType.Default).Build();
    private static readonly Field VisitorOverflowField = new Field.Builder().Name(nameof(Overflow)).Title(nameof(Overflow)).Description("Overflow").DataType(BoolType.Default).Build();
    private static readonly Field VisitorOverloadField = new Field.Builder().Name(nameof(Overload)).Title(nameof(Overload)).Description("Overload").DataType(BoolType.Default).Build();
    private static readonly Field VisitorHasFirstExtensionField = new Field.Builder().Name(nameof(HasFirstExtension)).Title(nameof(HasFirstExtension)).Description("HasFirstExtension").DataType(BoolType.Default).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorMrvaField, VisitorRamldField, VisitorRamhdField, VisitorMsawField, VisitorApwField, VisitorClamField, VisitorStcaField, VisitorAfdaField, VisitorRimcaField, VisitorOverflowField, VisitorOverloadField, VisitorHasFirstExtensionField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueMrva = Mrva;
        BoolType.Accept(visitor, VisitorMrvaField, ref valueMrva);
        if (!EqualityComparer<bool>.Default.Equals(valueMrva, Mrva)) Mrva = valueMrva;
        var valueRamld = Ramld;
        BoolType.Accept(visitor, VisitorRamldField, ref valueRamld);
        if (!EqualityComparer<bool>.Default.Equals(valueRamld, Ramld)) Ramld = valueRamld;
        var valueRamhd = Ramhd;
        BoolType.Accept(visitor, VisitorRamhdField, ref valueRamhd);
        if (!EqualityComparer<bool>.Default.Equals(valueRamhd, Ramhd)) Ramhd = valueRamhd;
        var valueMsaw = Msaw;
        BoolType.Accept(visitor, VisitorMsawField, ref valueMsaw);
        if (!EqualityComparer<bool>.Default.Equals(valueMsaw, Msaw)) Msaw = valueMsaw;
        var valueApw = Apw;
        BoolType.Accept(visitor, VisitorApwField, ref valueApw);
        if (!EqualityComparer<bool>.Default.Equals(valueApw, Apw)) Apw = valueApw;
        var valueClam = Clam;
        BoolType.Accept(visitor, VisitorClamField, ref valueClam);
        if (!EqualityComparer<bool>.Default.Equals(valueClam, Clam)) Clam = valueClam;
        var valueStca = Stca;
        BoolType.Accept(visitor, VisitorStcaField, ref valueStca);
        if (!EqualityComparer<bool>.Default.Equals(valueStca, Stca)) Stca = valueStca;
        var valueAfda = Afda;
        BoolType.Accept(visitor, VisitorAfdaField, ref valueAfda);
        if (!EqualityComparer<bool>.Default.Equals(valueAfda, Afda)) Afda = valueAfda;
        var valueRimca = Rimca;
        BoolType.Accept(visitor, VisitorRimcaField, ref valueRimca);
        if (!EqualityComparer<bool>.Default.Equals(valueRimca, Rimca)) Rimca = valueRimca;
        var valueOverflow = Overflow;
        BoolType.Accept(visitor, VisitorOverflowField, ref valueOverflow);
        if (!EqualityComparer<bool>.Default.Equals(valueOverflow, Overflow)) Overflow = valueOverflow;
        var valueOverload = Overload;
        BoolType.Accept(visitor, VisitorOverloadField, ref valueOverload);
        if (!EqualityComparer<bool>.Default.Equals(valueOverload, Overload)) Overload = valueOverload;
        var valueHasFirstExtension = HasFirstExtension;
        BoolType.Accept(visitor, VisitorHasFirstExtensionField, ref valueHasFirstExtension);
        if (!EqualityComparer<bool>.Default.Equals(valueHasFirstExtension, HasFirstExtension)) HasFirstExtension = valueHasFirstExtension;
    }
}
