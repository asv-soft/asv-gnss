using Asv.IO;

namespace Asv.Gnss;

public partial class UbxMonMsgpp
{
    private static readonly Field Msg1Field = UbxVisitor.Field(nameof(Msg1), new ArrayType(UInt16Type.Default, 8), "Number of successfully parsed messages for each protocol on port0");
    private static readonly Field Msg2Field = UbxVisitor.Field(nameof(Msg2), new ArrayType(UInt16Type.Default, 8), "Number of successfully parsed messages for each protocol on port1");
    private static readonly Field Msg3Field = UbxVisitor.Field(nameof(Msg3), new ArrayType(UInt16Type.Default, 8), "Number of successfully parsed messages for each protocol on port2");
    private static readonly Field Msg4Field = UbxVisitor.Field(nameof(Msg4), new ArrayType(UInt16Type.Default, 8), "Number of successfully parsed messages for each protocol on port3");
    private static readonly Field Msg5Field = UbxVisitor.Field(nameof(Msg5), new ArrayType(UInt16Type.Default, 8), "Number of successfully parsed messages for each protocol on port4");
    private static readonly Field Msg6Field = UbxVisitor.Field(nameof(Msg6), new ArrayType(UInt16Type.Default, 8), "Number of successfully parsed messages for each protocol on port5");
    private static readonly Field SkippedField = UbxVisitor.Field(nameof(Skipped), new ArrayType(UInt32Type.Default, 6), "Number skipped bytes for each port");

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        base.Accept(visitor);
        UbxVisitor.VisitFixedArray(visitor, Msg1Field, _msg1,
            static (IVisitor v, Field f, IFieldType t, ref ushort value) => UInt16Type.Accept(v, f, t, ref value));
        UbxVisitor.VisitFixedArray(visitor, Msg2Field, _msg2,
            static (IVisitor v, Field f, IFieldType t, ref ushort value) => UInt16Type.Accept(v, f, t, ref value));
        UbxVisitor.VisitFixedArray(visitor, Msg3Field, _msg3,
            static (IVisitor v, Field f, IFieldType t, ref ushort value) => UInt16Type.Accept(v, f, t, ref value));
        UbxVisitor.VisitFixedArray(visitor, Msg4Field, _msg4,
            static (IVisitor v, Field f, IFieldType t, ref ushort value) => UInt16Type.Accept(v, f, t, ref value));
        UbxVisitor.VisitFixedArray(visitor, Msg5Field, _msg5,
            static (IVisitor v, Field f, IFieldType t, ref ushort value) => UInt16Type.Accept(v, f, t, ref value));
        UbxVisitor.VisitFixedArray(visitor, Msg6Field, _msg6,
            static (IVisitor v, Field f, IFieldType t, ref ushort value) => UInt16Type.Accept(v, f, t, ref value));
        UbxVisitor.VisitFixedArray(visitor, SkippedField, _skipped,
            static (IVisitor v, Field f, IFieldType t, ref uint value) => UInt32Type.Accept(v, f, t, ref value));
    }
}
