using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixFieldI247Frn001Type010
{
    private static readonly Field VisitorSacField = new Field.Builder().Name(nameof(Sac)).Title(nameof(Sac)).Description("Sac").DataType(UInt8Type.Default).Metadata("enumType", typeof(SystemAreaCode).FullName).Build();
    private static readonly Field VisitorSicField = new Field.Builder().Name(nameof(Sic)).Title(nameof(Sic)).Description("Sic").DataType(UInt8Type.Default).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorSacField, VisitorSicField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueSac = (byte)Sac;
        UInt8Type.Accept(visitor, VisitorSacField, ref valueSac);
        if (!EqualityComparer<byte>.Default.Equals(valueSac, (byte)Sac)) Sac = (SystemAreaCode)valueSac;
        var valueSic = Sic;
        UInt8Type.Accept(visitor, VisitorSicField, ref valueSic);
        if (!EqualityComparer<byte>.Default.Equals(valueSic, Sic)) Sic = valueSic;
    }
}
