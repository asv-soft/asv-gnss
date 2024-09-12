using System;
using System.ComponentModel;
using System.IO;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Asv.Common;
using Newtonsoft.Json;
using Spectre.Console.Cli;

namespace Asv.Gnss.Shell
{
    internal class UbxPvtCommand : Command<UbxPvtCommand.Settings>
    {
        public sealed class Settings : CommandSettings
        {
            /// <summary>
            /// Connection string for UBX.
            /// </summary>
            [Description("Connection string for UBX")]
            [CommandArgument(0, "[connectionString]")]
            public string Cs { get; set; } = "serial:COM10?br=115200";

            [Description("On/Off UBX")]
            [CommandArgument(1, "[IsEnabled]")]
            public bool IsEnabled { get; set; } = true;

            /// <summary>
            /// Message rate for UBX (Hz)
            /// </summary>
            [Description("Pvt message rate for UBX (Hz)")]
            [CommandArgument(2, "[PvtRate]")]
            public byte RateRate { get; set; } = 1;
        }

        public override int Execute(CommandContext context, Settings settings)
        {
            var waitForProcessShutdownStart = new ManualResetEventSlim();
            AppDomain.CurrentDomain.ProcessExit += (sender, e) =>
            {
                // We got a SIGTERM, signal that graceful shutdown has started
                waitForProcessShutdownStart.Set();
            };
            Console.CancelKeyPress += (sender, e) =>
            {
                e.Cancel = true;
                waitForProcessShutdownStart.Set();
            };

            using var device = new UbxPvtLogger(new UbxPvtLoggerConfig
                { ConnectionString = settings.Cs, IsEnabled = settings.IsEnabled, PvtRate = settings.RateRate });
            device.Init();
            Test(device).Wait();

            // Wait for shutdown to start
            waitForProcessShutdownStart.Wait();
            
            return 0;
        }

        public async Task Test(IPvtLogger logger)
        {

            var s = JsonSerializer.Create(new JsonSerializerSettings());
            logger.OnPvtInfo.Subscribe(_ =>
            {
                s.Serialize(Console.Out, _);
                Console.WriteLine();
            });
            logger.OnNmea.Select(GetNmeaMessages)
                .Buffer(TimeSpan.FromSeconds(5))
                .Subscribe(_ =>
                {
                    using var wrt = File.AppendText($"GnssPvtLog_{DateTime.UtcNow:dd-MM-yy}.txt");
                    foreach (var value in _)
                    {
                        wrt.Write(value);
                    }
                    wrt.Flush();
                });
        }
        
        private string GetNmeaMessages(Nmea0183MessageBase msg)
        {
            var byteBuff = new byte[1024];
            var byteSpan = new Span<byte>(byteBuff);
            var origSpan = byteSpan;
            msg.Serialize(ref byteSpan);
            var length = origSpan.Length - byteSpan.Length;
            return Encoding.ASCII.GetString(origSpan[..length]);
        }
    }
}