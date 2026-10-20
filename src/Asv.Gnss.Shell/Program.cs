using System.Text;
using ConsoleAppFramework;

namespace Asv.Gnss;

internal static class Program
{
    public static async Task Main(string[] args)
    {
        Console.InputEncoding = Encoding.UTF8;
        Console.OutputEncoding = Encoding.UTF8;
        Console.BackgroundColor = ConsoleColor.Black;

        var app = ConsoleApp.Create();

        // app.Add<NmeaCommand>();
        app.Add<RtcmV3Command>();
        app.Add<AsterixStreamCommand>();
        await app.RunAsync(args);
    }
}
