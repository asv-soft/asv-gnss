using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Asv.IO;
using Xunit;

namespace Asv.Gnss;

public class AdsbMessageTest
{
    private const string Identification = "8D4840D6202CC371C32CE0576098";

    [Fact]
    public void SetRawData_OwnsStorageRefreshesPayloadAndPreservesStateOnFailure()
    {
        var bytes = Convert.FromHexString(Identification);
        var message = new AdsbDfMessage();
        message.SetRawData(bytes);
        bytes[0] = 0;
        Assert.Equal((byte)0x8D, message.RawData.Span[0]);
        Assert.Equal("KLM1023", message.ExtendedSquitter!.Identification!.Callsign);
        var decoded = message.ExtendedSquitter;
        var original = message.RawData;
        var corrupt = original.ToArray(); corrupt[^1] ^= 1;
        Assert.Throws<ProtocolParserCrcException>(() => message.SetRawData(corrupt));
        Assert.Equal(original, message.RawData);
        Assert.Same(decoded, message.ExtendedSquitter);
        Assert.Throws<FormatException>(() => message.SetRawData(new byte[8]));
        Assert.Throws<FormatException>(() => message.SetRawData(new byte[] { 0x8D, 0, 0, 0, 0, 0, 0 }));
        message.SetRawData(Convert.FromHexString("02E99619FACDAE"));
        Assert.Null(message.ExtendedSquitter);
        Assert.Equal((ushort)0, message.Id);
    }

    [Fact]
    public void Serialize_UsesExactSizedSliceAndNeverOverwritesAdjacentStorage()
    {
        var message = new AdsbDfMessage(); message.SetRawData(Convert.FromHexString(Identification));
        var output = Enumerable.Repeat((byte)0xAA, message.GetByteSize() + 6).ToArray();
        var buffer = output.AsSpan(2, message.GetByteSize() + 2);
        message.Serialize(ref buffer);
        Assert.Equal(2, buffer.Length);
        Assert.Equal(Encoding.ASCII.GetBytes("*" + Identification + ";"), output[2..^4]);
        Assert.All(output[..2], value => Assert.Equal((byte)0xAA, value));
        Assert.All(output[^4..], value => Assert.Equal((byte)0xAA, value));
        Assert.Throws<ArgumentException>(() => SerializeTooSmall(message));
    }

    [Fact]
    public void Serialize_RejectsInvalidTransportMetadataWithoutTruncatingTimestamp()
    {
        var message = new AdsbDfMessage { FrameFormat = AdsbFrameFormat.Beast };
        Assert.Throws<InvalidOperationException>(() => message.GetByteSize());
        message.ReceiverTimestamp = 0; message.SignalLevel = 1;
        Assert.Equal(16, message.GetByteSize());
        message.ReceiverTimestamp = 0x1000000000000;
        Assert.Throws<ArgumentOutOfRangeException>(() => message.GetByteSize());
        message.ReceiverTimestamp = 0; message.FrameFormat = AdsbFrameFormat.Avr; message.AvrMarker = '\u012A';
        Assert.Throws<ArgumentOutOfRangeException>(() => message.GetByteSize());
        message.FrameFormat = (AdsbFrameFormat)99;
        Assert.Throws<ArgumentOutOfRangeException>(() => message.GetByteSize());
    }

    [Fact]
    public void Accept_ExposesCachedReadonlyMetadataTypedValuesAndRawBytesWithoutMutation()
    {
        var message = new AdsbDfMessage(); message.SetRawData(Convert.FromHexString(Identification));
        var original = message.RawData;
        var payload = message.ExtendedSquitter;
        var first = new Reader(); var second = new Reader();
        message.Accept(first); message.Accept(second);
        Assert.Equal((byte)17, first.Values["DownlinkFormat"]);
        Assert.Equal(0x4840D6u, first.Values["IcaoAddress"]);
        Assert.Equal("KLM1023", first.Values["Callsign"]);
        Assert.Equal(Convert.FromHexString(Identification), first.Bytes);
        Assert.Equal(0, first.Depth);
        Assert.Equal(first.Fields.Count, second.Fields.Count);
        for (var i = 0; i < first.Fields.Count; i++)
        {
            Assert.Same(first.Fields[i], second.Fields[i]);
            Assert.False(string.IsNullOrWhiteSpace(first.Fields[i].GetDescription()));
            Assert.Equal(true, first.Fields[i].Metadata["readOnly"]);
        }
        Assert.Equal(original, message.RawData);
        Assert.Same(payload, message.ExtendedSquitter);
        Assert.Equal("KLM1023", message.ExtendedSquitter!.Identification!.Callsign);
        Assert.Throws<ArgumentNullException>(() => message.Accept(null!));
    }

    private static void SerializeTooSmall(AdsbDfMessage message)
    {
        Span<byte> span = new byte[message.GetByteSize() - 1];
        message.Serialize(ref span);
    }

    private sealed class Reader : UInt8Type.IVisitor, UInt32Type.IVisitor, UInt32OptionalType.IVisitor,
        UInt64OptionalType.IVisitor, UInt8OptionalType.IVisitor, StringType.IVisitor, ArrayType.IVisitor, StructType.IVisitor
    {
        public List<Field> Fields { get; } = [];
        public Dictionary<string, object?> Values { get; } = [];
        public List<byte> Bytes { get; } = [];
        public int Depth { get; private set; }
        public void Visit(Field f, UInt8Type t, ref byte v) { Fields.Add(f); Values[f.Name] = v; if (f.Name == "RawData") Bytes.Add(v); v = 0; }
        public void Visit(Field f, UInt32Type t, ref uint v) { Fields.Add(f); Values[f.Name] = v; v = 0; }
        public void Visit(Field f, UInt32OptionalType t, ref uint? v) { Fields.Add(f); Values[f.Name] = v; v = 0; }
        public void Visit(Field f, UInt64OptionalType t, ref ulong? v) { Fields.Add(f); Values[f.Name] = v; v = 0; }
        public void Visit(Field f, UInt8OptionalType t, ref byte? v) { Fields.Add(f); Values[f.Name] = v; v = 0; }
        public void Visit(Field f, StringType t, ref string v) { Fields.Add(f); Values[f.Name] = v; v = "modified"; }
        public void BeginArray(Field f, ArrayType t) { Fields.Add(f); Depth++; }
        public void EndArray() => Depth--;
        public void BeginStruct(Field f, StructType t) { Fields.Add(f); Depth++; }
        public void EndStruct() => Depth--;
        public void VisitUnknown(Field f, IFieldType t) => throw new InvalidOperationException(f.Name);
    }
}
