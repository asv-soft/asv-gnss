using ConsoleAppFramework;
using Spectre.Console;

namespace Asv.Gnss;

public sealed class FileInspectCommand
{
    /// <summary>Inspect messages from all implemented stream protocols in a local file.</summary>
    /// <param name="file">-f, input file path (any extension)</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [Command("inspect")]
    public async Task<int> RunAsync(string file, CancellationToken cancellationToken = default)
    {
        try
        {
            ProtocolFileCapture capture;
            if (Console.IsOutputRedirected)
            {
                capture = await ProtocolFileReader.ReadAsync(file, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                capture = await AnsiConsole.Status()
                    .Spinner(Spinner.Known.Dots)
                    .StartAsync("Reading and identifying messages...",
                        _ => ProtocolFileReader.ReadAsync(file, cancellationToken)).ConfigureAwait(false);
            }

            var state = new FileInspectorState(capture.Messages.Count, 14);
            var detailIndex = -2;
            var detailText = string.Empty;
            if (Console.IsInputRedirected || Console.IsOutputRedirected || !AnsiConsole.Profile.Capabilities.Interactive)
            {
                AnsiConsole.Write(FileInspectorView.Render(capture, state, SelectedDetails(),
                    AnsiConsole.Profile.Width, Math.Max(24, AnsiConsole.Profile.Height)));
                return 0;
            }

            AnsiConsole.AlternateScreen(() =>
            {
                AnsiConsole.Live(Render()).AutoClear(true).Start(context =>
                {
                    var width = AnsiConsole.Profile.Width;
                    var height = AnsiConsole.Profile.Height;
                    context.Refresh();
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        var refresh = false;
                        if (Console.KeyAvailable)
                        {
                            var key = Console.ReadKey(intercept: true).Key;
                            if (key is ConsoleKey.Escape or ConsoleKey.Q)
                            {
                                break;
                            }
                            refresh = state.HandleKey(key);
                        }

                        if (width != AnsiConsole.Profile.Width || height != AnsiConsole.Profile.Height)
                        {
                            width = AnsiConsole.Profile.Width;
                            height = AnsiConsole.Profile.Height;
                            refresh = true;
                        }
                        if (refresh)
                        {
                            context.UpdateTarget(Render());
                        }
                        if (cancellationToken.WaitHandle.WaitOne(50))
                        {
                            break;
                        }
                    }
                });
            });
            cancellationToken.ThrowIfCancellationRequested();
            return 0;

            string SelectedDetails()
            {
                if (detailIndex != state.SelectedIndex)
                {
                    detailIndex = state.SelectedIndex;
                    detailText = detailIndex < 0
                        ? "No recognized messages were found in this file."
                        : MessageDetailFormatter.Format(capture.Messages[detailIndex].Message);
                }
                return detailText;
            }

            Spectre.Console.Rendering.IRenderable Render() => FileInspectorView.Render(capture, state,
                SelectedDetails(), AnsiConsole.Profile.Width, AnsiConsole.Profile.Height);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            return 130;
        }
        catch (Exception error) when (error is IOException or UnauthorizedAccessException or ArgumentException)
        {
            AnsiConsole.MarkupLineInterpolated($"[red]Unable to inspect file:[/] {error.Message}");
            return 1;
        }
    }
}
