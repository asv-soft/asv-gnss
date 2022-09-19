using System;
using System.ComponentModel;
using Spectre.Console.Cli;

namespace Asv.Gnss.Shell
{
    internal class UbxCommand : Command<UbxCommand.Settings>
    {
        public sealed class Settings : CommandSettings
        {
            [Description("Connection string for EVSG")]
            [CommandArgument(0, "[connectionString]")]
            public string Cs { get; set; } = "serial:COM10?br=115200";
        }

        public override int Execute(CommandContext context, Settings settings)
        {
            using var device = new UbxDevice(settings.Cs);
            while (true)
            {
                try
                {
                    var result = device.GetMonHw().Result;
                    var result2 = device.GetNavSat().Result;
                    Console.WriteLine(result.Name);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Timeout");
                }    
            }
            
            return 0;
        }
    }
}