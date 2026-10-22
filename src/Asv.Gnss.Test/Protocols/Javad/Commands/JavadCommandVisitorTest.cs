using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Asv.IO;
using Xunit;

namespace Asv.Gnss;

public class JavadCommandVisitorTest
{
    [Fact]
    public void Command_EditLabelAndChecksum_MatchesIndependentChecksumVector()
    {
        var command = JavadCommand.Parse("print,/par/rcv/id");
        var tree = command.CommandValue;
        var visitor = new EditingVisitor(command.VisitorType);
        visitor.Strings["Label"] = "probe";
        visitor.Booleans["HasChecksum"] = true;

        command.Accept(visitor);

        Assert.Equal("%probe%print,/par/rcv/id@62\r\n", Serialize(command));
        Assert.Same(tree, command.CommandValue);
        Assert.True(JavadCommand.Parse(command.ToString()).HasChecksum);
    }

    [Fact]
    public void Command_EditNestedArgumentAndOptions_RebuildsValidatedTreeAndChecksum()
    {
        var command = JavadCommand
            .Parse("{em,,jps/{RT,RD}}:{1,0,0}")
            .WithLabel("schedule")
            .WithChecksum();
        var oldTree = command.CommandValue;
        var visitor = new EditingVisitor(command.VisitorType);
        visitor.Strings["CommandValue.Items.2.Items.1.Scalar"] = "SI";
        visitor.Strings["CommandValue.Options.Items.0.Scalar"] = "2";

        command.Accept(visitor);

        Assert.Equal("%schedule%{em,,jps/{RT,SI}}:{2,0,0}", command.WithChecksum(false).ToString());
        Assert.NotSame(oldTree, command.CommandValue);
        Assert.Same(oldTree.Items[0], command.CommandValue.Items[0]);
        Assert.Same(oldTree.Items[2].Items[0], command.CommandValue.Items[2].Items[0]);
        Assert.Equal("RD", oldTree.Items[2].Items[1].Scalar);
        Assert.True(JavadCommand.Parse(command.ToString()).HasChecksum);
    }

    [Fact]
    public void Command_NoOp_PreservesExactWireAndCachedMetadata()
    {
        const string body = "%probe%print , /par/rcv/id @";
        var wire =
            body
            + JavadChecksum
                .Compute8(Encoding.ASCII.GetBytes(body))
                .ToString("x2", CultureInfo.InvariantCulture);
        var command = JavadCommand.Parse(wire);
        var tree = command.CommandValue;
        var arguments = command.Arguments;
        var type = command.VisitorType;

        command.Accept(new EditingVisitor(type));

        Assert.Equal(wire + "\r\n", Serialize(command));
        Assert.Same(tree, command.CommandValue);
        Assert.Equal(arguments, command.Arguments);
        Assert.Same(type, command.VisitorType);
        Assert.Same(JavadValueVisitor.GetType(tree), JavadValueVisitor.GetType(tree));
    }

    [Fact]
    public void Line_EditStatementAndCondition_PreservesOtherStatementsAndComment()
    {
        var line = JavadCommandLine.Parse("%a%event,\"start\" && print,rcv/id # keep this comment");
        var first = line.Statements[0];
        var second = line.Statements[1];
        for (var index = 0; index < 10; index++)
            first.Tags.Add("tag" + index, index);
        var visitor = new EditingVisitor(line.VisitorType);
        visitor.Strings["Statements.0.CommandValue.Items.1.Scalar"] = "changed;#||";
        visitor.Integers["Separators.0"] = (int)JavadStatementSeparator.OnFailure;

        line.Accept(visitor);

        Assert.Equal(
            "%a%{event,\"changed;#||\"}||print,rcv/id # keep this comment",
            line.ToString()
        );
        Assert.Equal("start", first.Arguments[0].Scalar);
        Assert.NotSame(first, line.Statements[0]);
        Assert.Same(second, line.Statements[1]);
        Assert.Equal(10, line.Statements[0].Tags.Count);
        line.Statements[0].Tags["tag9"] = "edited";
        Assert.Equal(9, first.Tags["tag9"]);
        var parsed = JavadCommandLine.Parse(line.ToString());
        Assert.Equal(2, parsed.Statements.Length);
        Assert.Equal(JavadStatementSeparator.OnFailure, parsed.Separators[0]);
    }

    [Fact]
    public void Line_NoOp_PreservesWhitespaceCommentsAndAllIdentities()
    {
        const string wire = "  %a%event,\"a;#&&||\"  && print , rcv/id || dm  # comment";
        var line = JavadCommandLine.Parse(wire);
        var statements = line.Statements;
        var separators = line.Separators;
        var type = line.VisitorType;

        line.Accept(new EditingVisitor(type));

        Assert.Equal(wire + "\r\n", Serialize(line));
        Assert.Equal(statements, line.Statements);
        Assert.Equal(separators, line.Separators);
        Assert.Same(type, line.VisitorType);
        for (var index = 0; index < statements.Length; index++)
            Assert.Same(statements[index], line.Statements[index]);
    }

