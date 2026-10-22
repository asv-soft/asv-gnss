namespace Asv.Gnss;

internal sealed class FileInspectorState
{
    private readonly int _messageCount;
    private int _maximumDetailsOffset;

    public FileInspectorState(int messageCount, int pageSize)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(messageCount);
        ArgumentOutOfRangeException.ThrowIfLessThan(pageSize, 1);
        _messageCount = messageCount;
        PageSize = pageSize;
        SelectedIndex = messageCount == 0 ? -1 : 0;
    }

    public int SelectedIndex { get; private set; }
    public int PageIndex => SelectedIndex < 0 ? 0 : SelectedIndex / PageSize;
    public int PageCount => _messageCount == 0 ? 0 : (_messageCount - 1) / PageSize + 1;
    public int PageSize { get; private set; }
    public int DetailsOffset { get; private set; }
    public int DetailsPageSize { get; set; } = 1;

    public bool HandleKey(ConsoleKey key)
    {
        var previousSelection = SelectedIndex;
        var previousDetailsOffset = DetailsOffset;
        if (key is ConsoleKey.PageUp or ConsoleKey.PageDown)
        {
            var direction = key == ConsoleKey.PageDown ? 1 : -1;
            DetailsOffset = (int)Math.Clamp((long)DetailsOffset + direction * Math.Max(1, DetailsPageSize),
                0, _maximumDetailsOffset);
        }
        else if (_messageCount > 0)
        {
            switch (key)
            {
                case ConsoleKey.LeftArrow when PageIndex > 0:
                    SelectedIndex -= PageSize;
                    break;
                case ConsoleKey.RightArrow when PageIndex + 1 < PageCount:
                    SelectedIndex = (int)Math.Min((long)SelectedIndex + PageSize, _messageCount - 1);
                    break;
                case ConsoleKey.UpArrow:
                    SelectedIndex = Math.Max(0, SelectedIndex - 1);
                    break;
                case ConsoleKey.DownArrow:
                    SelectedIndex = Math.Min(_messageCount - 1, SelectedIndex + 1);
                    break;
                case ConsoleKey.Home:
                    SelectedIndex = 0;
                    break;
                case ConsoleKey.End:
                    SelectedIndex = _messageCount - 1;
                    break;
            }
        }

        if (previousSelection != SelectedIndex)
        {
            DetailsOffset = 0;
            _maximumDetailsOffset = 0;
        }

        return previousSelection != SelectedIndex || previousDetailsOffset != DetailsOffset;
    }

    public void Resize(int pageSize)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(pageSize, 1);
        PageSize = pageSize;
    }

    public void ClampDetails(int lineCount, int visibleLines)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(lineCount);
        ArgumentOutOfRangeException.ThrowIfLessThan(visibleLines, 1);
        DetailsPageSize = visibleLines;
        _maximumDetailsOffset = Math.Max(0, lineCount - visibleLines);
        DetailsOffset = Math.Min(DetailsOffset, _maximumDetailsOffset);
    }
}
