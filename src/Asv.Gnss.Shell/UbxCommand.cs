using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Asv.Gnss.Shell
{
    internal class UbxCommand : Command<UbxCommand.Settings>
    {
        public sealed class Settings : CommandSettings
        {
            [Description("Connection string for UBX")]
            [CommandArgument(0, "[connectionString]")]
            public string Cs { get; set; } = "serial:COM10?br=115200";
        }

        public override int Execute(CommandContext context, Settings settings)
        {
            using var device = new UbxDevice(settings.Cs);
            Test(device).Wait();
            device.SetupByDefault().Wait();
            device.SetSurveyInMode().Wait();
            
            return 0;
        }

        public async Task Test(IUbxDevice device)
        {
            var root = new Tree(device.Connection.Stream.Name);
            
            root.Print(await device.GetMonVer());
            root.Print(await device.GetCfgPort(0),0);
            root.Print(await device.GetCfgPort(1), 1);
            root.Print(await device.GetCfgPort(2), 2);
            root.Print(await device.GetCfgPort(3), 3);
            root.Print(await device.GetCfgPort(4), 4);

            AnsiConsole.Write(root);
        }
    }


    public static class AnsiConsoleHelper
    {
        public static void Print(this Tree root, UbxCfgPrt data, byte port)
        {
            var node = root.AddNode($"[green]{data.Name}({port})[/]");
            node.PrintKeyValue(nameof(data.Config.PortType), data.Config.PortType.ToString("G"));
        }

        public static void Print(this Tree root, UbxMonVer data)
        {
            var node = root.AddNode($"[green]{data.Name}[/]");
            node.PrintKeyValue(nameof(data.Software),data.Software);
            node.PrintKeyValue(nameof(data.Hardware), data.Hardware);
            node.PrintKeyValue(nameof(data.Extensions), data.Extensions);
        }

        public static void PrintKeyValue(this TreeNode node, string key,string value)
        {
            node.AddNode($"[yellow]{key,-20}[/]:{value}");
        }
        public static void PrintKeyValue(this TreeNode node, string key, IEnumerable<string> value)
        {
            var subNode = node.AddNode($"[yellow]{key,-20}[/]");
            foreach (var v in value)
            {
                subNode.AddNode(v);
            }
        }
    }
}