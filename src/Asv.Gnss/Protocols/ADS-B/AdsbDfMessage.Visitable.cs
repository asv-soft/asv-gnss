using System;
using Asv.IO;

namespace Asv.Gnss;

public partial class AdsbDfMessage
{
    private static readonly Field FrameFormatField = new Field.Builder().Name(nameof(FrameFormat)).Title(nameof(FrameFormat)).Description("Receiver transport encoding.").DataType(StringType.Ascii).Metadata("readOnly", true).Build();
    private static readonly Field DownlinkFormatField = new Field.Builder().Name(nameof(DownlinkFormat)).Title(nameof(DownlinkFormat)).Description("Five-bit Mode S downlink format.").DataType(new UInt8Type(0, 31)).Metadata("readOnly", true).Build();
    private static readonly Field HeaderBitsField = new Field.Builder().Name(nameof(HeaderBits)).Title(nameof(HeaderBits)).Description("Three low header bits: capability or control according to the DF.").DataType(new UInt8Type(0, 7)).Metadata("readOnly", true).Build();
    private static readonly Field IcaoAddressField = new Field.Builder().Name(nameof(IcaoAddress)).Title(nameof(IcaoAddress)).Description("Explicit 24-bit ICAO address. Unverified address/data-parity recovery is deliberately excluded.").DataType(new UInt32OptionalType(0, 0xFFFFFF)).FormatString("X6").Metadata("readOnly", true).Build();
    private static readonly Field ParityField = new Field.Builder().Name(nameof(Parity)).Title(nameof(Parity)).Description("Transmitted 24-bit parity, including any interrogator/address/data overlay.").DataType(new UInt32Type(0, 0xFFFFFF)).FormatString("X6").Metadata("readOnly", true).Build();
    private static readonly Field ParitySyndromeField = new Field.Builder().Name(nameof(ParitySyndrome)).Title(nameof(ParitySyndrome)).Description("Computed payload CRC XOR transmitted parity. A nonzero result is expected for address/data parity.").DataType(new UInt32Type(0, 0xFFFFFF)).FormatString("X6").Metadata("readOnly", true).Build();
    private static readonly Field ParityStatusField = new Field.Builder().Name(nameof(ParityStatus)).Title(nameof(ParityStatus)).Description("Validation possible without an aircraft-address cache; unverified does not mean a valid CRC.").DataType(StringType.Ascii).Metadata("readOnly", true).Build();
    private static readonly Field ReceiverTimestampField = new Field.Builder().Name(nameof(ReceiverTimestamp)).Title(nameof(ReceiverTimestamp)).Description("Unmodified 48-bit receiver clock value. Clock interpretation depends on the receiver; this is not UTC.").DataType(new UInt64OptionalType(0, AdsbFrameCodec.MaximumTimestamp)).Metadata("readOnly", true).Build();
    private static readonly Field SignalLevelField = new Field.Builder().Name(nameof(SignalLevel)).Title(nameof(SignalLevel)).Description("Unscaled receiver signal level from 0 to 255, when present.").DataType(UInt8OptionalType.Default).Metadata("readOnly", true).Build();
    private static readonly Field ShortRawField = RawField(7);
    private static readonly Field LongRawField = RawField(14);
    private static readonly Field ExtendedSquitterField = new Field.Builder().Name(nameof(ExtendedSquitter))
        .Title("Extended squitter").Description("Decoded ADS-B message element, when the DF and control field carry ADS-B.")
        .DataType(AdsbExtendedSquitter.VisitorType).Metadata("readOnly", true).Build();

    private static Field RawField(int size) => new Field.Builder().Name(nameof(RawData)).Title("Native Mode S frame")
        .Description("Unmodified native Mode S bytes, excluding AVR/Beast receiver framing.")
        .DataType(new ArrayType(UInt8Type.Default, size)).FormatString("X2").Metadata("readOnly", true).Build();

    /// <inheritdoc />
    public void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        string valueFrameFormat = FrameFormat.ToString();
        StringType.Accept(visitor, FrameFormatField, ref valueFrameFormat);
        byte valueDownlinkFormat = DownlinkFormat;
        UInt8Type.Accept(visitor, DownlinkFormatField, ref valueDownlinkFormat);
        byte valueHeaderBits = HeaderBits;
        UInt8Type.Accept(visitor, HeaderBitsField, ref valueHeaderBits);
        uint? valueIcaoAddress = IcaoAddress;
        UInt32OptionalType.Accept(visitor, IcaoAddressField, ref valueIcaoAddress);
        uint valueParity = Parity;
        UInt32Type.Accept(visitor, ParityField, ref valueParity);
        uint valueParitySyndrome = ParitySyndrome;
        UInt32Type.Accept(visitor, ParitySyndromeField, ref valueParitySyndrome);
        string valueParityStatus = ParityStatus.ToString();
        StringType.Accept(visitor, ParityStatusField, ref valueParityStatus);
        ulong? valueReceiverTimestamp = ReceiverTimestamp;
        UInt64OptionalType.Accept(visitor, ReceiverTimestampField, ref valueReceiverTimestamp);
        byte? valueSignalLevel = SignalLevel;
        UInt8OptionalType.Accept(visitor, SignalLevelField, ref valueSignalLevel);
        var rawField = _rawData.Length == 7 ? ShortRawField : LongRawField;
        ArrayType.Accept(visitor, rawField, (index, target, field, type) =>
        {
            var value = _rawData[index];
            UInt8Type.Accept(target, field, type, ref value);
        });
        if (ExtendedSquitter != null) StructType.Accept(visitor, ExtendedSquitterField, ExtendedSquitter);
    }
}
