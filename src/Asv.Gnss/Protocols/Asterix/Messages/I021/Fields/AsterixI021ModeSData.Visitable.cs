using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixI021ModeSData : IVisitable
{
    private static readonly Field VisitorMbDataField = new Field.Builder().Name(nameof(MbData)).Title(nameof(MbData)).Description("MbData").DataType(new ListType(UInt8Type.Default, 7, 7)).Build();
    private static readonly Field VisitorBds1Field = new Field.Builder().Name(nameof(Bds1)).Title(nameof(Bds1)).Description("Bds1").DataType(UInt8Type.Default).Build();
    private static readonly Field VisitorBds2Field = new Field.Builder().Name(nameof(Bds2)).Title(nameof(Bds2)).Description("Bds2").DataType(UInt8Type.Default).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorMbDataField, VisitorBds1Field, VisitorBds2Field]);

    /// <inheritdoc />
    public void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueMbData = MbData;
        AsterixVisitor.VisitArray(visitor, VisitorMbDataField, ref valueMbData,
            static () => new byte(),
            static (IVisitor visitor, Field field, IFieldType type, ref byte item) =>
            {
                UInt8Type.Accept(visitor, field, type, ref item);
            });
        var valueBds1 = Bds1;
        UInt8Type.Accept(visitor, VisitorBds1Field, ref valueBds1);
        if (!EqualityComparer<byte>.Default.Equals(valueBds1, Bds1)) Bds1 = valueBds1;
        var valueBds2 = Bds2;
        UInt8Type.Accept(visitor, VisitorBds2Field, ref valueBds2);
        if (!EqualityComparer<byte>.Default.Equals(valueBds2, Bds2)) Bds2 = valueBds2;
    }
}
