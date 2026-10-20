using Asv.IO;

namespace Asv.Gnss;

public abstract partial class UbxInfBase
{
    private static readonly Field MessageField = UbxVisitor.Field(nameof(Message), new StringOptionalType(EncodingId.Ascii, 0, 65535), "Receiver diagnostic text from the variable-length INF payload.");

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        base.Accept(visitor);
        StringOptionalType.Accept(visitor, MessageField, MessageField.DataType, ref _message);
    }
}
