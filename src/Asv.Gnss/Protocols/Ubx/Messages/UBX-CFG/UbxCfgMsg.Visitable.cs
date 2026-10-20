using Asv.IO;

namespace Asv.Gnss;

public partial class UbxCfgMsgPool
{
    private static readonly Field MsgClassField = UbxVisitor.Field(nameof(MsgClass), UInt8Type.Default, "UBX class identifier of the message whose output rate is queried or configured.");

    private static readonly Field MsgIdField = UbxVisitor.Field(nameof(MsgId), UInt8Type.Default, "UBX message identifier within MsgClass.");

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        base.Accept(visitor);
        UInt8Type.Accept(visitor, MsgClassField, ref _msgClass);
        UInt8Type.Accept(visitor, MsgIdField, ref _msgId);
    }
}

public partial class UbxCfgMsg
{
    private static readonly Field MsgClassField = UbxVisitor.Field(nameof(MsgClass), UInt8Type.Default, "UBX class identifier of the message whose output rate is queried or configured.");

    private static readonly Field MsgIdField = UbxVisitor.Field(nameof(MsgId), UInt8Type.Default, "UBX message identifier within MsgClass.");

    private static readonly Field CurrentPortRateField = UbxVisitor.Field(nameof(CurrentPortRate), UInt8OptionalType.Default, "Output interval for the current port in navigation epochs; 0 disables output. A non-null value selects the three-byte form.");

    private static readonly Field PortsField = UbxVisitor.Field(nameof(Ports), new ArrayType(UInt8Type.Default, 6), "Per-port output intervals in navigation epochs for the eight-byte message form: DDC, UART1, UART2, USB, SPI, and one reserved byte. Zero disables output on a port.");

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        base.Accept(visitor);
        UInt8Type.Accept(visitor, MsgClassField, ref _msgClass);
        UInt8Type.Accept(visitor, MsgIdField, ref _msgId);
        UInt8OptionalType.Accept(visitor, CurrentPortRateField, ref _currentPortRate);
        UbxVisitor.VisitArray(visitor, PortsField, ref _ports, static () => (byte)0,
            static (IVisitor itemVisitor, Field field, IFieldType type, ref byte value) =>
                UInt8Type.Accept(itemVisitor, field, type, ref value));
    }
}
