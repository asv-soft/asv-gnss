namespace Asv.Gnss.Shell
{
    using Spectre.Console.Cli;

    internal static class Program
    {
        private static int Main(string[] args)
        {
            var app = new CommandApp();
            app.Configure(config =>
            {
                config.AddCommand<PrintBytesCommand>("print");
                config.AddCommand<UbxCommand>("ubx");
                config.AddCommand<UbxTrackCommand>("pvt");
#if DEBUG
                config.PropagateExceptions();
                config.ValidateExamples();
#endif
            });
            return app.Run(args);
        }
    }
}
