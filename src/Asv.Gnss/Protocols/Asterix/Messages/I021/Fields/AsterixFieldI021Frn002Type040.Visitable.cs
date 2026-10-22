using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixFieldI021Frn002Type040
{
    private static readonly Field VisitorAtpField = new Field.Builder().Name(nameof(Atp)).Title(nameof(Atp)).Description("Atp").DataType(UInt8Type.Default).Build();
    private static readonly Field VisitorArcField = new Field.Builder().Name(nameof(Arc)).Title(nameof(Arc)).Description("Arc").DataType(UInt8Type.Default).Build();
    private static readonly Field VisitorRcField = new Field.Builder().Name(nameof(Rc)).Title(nameof(Rc)).Description("Rc").DataType(BoolType.Default).Build();
    private static readonly Field VisitorRabField = new Field.Builder().Name(nameof(Rab)).Title(nameof(Rab)).Description("Rab").DataType(BoolType.Default).Build();
    private static readonly Field VisitorDcrField = new Field.Builder().Name(nameof(Dcr)).Title(nameof(Dcr)).Description("Dcr").DataType(BoolType.Default).Build();
    private static readonly Field VisitorGbsField = new Field.Builder().Name(nameof(Gbs)).Title(nameof(Gbs)).Description("Gbs").DataType(BoolType.Default).Build();
    private static readonly Field VisitorSimField = new Field.Builder().Name(nameof(Sim)).Title(nameof(Sim)).Description("Sim").DataType(BoolType.Default).Build();
    private static readonly Field VisitorTstField = new Field.Builder().Name(nameof(Tst)).Title(nameof(Tst)).Description("Tst").DataType(BoolType.Default).Build();
    private static readonly Field VisitorSaaField = new Field.Builder().Name(nameof(Saa)).Title(nameof(Saa)).Description("Saa").DataType(BoolType.Default).Build();
    private static readonly Field VisitorClField = new Field.Builder().Name(nameof(Cl)).Title(nameof(Cl)).Description("Cl").DataType(UInt8Type.Default).Build();
    private static readonly Field VisitorIpcField = new Field.Builder().Name(nameof(Ipc)).Title(nameof(Ipc)).Description("Ipc").DataType(BoolType.Default).Build();
    private static readonly Field VisitorNogoField = new Field.Builder().Name(nameof(Nogo)).Title(nameof(Nogo)).Description("Nogo").DataType(BoolType.Default).Build();
    private static readonly Field VisitorCprField = new Field.Builder().Name(nameof(Cpr)).Title(nameof(Cpr)).Description("Cpr").DataType(BoolType.Default).Build();
    private static readonly Field VisitorLdpjField = new Field.Builder().Name(nameof(Ldpj)).Title(nameof(Ldpj)).Description("Ldpj").DataType(BoolType.Default).Build();
    private static readonly Field VisitorRcfField = new Field.Builder().Name(nameof(Rcf)).Title(nameof(Rcf)).Description("Rcf").DataType(BoolType.Default).Build();
    private static readonly Field VisitorHasFirstExtensionField = new Field.Builder().Name(nameof(HasFirstExtension)).Title(nameof(HasFirstExtension)).Description("HasFirstExtension").DataType(BoolType.Default).Build();
    private static readonly Field VisitorHasSecondExtensionField = new Field.Builder().Name(nameof(HasSecondExtension)).Title(nameof(HasSecondExtension)).Description("HasSecondExtension").DataType(BoolType.Default).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorAtpField, VisitorArcField, VisitorRcField, VisitorRabField, VisitorDcrField, VisitorGbsField, VisitorSimField, VisitorTstField, VisitorSaaField, VisitorClField, VisitorIpcField, VisitorNogoField, VisitorCprField, VisitorLdpjField, VisitorRcfField, VisitorHasFirstExtensionField, VisitorHasSecondExtensionField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueAtp = Atp;
        UInt8Type.Accept(visitor, VisitorAtpField, ref valueAtp);
        if (!EqualityComparer<byte>.Default.Equals(valueAtp, Atp)) Atp = valueAtp;
        var valueArc = Arc;
        UInt8Type.Accept(visitor, VisitorArcField, ref valueArc);
        if (!EqualityComparer<byte>.Default.Equals(valueArc, Arc)) Arc = valueArc;
        var valueRc = Rc;
        BoolType.Accept(visitor, VisitorRcField, ref valueRc);
        if (!EqualityComparer<bool>.Default.Equals(valueRc, Rc)) Rc = valueRc;
        var valueRab = Rab;
        BoolType.Accept(visitor, VisitorRabField, ref valueRab);
        if (!EqualityComparer<bool>.Default.Equals(valueRab, Rab)) Rab = valueRab;
        var valueDcr = Dcr;
        BoolType.Accept(visitor, VisitorDcrField, ref valueDcr);
        if (!EqualityComparer<bool>.Default.Equals(valueDcr, Dcr)) Dcr = valueDcr;
        var valueGbs = Gbs;
        BoolType.Accept(visitor, VisitorGbsField, ref valueGbs);
        if (!EqualityComparer<bool>.Default.Equals(valueGbs, Gbs)) Gbs = valueGbs;
        var valueSim = Sim;
        BoolType.Accept(visitor, VisitorSimField, ref valueSim);
        if (!EqualityComparer<bool>.Default.Equals(valueSim, Sim)) Sim = valueSim;
        var valueTst = Tst;
        BoolType.Accept(visitor, VisitorTstField, ref valueTst);
        if (!EqualityComparer<bool>.Default.Equals(valueTst, Tst)) Tst = valueTst;
        var valueSaa = Saa;
        BoolType.Accept(visitor, VisitorSaaField, ref valueSaa);
        if (!EqualityComparer<bool>.Default.Equals(valueSaa, Saa)) Saa = valueSaa;
        var valueCl = Cl;
        UInt8Type.Accept(visitor, VisitorClField, ref valueCl);
        if (!EqualityComparer<byte>.Default.Equals(valueCl, Cl)) Cl = valueCl;
        var valueIpc = Ipc;
        BoolType.Accept(visitor, VisitorIpcField, ref valueIpc);
        if (!EqualityComparer<bool>.Default.Equals(valueIpc, Ipc)) Ipc = valueIpc;
        var valueNogo = Nogo;
        BoolType.Accept(visitor, VisitorNogoField, ref valueNogo);
        if (!EqualityComparer<bool>.Default.Equals(valueNogo, Nogo)) Nogo = valueNogo;
        var valueCpr = Cpr;
        BoolType.Accept(visitor, VisitorCprField, ref valueCpr);
        if (!EqualityComparer<bool>.Default.Equals(valueCpr, Cpr)) Cpr = valueCpr;
        var valueLdpj = Ldpj;
        BoolType.Accept(visitor, VisitorLdpjField, ref valueLdpj);
        if (!EqualityComparer<bool>.Default.Equals(valueLdpj, Ldpj)) Ldpj = valueLdpj;
        var valueRcf = Rcf;
        BoolType.Accept(visitor, VisitorRcfField, ref valueRcf);
        if (!EqualityComparer<bool>.Default.Equals(valueRcf, Rcf)) Rcf = valueRcf;
        var valueHasFirstExtension = HasFirstExtension;
        BoolType.Accept(visitor, VisitorHasFirstExtensionField, ref valueHasFirstExtension);
        if (!EqualityComparer<bool>.Default.Equals(valueHasFirstExtension, HasFirstExtension)) HasFirstExtension = valueHasFirstExtension;
        var valueHasSecondExtension = HasSecondExtension;
        BoolType.Accept(visitor, VisitorHasSecondExtensionField, ref valueHasSecondExtension);
        if (!EqualityComparer<bool>.Default.Equals(valueHasSecondExtension, HasSecondExtension)) HasSecondExtension = valueHasSecondExtension;
    }
}
