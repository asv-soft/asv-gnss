using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class RtcmV3Message1033
{
    private static readonly Field RtcmReceiverTypeDescriptorCounterIField = RtcmV3Visitor.Field(nameof(ReceiverTypeDescriptorCounterI), UInt32Type.Default, "Number of characters in the name of the receiver type");
    private static readonly Field RtcmReceiverTypeDescriptorField = RtcmV3Visitor.Field(nameof(ReceiverTypeDescriptor), StringOptionalType.Utf8, "Receiver Type Descriptor");
    private static readonly Field RtcmReceiverFirmwareVersionCounterJField = RtcmV3Visitor.Field(nameof(ReceiverFirmwareVersionCounterJ), UInt32Type.Default, "Number of characters in the name of the receiver firmware");
    private static readonly Field RtcmReceiverFirmwareVersionField = RtcmV3Visitor.Field(nameof(ReceiverFirmwareVersion), StringOptionalType.Utf8, "Receiver Firmware Version");
    private static readonly Field RtcmReceiverSerialNumberCounterKField = RtcmV3Visitor.Field(nameof(ReceiverSerialNumberCounterK), UInt32Type.Default, "Number of characters in the name of the receiver serial number");
    private static readonly Field RtcmReceiverSerialNumberField = RtcmV3Visitor.Field(nameof(ReceiverSerialNumber), StringOptionalType.Utf8, "Receiver Serial Number");

    internal new static readonly StructType RtcmVisitorType = new([..RtcmV3Message1008.RtcmVisitorType.Fields, RtcmReceiverTypeDescriptorCounterIField, RtcmReceiverTypeDescriptorField, RtcmReceiverFirmwareVersionCounterJField, RtcmReceiverFirmwareVersionField, RtcmReceiverSerialNumberCounterKField, RtcmReceiverSerialNumberField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        base.Accept(visitor);

        var valueReceiverTypeDescriptorCounterI = ReceiverTypeDescriptorCounterI;
        UInt32Type.Accept(visitor, RtcmReceiverTypeDescriptorCounterIField, RtcmReceiverTypeDescriptorCounterIField.DataType, ref valueReceiverTypeDescriptorCounterI);
        if (!EqualityComparer<uint>.Default.Equals(valueReceiverTypeDescriptorCounterI, ReceiverTypeDescriptorCounterI))
        {
            ReceiverTypeDescriptorCounterI = valueReceiverTypeDescriptorCounterI;
        }

        string? valueReceiverTypeDescriptor = ReceiverTypeDescriptor;
        StringOptionalType.Accept(visitor, RtcmReceiverTypeDescriptorField, RtcmReceiverTypeDescriptorField.DataType, ref valueReceiverTypeDescriptor);
        if (!EqualityComparer<string>.Default.Equals(valueReceiverTypeDescriptor, ReceiverTypeDescriptor))
        {
            ReceiverTypeDescriptor = valueReceiverTypeDescriptor!;
        }

        var valueReceiverFirmwareVersionCounterJ = ReceiverFirmwareVersionCounterJ;
        UInt32Type.Accept(visitor, RtcmReceiverFirmwareVersionCounterJField, RtcmReceiverFirmwareVersionCounterJField.DataType, ref valueReceiverFirmwareVersionCounterJ);
        if (!EqualityComparer<uint>.Default.Equals(valueReceiverFirmwareVersionCounterJ, ReceiverFirmwareVersionCounterJ))
        {
            ReceiverFirmwareVersionCounterJ = valueReceiverFirmwareVersionCounterJ;
        }

        string? valueReceiverFirmwareVersion = ReceiverFirmwareVersion;
        StringOptionalType.Accept(visitor, RtcmReceiverFirmwareVersionField, RtcmReceiverFirmwareVersionField.DataType, ref valueReceiverFirmwareVersion);
        if (!EqualityComparer<string>.Default.Equals(valueReceiverFirmwareVersion, ReceiverFirmwareVersion))
        {
            ReceiverFirmwareVersion = valueReceiverFirmwareVersion!;
        }

        var valueReceiverSerialNumberCounterK = ReceiverSerialNumberCounterK;
        UInt32Type.Accept(visitor, RtcmReceiverSerialNumberCounterKField, RtcmReceiverSerialNumberCounterKField.DataType, ref valueReceiverSerialNumberCounterK);
        if (!EqualityComparer<uint>.Default.Equals(valueReceiverSerialNumberCounterK, ReceiverSerialNumberCounterK))
        {
            ReceiverSerialNumberCounterK = valueReceiverSerialNumberCounterK;
        }

        string? valueReceiverSerialNumber = ReceiverSerialNumber;
        StringOptionalType.Accept(visitor, RtcmReceiverSerialNumberField, RtcmReceiverSerialNumberField.DataType, ref valueReceiverSerialNumber);
        if (!EqualityComparer<string>.Default.Equals(valueReceiverSerialNumber, ReceiverSerialNumber))
        {
            ReceiverSerialNumber = valueReceiverSerialNumber!;
        }
    }
}
