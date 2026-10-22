using System;
using System.Globalization;
using System.IO;
using System.Linq;
using Asv.IO;
using Spectre.Console;
using Spectre.Console.Rendering;
using Xunit;

namespace Asv.Gnss;

public class FileInspectorViewTest
{
    [Fact]
    public void HandleKey_LeftRight_PreservesRowAndClampsPartialLastPage()
    {
        var state = new FileInspectorState(23, 10);
        for (var index = 0; index < 8; index++)
        {
            Assert.True(state.HandleKey(ConsoleKey.DownArrow));
        }

        Assert.True(state.HandleKey(ConsoleKey.RightArrow));
        Assert.Equal(18, state.SelectedIndex);
        Assert.Equal(1, state.PageIndex);
        Assert.True(state.HandleKey(ConsoleKey.RightArrow));
        Assert.Equal(22, state.SelectedIndex);
        Assert.Equal(2, state.PageIndex);
        Assert.Equal(3, state.PageCount);
        Assert.False(state.HandleKey(ConsoleKey.RightArrow));
        Assert.True(state.HandleKey(ConsoleKey.LeftArrow));
        Assert.Equal(12, state.SelectedIndex);
        Assert.Equal(1, state.PageIndex);
    }

    [Fact]
    public void HandleKey_Selection_CrossesPagesAndStopsAtFileBoundaries()
    {
        var state = new FileInspectorState(11, 10);
        Assert.False(state.HandleKey(ConsoleKey.UpArrow));
        Assert.True(state.HandleKey(ConsoleKey.End));
        Assert.Equal(10, state.SelectedIndex);
        Assert.Equal(1, state.PageIndex);
        Assert.False(state.HandleKey(ConsoleKey.DownArrow));
        Assert.True(state.HandleKey(ConsoleKey.UpArrow));
        Assert.Equal(9, state.SelectedIndex);
        Assert.Equal(0, state.PageIndex);
        Assert.True(state.HandleKey(ConsoleKey.Home));
        Assert.Equal(0, state.SelectedIndex);
        Assert.False(state.HandleKey(ConsoleKey.LeftArrow));
        Assert.False(state.HandleKey(ConsoleKey.F1));
    }

    [Fact]
    public void Resize_KeepsSelectedMessageAndRecomputesPage()
    {
        var state = new FileInspectorState(43, 10);
        state.HandleKey(ConsoleKey.End);
        state.Resize(15);
        Assert.Equal(42, state.SelectedIndex);
        Assert.Equal(2, state.PageIndex);
        Assert.Equal(3, state.PageCount);
        Assert.Equal(15, state.PageSize);
    }

    [Fact]
    public void HandleKey_DetailsPaging_ClampsAndResetsWhenSelectionChanges()
    {
        var state = new FileInspectorState(2, 10);
        state.ClampDetails(23, 10);
        Assert.True(state.HandleKey(ConsoleKey.PageDown));
        Assert.Equal(10, state.DetailsOffset);
        Assert.True(state.HandleKey(ConsoleKey.PageDown));
        Assert.Equal(13, state.DetailsOffset);
        Assert.False(state.HandleKey(ConsoleKey.PageDown));
        Assert.True(state.HandleKey(ConsoleKey.PageUp));
        Assert.Equal(3, state.DetailsOffset);
        state.ClampDetails(5, 10);
        Assert.Equal(0, state.DetailsOffset);
        state.ClampDetails(23, 10);
        state.HandleKey(ConsoleKey.PageDown);
        Assert.True(state.HandleKey(ConsoleKey.DownArrow));
        Assert.Equal(0, state.DetailsOffset);
        Assert.False(state.HandleKey(ConsoleKey.PageDown));
    }

    [Fact]
    public void Render_SelectedMessage_ShowsColumnsPagedRowsAndContentsPanel()
    {
        var capture = CreateCapture(32);
        var state = new FileInspectorState(32, 14);
        state.HandleKey(ConsoleKey.RightArrow);
        var output = Render(capture, state, "Selected object 15\nPayload: 12345", 120, 24);
        Assert.Contains("Index", output);
        Assert.Contains("Protocol", output);
        Assert.Contains("Message", output);
        Assert.Contains("Message contents", output);
        Assert.Contains("> 15", output);
        Assert.Contains("Message-015", output);
        Assert.Contains("Message-028", output);
        Assert.DoesNotContain("Message-001", output);
        Assert.DoesNotContain("Message-029", output);
        Assert.Contains("Selected object 15", output);
        Assert.Contains("Payload: 12345", output);
        Assert.Contains("Page 2 / 3", output);
    }

