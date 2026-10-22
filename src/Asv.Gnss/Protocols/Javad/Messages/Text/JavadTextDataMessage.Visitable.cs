using System.Collections.Immutable;
using Asv.IO;

namespace Asv.Gnss;

public sealed partial class JavadTextDataMessage
{
    private static readonly Field MessageTypeField = JavadVisitor.Field(
        nameof(MessageType),
        new StringType(EncodingId.Ascii, 1, 4095),
        "Text data format, including CGGTTS."
    );
    private ImmutableArray<JavadValue> _visitorFields;
    private JavadValue? _visitorData;
    private Field? _visitorDataField;
    private StructType? _visitorType;

    /// <summary>Returns cached metadata matching the current nested text-data shape.</summary>
    public StructType GetVisitorType()
    {
        PrepareVisitor();
        return _visitorType!;
    }

    public override void Accept(IVisitor visitor)
    {
        base.Accept(visitor);
        var messageType = MessageType;
        StringType.Accept(visitor, MessageTypeField, ref messageType);
        if (messageType != MessageType)
            MessageType = messageType;
        PrepareVisitor();
        var data = _visitorData!;
        JavadValueVisitor.Visit(visitor, _visitorDataField!, ref data);
        if (!ReferenceEquals(data, _visitorData))
        {
            Data = data.Items;
            CacheVisitor(data);
        }
    }

    private void PrepareVisitor()
    {
        if (_visitorType is not null && _visitorFields == Data)
            return;
        CacheVisitor(JavadValue.List([.. Data]));
    }

    private void CacheVisitor(JavadValue value)
    {
        _visitorFields = Data;
        _visitorData = value;
        _visitorDataField = JavadVisitor.Field(
            nameof(Data),
            JavadValueVisitor.GetType(value),
            "Nested fields of the selected text-data format."
        );
        _visitorType = new([ExtensionDataField, MessageTypeField, _visitorDataField]);
    }
}
