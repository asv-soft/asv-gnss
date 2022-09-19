using System;
using System.ComponentModel;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Spectre.Console.Cli;

namespace Asv.Gnss.Shell
{
    internal class PrintBytesCommand:Command<PrintBytesCommand.Settings>
    {
        public sealed class Settings : CommandSettings
        {
            [Description("Connection string for EVSG")]
            [CommandArgument(0, "[connectionString]")]
            public string Cs { get; set; } = "tcp://10.10.6.137:64536";
        }

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
                Console.WriteLine($"=========== BEGIN {_.ProtocolId}.{_.Name}[{_.MessageStringId}]=============");
                Console.WriteLine(JsonConvert.SerializeObject(_, Formatting.Indented, new StringEnumConverter()));
            });
            Console.ReadLine();
            return 0;
        }
    }
}