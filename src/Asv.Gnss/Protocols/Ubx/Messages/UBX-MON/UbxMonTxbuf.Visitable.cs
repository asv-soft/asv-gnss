using Asv.IO;

namespace Asv.Gnss;

public partial class UbxMonTxbuf
{
    private static readonly Field PendingField = UbxVisitor.Field(nameof(Pending), new ArrayType(UInt16Type.Default, 6), "Number of bytes pending in transmitter buffer for each target", "byte");
    private static readonly Field UsageField = UbxVisitor.Field(nameof(Usage), new ArrayType(UInt8Type.Default, 6), "Maximum usage transmitter buffer during the last sysmon period for each target", "%");
    private static readonly Field PeakUsageField = UbxVisitor.Field(nameof(PeakUsage), new ArrayType(UInt8Type.Default, 6), "Maximum usage transmitter buffer for each target", "%");
    private static readonly Field TUsageField = UbxVisitor.Field(nameof(TUsage), UInt8Type.Default, "Maximum usage of transmitter buffer during the last sysmon period for all targets", "%");
    private static readonly Field TPeakUsageField = UbxVisitor.Field(nameof(TPeakUsage), UInt8Type.Default, "Maximum usage of transmitter buffer for all targets", "%");
    private static readonly Field ErrorsField = UbxVisitor.Field(nameof(Errors), UInt8Type.Default, "Error bitmask (error bits in region below)");
    private static readonly Field IsAllocField = UbxVisitor.Field(nameof(IsAlloc), BoolType.Default, "Allocation error (TX buffer full)");
    private static readonly Field IsMemField = UbxVisitor.Field(nameof(IsMem), BoolType.Default, "Memory Allocation error");
    private static readonly Field IsLimitField = UbxVisitor.Field(nameof(IsLimit), BoolType.Default, "Buffer limit of corresponding target reached");
    private static readonly Field Reserved1Field = UbxVisitor.Field(nameof(Reserved1), UInt8Type.Default, "Reserved");

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
        UInt8Type.Accept(visitor, TUsageField, TUsageField.DataType, ref _tUsage);
        UInt8Type.Accept(visitor, TPeakUsageField, TPeakUsageField.DataType, ref _tPeakUsage);
        UInt8Type.Accept(visitor, ErrorsField, ErrorsField.DataType, ref _errors);
        BoolType.Accept(visitor, IsAllocField, IsAllocField.DataType, ref _isAlloc);
        BoolType.Accept(visitor, IsMemField, IsMemField.DataType, ref _isMem);
        BoolType.Accept(visitor, IsLimitField, IsLimitField.DataType, ref _isLimit);
        UInt8Type.Accept(visitor, Reserved1Field, Reserved1Field.DataType, ref _reserved1);
    }
}