    [Theory]
    [InlineData(64, 14)]
    [InlineData(80, 24)]
    [InlineData(120, 36)]
    public void Render_TerminalSizes_CentersTableAndPanelWithinViewport(int width, int height)
    {
        var capture = CreateCapture(100);
        var state = new FileInspectorState(100, 14);
        var output = Render(capture, state, "Payload: 12345", width, height);
        var lines = output.TrimEnd('\r', '\n').Split('\n');
        Assert.InRange(lines.Length, 1, height);
        Assert.All(lines, line => Assert.InRange(new Segment(line.TrimEnd('\r')).CellCount(), 0, width));
        Assert.Contains("Index", output);
        Assert.Contains("Protocol", output);
        Assert.Contains("Message contents", output);
        Assert.Contains("Payload: 12345", output);
        Assert.StartsWith(" ", lines.First(line => line.Contains("GNSS FILE INSPECTOR", StringComparison.Ordinal)));
    }

    [Fact]
    public void Render_FileData_DisplaysLiteralMarkupAndRemovesControlCharacters()
    {
        var message = new DisplayMessage("[blue]", new ProtocolInfo("[red]", "test"));
        var capture = new ProtocolFileCapture("/tmp/[red]log[/]\u001b[2J", 1,
            [new ProtocolFileMessage(1, message)], ["[red]"], 0);
        var output = Render(capture, new FileInspectorState(1, 14), "[green]raw[/]\u001b[2J\0", 120, 24);
        Assert.Contains("[blue]", output);
        Assert.Contains("[red]", output);
        Assert.Contains("[green]raw[/]", output);
        Assert.Contains("/tmp/[red]log[/]", output);
        Assert.DoesNotContain('\u001b', output);
        Assert.DoesNotContain('\0', output);
    }

    [Fact]
    public void Render_WrappedContents_CanScrollToEndOfLongLine()
    {
        var capture = CreateCapture(1);
        var state = new FileInspectorState(1, 14);
        var details = new string('x', 2000) + "TAIL-MARKER";
        var firstPage = Render(capture, state, details, 80, 24);
        Assert.DoesNotContain("TAIL-MARKER", firstPage);
        while (state.HandleKey(ConsoleKey.PageDown))
        {
        }

        var lastPage = Render(capture, state, details, 80, 24);
        Assert.Contains("TAIL-MARKER", lastPage.Replace("\r", string.Empty).Replace("\n", string.Empty)
            .Replace(" ", string.Empty).Replace("│", string.Empty));
        Assert.True(state.DetailsOffset > 0);
    }

    [Fact]
    public void Render_EmptyCapture_ShowsEmptyStateAndNavigationIsSafe()
    {
        var capture = CreateCapture(0);
        var state = new FileInspectorState(0, 14);
        Assert.Equal(-1, state.SelectedIndex);
        Assert.False(state.HandleKey(ConsoleKey.LeftArrow));
        Assert.False(state.HandleKey(ConsoleKey.RightArrow));
        Assert.False(state.HandleKey(ConsoleKey.End));
        var output = Render(capture, state, string.Empty, 120, 24);
        Assert.Contains("No messages", output);
        Assert.Contains("No message selected.", output);
        Assert.Contains("Page 0 / 0", output);
    }

    [Fact]
    public void Render_TinyTerminal_ShowsResizeHint()
    {
        var output = Render(CreateCapture(2), new FileInspectorState(2, 10), "Payload", 40, 10);
        Assert.Contains("2 messages", output);
        Assert.Contains("64 x 14", output);
    }

    private static ProtocolFileCapture CreateCapture(int count) => new("/tmp/recording.ubx", 2048,
        Enumerable.Range(1, count).Select(index => new ProtocolFileMessage(index,
            new DisplayMessage($"Message-{index:000}", UbxProtocol.Info))).ToArray(),
        [UbxProtocol.Info.Id], 0);

    private static string Render(ProtocolFileCapture capture, FileInspectorState state, string details,
        int width, int height)
    {
        using var writer = new StringWriter(CultureInfo.InvariantCulture);
        var console = AnsiConsole.Create(new AnsiConsoleSettings
        {
            Ansi = AnsiSupport.No,
            ColorSystem = ColorSystemSupport.NoColors,
            Interactive = InteractionSupport.No,
            // CI profile enrichment can override AnsiSupport.No (for example on GitHub Actions).
            Enrichment = new ProfileEnrichment { UseDefaultEnrichers = false },
            Out = new AnsiConsoleOutput(writer),
        });
        console.Profile.Width = width;
        console.Profile.Height = height;
        console.Write(FileInspectorView.Render(capture, state, details, width, height));
        return writer.ToString();
    }

    private sealed class DisplayMessage(string name, ProtocolInfo protocol) : IProtocolMessage
    {
        private ProtocolTags _tags = [];
        public ProtocolInfo Protocol => protocol;
        public string Name => name;
        public ref ProtocolTags Tags => ref _tags;
        public string GetIdAsString() => "1";
        public int GetByteSize() => 0;
        public void Serialize(ref Span<byte> buffer) => throw new NotSupportedException();
        public void Deserialize(ref ReadOnlySpan<byte> buffer) => throw new NotSupportedException();
    }
}