    [Fact]
    public void Line_ExternallyVisitedSharedStatement_RefreshesEveryLineBeforeSerialization()
    {
        var line = JavadCommandLine.Parse("print,rcv/id  # retained");
        var command = line.Statements[0];
        var shared = new JavadCommandLine(command).Then(JavadCommand.DisableMessages());
        var visitor = new EditingVisitor(command.VisitorType);
        visitor.Strings["Label"] = "updated";

        command.Accept(visitor);

        Assert.Equal("%updated%print,rcv/id # retained\r\n", Serialize(line));
        Assert.Equal("%updated%print,rcv/id;dm\r\n", Serialize(shared));
        Assert.Equal(line.ToString().Length + 2, line.GetByteSize());
        Assert.Same(command, line.Statements[0]);
        Assert.Same(command, shared.Statements[0]);
    }

    [Theory]
    [InlineData("CommandValue.Items.0.Scalar", "unknown")]
    [InlineData("CommandValue.Items.1.Scalar", "rcv/id;dm")]
    [InlineData("Label", "bad%label")]
    public void Command_InvalidEdits_LeaveOriginalWireAndTreeUnchanged(string path, string edit)
    {
        var command = JavadCommand.Parse("%id%print,rcv/id");
        var original = command.CommandValue;
        var visitor = new EditingVisitor(command.VisitorType);
        visitor.Strings[path] = edit;

        Assert.ThrowsAny<ArgumentException>(() => command.Accept(visitor));

        Assert.Equal("%id%print,rcv/id", command.ToString());
        Assert.Same(original, command.CommandValue);
    }

    [Fact]
    public void Line_InvalidCondition_DoesNotPublishEarlierStatementEdits()
    {
        var line = JavadCommandLine.Parse("event,\"before\"&&dm");
        var original = line.Statements;
        var visitor = new EditingVisitor(line.VisitorType);
        visitor.Strings["Statements.0.CommandValue.Items.1.Scalar"] = "after";
        visitor.Integers["Separators.0"] = 3;

        Assert.Throws<ArgumentOutOfRangeException>(() => line.Accept(visitor));

        Assert.Equal("event,\"before\"&&dm", line.ToString());
        Assert.Equal(original, line.Statements);
        Assert.Equal("before", line.Statements[0].Arguments[0].Scalar);
    }

    [Fact]
    public void Value_VisitsHeterogeneousFutureFieldsAndPreservesReceiverGrammar()
    {
        var original = JavadValue
            .List(
                JavadValue.Omitted,
                JavadValue.List(JavadValue.TextAtom("N55o12'34.5\""), JavadValue.String("one"))
            )
            .WithName("future");
        var value = original;
        var visitor = new EditingVisitor(JavadValueVisitor.GetType(value));
        visitor.Strings["Items.1.Items.0.Scalar"] = "UTC=01:02:03;@%";
        visitor.Strings["Items.1.Items.1.Scalar"] = "two";
        visitor.Strings["Name"] = null;

        JavadValueVisitor.VisitFields(visitor, ref value);

        Assert.Null(value.Name);
        Assert.Same(original.Items[0], value.Items[0]);
        Assert.True(value.Items[1].Items[0].IsReceiverText);
        Assert.Equal("UTC=01:02:03;@%", value.Items[1].Items[0].Scalar);
        Assert.Equal("two", value.Items[1].Items[1].Scalar);
        Assert.Equal("one", original.Items[1].Items[1].Scalar);
    }

    [Fact]
    public void Value_NoOp_PreservesOmittedAndAbsentOptions()
    {
        var value = JavadValue.List(JavadValue.Omitted, JavadValue.Atom("future"));
        var original = value;
        var visitor = new EditingVisitor(JavadValueVisitor.GetType(value));

        JavadValueVisitor.VisitFields(visitor, ref value);

        Assert.Same(original, value);
        Assert.Same(JavadValue.Omitted, value.Items[0]);
        Assert.Null(value.Options);
        Assert.Contains("Items.0.Scalar", visitor.Seen);
        Assert.Contains("Items.0.Options", visitor.Seen);
    }

    [Theory]
    [InlineData("IsList")]
    [InlineData("IsReceiverText")]
    public void Value_ReadOnlyDiscriminatorEdit_RejectsWithoutReplacement(string path)
    {
        var original = JavadValue.Atom("unchanged");
        var value = original;
        var visitor = new EditingVisitor(JavadValueVisitor.GetType(value));
        visitor.Booleans[path] = true;

        Assert.Throws<InvalidOperationException>(() =>
            JavadValueVisitor.VisitFields(visitor, ref value)
        );

        Assert.Same(original, value);
    }

