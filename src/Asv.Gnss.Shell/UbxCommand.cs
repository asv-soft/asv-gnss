namespace Asv.Gnss.Shell
{
    using System.ComponentModel;
    using System.Threading.Tasks;
    using Spectre.Console;
    using Spectre.Console.Cli;

    /// <summary>
    /// Represents a UBX command.
    /// </summary>
    internal class UbxCommand : Command<UbxCommand.Settings>
    {
        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="context">The command context.</param>
        /// <param name="settings">The settings for the command.</param>
        /// <returns>Returns an integer value indicating the status of the execution.</returns>
        public override int Execute(CommandContext context, Settings settings)
        {
            using var device = new UbxDevice(settings.Cs);
            this.Test(device).Wait();
            device.SetupByDefault().Wait();
            device.SetSurveyInMode().Wait();

            return 0;
        }

        /// <summary>
        /// Perform a series of tests on the specified device.
        /// </summary>
        /// <param name="device">The IUbxDevice to test.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public async Task Test(IUbxDevice device)
        {
            var root = new Tree(device.Connection.Stream.Name);
            root.Print(await device.GetMonVer());
            root.Print(await device.GetCfgPort(0), 0);
            root.Print(await device.GetCfgPort(1), 1);
            root.Print(await device.GetCfgPort(2), 2);
            root.Print(await device.GetCfgPort(3), 3);
            root.Print(await device.GetCfgPort(4), 4);

            AnsiConsole.Write(root);
        }

        /// <summary>
        /// Represents the settings for the application.
        /// </summary>
        public sealed class Settings : CommandSettings
        {
            /// <summary>
            /// Gets or sets connection string for UBX.
            /// </summary>
            [Description("Connection string for UBX")]
            [CommandArgument(0, "[connectionString]")]
            public string Cs { get; set; } = "serial:COM10?br=115200";
        }
    }
}
