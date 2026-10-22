using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixFieldI048Frn007Type130
{
    private static readonly Field VisitorFlagsField = new Field.Builder().Name(nameof(Flags)).Title(nameof(Flags)).Description("Flags").DataType(UInt8Type.Default).Metadata("readOnly", true).Build();
    private static readonly Field VisitorSrlField = new Field.Builder().Name(nameof(Srl)).Title(nameof(Srl)).Description("Srl").DataType(UInt8OptionalType.Default).Build();
    private static readonly Field VisitorSrrField = new Field.Builder().Name(nameof(Srr)).Title(nameof(Srr)).Description("Srr").DataType(UInt8OptionalType.Default).Build();
    private static readonly Field VisitorSamField = new Field.Builder().Name(nameof(Sam)).Title(nameof(Sam)).Description("Sam").DataType(Int8OptionalType.Default).Build();
    private static readonly Field VisitorPrlField = new Field.Builder().Name(nameof(Prl)).Title(nameof(Prl)).Description("Prl").DataType(Int16OptionalType.Default).Build();
    private static readonly Field VisitorPamField = new Field.Builder().Name(nameof(Pam)).Title(nameof(Pam)).Description("Pam").DataType(Int8OptionalType.Default).Build();
    private static readonly Field VisitorRpdField = new Field.Builder().Name(nameof(Rpd)).Title(nameof(Rpd)).Description("Rpd").DataType(Int8OptionalType.Default).Build();
    private static readonly Field VisitorApdField = new Field.Builder().Name(nameof(Apd)).Title(nameof(Apd)).Description("Apd").DataType(Int8OptionalType.Default).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorFlagsField, VisitorSrlField, VisitorSrrField, VisitorSamField, VisitorPrlField, VisitorPamField, VisitorRpdField, VisitorApdField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueFlags = Flags;
        UInt8Type.Accept(visitor, VisitorFlagsField, ref valueFlags);
        var valueSrl = Srl;
        UInt8OptionalType.Accept(visitor, VisitorSrlField, ref valueSrl);
        if (!EqualityComparer<byte?>.Default.Equals(valueSrl, Srl)) Srl = valueSrl;
        var valueSrr = Srr;
        UInt8OptionalType.Accept(visitor, VisitorSrrField, ref valueSrr);
        if (!EqualityComparer<byte?>.Default.Equals(valueSrr, Srr)) Srr = valueSrr;
        var valueSam = Sam;
        Int8OptionalType.Accept(visitor, VisitorSamField, ref valueSam);
        if (!EqualityComparer<sbyte?>.Default.Equals(valueSam, Sam)) Sam = valueSam;
        var valuePrl = Prl;
        Int16OptionalType.Accept(visitor, VisitorPrlField, ref valuePrl);
        if (!EqualityComparer<short?>.Default.Equals(valuePrl, Prl)) Prl = valuePrl;
        var valuePam = Pam;
        Int8OptionalType.Accept(visitor, VisitorPamField, ref valuePam);
        if (!EqualityComparer<sbyte?>.Default.Equals(valuePam, Pam)) Pam = valuePam;
        var valueRpd = Rpd;
        Int8OptionalType.Accept(visitor, VisitorRpdField, ref valueRpd);
        if (!EqualityComparer<sbyte?>.Default.Equals(valueRpd, Rpd)) Rpd = valueRpd;
        var valueApd = Apd;
        Int8OptionalType.Accept(visitor, VisitorApdField, ref valueApd);
        if (!EqualityComparer<sbyte?>.Default.Equals(valueApd, Apd)) Apd = valueApd;
    }
}
