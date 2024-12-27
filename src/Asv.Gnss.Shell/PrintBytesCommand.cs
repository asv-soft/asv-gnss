namespace Asv.Gnss.Shell
{
    using System;
    using System.ComponentModel;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Spectre.Console.Cli;

    /// <summary>
    /// Represents a command that prints bytes received from a connection.
    /// </summary>
    internal class PrintBytesCommand : Command<PrintBytesCommand.Settings>
    {
        /// <summary>
        /// Executes the command. </summary>
        /// <param name="context">Command context.</param> <param name="settings">Execution settings.</param>
        /// <returns>Integer value indicating the execution status.</returns>**/
        public override int Execute(CommandContext context, Settings settings)
        {
            // create connection with default parsers: Nmea,RTCMv2,RTCMv3,ComNav,Ubx,Sbf
            var connection = GnssFactory.CreateDefault(settings.Cs);
            connection.Stream.Subscribe(_ =>
            {
                Console.Write($"var data = new byte[{_.Length}] = {{");
                foreach (var b in _)
                {
                    Console.Write("0x");
                    Console.Write(b.ToString("X2"));
                    Console.Write(", ");
                }

                Console.WriteLine("};");
                Console.WriteLine("===========END=============");
            });
            connection.OnMessage.Subscribe(_ =>
            {
                Console.WriteLine(
                    $"=========== BEGIN {_.ProtocolId}.{_.Name}[{_.MessageStringId}]============="
                );
                Console.WriteLine(
                    JsonConvert.SerializeObject(_, Formatting.Indented, new StringEnumConverter())
                );
            });
            Console.ReadLine();
            return 0;
        }

        /// <summary>
        /// Represents the settings for a command.
        /// </summary>
        public sealed class Settings : CommandSettings
        {
            /// <summary>
            /// Gets or sets the connection string for EVSG.
            /// </summary>
            /// <value>
            /// The connection string for EVSG.
            /// </value>
            [Description("Connection string for EVSG")]
            [CommandArgument(0, "[connectionString]")]
            public string Cs { get; set; } = "tcp://10.10.6.137:64536";
        }
    }
}
