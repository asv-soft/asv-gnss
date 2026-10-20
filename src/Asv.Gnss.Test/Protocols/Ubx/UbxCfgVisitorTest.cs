using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO.Ports;
using Asv.Common;
using Asv.IO;
using Xunit;

namespace Asv.Gnss;

public class UbxCfgVisitorTest
{
    [Fact]
    public void Accept_AntennaFields_ShouldUpdatePackedWireAndNumericFlagMetadata()
    {
        var message = new UbxCfgAnt();
        var visitor = new CfgVisitor
        {
            Updates =
            {
                [nameof(UbxCfgAnt.Reconfig)] = true,
                [nameof(UbxCfgAnt.PinSwitch)] = (byte)4,
                [nameof(UbxCfgAnt.PinSCD)] = (byte)5,
                [nameof(UbxCfgAnt.PinOCD)] = (byte)6,
                [nameof(UbxCfgAnt.Flags)] = (ushort)31
            }
        };

        message.Accept(visitor);

        var frame = UbxTestHelper.Serialize(message);
        Assert.Equal((ushort)31, BinaryPrimitives.ReadUInt16LittleEndian(frame.AsSpan(6)));
        Assert.Equal((ushort)(4 | (5 << 5) | (6 << 10) | 0x8000), BinaryPrimitives.ReadUInt16LittleEndian(frame.AsSpan(8)));
        Assert.Equal(nameof(UbxCfgAntFlags.Recovery), visitor.Fields[nameof(UbxCfgAnt.Flags)].GetEnum<ushort>()[16].Name);
    }

    [Fact]
    public void Accept_NavigationTypedFields_ShouldUpdateSignedScaledAndEnumValues()
    {
        var message = new UbxCfgNav5();
        var visitor = new CfgVisitor
        {
            Updates =
            {
                [nameof(UbxCfgNav5.PlatformModel)] = (int)UbxCfgNav5.ModelEnum.Automotive,
                [nameof(UbxCfgNav5.MinimumElevation)] = (sbyte)-5,
                [nameof(UbxCfgNav5.FixedAltitude)] = 123.45,
                [nameof(UbxCfgNav5.ApplyUTC)] = false
            }
        };

        message.Accept(visitor);

        var frame = UbxTestHelper.Serialize(message);
        Assert.Equal((byte)4, frame[8]);
        Assert.Equal(12345, BinaryPrimitives.ReadInt32LittleEndian(frame.AsSpan(10)));
        Assert.Equal((byte)251, frame[18]);
        Assert.Equal(0, BinaryPrimitives.ReadUInt16LittleEndian(frame.AsSpan(6)) & 0x0400);
        Assert.Equal(UbxCfgNav5.ModelEnum.Automotive, message.PlatformModel);
        Assert.Equal("m", visitor.Fields[nameof(UbxCfgNav5.FixedAltitude)].GetUnits());
        Assert.Equal(nameof(UbxCfgNav5.ModelEnum.Automotive), visitor.Fields[nameof(UbxCfgNav5.PlatformModel)].GetEnum<int>()[4].Name);
    }

