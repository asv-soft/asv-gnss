using System;
using Asv.IO;
using Xunit;

namespace Asv.Gnss;

public class MessageDetailFormatterTest
{
    [Fact]
    public void Format_UbxMessage_KeepsTypedPayloadAndExcludesRefTags()
    {
        var message = new UbxNavPvt { Year = 0, Latitude = 12.5, Longitude = -23.75 };

        var text = MessageDetailFormatter.Format(message);

        Assert.Contains("\"Latitude\": 12.5", text);
        Assert.Contains("\"Longitude\": -23.75", text);
        Assert.Contains("\"Year\": 0", text);
        Assert.Contains("\"UtcTime\"", text);
        Assert.DoesNotContain("Unable to format message", text);
        Assert.DoesNotContain("\"Tags\"", text);
        Assert.Equal(0, message.Year);
        Assert.Equal(12.5, message.Latitude);
    }

    [Fact]
    public void Format_ThrowingComputedProperty_KeepsOtherFieldsAndReportsUnreadableField()
    {
        var text = MessageDetailFormatter.Format(new GetterFailureMessage { Payload = 42 });

        Assert.Contains("\"Payload\": 42", text);
        Assert.Contains("Unavailable fields", text);
        Assert.Contains("Unreadable", text);
        Assert.DoesNotContain("Unable to format message", text);
    }

    [Fact]
    public void Format_NonVisitableMessage_IncludesNestedCorrectionsWithoutMutatingThem()
    {
        var item = new DObservationItem(NavigationSystemEnum.SYS_GPS) { Prn = 24, Prc = -7.46, Iod = 42 };
        var message = new RtcmV2Message1 { ObservationItems = [item] };

        var text = MessageDetailFormatter.Format(message);

        Assert.Contains("\"Prn\": 24", text);
        Assert.Contains("\"Prc\": -7.46", text);
        Assert.Contains("\"Iod\": 42", text);
        Assert.DoesNotContain("Unable to format message", text);
        Assert.Same(item, Assert.Single(message.ObservationItems));
        Assert.Equal(-7.46, item.Prc);
    }

    [Fact]
    public void Format_EnumerableAsterixMessage_IncludesRecordFields()
    {
        var message = new AsterixMessageI020
        {
            new AsterixRecordI020
            {
                TrackNumber = new AsterixFieldI020Frn006Type161 { TrackNumber = 1234 }
            }
        };

        var text = MessageDetailFormatter.Format(message);

        Assert.Contains("1234", text);
        Assert.DoesNotContain("Unable to format message", text);
        Assert.DoesNotContain("Unavailable fields", text);
    }

    [Fact]
    public void Format_LargePayload_StopsAtDisplayLimitAndReportsTruncation()
    {
        var message = new DetailMessage { Payload = new string('X', MessageDetailFormatter.MaximumCharacters * 4) };

        var text = MessageDetailFormatter.Format(message);

        Assert.Contains("Content truncated for display", text);
        Assert.Contains("\"Payload\"", text);
        Assert.InRange(text.Length, MessageDetailFormatter.MaximumCharacters,
            MessageDetailFormatter.MaximumCharacters + 100);
    }

    [Fact]
    public void Format_LargeCollectionOfEscapedStrings_StopsWithoutLosingLimitDiagnostic()
    {
        var values = new string[10000];
        Array.Fill(values, "\\\"\n[red]\u001b");

        var text = MessageDetailFormatter.Format(new DetailMessage { Payload = values });

        Assert.Contains("Content truncated for display", text);
        Assert.Contains("\\u001b", text);
        Assert.DoesNotContain('\u001b', text);
        Assert.InRange(text.Length, MessageDetailFormatter.MaximumCharacters,
            MessageDetailFormatter.MaximumCharacters + 100);
        Assert.Equal(10000, values.Length);
    }

    [Fact]
    public void Format_CyclicOrUnboundedObjectGraph_RemainsBounded()
    {
        var cyclic = new CyclicNode();
        cyclic.Next = cyclic;

        var text = MessageDetailFormatter.Format(new DetailMessage { Payload = cyclic });
        var nested = MessageDetailFormatter.Format(new DetailMessage { Payload = new UnboundedNode() });

        Assert.Contains("\"Value\": 42", text);
        Assert.DoesNotContain("Unable to format message", text);
        Assert.Contains("Content truncated for display", nested);
        Assert.InRange(nested.Length, 1, MessageDetailFormatter.MaximumCharacters + 100);
        Assert.Same(cyclic, cyclic.Next);
    }

    [Fact]
    public void Format_TerminalControlCharacters_UsesLiteralJsonEscapes()
    {
        var text = MessageDetailFormatter.Format(new DetailMessage { Payload = "[red]value[/]\u001b[2J\u0007" });

        Assert.Contains("[red]value[/]", text);
        Assert.Contains("\\u001b[2J\\u0007", text);
        Assert.DoesNotContain('\u001b', text);
        Assert.DoesNotContain('\u0007', text);
    }

    private class DetailMessage : IProtocolMessage
    {
        private ProtocolTags _tags = [];
        public object? Payload { get; init; }
        public ProtocolInfo Protocol => UbxProtocol.Info;
        public string Name => "Detail test";
        public ref ProtocolTags Tags => ref _tags;
        public string GetIdAsString() => "1";
        public void Serialize(ref Span<byte> buffer) => throw new NotSupportedException();
        public void Deserialize(ref ReadOnlySpan<byte> buffer) => throw new NotSupportedException();
        public int GetByteSize() => throw new NotSupportedException();
    }

    private sealed class GetterFailureMessage : DetailMessage
    {
        public string Unreadable => throw new InvalidOperationException("Example getter failure.");
    }

    private sealed class CyclicNode
    {
        public int Value => 42;
        public CyclicNode? Next { get; set; }
    }

    private sealed class UnboundedNode
    {
        public UnboundedNode Next => new();
    }
}
