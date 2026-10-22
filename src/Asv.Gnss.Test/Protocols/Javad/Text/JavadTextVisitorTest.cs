using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Text;
using Asv.IO;
using Xunit;

namespace Asv.Gnss;

public sealed class JavadTextVisitorTest
{
    [Fact]
    public void EveryTextAndInteractiveModel_VisitsWithoutChangingFrameOrMetadata()
    {
        var factory = Factory();
        Assert.Equal(32, factory.GetSupportedIds().Count());
        foreach (var id in factory.GetSupportedIds())
        {
            var message = factory.Create(id)!;
            var frame = Serialize(message);
            var first = new MetadataVisitor();
            message.Accept(first);
            var second = new MetadataVisitor();
            message.Accept(second);
            Assert.Equal(frame, Serialize(message));
            Assert.NotEmpty(first.Fields);
            Assert.Equal(first.Fields.Count, second.Fields.Count);
            for (var index = 0; index < first.Fields.Count; index++)
            {
                Assert.Same(first.Fields[index].Field, second.Fields[index].Field);
                Assert.Same(first.Fields[index].Type, second.Fields[index].Type);
            }
            message.Accept(new PrintValueVisitor(new StringBuilder(), false));
            Assert.Equal(frame, Serialize(message));
        }
    }

    [Fact]
    public void MetadataFieldOrder_MatchesTraversalAtEveryNestedLevel()
    {
        var parameters = new JavadParametersMessage { Parameters = "{first,{second,third}}" };
        var partialParameters = new JavadParametersMessage { Parameters = "{incomplete," };
        var textData = Assert.IsType<JavadTextDataMessage>(
            Decode(
                "TD",
                Encoding.ASCII.GetBytes(
                    "TD036,CGGTTS,{FY=2005,REF=UTC(USNO),CH=816 (UNIVERSAL)},@7C"
                )
            )
        );
        (JavadMessageBase Message, StructType Metadata)[] cases =
        [
            (
                Decode("GS", Frame("GS", ",GPSVST,1,{09,08,009,{45,,38},01},@")),
                JavadGpsSatelliteStatusMessage.StructType
            ),
            (
                Decode("NS", Frame("NS", ",GLSVST,1,{08,-4,27,198,{42},63},@")),
                JavadGlonassSatelliteStatusMessage.StructType
            ),
            (
                Decode("DL", Frame("DL", ",DLINK,1,{A,T,base,9,108,0,98.5},@")),
                JavadDataLinkStatusMessage.StructType
            ),
            (
                Decode(
                    "TM",
                    Frame(
                        "TM",
                        ",TIMING,V,120000.00,0,A,{08,4},{{V,123.4,-0.2},{N,,}},1.1,0.03,0.004,1,@"
                    )
                ),
                JavadClockOffsetsMessage.StructType
            ),
            (
                Decode("RM", Frame("RM", ",RAIM,V,120000.00,2,2,G01,R02,@")),
                JavadRaimMessage.StructType
            ),
            (
                new JavadReferenceStationStatusMessage(),
                JavadReferenceStationStatusMessage.StructType
            ),
            (new JavadImuAttitudeMessage(), JavadImuAttitudeMessage.StructType),
            (new JavadWrapperMessage { Data = [1, 2] }, JavadWrapperMessage.StructType),
            (new JavadEventMessage { Data = [3, 4] }, JavadEventMessage.StructType),
            (new JavadReplyMessage { Text = "%label% reply" }, JavadReplyMessage.StructType),
            (parameters, parameters.GetVisitorType()),
            (partialParameters, partialParameters.GetVisitorType()),
            (textData, textData.GetVisitorType()),
        ];
        foreach (var (message, metadata) in cases)
        {
            var visitor = new FieldOrderVisitor(metadata);
            message.Accept(visitor);
            visitor.Complete();
        }
    }

