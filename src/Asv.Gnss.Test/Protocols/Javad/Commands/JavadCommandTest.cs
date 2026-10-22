using System;
using System.Globalization;
using System.Linq;
using System.Text;
using Asv.IO;
using Xunit;

namespace Asv.Gnss;

public class JavadCommandTest
{
    [Theory]
    [InlineData("set,/par/dev/ser/c/rate,115200", "set")]
    [InlineData("print,/par/raw/curmsint:on", "print")]
    [InlineData("list", "list")]
    [InlineData("em,,nmea/GGA:{,,1}", "em")]
    [InlineData("out,,nmea/GGA", "out")]
    [InlineData("dm,,nmea/{GGA,ZDA}", "dm")]
    [InlineData("init,/par/", "init")]
    [InlineData("create,:a", "create")]
    [InlineData("remove,/log/", "remove")]
    [InlineData("event,\"EVENT{DATA,SENT}\"", "event")]
    [InlineData("get,NAME:{,190,,,-257}", "get")]
    [InlineData("put,/log/NAME:{,4096},3870034", "put")]
    [InlineData("fld,8PZFM10IL8G,/dev/usb/a:{20,16384}", "fld")]
    public void ManualCommandVectors_PreserveAllCommandSpellings(string text, string name)
    {
        var command = JavadCommand.Parse(text);
        Assert.Equal(name, command.Id);
        Assert.Equal(text + "\r\n", Serialize(command));
        Assert.Contains(JavadCommandCatalog.All, x => x.Name == name);
    }

    [Fact]
    public void Factories_EncodeControlAndParameterOperations()
    {
        var path = new JavadObjectPath("/par/dev/ser/a/rate");
        Assert.Equal(
            "set,/par/dev/ser/a/rate,9600",
            JavadCommand.Set(path, JavadValue.Integer(9600)).ToString()
        );
        Assert.Equal("print,/par/dev/ser/a/rate:on", JavadCommand.Print(path, true).ToString());
        Assert.Equal("list", JavadCommand.List().ToString());
        Assert.Equal("list,/par/dev/ser/a/rate", JavadCommand.List(path).ToString());
        Assert.Equal(
            "em,,nmea/GGA",
            JavadCommand.EnableMessages(JavadValue.Atom("nmea/GGA")).ToString()
        );
        Assert.Equal(
            "out,,nmea/GGA",
            JavadCommand.OutputMessages(JavadValue.Atom("nmea/GGA")).ToString()
        );
        Assert.Equal("dm", JavadCommand.DisableMessages().ToString());
        Assert.Equal("dm,/dev/ser/a", JavadCommand.DisableMessages(new("/dev/ser/a")).ToString());
        Assert.Equal("init,/par/", JavadCommand.Initialize(new("/par/")).ToString());
        Assert.Equal("create,:b", JavadCommand.Create(log: "b").ToString());
        Assert.Equal("remove,/log/NAME", JavadCommand.Remove(new("/log/NAME")).ToString());
        Assert.Equal(
            "get,/log/NAME:{,4096},3870034",
            JavadCommand.Get(new("/log/NAME"), 3870034, JavadValue.Parse("{,4096}")).ToString()
        );
        Assert.Equal("put,/log/NAME", JavadCommand.Put(new("/log/NAME")).ToString());
        Assert.Equal(
            "fld,\"8PZFM10IL8G\",/dev/usb/a:{20,16384}",
            JavadCommand
                .FirmwareLoad("8PZFM10IL8G", new("/dev/usb/a"), JavadValue.Parse("{20,16384}"))
                .ToString()
        );
    }

    [Fact]
    public void LabelsAndChecksum_MatchIndependentAppendixAlgorithmVector()
    {
        var command = JavadCommand.Print(new("/par/rcv/id")).WithLabel("probe").WithChecksum();
        Assert.Equal("%probe%print,/par/rcv/id@62", command.ToString());
        var parsed = JavadCommand.Parse(command.ToString());
        Assert.Equal("probe", parsed.Label);
        Assert.True(parsed.HasChecksum);
        Assert.Equal("%probe%print,/par/rcv/id", parsed.WithChecksum(false).ToString());
        Assert.Throws<FormatException>(() => JavadCommand.Parse("%probe%print,/par/rcv/id@00"));
        Assert.Throws<FormatException>(() => JavadCommand.Parse("print,/par/rcv/id@XYZ"));
    }

    [Fact]
    public void ExplicitCommandListOptions_ArePreservedWhenAddingLabel()
    {
        var command = JavadCommand.Parse("{em,,jps/{RT,RD}}:{1,0,0}").WithLabel("schedule");
        Assert.Equal("%schedule%{em,,jps/{RT,RD}}:{1,0,0}", command.ToString());
        Assert.Equal("{1,0,0}", command.CommandValue.Options!.ToString());
    }

    [Fact]
    public void Deserialize_ConsumesOnlyOneCompleteStatement()
    {
        var command = new JavadCommand();
        ReadOnlySpan<byte> source = "%id%print,/par/rcv/id\r\nnext"u8;
        command.Deserialize(ref source);
        Assert.Equal("id", command.Label);
        Assert.Equal("next", Encoding.ASCII.GetString(source));
        Assert.Equal("%id%print,/par/rcv/id\r\n", Serialize(command));
    }

