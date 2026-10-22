using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixFieldI034Frn006Type050
{
    private static readonly Field VisitorSelectorField = new Field.Builder().Name(nameof(Selector)).Title(nameof(Selector)).Description("Selector").DataType(AsterixI034StatusSubfieldSelector.AsterixVisitorType).Metadata("readOnly", true).Build();
    private static readonly Field VisitorCommonField = new Field.Builder().Name(nameof(Common)).Title(nameof(Common)).Description("Common").DataType(new OptionalStructType(AsterixI034SystemConfigurationCommon.AsterixVisitorType)).Build();
    private static readonly Field VisitorPsrField = new Field.Builder().Name(nameof(Psr)).Title(nameof(Psr)).Description("Psr").DataType(new OptionalStructType(AsterixI034SystemConfigurationPsr.AsterixVisitorType)).Build();
    private static readonly Field VisitorModeSField = new Field.Builder().Name(nameof(ModeS)).Title(nameof(ModeS)).Description("ModeS").DataType(new OptionalStructType(AsterixI034SystemConfigurationModeS.AsterixVisitorType)).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorSelectorField, VisitorCommonField, VisitorPsrField, VisitorModeSField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueSelector = Selector;
        StructType.Accept(visitor, VisitorSelectorField, valueSelector);
        var valueCommon = Common;
        OptionalStructType.Accept(visitor, VisitorCommonField, ref valueCommon);
        if (!ReferenceEquals(valueCommon, Common)) Common = valueCommon;
        var valuePsr = Psr;
        OptionalStructType.Accept(visitor, VisitorPsrField, ref valuePsr);
        if (!ReferenceEquals(valuePsr, Psr)) Psr = valuePsr;
        var valueModeS = ModeS;
        OptionalStructType.Accept(visitor, VisitorModeSField, ref valueModeS);
        if (!ReferenceEquals(valueModeS, ModeS)) ModeS = valueModeS;
    }
}
