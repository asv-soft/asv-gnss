using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixFieldI021Frn045Type260
{
    private static readonly Field VisitorMbDataField = new Field.Builder().Name(nameof(MbData)).Title(nameof(MbData)).Description("MbData").DataType(new ListType(UInt8Type.Default, 7, 7)).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorMbDataField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueMbData = MbData;
        AsterixVisitor.VisitArray(visitor, VisitorMbDataField, ref valueMbData,
            static () => new byte(),
            static (IVisitor visitor, Field field, IFieldType type, ref byte item) =>
            {
                UInt8Type.Accept(visitor, field, type, ref item);
            });
    }
}
