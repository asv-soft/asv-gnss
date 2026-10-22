using System.Globalization;
using System.Text;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace Asv.Gnss;

internal static class FileInspectorView
{
    public static IRenderable Render(ProtocolFileCapture capture, FileInspectorState state, string details,
        int width, int height)
    {
        if (width < 64 || height < 14)
        {
            return new Text($"File inspector: {capture.Messages.Count:N0} messages. Enlarge the terminal to at least 64 x 14.");
        }

        var applicationWidth = Math.Min(width - 4, 156);
        var pageSize = Math.Clamp(height - 10, 1, 24);
        state.Resize(pageSize);
        var tableWidth = Math.Max(30, applicationWidth * 45 / 100);
        var detailsWidth = applicationWidth - tableWidth - 1;
        var bodyHeight = pageSize + 4;
        var visibleDetails = bodyHeight - 2;
        var detailLines = WrapLines(state.SelectedIndex < 0 ? "No message selected." : details,
            detailsWidth - 4);
        state.ClampDetails(detailLines.Count, visibleDetails);

        var table = CreateTable(capture, state, tableWidth);
        var content = string.Join('\n', detailLines.Skip(state.DetailsOffset).Take(visibleDetails));
        var detailPanel = new Panel(new Text(content) { Overflow = Overflow.Crop })
        {
            Header = new PanelHeader("Message contents"),
            Border = BoxBorder.Rounded,
            BorderStyle = new Style(Color.DodgerBlue1),
            Padding = new Padding(1, 0),
            Width = detailsWidth,
            Height = bodyHeight,
            Expand = true,
        };
        var body = new Grid { Width = applicationWidth };
        body.AddColumn(new GridColumn { Width = tableWidth, Padding = new Padding(0) });
        body.AddColumn(new GridColumn { Width = 1, Padding = new Padding(0) });
        body.AddColumn(new GridColumn { Width = detailsWidth, Padding = new Padding(0) });
        body.AddRow(table, Text.Empty, detailPanel);

        var page = state.PageCount == 0 ? "Page 0 / 0" : $"Page {state.PageIndex + 1} / {state.PageCount}";
        var summary = $"{page}  |  {capture.Messages.Count:N0} messages  |  {capture.ByteCount:N0} bytes  |  {capture.ErrorCount:N0} parser errors";
        var rows = new Rows(
            new Text("GNSS FILE INSPECTOR", new Style(Color.DodgerBlue1, decoration: Decoration.Bold)),
            new Text(Safe(capture.FilePath), new Style(Color.Grey)) { Overflow = Overflow.Ellipsis },
            body,
            new Text(summary, new Style(Color.Grey)) { Overflow = Overflow.Ellipsis },
            new Text("←/→ page  ↑/↓ select  PgUp/PgDn contents  Home/End  Q/Esc exit",
                new Style(Color.Grey)) { Overflow = Overflow.Ellipsis });
        return Align.Center(Align.Left(rows).Width(applicationWidth), VerticalAlignment.Middle)
            .Width(width).Height(height);
    }

    private static Table CreateTable(ProtocolFileCapture capture, FileInspectorState state, int width)
    {
        var table = new Table
        {
            Border = TableBorder.Rounded,
            BorderStyle = new Style(Color.Grey),
            Width = width,
            Expand = true,
        };
        var indexWidth = Math.Clamp(capture.Messages.Count.ToString(CultureInfo.InvariantCulture).Length + 2,
            5, Math.Max(5, width - 25));
        const int protocolWidth = 8;
        var nameWidth = Math.Max(7, width - indexWidth - protocolWidth - 10);
        table.AddColumn(new TableColumn("Index") { Width = indexWidth, NoWrap = true });
        table.AddColumn(new TableColumn("Protocol") { Width = protocolWidth, NoWrap = true });
        table.AddColumn(new TableColumn("Message") { Width = nameWidth, NoWrap = true });

        var start = state.PageIndex * state.PageSize;
        var end = Math.Min(start + state.PageSize, capture.Messages.Count);
        for (var index = start; index < end; index++)
        {
            var entry = capture.Messages[index];
            var selected = index == state.SelectedIndex;
            var style = selected
                ? new Style(Color.White, new Color(48, 48, 96), Decoration.Bold)
                : Style.Plain;
            table.AddRow(
                Cell($"{(selected ? '>' : ' ')} {entry.Index}", style),
                Cell(entry.Message.Protocol.Id, style),
                Cell(entry.Message.Name, style));
        }

        if (capture.Messages.Count == 0)
        {
            table.AddRow(Text.Empty, Text.Empty, new Text("No messages"));
        }

        return table;
    }

    private static Text Cell(string value, Style style) => new(Safe(value), style) { Overflow = Overflow.Ellipsis };

    private static List<string> WrapLines(string value, int width) => Segment.SplitLines(
            [new Segment(Safe(value, multiline: true))], Math.Max(1, width), null)
        .Select(line => string.Concat(line.Select(segment => segment.Text)))
        .ToList();

    private static string Safe(string value, bool multiline = false)
    {
        var result = new StringBuilder(value.Length);
        foreach (var character in value)
        {
            if (character == '\r')
            {
                continue;
            }

            result.Append(character switch
            {
                '\n' when multiline => '\n',
                '\t' => ' ',
                _ when char.IsControl(character) => '�',
                _ => character,
            });
        }

        return result.ToString();
    }
}
