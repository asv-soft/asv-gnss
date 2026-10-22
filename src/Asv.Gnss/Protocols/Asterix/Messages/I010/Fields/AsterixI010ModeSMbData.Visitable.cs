using System;
using Asv.IO;

namespace Asv.Gnss;

public partial record struct AsterixI010ModeSMbData : IVisitable
{
    private static readonly Field DataField = new Field.Builder().Name(nameof(Data)).Title(nameof(Data)).Description("Data").DataType(new ListType(UInt8Type.Default, 0, 8)).Metadata("readOnly", true).Build();

    /// <inheritdoc />
    public void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueData = Data;
        AsterixVisitor.VisitArray(visitor, DataField, ref valueData, static () => (byte)0,
            static (IVisitor v, Field field, IFieldType type, ref byte item) => UInt8Type.Accept(v, field, type, ref item), canResize: false);
    }
}