    [Theory]
    [InlineData(
        "GS",
        ",GPSVST,02,{09,08,009,{45,,38,,,42},01,extension},{12,22,183,{40,39},03},future,@"
    )]
    [InlineData("NS", ",GLSVST,01,{08,-4,27,198,{42},63},future,@")]
    [InlineData(
        "TX",
        ",TEXT, 1 , { 10/22 , 12:34:56 , \"  padded, text  \" , T , A , 009 } ,future,@"
    )]
    [InlineData(
        "NP",
        ",NAVPOS,V,123456.75,0,RF,{09,,3},WGS84,N55o12'30.000000\",W037o45'00.000000\",123.4,V,-12.5,1.2,2.3,0.04,0.05,36,-3.6,89,N,,0.1,0.2,98.5,009,future,@"
    )]
    public void TypedInspection_PreservesReceivedWhitespacePaddingAndUnknownNodes(
        string id,
        string payload
    )
    {
        var frame = Frame(id, payload);
        var message = Assert.IsAssignableFrom<JavadTextMessage>(Decode(id, frame));
        var fields = message.Fields;
        message.Accept(new NoOpVisitor());
        Assert.True(fields == message.Fields);
        for (var index = 0; index < fields.Length; index++)
            Assert.Same(fields[index], message.Fields[index]);
        Assert.Equal(frame, Serialize(message));
    }

    [Fact]
    public void NullableScalarEdits_UseTheirActualTypesAndSerialize()
    {
        var message = Assert.IsType<JavadImuAttitudeMessage>(
            Decode("QQ", Frame("QQ", ",INSATT,N,123456.00,01.00,02.00,03.00,future,@"))
        );
        message.Accept(new ScalarEditVisitor());
        var decoded = Assert.IsType<JavadImuAttitudeMessage>(Decode("QQ", Serialize(message)));
        Assert.Equal('V', decoded.SolutionValidity);
        Assert.Equal(TimeSpan.FromHours(1), decoded.UtcTime);
        Assert.Equal(12.5, decoded.RollDegrees);
        Assert.Null(decoded.PitchDegrees);
        Assert.Equal(3, decoded.HeadingDegrees);
        Assert.Equal("future", decoded.Fields[^1].Scalar);
        Assert.IsType<CharOptionalType>(
            JavadImuAttitudeMessage.StructType["SolutionValidity"]!.DataType
        );
        Assert.IsType<TimeSpanOptionalType>(
            JavadImuAttitudeMessage.StructType["UtcTime"]!.DataType
        );
        Assert.IsType<DoubleOptionalType>(
            JavadImuAttitudeMessage.StructType["RollDegrees"]!.DataType
        );
    }

    [Fact]
    public void NestedSatelliteAndSignalEdits_PreserveUnknownRecordAndMessageFields()
    {
        var message = Assert.IsType<JavadGpsSatelliteStatusMessage>(
            Decode(
                "GS",
                Frame("GS", ",GPSVST,01,{09,08,009,{45,,38},01,group-future},message-future,@")
            )
        );
        var extra = message.Fields[^1];
        var nestedExtra = message.Satellites[0].ExtraFields[0];
        message.Accept(new SatelliteEditVisitor());
        var satellite = Assert.Single(message.Satellites);
        Assert.Equal(77, satellite.AzimuthDegrees);
        Assert.Equal(new double?[] { 46, null, 39 }, satellite.SignalToNoiseDbHz);
        Assert.Same(extra, message.Fields[^1]);
        Assert.Same(nestedExtra, satellite.ExtraFields[0]);
        var decoded = Assert.IsType<JavadGpsSatelliteStatusMessage>(
            Decode("GS", Serialize(message))
        );
        Assert.Equal(77, decoded.Satellites[0].AzimuthDegrees);
        Assert.Equal("01", message.Fields[0].Scalar);
    }

    [Fact]
    public void NestedRecordVisitor_PreservesRecordApiAndOnlyReplacesEditedAst()
    {
        var value = JavadValue.List(
            JavadValue.TextAtom("A"),
            JavadValue.TextAtom("T"),
            JavadValue.TextAtom("009"),
            JavadValue.Integer(8),
            JavadValue.Integer(9),
            JavadValue.Integer(0),
            JavadValue.Float(99),
            JavadValue.Omitted,
            JavadValue.Omitted,
            JavadValue.TextAtom("extra")
        );
        var record = new JavadDataLinkStatus(value);
        record.Accept(new NoOpVisitor());
        Assert.Same(value, record.Value);
        record.Deconstruct(out var deconstructed);
        Assert.Same(value, deconstructed);
        var copy = record with { Value = value };
        Assert.Equal(record, copy);
        record.Accept(new LinkEditVisitor());
        Assert.Equal("replacement", record.ReferenceStationId);
        Assert.Same(value.Items[^1], record.Value.Items[^1]);
        Assert.Same(value, copy.Value);
    }

