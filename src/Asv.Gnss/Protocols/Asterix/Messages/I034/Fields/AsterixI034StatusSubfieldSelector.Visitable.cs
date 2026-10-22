using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixI034StatusSubfieldSelector : IVisitable
{
    private static readonly Field VisitorRawField = new Field.Builder().Name(nameof(Raw)).Title(nameof(Raw)).Description("Raw").DataType(UInt8Type.Default).Build();
    private static readonly Field VisitorHasCommonPartField = new Field.Builder().Name(nameof(HasCommonPart)).Title(nameof(HasCommonPart)).Description("HasCommonPart").DataType(BoolType.Default).Build();
    private static readonly Field VisitorHasPsrPartField = new Field.Builder().Name(nameof(HasPsrPart)).Title(nameof(HasPsrPart)).Description("HasPsrPart").DataType(BoolType.Default).Build();
    private static readonly Field VisitorHasModeSPartField = new Field.Builder().Name(nameof(HasModeSPart)).Title(nameof(HasModeSPart)).Description("HasModeSPart").DataType(BoolType.Default).Build();
    private static readonly Field VisitorHasExtensionField = new Field.Builder().Name(nameof(HasExtension)).Title(nameof(HasExtension)).Description("HasExtension").DataType(BoolType.Default).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorRawField, VisitorHasCommonPartField, VisitorHasPsrPartField, VisitorHasModeSPartField, VisitorHasExtensionField]);

    /// <inheritdoc />
    public void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueRaw = Raw;
        UInt8Type.Accept(visitor, VisitorRawField, ref valueRaw);
        if (!EqualityComparer<byte>.Default.Equals(valueRaw, Raw)) Raw = valueRaw;
        var valueHasCommonPart = HasCommonPart;
        BoolType.Accept(visitor, VisitorHasCommonPartField, ref valueHasCommonPart);
        if (!EqualityComparer<bool>.Default.Equals(valueHasCommonPart, HasCommonPart)) HasCommonPart = valueHasCommonPart;
        var valueHasPsrPart = HasPsrPart;
        BoolType.Accept(visitor, VisitorHasPsrPartField, ref valueHasPsrPart);
        if (!EqualityComparer<bool>.Default.Equals(valueHasPsrPart, HasPsrPart)) HasPsrPart = valueHasPsrPart;
        var valueHasModeSPart = HasModeSPart;
        BoolType.Accept(visitor, VisitorHasModeSPartField, ref valueHasModeSPart);
        if (!EqualityComparer<bool>.Default.Equals(valueHasModeSPart, HasModeSPart)) HasModeSPart = valueHasModeSPart;
        var valueHasExtension = HasExtension;
        BoolType.Accept(visitor, VisitorHasExtensionField, ref valueHasExtension);
        if (!EqualityComparer<bool>.Default.Equals(valueHasExtension, HasExtension)) HasExtension = valueHasExtension;
    }
}