    [Fact]
    public void Synchronization_UsesEmptyLabelAndCorrelatesExplicitReplies()
    {
        var command = JavadCommand.Synchronize();
        Assert.Equal("%%", command.ToString());
        Assert.True(JavadCommandResponse.Parse("RE", "%%").Matches(command));
        Assert.False(JavadCommandResponse.Parse("RE", "").Matches(command));
        Assert.False(
            JavadCommandResponse.Parse("RE", "42").Matches(JavadCommand.Print(new("/par/rcv/id")))
        );
    }

    [Fact]
    public void Responses_PreservePartialListsAndDecodeErrorCode()
    {
        var first = JavadCommandResponse.Parse("RE", "%query%{main=\"2.5 Sep,13,2006 p2\",boot=0,");
        Assert.Equal("query", first.Label);
        Assert.False(first.TryGetValue(out _));
        var last = JavadCommandResponse.Parse("RE", "%query%hw=71}");
        var value = JavadValue.Parse(first.Text + last.Text);
        Assert.Equal(3, value.Items.Length);
        Assert.Equal("main", value.Items[0].Name);
        Assert.Equal("2.5 Sep,13,2006 p2", value.Items[0].Scalar);
        var error = JavadCommandResponse.Parse("ER", "%set%{4,value out of range}");
        Assert.True(error.IsError);
        Assert.Equal(4, error.ErrorCode);
        Assert.True(
            error.Matches(
                JavadCommand
                    .Set(new("/par/dev/ser/a/rate"), JavadValue.Integer(1000000))
                    .WithLabel("set")
            )
        );
        Assert.Equal("  first bytes", JavadCommandResponse.Parse("RE", "  first bytes").Text);
        Assert.Equal("  first bytes", JavadCommandResponse.Parse("RE", "%file%  first bytes").Text);
    }

    [Fact]
    public void Line_RespectsConditionalSeparatorsAndQuotedCommentCharacters()
    {
        const string text = "%a%event,\"a;#&&||\"&&print,rcv/id||event,failed;dm # comment";
        var line = JavadCommandLine.Parse(text);
        Assert.Equal(4, line.Statements.Length);
        Assert.Equal(
            new[]
            {
                JavadStatementSeparator.OnSuccess,
                JavadStatementSeparator.OnFailure,
                JavadStatementSeparator.Always,
            },
            line.Separators
        );
        Assert.Equal("a;#&&||", line.Statements[0].Arguments[0].Scalar);
        Assert.Equal(text + "\r\n", Serialize(line));
        Assert.Equal(
            "print,rcv/id&&dm",
            new JavadCommandLine(JavadCommand.Print(new("rcv/id")))
                .Then(JavadCommand.DisableMessages(), JavadStatementSeparator.OnSuccess)
                .ToString()
        );
        Assert.Equal(
            "print,rcv/id;dm",
            JavadCommandLine
                .Parse("print,rcv/id # a comment")
                .Then(JavadCommand.DisableMessages())
                .ToString()
        );
    }

    [Theory]
    [InlineData("hello\rset,/par/power,off")]
    [InlineData("hello\nset,/par/power,off")]
    [InlineData("hello\0")]
    [InlineData("Привет")]
    public void Strings_RejectNonAsciiAndLineInjection(string text) =>
        Assert.Throws<ArgumentException>(() => JavadCommand.Event(text));

    [Theory]
    [InlineData("bad%label")]
    [InlineData("bad;set")]
    [InlineData("bad#label")]
    [InlineData("bad&&label")]
    public void Labels_RejectStatementInjection(string label) =>
        Assert.Throws<ArgumentException>(() => JavadCommand.Synchronize(label));

    [Fact]
    public void QuotedStrings_EscapeGrammarAndRejectOpaqueReceivedTextInCommands()
    {
        var command = JavadCommand.Event("quote\" and slash\\;#%@");
        Assert.Equal("event,\"quote\\\" and slash\\\\;#%@\"", command.ToString());
        Assert.Equal(
            "quote\" and slash\\;#%@",
            JavadCommand.Parse(command.ToString()).Arguments[0].Scalar
        );
        Assert.Throws<ArgumentException>(() =>
            new JavadCommand("event", JavadValue.TextAtom("a;set,/par/power,off"))
        );
        Assert.Throws<ArgumentException>(() =>
            new JavadCommand("set", JavadValue.List(JavadValue.TextAtom("a;dm")))
        );
        Assert.Throws<ArgumentOutOfRangeException>(() => JavadCommand.Event(new string('a', 64)));
    }

    private static string Serialize(IProtocolMessage message)
    {
        var bytes = new byte[message.GetByteSize() + 2];
        bytes[^2] = 0xAB;
        bytes[^1] = 0xCD;
        Span<byte> remaining = bytes;
        message.Serialize(ref remaining);
        Assert.Equal(new byte[] { 0xAB, 0xCD }, remaining.ToArray());
        return Encoding.ASCII.GetString(bytes.AsSpan(0, bytes.Length - 2));
    }
}
