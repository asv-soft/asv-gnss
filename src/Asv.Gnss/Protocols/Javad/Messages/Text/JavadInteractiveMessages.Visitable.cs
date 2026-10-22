using Asv.IO;

namespace Asv.Gnss;

public abstract partial class JavadResponseMessage
{
    private static readonly Field TextField = JavadVisitor.Field(
        nameof(Text),
        new StringType(EncodingId.Ascii, 0, 4095),
        "Unchecksummed reply text, including an optional command label and incomplete reply fragments."
    );
    public static StructType StructType { get; } = new([ExtensionDataField, TextField]);

    public override void Accept(IVisitor visitor)
    {
        base.Accept(visitor);
        StringType.Accept(visitor, TextField, ref _text);
    }
}

public sealed partial class JavadParametersMessage
{
    private static readonly Field PartialParametersField = JavadVisitor.Field(
        nameof(Parameters),
        new StringType(EncodingId.Ascii, 0, 4095),
        "Parameter tree fragment. Incomplete replies remain literal text."
    );
    private static readonly StructType PartialVisitorType = new([
        ExtensionDataField,
        PartialParametersField,
    ]);
    private string? _visitorParameters;
    private JavadValue? _visitorValue;
    private Field? _visitorValueField;
    private StructType? _visitorType;

    /// <summary>Returns cached metadata for a complete parameter tree or an incomplete text fragment.</summary>
    public StructType GetVisitorType()
    {
        PrepareVisitor();
        return _visitorType!;
    }

    public override void Accept(IVisitor visitor)
    {
        base.Accept(visitor);
        PrepareVisitor();
        if (_visitorValue is { } value)
        {
            var original = value;
            JavadValueVisitor.Visit(visitor, _visitorValueField!, ref value);
            if (!ReferenceEquals(value, original))
            {
                Parameters = value.ToString();
                CacheVisitor(value);
            }
        }
        else
        {
            var text = Parameters;
            StringType.Accept(visitor, PartialParametersField, ref text);
            if (text != Parameters)
                Parameters = text;
        }
    }

    private void PrepareVisitor()
    {
        if (_visitorType is not null && _visitorParameters == Parameters)
            return;
        TryGetValue(out var value);
        CacheVisitor(value);
    }

    private void CacheVisitor(JavadValue? value)
    {
        _visitorParameters = Parameters;
        _visitorValue = value;
        if (value is null)
        {
            _visitorValueField = null;
            _visitorType = PartialVisitorType;
            return;
        }
        _visitorValueField = JavadVisitor.Field(
            nameof(Parameters),
            JavadValueVisitor.GetType(value),
            "Complete parameter tree."
        );
        _visitorType = new([ExtensionDataField, _visitorValueField]);
    }
}

public sealed partial class JavadWrapperMessage
{
    private static readonly Field SourceIdField = JavadVisitor.Field(
        nameof(SourceId),
        UInt8Type.Default,
        "Wrapped stream identifier."
    );
    private static readonly Field DataField = JavadVisitor.Field(
        nameof(Data),
        new ListType(UInt8Type.Default, 0, 4092),
        "Wrapped stream bytes."
    );
    public static StructType StructType { get; } =
        new([ExtensionDataField, SourceIdField, DataField]);

    public override void Accept(IVisitor visitor)
    {
        base.Accept(visitor);
        UInt8Type.Accept(visitor, SourceIdField, ref _sourceId);
        JavadVisitor.VisitArray(
            visitor,
            DataField,
            ref _data,
            static () => (byte)0,
            static (IVisitor v, Field f, IFieldType t, ref byte value) =>
                UInt8Type.Accept(v, f, t, ref value)
        );
    }
}

public sealed partial class JavadEventMessage
{
    private static readonly Field TimeField = JavadVisitor.Field(
        nameof(ReceiverTimeMilliseconds),
        UInt32Type.Default,
        "Receiver time modulo one day.",
        "ms"
    );
    private static readonly Field EventTypeField = JavadVisitor.Field(
        nameof(EventType),
        UInt8Type.Default,
        "Event discriminator."
    );
    private static readonly Field DataField = JavadVisitor.Field(
        nameof(Data),
        new ListType(UInt8Type.Default, 0, 4089),
        "Event-specific bytes."
    );
    public static StructType StructType { get; } =
        new([ExtensionDataField, TimeField, EventTypeField, DataField]);

    public override void Accept(IVisitor visitor)
    {
        base.Accept(visitor);
        UInt32Type.Accept(visitor, TimeField, ref _receiverTimeMilliseconds);
        UInt8Type.Accept(visitor, EventTypeField, ref _eventType);
        JavadVisitor.VisitArray(
            visitor,
            DataField,
            ref _data,
            static () => (byte)0,
            static (IVisitor v, Field f, IFieldType t, ref byte value) =>
                UInt8Type.Accept(v, f, t, ref value)
        );
    }
}