    [Fact]
    public void Accept_RateFields_ShouldPreserveNativeTypesAndSerializerScaling()
    {
        var message = new UbxCfgRate();
        var visitor = new CfgVisitor
        {
            Updates =
            {
                [nameof(UbxCfgRate.RateHz)] = 2.0,
                [nameof(UbxCfgRate.NavRate)] = (ushort)3,
                [nameof(UbxCfgRate.TimeSystem)] = (int)UbxCfgRate.TimeSystemEnum.Galileo
            }
        };

        message.Accept(visitor);

        Assert.Equal(UbxTestHelper.CreateFrame(0x06, 0x08, [0xF4, 0x01, 3, 0, 4, 0]), UbxTestHelper.Serialize(message));
        Assert.Equal("Hz", visitor.Fields[nameof(UbxCfgRate.RateHz)].GetUnits());
        Assert.IsType<Int32Type>(visitor.Fields[nameof(UbxCfgRate.TimeSystem)].DataType);
        Assert.Equal(nameof(UbxCfgRate.TimeSystemEnum.Galileo), visitor.Fields[nameof(UbxCfgRate.TimeSystem)].GetEnum<int>()[4].Name);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Accept_MessageRateRead_ShouldPreserveSelectedFormAndArrayReference(bool currentPort)
    {
        var message = new UbxCfgMsg
        {
            MsgClass = 1,
            MsgId = 7,
            CurrentPortRate = currentPort ? (byte)2 : null,
            Ports = currentPort ? null! : [1, 2, 3, 4, 5, 0]
        };
        var originalPorts = message.Ports;
        var before = UbxTestHelper.Serialize(message);
        var visitor = new CfgVisitor();

        message.Accept(visitor);

        Assert.Equal(before, UbxTestHelper.Serialize(message));
        Assert.Same(originalPorts, message.Ports);
        Assert.Equal(currentPort ? (byte?)2 : null, message.CurrentPortRate);
        Assert.Equal(6, Assert.IsType<ArrayType>(visitor.Fields[nameof(UbxCfgMsg.Ports)].DataType).Size);
        Assert.IsType<UInt8OptionalType>(visitor.Fields[nameof(UbxCfgMsg.CurrentPortRate)].DataType);
    }

    [Fact]
    public void Accept_MessageRateArrayEdit_ShouldWriteEditedElementToWire()
    {
        var message = new UbxCfgMsg { MsgClass = 1, MsgId = 7, Ports = [1, 2, 3, 4, 5, 0] };
        var visitor = new CfgVisitor { EditedPortIndex = 2, EditedPortValue = 9 };

        message.Accept(visitor);

        Assert.Equal(UbxTestHelper.CreateFrame(0x06, 0x01, [1, 7, 1, 2, 9, 4, 5, 0]), UbxTestHelper.Serialize(message));
        Assert.Null(message.CurrentPortRate);
    }

    [Fact]
    public void Accept_OptionalDeviceMaskReadAndEdit_ShouldPreserveNullFormAndWriteRequestedSelection()
    {
        var message = new UbxCfgCfg { SaveMask = UbxCfgSection.IoPort };
        var before = UbxTestHelper.Serialize(message);
        var reader = new CfgVisitor();

        message.Accept(reader);

        Assert.Null(message.DeviceMask);
        Assert.Equal(before, UbxTestHelper.Serialize(message));
        Assert.Equal(12, BinaryPrimitives.ReadUInt16LittleEndian(before.AsSpan(4)));
        var editor = new CfgVisitor { Updates = { [nameof(UbxCfgCfg.DeviceMask)] = (byte)UbxCfgDeviceMask.DevFlash } };
        message.Accept(editor);
        var edited = UbxTestHelper.Serialize(message);
        Assert.Equal(13, BinaryPrimitives.ReadUInt16LittleEndian(edited.AsSpan(4)));
        Assert.Equal((byte)2, edited[18]);
        Assert.Equal(UbxCfgDeviceMask.DevFlash, message.DeviceMask);
        Assert.Equal(nameof(UbxCfgDeviceMask.DevFlash), editor.Fields[nameof(UbxCfgCfg.DeviceMask)].GetEnum<byte>()[2].Name);
    }

    [Fact]
    public void Accept_ResetEnumEdit_ShouldUseDeclaredUnderlyingWidthsAndNumericMetadata()
    {
        var message = new UbxCfgRst();
        var visitor = new CfgVisitor
        {
            Updates =
            {
                [nameof(UbxCfgRst.Bbr)] = (ushort)BbrMask.ColdStart,
                [nameof(UbxCfgRst.Mode)] = (byte)ResetMode.ControlledSoftwareResetGnssOnly
            }
        };

        message.Accept(visitor);

        Assert.Equal(UbxTestHelper.CreateFrame(0x06, 0x04, [0xFF, 0xFF, 2, 0]), UbxTestHelper.Serialize(message));
        Assert.IsType<UInt16Type>(visitor.Fields[nameof(UbxCfgRst.Bbr)].DataType);
        Assert.IsType<UInt8Type>(visitor.Fields[nameof(UbxCfgRst.Mode)].DataType);
        Assert.Equal(nameof(BbrMask.ColdStart), visitor.Fields[nameof(UbxCfgRst.Bbr)].GetEnum<ushort>()[ushort.MaxValue].Name);
    }

    [Fact]
    public void Accept_NestedUartEdit_ShouldUpdateCommonAndTransportFieldsWithoutReplacingConfig()
    {
        var config = new UbxCfgPrtConfigUart { PortId = 1 };
        var message = new UbxCfgPrt { Config = config };
        var visitor = new CfgVisitor
        {
            Updates =
            {
                [nameof(UbxCfgPrtConfig.IsInNmeaProtocol)] = false,
                [nameof(UbxCfgPrtConfigUart.BoundRate)] = 57600,
                [nameof(UbxCfgPrtConfigUart.Parity)] = (int)Parity.Even
            }
        };

        message.Accept(visitor);

        Assert.Same(config, message.Config);
        Assert.False(config.IsInNmeaProtocol);
        Assert.Equal(Parity.Even, config.Parity);
        var frame = UbxTestHelper.Serialize(message);
        Assert.Equal(57600U, BinaryPrimitives.ReadUInt32LittleEndian(frame.AsSpan(14)));
        Assert.Equal(0, BinaryPrimitives.ReadUInt16LittleEndian(frame.AsSpan(18)) & 2);
        var metadata = Assert.IsType<OptionalStructType>(visitor.Fields[nameof(UbxCfgPrt.Config)].DataType);
        Assert.Contains(metadata.BaseType.Fields, field => field.Name == nameof(UbxCfgPrtConfig.IsInUbxProtocol));
        Assert.Contains(metadata.BaseType.Fields, field => field.Name == nameof(UbxCfgPrtConfigUart.BoundRate));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Accept_NullConfig_ShouldRemainAbsentUnlessVisitorExplicitlyRequestsCreation(bool create)
    {
        var message = new UbxCfgPrt();
        var visitor = new CfgVisitor { CreateConfig = create };

        message.Accept(visitor);

        Assert.False(visitor.ConfigInitiallyPresent);
        Assert.Equal(create, visitor.ConfigFinallyPresent);
        if (create)
        {
            Assert.IsType<UbxCfgPrtConfigUart>(message.Config);
            Assert.Contains(nameof(UbxCfgPrtConfigUart.BoundRate), visitor.Fields.Keys);
        }
        else
        {
            Assert.Null(message.Config);
            Assert.Single(visitor.Fields);
        }
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Accept_NestedUsbOrSpi_ShouldVisitCommonFieldsAndPreserveConcreteType(bool spi)
    {
        UbxCfgPrtConfig config = spi ? new UbxCfgPrtConfigSpi() : new UbxCfgPrtConfigUsb();
        var message = new UbxCfgPrt { Config = config };
        var visitor = new CfgVisitor { Updates = { [nameof(UbxCfgPrtConfig.IsInNmeaProtocol)] = false } };

        message.Accept(visitor);

        Assert.Same(config, message.Config);
        Assert.False(config.IsInNmeaProtocol);
        Assert.Equal(spi ? UbxCfgPrtType.Spi : UbxCfgPrtType.Usb, config.PortType);
        Assert.Contains(nameof(UbxCfgPrtConfig.PortType), visitor.Fields.Keys);
        Assert.DoesNotContain(nameof(UbxCfgPrtConfigUart.BoundRate), visitor.Fields.Keys);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Accept_RecreateUsbOrSpi_ShouldKeepConcreteTypeAndMatchingMetadata(bool spi)
    {
        UbxCfgPrtConfig original = spi ? new UbxCfgPrtConfigSpi() : new UbxCfgPrtConfigUsb();
        original.IsInNmeaProtocol = false;
        var message = new UbxCfgPrt { Config = original };
        var visitor = new CfgVisitor { CreateConfig = true };

        message.Accept(visitor);

        Assert.NotSame(original, message.Config);
        if (spi)
        {
            Assert.IsType<UbxCfgPrtConfigSpi>(message.Config);
        }
        else
        {
            Assert.IsType<UbxCfgPrtConfigUsb>(message.Config);
        }
        Assert.True(message.Config.IsInNmeaProtocol);
        Assert.False(original.IsInNmeaProtocol);
        Assert.True(visitor.ConfigInitiallyPresent);
        Assert.True(visitor.ConfigFinallyPresent);
        var type = Assert.IsType<OptionalStructType>(visitor.Fields[nameof(UbxCfgPrt.Config)].DataType);
        Assert.Equal(9, type.BaseType.Fields.Length);
        Assert.DoesNotContain(type.BaseType.Fields, field => field.Name == nameof(UbxCfgPrtConfigUart.BoundRate));
        Assert.DoesNotContain(nameof(UbxCfgPrtConfigUart.BoundRate), visitor.Fields.Keys);
    }

    [Fact]
    public void Accept_RecreateCustomPortConfig_ShouldRejectWithoutReplacingExistingObject()
    {
        var original = new CustomPortConfig();
        var message = new UbxCfgPrt { Config = original };

        Assert.Throws<NotSupportedException>(() => message.Accept(new CfgVisitor { CreateConfig = true }));

        Assert.Same(original, message.Config);
    }

    [Fact]
    public void Accept_FixedLocationEdit_ShouldWriteChangedCoordinatesInDocumentedUnits()
    {
        var message = new UbxCfgTMode3
        {
            Mode = TMode3Enum.FixedMode,
            IsGivenInLLA = true,
            Location = new GeoPoint(45, 60, 100)
        };
        var visitor = new CfgVisitor { Updates = { ["Latitude"] = 46.25, ["Longitude"] = 61.5, ["Altitude"] = 101.2345 } };

        message.Accept(visitor);

        var frame = UbxTestHelper.Serialize(message);
        Assert.Equal(462500000, BinaryPrimitives.ReadInt32LittleEndian(frame.AsSpan(10)));
        Assert.Equal(615000000, BinaryPrimitives.ReadInt32LittleEndian(frame.AsSpan(14)));
        Assert.Equal(10123, BinaryPrimitives.ReadInt32LittleEndian(frame.AsSpan(18)));
        Assert.Equal((byte)45, frame[24]);
        Assert.Equal("m", visitor.Fields["Altitude"].GetUnits());
    }

    [Fact]
    public void Accept_PollFields_ShouldUpdatePollPayloadWithoutAddingResponseFields()
    {
        var message = new UbxCfgMsgPool { MsgClass = 1, MsgId = 7 };
        var visitor = new CfgVisitor { Updates = { [nameof(UbxCfgMsgPool.MsgClass)] = (byte)2, [nameof(UbxCfgMsgPool.MsgId)] = (byte)0x13 } };

        message.Accept(visitor);

        Assert.Equal(UbxTestHelper.CreateFrame(0x06, 0x01, [2, 0x13]), UbxTestHelper.Serialize(message));
        Assert.Equal(2, visitor.Fields.Count);
    }

    private sealed class CfgVisitor : UInt8Type.IVisitor, UInt8OptionalType.IVisitor, UInt16Type.IVisitor,
        UInt32Type.IVisitor, Int32Type.IVisitor, Int8Type.IVisitor, DoubleType.IVisitor, BoolType.IVisitor,
        ArrayType.IVisitor, OptionalStructType.IVisitor
    {
        private int _portIndex;
        private bool _insideConfig;
        public Dictionary<string, object?> Updates { get; } = [];
        public Dictionary<string, Field> Fields { get; } = [];
        public int? EditedPortIndex { get; init; }
        public byte EditedPortValue { get; init; }
        public bool CreateConfig { get; init; }
        public bool ConfigInitiallyPresent { get; private set; }
        public bool ConfigFinallyPresent { get; private set; }

        public void VisitUnknown(Field field, IFieldType type) => Fields[field.Name] = field;

        public void Visit(Field field, UInt8Type type, ref byte value)
        {
            VisitValue(field, ref value);
            if (field.Name == nameof(UbxCfgMsg.Ports) && _portIndex++ == EditedPortIndex)
            {
                value = EditedPortValue;
            }
        }

        public void Visit(Field field, UInt8OptionalType type, ref byte? value) => VisitValue(field, ref value);
        public void Visit(Field field, UInt16Type type, ref ushort value) => VisitValue(field, ref value);
        public void Visit(Field field, UInt32Type type, ref uint value) => VisitValue(field, ref value);
        public void Visit(Field field, Int32Type type, ref int value) => VisitValue(field, ref value);
        public void Visit(Field field, Int8Type type, ref sbyte value) => VisitValue(field, ref value);
        public void Visit(Field field, DoubleType type, ref double value) => VisitValue(field, ref value);
        public void Visit(Field field, BoolType type, ref bool value) => VisitValue(field, ref value);

        public void BeginArray(Field field, ArrayType fieldType)
        {
            Fields[field.Name] = field;
            _portIndex = 0;
        }

        public void EndArray() { }

        public void BeginOptionalStruct(Field field, OptionalStructType type, bool isPresent, out bool createNew)
        {
            Fields[field.Name] = field;
            _insideConfig = field.Name == nameof(UbxCfgPrt.Config);
            createNew = _insideConfig && CreateConfig;
            if (_insideConfig)
            {
                ConfigInitiallyPresent = isPresent;
            }
        }

        public void EndOptionalStruct(bool isPresent)
        {
            if (_insideConfig)
            {
                ConfigFinallyPresent = isPresent;
                _insideConfig = false;
            }
        }

        private void VisitValue<T>(Field field, ref T value)
        {
            Fields[field.Name] = field;
            if (Updates.TryGetValue(field.Name, out var update))
            {
                value = (T)update!;
            }
        }
    }

    private sealed class CustomPortConfig : UbxCfgPrtConfig
    {
        public override UbxCfgPrtType PortType => UbxCfgPrtType.Uart;
        public override void Deserialize(ref ReadOnlySpan<byte> buffer) => throw new NotSupportedException();
        public override void Serialize(ref Span<byte> buffer) => throw new NotSupportedException();
        public override int GetByteSize() => 20;
        public override void Randomize(Random random) => throw new NotSupportedException();
    }
}
