using Asv.IO;

namespace Asv.Gnss;

public partial class UbxMonRxbuf
{
    private static readonly Field PendingField = UbxVisitor.Field(nameof(Pending), new ArrayType(UInt16Type.Default, 6), "Number of bytes pending in transmitter buffer for each target", "byte");
    private static readonly Field UsageField = UbxVisitor.Field(nameof(Usage), new ArrayType(UInt8Type.Default, 6), "Maximum usage transmitter buffer during the last sysmon period for each target", "%");
    private static readonly Field PeakUsageField = UbxVisitor.Field(nameof(PeakUsage), new ArrayType(UInt8Type.Default, 6), "Maximum usage transmitter buffer for each target", "%");

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        base.Accept(visitor);
        UbxVisitor.VisitFixedArray(visitor, PendingField, _pending,
            static (IVisitor v, Field f, IFieldType t, ref ushort value) => UInt16Type.Accept(v, f, t, ref value));
        UbxVisitor.VisitFixedArray(visitor, UsageField, _usage,
            static (IVisitor v, Field f, IFieldType t, ref byte value) => UInt8Type.Accept(v, f, t, ref value));
        UbxVisitor.VisitFixedArray(visitor, PeakUsageField, _peakUsage,
            static (IVisitor v, Field f, IFieldType t, ref byte value) => UInt8Type.Accept(v, f, t, ref value));
    }
}