    [Fact]
    public void Accept_NullVisitorOrValue_ThrowsBeforeMutation()
    {
        var command = JavadCommand.Parse("dm");
        var line = new JavadCommandLine(command);
        var value = JavadValue.Omitted;
        Assert.Throws<ArgumentNullException>(() => command.Accept(null!));
        Assert.Throws<ArgumentNullException>(() => line.Accept(null!));
        Assert.Throws<ArgumentNullException>(() => JavadValueVisitor.VisitFields(null!, ref value));
        Assert.Throws<ArgumentNullException>(() => JavadValueVisitor.GetType(null!));
        Assert.Equal("dm", command.ToString());
        Assert.Same(command, line.Statements[0]);
    }

    [Fact]
    public void Accept_WarmedInspection_DoesNotAllocate()
    {
        var line = JavadCommandLine.Parse("%id%{em,,jps/{RT,RD}}:{1,0,0}&&dm # retain");
        var visitor = new NoOpVisitor();
        for (var index = 0; index < 1000; index++)
            line.Accept(visitor);
        var before = GC.GetAllocatedBytesForCurrentThread();

        for (var index = 0; index < 100; index++)
            line.Accept(visitor);

        Assert.Equal(0, GC.GetAllocatedBytesForCurrentThread() - before);
    }

    private static string Serialize(IProtocolMessage message)
    {
        var bytes = new byte[message.GetByteSize()];
        Span<byte> target = bytes;
        message.Serialize(ref target);
        Assert.Empty(target.ToArray());
        return Encoding.ASCII.GetString(bytes);
    }

    private sealed class EditingVisitor(StructType root)
        : StructType.IVisitor,
            OptionalStructType.IVisitor,
            StringOptionalType.IVisitor,
            StringType.IVisitor,
            BoolType.IVisitor,
            Int32Type.IVisitor
    {
        private readonly List<string> _path = [];
        private readonly Stack<StructType> _types = new([root]);
        public Dictionary<string, string?> Strings { get; } = [];
        public Dictionary<string, bool> Booleans { get; } = [];
        public Dictionary<string, int> Integers { get; } = [];
        public HashSet<string> Seen { get; } = [];

        private string Check(Field field, IFieldType type)
        {
            Assert.Same(field, _types.Peek()[field.Name]);
            Assert.Same(field.DataType, type);
            var path = _path.Count == 0 ? field.Name : string.Join('.', _path) + "." + field.Name;
            Seen.Add(path);
            return path;
        }

        public void VisitUnknown(Field field, IFieldType type) => Check(field, type);

        public void BeginStruct(Field field, StructType type)
        {
            Check(field, type);
            _path.Add(field.Name);
            _types.Push(type);
        }

        public void EndStruct()
        {
            _path.RemoveAt(_path.Count - 1);
            _types.Pop();
        }

        public void BeginOptionalStruct(
            Field field,
            OptionalStructType type,
            bool isPresent,
            out bool createNew
        )
        {
            Check(field, type);
            _path.Add(field.Name);
            _types.Push(type.BaseType);
            createNew = false;
        }

        public void EndOptionalStruct(bool isPresent) => EndStruct();

        public void Visit(Field field, StringOptionalType type, ref string? value)
        {
            if (Strings.TryGetValue(Check(field, type), out var edit))
                value = edit;
        }

        public void Visit(Field field, StringType type, ref string value)
        {
            if (Strings.TryGetValue(Check(field, type), out var edit))
                value = edit!;
        }

        public void Visit(Field field, BoolType type, ref bool value)
        {
            if (Booleans.TryGetValue(Check(field, type), out var edit))
                value = edit;
        }

        public void Visit(Field field, Int32Type type, ref int value)
        {
            if (Integers.TryGetValue(Check(field, type), out var edit))
                value = edit;
        }
    }

    private sealed class NoOpVisitor
        : StructType.IVisitor,
            OptionalStructType.IVisitor,
            StringOptionalType.IVisitor,
            StringType.IVisitor,
            BoolType.IVisitor,
            Int32Type.IVisitor
    {
        public void VisitUnknown(Field field, IFieldType type) { }

        public void BeginStruct(Field field, StructType type) { }

        public void EndStruct() { }

        public void BeginOptionalStruct(
            Field field,
            OptionalStructType type,
            bool isPresent,
            out bool createNew
        ) => createNew = false;

        public void EndOptionalStruct(bool isPresent) { }

        public void Visit(Field field, StringOptionalType type, ref string? value) { }

        public void Visit(Field field, StringType type, ref string value) { }

        public void Visit(Field field, BoolType type, ref bool value) { }

        public void Visit(Field field, Int32Type type, ref int value) { }
    }
}
