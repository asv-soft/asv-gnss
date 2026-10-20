using Asv.IO;

namespace Asv.Gnss;

public abstract partial class UbxAckBase
{
    private static readonly Field AckClassIdField = UbxVisitor.Field(nameof(AckClassId), UInt8Type.Default, "Class ID of the Acknowledged Message");
    private static readonly Field AckSubclassIdField = UbxVisitor.Field(nameof(AckSubclassId), UInt8Type.Default, "Message ID of the Acknowledged Message");

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        base.Accept(visitor);
        UInt8Type.Accept(visitor, AckClassIdField, AckClassIdField.DataType, ref _ackClassId);
        UInt8Type.Accept(visitor, AckSubclassIdField, AckSubclassIdField.DataType, ref _ackSubclassId);
    }
}