    [Fact]
    public void EveryNestedRecordVisitor_RejectsNullVisitorWithTheArgumentName()
    {
        static JavadValue Group(int count) =>
            JavadValue.List([.. Enumerable.Repeat(JavadValue.Omitted, count)]);

        IVisitable[] records =
        [
            new JavadSatelliteTextStatus(
                JavadValue.List(
                    JavadValue.Omitted,
                    JavadValue.Omitted,
                    JavadValue.Omitted,
                    JavadValue.List(),
                    JavadValue.Omitted
                )
            ),
            new JavadDataLinkStatus(Group(7)),
            new JavadReferenceLocationStatus(Group(4)),
            new JavadCorrectionText(Group(6)),
            new JavadTimeResidual(Group(3)),
            new JavadClockOffset(Group(3)),
            new JavadReferenceStationParameters(Group(13)),
        ];
        foreach (var record in records)
        {
            var error = Assert.Throws<ArgumentNullException>(() => record.Accept(null!));
            Assert.Equal("visitor", error.ParamName);
        }
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void RepeatedGroups_CanResizeByCountOrListAndKeepTrailingFields(bool useCount)
    {
        var message = Assert.IsType<JavadDataLinkStatusMessage>(
            Decode(
                "DL",
                Frame("DL", ",DLINK,02,{A,T,first,1,2,0,99},{B,T,second,3,4,0,98},future,@")
            )
        );
        var tail = message.Fields[^1];
        message.Accept(new ResizeVisitor(useCount));
        Assert.Equal(1, message.LinkCount);
        Assert.Equal("first", Assert.Single(message.Links).ReferenceStationId);
        Assert.Same(tail, message.Fields[^1]);
        var decoded = Assert.IsType<JavadDataLinkStatusMessage>(Decode("DL", Serialize(message)));
        Assert.Equal(1, decoded.LinkCount);
        Assert.Equal("future", decoded.Fields[^1].Scalar);
    }

    [Fact]
    public void OptionalClockGroup_CanReplaceAnExistingGroupThroughTheStandardVisitorContract()
    {
        var message = Assert.IsType<JavadClockOffsetsMessage>(
            Decode(
                "TM",
                Frame(
                    "TM",
                    ",TIMING,V,120000.00,0,A,{08,4},{{V,123.4,-0.2,extra},{N,,}},1.1,0.03,0.004,1,future,@"
                )
            )
        );
        message.Accept(new ReplaceClocksVisitor());
        Assert.NotNull(message.GpsClock);
        Assert.Null(message.GpsClock.OffsetMeters);
        Assert.Null(message.GpsClock.Validity);
        Assert.NotNull(message.GlonassClock);
        Assert.Equal("future", message.Fields[^1].Scalar);
        _ = Decode("TM", Serialize(message));
    }

    [Fact]
    public void UnknownTextFields_CanBeEditedThroughTheSeparateAstVisitor()
    {
        var message = Assert.IsType<JavadRtcmStatusMessage>(
            Decode("MS", Frame("MS", ",RTCMST,009,0108,0009,98.5,{future,{leaf}},@"))
        );
        var root = JavadValue.List([.. message.Fields]);
        JavadValueVisitor.VisitFields(new AstEditVisitor("leaf", "edited"), ref root);
        message.Fields = root.Items;
        Assert.Equal("edited", message.Fields[^1].Items[1].Items[0].Scalar);
        Assert.Equal(9, message.AgeSeconds);
        Assert.Equal("009", message.Fields[0].Scalar);
        _ = Decode("MS", Serialize(message));
    }

    [Fact]
    public void DynamicParameterAndTextDataVisitors_CacheShapeAndPreserveLiteralNoOpBytes()
    {
        const string golden = "TD036,CGGTTS,{FY=2005,REF=UTC(USNO),CH=816 (UNIVERSAL)},@7C";
        var message = Assert.IsType<JavadTextDataMessage>(
            Decode("TD", Encoding.ASCII.GetBytes(golden))
        );
        var metadata = message.GetVisitorType();
        var original = message.Fields;
        message.Accept(new NoOpVisitor());
        Assert.Same(metadata, message.GetVisitorType());
        Assert.True(original == message.Fields);
        Assert.Equal(golden, Encoding.ASCII.GetString(Serialize(message)));
        message.Accept(new AstEditVisitor("FY=2005", "FY=2026"));
        Assert.Equal("2026", message.CggttsHeaders["FY"]);
        Assert.NotSame(metadata, message.GetVisitorType());

        var parameters = new JavadParametersMessage { Parameters = " { a , { b , \" c \" } } " };
        var parameterBytes = Serialize(parameters);
        var parameterMetadata = parameters.GetVisitorType();
        parameters.Accept(new NoOpVisitor());
        Assert.Same(parameterMetadata, parameters.GetVisitorType());
        Assert.Equal(parameterBytes, Serialize(parameters));
        parameters.Accept(new AstEditVisitor("b", "changed"));
        Assert.True(parameters.TryGetValue(out var changed));
        Assert.Equal("changed", changed!.Items[1].Items[0].Scalar);

        parameters.Parameters = "{incomplete,";
        var partial = parameters.GetVisitorType();
        Assert.IsType<StringType>(partial["Parameters"]!.DataType);
        parameters.Accept(new NoOpVisitor());
        Assert.Equal("{incomplete,", parameters.Parameters);
        Assert.Same(partial, parameters.GetVisitorType());
    }

    [Fact]
    public void PopulatedSatelliteInspection_DoesNotAllocateOrReplaceAstAfterWarmup()
    {
        var message = Assert.IsType<JavadGpsSatelliteStatusMessage>(
            Decode(
                "GS",
                Frame(
                    "GS",
                    ",GPSVST,02,{09,08,009,{45,,38,,,42},01,extra},{12,22,183,{40,39},03},future,@"
                )
            )
        );
        var visitor = new NoOpVisitor();
        for (var index = 0; index < 100; index++)
            message.Accept(visitor);
        var original = message.Fields;
        var before = GC.GetAllocatedBytesForCurrentThread();
        for (var index = 0; index < 100; index++)
            message.Accept(visitor);
        var bytes = GC.GetAllocatedBytesForCurrentThread() - before;
        Assert.Equal(0, bytes);
        Assert.True(original == message.Fields);
    }

    [Fact]
    public void InteractiveStorage_IsUpdatedBeforeElementAndScalarCallbacksReturn()
    {
        var wrapper = new JavadWrapperMessage();
        wrapper.Accept(new DirectStorageVisitor(wrapper));
        Assert.Equal((byte)7, wrapper.SourceId);
        Assert.Equal(new byte[] { 8, 8 }, wrapper.Data);
        var eventMessage = new JavadEventMessage();
        eventMessage.Accept(new DirectStorageVisitor(eventMessage));
        Assert.Equal(9u, eventMessage.ReceiverTimeMilliseconds);
        Assert.Equal((byte)7, eventMessage.EventType);
        Assert.Equal(new byte[] { 8, 8 }, eventMessage.Data);
        var reply = new JavadReplyMessage { Text = "before" };
        reply.Accept(new DirectStorageVisitor(reply));
        Assert.Equal("after", reply.Text);
        _ = Decode(">>", Serialize(wrapper));
        _ = Decode("==", Serialize(eventMessage));
        _ = Decode("RE", Serialize(reply));
    }

    private class NoOpVisitor
        : IVisitor,
            Int32OptionalType.IVisitor,
            DoubleOptionalType.IVisitor,
            CharOptionalType.IVisitor,
            TimeSpanOptionalType.IVisitor,
            StringOptionalType.IVisitor,
            StringType.IVisitor,
            BoolType.IVisitor,
            Int32Type.IVisitor,
            UInt8Type.IVisitor,
            UInt32Type.IVisitor,
            StructType.IVisitor,
            OptionalStructType.IVisitor,
            ListType.IVisitor
    {
        protected virtual void Seen(Field field, IFieldType type) { }

        public virtual void Visit(Field field, Int32OptionalType type, ref int? value) =>
            Seen(field, type);

        public virtual void Visit(Field field, DoubleOptionalType type, ref double? value) =>
            Seen(field, type);

        public virtual void Visit(Field field, CharOptionalType type, ref char? value) =>
            Seen(field, type);

        public virtual void Visit(Field field, TimeSpanOptionalType type, ref TimeSpan? value) =>
            Seen(field, type);

        public virtual void Visit(Field field, StringOptionalType type, ref string? value) =>
            Seen(field, type);

        public virtual void Visit(Field field, StringType type, ref string value) =>
            Seen(field, type);

        public virtual void Visit(Field field, BoolType type, ref bool value) => Seen(field, type);

        public virtual void Visit(Field field, Int32Type type, ref int value) => Seen(field, type);

        public virtual void Visit(Field field, UInt8Type type, ref byte value) => Seen(field, type);

        public virtual void Visit(Field field, UInt32Type type, ref uint value) =>
            Seen(field, type);

        public virtual void BeginList(Field field, ListType type, ref uint size) =>
            Seen(field, type);

        public virtual void EndList() { }

        public virtual void BeginStruct(Field field, StructType type) => Seen(field, type);

        public virtual void EndStruct() { }

        public virtual void BeginOptionalStruct(
            Field field,
            OptionalStructType type,
            bool isPresent,
            out bool createNew
        )
        {
            Seen(field, type);
            createNew = false;
        }

        public virtual void EndOptionalStruct(bool isPresent) { }

        public void VisitUnknown(Field field, IFieldType type) =>
            throw new InvalidOperationException($"Unvisited field {field.Name}: {type.Name}");
    }

    private sealed class MetadataVisitor : NoOpVisitor
    {
        public List<(Field Field, IFieldType Type)> Fields { get; } = [];

        protected override void Seen(Field field, IFieldType type) => Fields.Add((field, type));
    }

    private sealed class FieldOrderVisitor : NoOpVisitor
    {
        private readonly Stack<Scope> _scopes = new();

        public FieldOrderVisitor(StructType root) => _scopes.Push(new Scope(root, null));

        protected override void Seen(Field field, IFieldType type)
        {
            var scope = _scopes.Peek();
            if (scope.Structure is { } structure)
            {
                Assert.True(scope.Index < structure.Fields.Length);
                var expected = structure.Fields[scope.Index++];
                Assert.Same(expected, field);
                Assert.Same(expected.DataType, type);
            }
            else
                Assert.Same(scope.ElementType, type);
        }

        public override void BeginList(Field field, ListType type, ref uint size)
        {
            Seen(field, type);
            _scopes.Push(new Scope(null, type.ElementType));
        }

        public override void EndList() => _scopes.Pop();

        public override void BeginStruct(Field field, StructType type)
        {
            Seen(field, type);
            _scopes.Push(new Scope(type, null));
        }

        public override void EndStruct() => CompleteStruct();

        public override void BeginOptionalStruct(
            Field field,
            OptionalStructType type,
            bool isPresent,
            out bool createNew
        )
        {
            Seen(field, type);
            _scopes.Push(new Scope(isPresent ? type.BaseType : null, null));
            createNew = false;
        }

        public override void EndOptionalStruct(bool isPresent) => CompleteStruct();

        private void CompleteStruct()
        {
            var scope = _scopes.Pop();
            Assert.Equal(scope.Structure?.Fields.Length ?? 0, scope.Index);
        }

        public void Complete()
        {
            Assert.Single(_scopes);
            CompleteStruct();
        }

        private sealed class Scope(StructType? structure, IFieldType? elementType)
        {
            public StructType? Structure { get; } = structure;
            public IFieldType? ElementType { get; } = elementType;
            public int Index { get; set; }
        }
    }

    private sealed class ScalarEditVisitor : NoOpVisitor
    {
        public override void Visit(Field field, CharOptionalType type, ref char? value)
        {
            if (field.Name == "SolutionValidity")
                value = 'V';
        }

        public override void Visit(Field field, TimeSpanOptionalType type, ref TimeSpan? value)
        {
            if (field.Name == "UtcTime")
                value = TimeSpan.FromHours(1);
        }

        public override void Visit(Field field, DoubleOptionalType type, ref double? value)
        {
            if (field.Name == "RollDegrees")
                value = 12.5;
            if (field.Name == "PitchDegrees")
                value = null;
        }
    }

    private sealed class SatelliteEditVisitor : NoOpVisitor
    {
        public override void Visit(Field field, Int32OptionalType type, ref int? value)
        {
            if (field.Name == "AzimuthDegrees")
                value = 77;
        }

        public override void Visit(Field field, DoubleOptionalType type, ref double? value)
        {
            if (field.Name == "SignalToNoiseDbHz")
                value += 1;
        }
    }

    private sealed class LinkEditVisitor : NoOpVisitor
    {
        public override void Visit(Field field, StringOptionalType type, ref string? value)
        {
            if (field.Name == "ReferenceStationId")
                value = "replacement";
        }
    }

    private sealed class ResizeVisitor(bool useCount) : NoOpVisitor
    {
        public override void Visit(Field field, Int32Type type, ref int value)
        {
            if (useCount && field.Name == "LinkCount")
                value = 1;
        }

        public override void BeginList(Field field, ListType type, ref uint size)
        {
            if (!useCount && field.Name == "Links")
                size = 1;
        }
    }

    private sealed class ReplaceClocksVisitor : NoOpVisitor
    {
        public override void BeginOptionalStruct(
            Field field,
            OptionalStructType type,
            bool isPresent,
            out bool createNew
        ) => createNew = field.Name == "Clocks";
    }

    private sealed class DirectStorageVisitor(JavadMessageBase message) : NoOpVisitor
    {
        public override void BeginList(Field field, ListType type, ref uint size)
        {
            if (field.Name == "Data")
                size = 2;
        }

        public override void Visit(Field field, UInt8Type type, ref byte value)
        {
            if (field.Name == "SourceId")
            {
                value = 7;
                Assert.Equal(value, ((JavadWrapperMessage)message).SourceId);
            }
            else if (field.Name == "EventType")
            {
                value = 7;
                Assert.Equal(value, ((JavadEventMessage)message).EventType);
            }
            else if (field.Name == "Data")
            {
                value = 8;
                var data = message is JavadWrapperMessage wrapper
                    ? wrapper.Data
                    : ((JavadEventMessage)message).Data;
                Assert.Equal(2, data.Length);
                Assert.Contains((byte)8, data);
            }
        }

        public override void Visit(Field field, UInt32Type type, ref uint value)
        {
            if (field.Name == "ReceiverTimeMilliseconds")
            {
                value = 9;
                Assert.Equal(value, ((JavadEventMessage)message).ReceiverTimeMilliseconds);
            }
        }

        public override void Visit(Field field, StringType type, ref string value)
        {
            if (field.Name == "Text")
            {
                value = "after";
                Assert.Equal(value, ((JavadResponseMessage)message).Text);
            }
        }
    }

    private sealed class AstEditVisitor(string before, string after) : NoOpVisitor
    {
        public override void Visit(Field field, StringOptionalType type, ref string? value)
        {
            if (field.Name == "Scalar" && value == before)
                value = after;
        }
    }

    private static IProtocolMessageFactory<JavadMessageBase, string> Factory()
    {
        var builder = new ProtocolMessageFactoryBuilder<JavadMessageBase, string>(
            JavadProtocol.Info
        );
        JavadTextMessageFactory.Register(builder);
        return builder.Build();
    }

    private static JavadMessageBase Decode(string id, byte[] frame)
    {
        var result = Factory().Create(id)!;
        ReadOnlySpan<byte> source = frame;
        result.Deserialize(ref source);
        Assert.True(source.IsEmpty);
        return result;
    }

    private static byte[] Serialize(JavadMessageBase message)
    {
        var result = new byte[message.GetByteSize()];
        var destination = result.AsSpan();
        message.Serialize(ref destination);
        Assert.True(destination.IsEmpty);
        return result;
    }

    // Independent Appendix A.1 rotate/XOR calculation for receiver-spelled text fixtures.
    private static byte[] Frame(string id, string payload)
    {
        var data = Encoding.ASCII.GetBytes(
            id + (payload.Length + 2).ToString("X3", CultureInfo.InvariantCulture) + payload
        );
        byte sum = 0;
        foreach (var value in data)
            sum = (byte)((((sum << 2) | (sum >> 6)) & 255) ^ value);
        sum = (byte)(((sum << 2) | (sum >> 6)) & 255);
        return
        [
            .. data,
            .. Encoding.ASCII.GetBytes(sum.ToString("X2", CultureInfo.InvariantCulture)),
        ];
    }
}
