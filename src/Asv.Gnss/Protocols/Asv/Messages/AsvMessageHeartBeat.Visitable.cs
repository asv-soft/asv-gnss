using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsvMessageHeartBeat
{
    private static readonly Field PayloadDeviceTypeField = ProtocolPayloadVisitor.Field(nameof(DeviceType), UInt16Type.Default, enumType: typeof(AsvDeviceType));
    private static readonly Field PayloadDeviceStateField = ProtocolPayloadVisitor.Field(nameof(DeviceState), UInt8Type.Default, enumType: typeof(AsvDeviceState));
    private static readonly Field PayloadReserved1Field = ProtocolPayloadVisitor.Field(nameof(Reserved1), UInt8Type.Default);
    private static readonly Field PayloadReserved2Field = ProtocolPayloadVisitor.Field(nameof(Reserved2), UInt8Type.Default);
    private static readonly Field PayloadReserved3Field = ProtocolPayloadVisitor.Field(nameof(Reserved3), UInt8Type.Default);
    private static readonly Field PayloadReserved4Field = ProtocolPayloadVisitor.Field(nameof(Reserved4), UInt8Type.Default);

    internal new static readonly StructType PayloadVisitorType = new([..AsvMessageBase.PayloadVisitorType.Fields, PayloadDeviceTypeField, PayloadDeviceStateField, PayloadReserved1Field, PayloadReserved2Field, PayloadReserved3Field, PayloadReserved4Field]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        base.Accept(visitor);

        AsvDeviceType valueDeviceType = DeviceType;
        var numericDeviceType = (ushort)valueDeviceType;
        UInt16Type.Accept(visitor, PayloadDeviceTypeField, PayloadDeviceTypeField.DataType, ref numericDeviceType);
        valueDeviceType = (AsvDeviceType)numericDeviceType;
        if (!EqualityComparer<AsvDeviceType>.Default.Equals(valueDeviceType, DeviceType))
        {
            DeviceType = valueDeviceType;
        }

        AsvDeviceState valueDeviceState = DeviceState;
        var numericDeviceState = (byte)valueDeviceState;
        UInt8Type.Accept(visitor, PayloadDeviceStateField, PayloadDeviceStateField.DataType, ref numericDeviceState);
        valueDeviceState = (AsvDeviceState)numericDeviceState;
        if (!EqualityComparer<AsvDeviceState>.Default.Equals(valueDeviceState, DeviceState))
        {
            DeviceState = valueDeviceState;
        }

        byte valueReserved1 = Reserved1;
        UInt8Type.Accept(visitor, PayloadReserved1Field, PayloadReserved1Field.DataType, ref valueReserved1);
        if (!EqualityComparer<byte>.Default.Equals(valueReserved1, Reserved1))
        {
            Reserved1 = valueReserved1;
        }

        byte valueReserved2 = Reserved2;
        UInt8Type.Accept(visitor, PayloadReserved2Field, PayloadReserved2Field.DataType, ref valueReserved2);
        if (!EqualityComparer<byte>.Default.Equals(valueReserved2, Reserved2))
        {
            Reserved2 = valueReserved2;
        }

        byte valueReserved3 = Reserved3;
        UInt8Type.Accept(visitor, PayloadReserved3Field, PayloadReserved3Field.DataType, ref valueReserved3);
        if (!EqualityComparer<byte>.Default.Equals(valueReserved3, Reserved3))
        {
            Reserved3 = valueReserved3;
        }

        byte valueReserved4 = Reserved4;
        UInt8Type.Accept(visitor, PayloadReserved4Field, PayloadReserved4Field.DataType, ref valueReserved4);
        if (!EqualityComparer<byte>.Default.Equals(valueReserved4, Reserved4))
        {
            Reserved4 = valueReserved4;
        }
    }
}
