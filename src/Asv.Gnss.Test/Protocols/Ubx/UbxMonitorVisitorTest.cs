using System;
using System.Collections.Generic;
using System.Linq;
using Asv.IO;
using Xunit;

namespace Asv.Gnss;

public class UbxMonitorVisitorTest
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Accept_Acknowledgement_ShouldExposeAndUpdateBothPayloadFields(bool negative)
    {
        UbxAckBase message = negative ? new UbxAckNak() : new UbxAckAck();
        message.AckClassId = 0x06;
        message.AckSubclassId = 0x24;
        var visitor = new MonitorVisitor(true);

        message.Accept(visitor);

        Assert.Equal(0x07, message.AckClassId);
        Assert.Equal(0x25, message.AckSubclassId);
        Assert.Equal(new[] { "AckClassId", "AckSubclassId" }, visitor.Fields.Keys);
        Assert.All(visitor.Fields.Values, field => Assert.False(string.IsNullOrWhiteSpace(field.GetDescription())));
        var frame = UbxTestHelper.Serialize(message);
        Assert.Equal(new byte[] { 0x07, 0x25 }, frame.AsSpan(6, 2).ToArray());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void Accept_InformationMessages_ShouldInheritStringTraversal(int kind)
    {
        UbxInfBase message = kind switch
        {
            0 => new UbxInfDebug(),
            1 => new UbxInfError(),
            2 => new UbxInfNotice(),
            _ => new UbxInfTest()
        };
        message.Message = "Receiver message";
        var reader = new MonitorVisitor();
        message.Accept(reader);
        Assert.Equal("Receiver message", message.Message);
        Assert.IsType<StringOptionalType>(Assert.Single(reader.Fields).Value.DataType);

        message.Accept(new MonitorVisitor(true));

        Assert.Equal("Receiver message edited", message.Message);
    }

    [Fact]
    public void Accept_ReceiveBuffers_ShouldVisitEveryFixedElementAndKeepArrayIdentity()
    {
        var message = new UbxMonRxbuf();
        for (var i = 0; i < 6; i++)
        {
            message.Pending[i] = (ushort)(100 + i);
            message.Usage[i] = (byte)i;
            message.PeakUsage[i] = (byte)(10 + i);
        }
        var pending = message.Pending;
        var usage = message.Usage;
        var peak = message.PeakUsage;
        var visitor = new MonitorVisitor(true);

        message.Accept(visitor);

        Assert.Same(pending, message.Pending);
        Assert.Same(usage, message.Usage);
        Assert.Same(peak, message.PeakUsage);
        Assert.Equal(Enumerable.Range(101, 6).Select(value => (ushort)value), message.Pending);
        Assert.Equal(Enumerable.Range(1, 6).Select(value => (byte)value), message.Usage);
        Assert.Equal(Enumerable.Range(11, 6).Select(value => (byte)value), message.PeakUsage);
        Assert.Equal(new[] { 6, 6, 6 }, visitor.ArraySizes);
        Assert.Equal(18, visitor.ValueVisits);
        Assert.Equal("byte", visitor.Fields["Pending"].GetUnits());
        Assert.Equal("%", visitor.Fields["Usage"].GetUnits());
    }

    [Fact]
    public void Accept_TransmitBuffers_ShouldExposeArraysAndErrorFlags()
    {
        var message = new UbxMonTxbuf { TUsage = 10, TPeakUsage = 20, Errors = 2, Reserved1 = 4 };
        var visitor = new MonitorVisitor(true);

        message.Accept(visitor);

        Assert.All(message.Pending, value => Assert.Equal(1, value));
        Assert.All(message.Usage, value => Assert.Equal(1, value));
        Assert.All(message.PeakUsage, value => Assert.Equal(1, value));
        Assert.Equal(11, message.TUsage);
        Assert.Equal(21, message.TPeakUsage);
        Assert.Equal(3, message.Errors);
        Assert.Equal(5, message.Reserved1);
        Assert.True(message.IsAlloc);
        Assert.True(message.IsMem);
        Assert.True(message.IsLimit);
        Assert.Equal(new[] { 6, 6, 6 }, visitor.ArraySizes);
        Assert.Equal(10, visitor.Fields.Count);
    }

    [Fact]
    public void Accept_MessageCounters_ShouldVisitSixProtocolArraysAndUnsignedSkippedCounts()
    {
        var message = new UbxMonMsgpp();
        message.Skipped[0] = uint.MaxValue;
        var visitor = new MonitorVisitor(true);

        message.Accept(visitor);

        foreach (var values in new[] { message.Msg1, message.Msg2, message.Msg3, message.Msg4, message.Msg5, message.Msg6 })
        {
            Assert.Equal(8, values.Length);
            Assert.All(values, value => Assert.Equal(1, value));
        }
        Assert.Equal(0x7FFFFFFFu, message.Skipped[0]);
        Assert.All(message.Skipped.Skip(1), value => Assert.Equal(0x80000000u, value));
        Assert.Equal(new[] { 8, 8, 8, 8, 8, 8, 6 }, visitor.ArraySizes);
        Assert.Equal(54, visitor.ValueVisits);
    }

    [Fact]
    public void Accept_VersionStrings_ShouldKeepListOnReadAndWriteChangedStringsAndNewExtensions()
    {
        var message = new UbxMonVer { Software = "FW", Hardware = "HW", Extensions = ["ROM", "PROTVER=27"] };
        var extensions = message.Extensions;
        message.Accept(new MonitorVisitor());
        Assert.Same(extensions, message.Extensions);
        Assert.Equal(new[] { "ROM", "PROTVER=27" }, message.Extensions);
        var visitor = new MonitorVisitor(true);

        message.Accept(visitor);

        Assert.Equal("FW edited", message.Software);
        Assert.Equal("HW edited", message.Hardware);
        Assert.Equal(new[] { "ROM edited", "PROTVER=27 edited", " edited" }, message.Extensions);
        Assert.IsType<ListType>(visitor.Fields["Extensions"].DataType);
        Assert.Equal((uint)30, Assert.IsType<StringType>(((ListType)visitor.Fields["Extensions"].DataType).ElementType).MaxSize);
    }

    [Fact]
    public void Accept_MoreThan255VersionExtensions_ShouldPreserveValidListOnRead()
    {
        var message = new UbxMonVer { Software = "FW", Hardware = "HW", Extensions = Enumerable.Repeat("EXT", 256).ToList() };
        var extensions = message.Extensions;
        var visitor = new MonitorVisitor();

        message.Accept(visitor);

        Assert.Same(extensions, message.Extensions);
        Assert.Equal(256, message.Extensions.Count);
        Assert.All(message.Extensions, value => Assert.Equal("EXT", value));
        Assert.Equal(258, visitor.ValueVisits);
    }

    [Fact]
    public void Accept_HardwareStatus_ShouldExposeEveryDiagnosticAndEnumMetadata()
    {
        var message = new UbxMonHw
        {
            PinSel = 1, PinBank = 2, PinDir = 3, PinVal = 4,
            Noise = 5, AgcCnt = 6, UsedMask = 7, VP = new byte[17],
            JamInd = 8, PinIrq = 9, PullH = 10, PullL = 11,
            CwJammingIndicator = 0.25, AgcMonitor = 0.5
        };
        var virtualPins = message.VP;
        message.Accept(new MonitorVisitor());
        Assert.Same(virtualPins, message.VP);
        var visitor = new MonitorVisitor(true);

        message.Accept(visitor);

        Assert.Equal(2, message.PinSel);
        Assert.Equal(3, message.PinBank);
        Assert.Equal(4, message.PinDir);
        Assert.Equal(5, message.PinVal);
        Assert.Equal(6, message.Noise);
        Assert.Equal(7, message.AgcCnt);
        Assert.Equal(UbxMonHw.AntennaSupervisorStateMachineStatus.Open, message.AStatus);
        Assert.Equal(UbxMonHw.AntennaPowerStatus.On, message.APower);
        Assert.Equal(UbxMonHw.JammingStateEnum.Critical, message.JammingState);
        Assert.True(message.RtcCalib);
        Assert.True(message.SafeBoot);
        Assert.True(message.XTalAbsent);
        Assert.Equal(8, message.UsedMask);
        Assert.Same(virtualPins, message.VP);
        Assert.All(message.VP, value => Assert.Equal(1, value));
        Assert.Equal(9, message.JamInd);
        Assert.Equal(10, message.PinIrq);
        Assert.Equal(11, message.PullH);
        Assert.Equal(12, message.PullL);
        Assert.Equal(0.5, message.CwJammingIndicator);
        Assert.Equal(0.75, message.AgcMonitor);
        Assert.Equal(20, visitor.Fields.Count);
        Assert.Equal(new[] { 17 }, visitor.ArraySizes);
        Assert.Equal("Open circuit", visitor.Fields["AStatus"].GetEnum<int>()[4].Name);
        Assert.Equal("On", visitor.Fields["APower"].GetEnum<int>()[1].Name);
        Assert.Equal("Critical", visitor.Fields["JammingState"].GetEnum<int>()[3].Name);
    }

    [Fact]
    public void Accept_RawNavigationWords_ShouldVisitHeaderAndFullWidthWordsWithoutChangingDecodedViews()
    {
        var gps = new GpsSubframe1();
        var glonass = new GlonassWord1();
        var message = new UbxRxmSfrbx
        {
            UbxGnssId = UbxGnssTypeEnum.GPS,
            SvId = 20, Reserved1 = 1, FreqId = -2, NumWords = 2, Chn = 3, Version = 2, Reserved2 = 4,
            RawData = [uint.MaxValue, 0x80000000], GpsSubFrame = gps, GlonassWord = glonass
        };
        var words = message.RawData;
        message.Accept(new MonitorVisitor());
        Assert.Same(words, message.RawData);
        Assert.Equal(new uint[] { uint.MaxValue, 0x80000000 }, message.RawData);
        var visitor = new MonitorVisitor(true);

        message.Accept(visitor);

        Assert.Equal(UbxGnssTypeEnum.GLONASS, message.UbxGnssId);
        Assert.Equal(21, message.SvId);
        Assert.Equal(3, message.Reserved1);
        Assert.Equal(3, message.SigId);
        Assert.Equal(-3, message.FreqId);
        Assert.Equal(3, message.NumWords);
        Assert.Equal(4, message.Chn);
        Assert.Equal(3, message.Version);
        Assert.Equal(5, message.Reserved2);
        Assert.Equal(new uint[] { 0x7FFFFFFF, 0, 0x80000000 }, message.RawData);
        Assert.Same(gps, message.GpsSubFrame);
        Assert.Same(glonass, message.GlonassWord);
        Assert.Equal(10, visitor.Fields.Count);
        Assert.Contains(nameof(UbxRxmSfrbx.SigId), visitor.Fields.Keys);
        Assert.Equal("GLONASS", visitor.Fields["UbxGnssId"].GetEnum<int>()[6].Name);
    }

    [Fact]
    public void Accept_RawNavigationWordsWithoutResize_ShouldUpdateOriginalArray()
    {
        var words = new uint[] { uint.MaxValue, 0x80000000 };
        var message = new UbxRxmSfrbx { RawData = words, NumWords = 2 };

        message.Accept(new MonitorVisitor(true, false));

        Assert.Same(words, message.RawData);
        Assert.Equal(new uint[] { 0x7FFFFFFF, 0 }, words);
        Assert.Equal(3, message.NumWords);
    }

    [Fact]
    public void Accept_NoOpOnUninitializedMessages_ShouldPreserveNullPropertiesAndPolls()
    {
        var versions = new UbxMonVer();
        var hardware = new UbxMonHw();
        var raw = new UbxRxmSfrbx { NumWords = 9 };
        var info = new UbxInfDebug();
        foreach (var message in new UbxMessageBase[] { versions, hardware, raw, info })
        {
            message.Accept(new MonitorVisitor());
        }
        Assert.Null(versions.Software);
        Assert.Null(versions.Hardware);
        Assert.Null(versions.Extensions);
        Assert.Null(hardware.VP);
        Assert.Null(raw.RawData);
        Assert.Equal(9, raw.NumWords);
        Assert.Null(info.Message);
        foreach (var message in new UbxMessageBase[] { new UbxMonVerPool(), new UbxMonHwPool(), new UbxMonMsgppPool(), new UbxMonRxbufPool(), new UbxMonTxbufPool(), new UbxRxmSfrbxPool() })
        {
            var visitor = new MonitorVisitor();
            var before = UbxTestHelper.Serialize(message);
            message.Accept(visitor);
            Assert.Empty(visitor.Fields);
            Assert.Equal(before, UbxTestHelper.Serialize(message));
        }
    }

    private sealed class MonitorVisitor(bool mutate = false, bool resizeLists = true) : UInt8Type.IVisitor, UInt16Type.IVisitor,
        UInt32Type.IVisitor, Int32Type.IVisitor, Int8Type.IVisitor, DoubleType.IVisitor,
        BoolType.IVisitor, StringOptionalType.IVisitor, StringType.IVisitor, ArrayType.IVisitor, ListType.IVisitor
    {
        public Dictionary<string, Field> Fields { get; } = [];
        public List<int> ArraySizes { get; } = [];
        public int ValueVisits { get; private set; }

        private void Record(Field field)
        {
            Fields[field.Name] = field;
            ValueVisits++;
        }

        public void Visit(Field field, UInt8Type type, ref byte value)
        {
            Record(field);
            if (mutate) value++;
        }

        public void Visit(Field field, UInt16Type type, ref ushort value)
        {
            Record(field);
            if (mutate) value++;
        }

        public void Visit(Field field, UInt32Type type, ref uint value)
        {
            Record(field);
            if (mutate) value ^= 0x80000000;
        }

        public void Visit(Field field, Int32Type type, ref int value)
        {
            Record(field);
            if (mutate) value = field.Name switch
            {
                "AStatus" => 4,
                "APower" => 1,
                "JammingState" => 3,
                "UbxGnssId" => 6,
                _ => value + 1
            };
        }

        public void Visit(Field field, Int8Type type, ref sbyte value)
        {
            Record(field);
            if (mutate) value--;
        }

        public void Visit(Field field, DoubleType type, ref double value)
        {
            Record(field);
            if (mutate) value += 0.25;
        }

        public void Visit(Field field, BoolType type, ref bool value)
        {
            Record(field);
            if (mutate) value = !value;
        }

        public void Visit(Field field, StringOptionalType type, ref string? value)
        {
            Record(field);
            if (mutate) value += " edited";
        }

        public void Visit(Field field, StringType type, ref string value)
        {
            Record(field);
            if (mutate) value += " edited";
        }

        public void BeginArray(Field field, ArrayType type)
        {
            Fields[field.Name] = field;
            ArraySizes.Add(type.Size);
        }

        public void EndArray()
        {
        }

        public void BeginList(Field field, ListType type, ref uint size)
        {
            Fields[field.Name] = field;
            if (mutate && resizeLists) size++;
        }

        public void EndList()
        {
        }

        public void VisitUnknown(Field field, IFieldType type)
        {
            throw new InvalidOperationException($"Unexpected field type: {field.Name} ({type.Name})");
        }
    }
}
