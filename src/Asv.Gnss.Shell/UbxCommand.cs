using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Asv.Gnss.Shell
{
    /// <summary>
    /// Represents a UBX command.
    /// </summary>
    internal class UbxCommand : Command<UbxCommand.Settings>
    {
        /// <summary>
        /// Represents the settings for the application.
        /// </summary>
        public sealed class Settings : CommandSettings
        {
            /// <summary>
            /// Connection string for EVSG.
            /// </summary>
            [Description("Connection string for EVSG")]
            [CommandArgument(0, "[connectionString]")]
            public string Cs { get; set; } = "serial:COM10?br=115200";
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="context">The command context.</param>
        /// <param name="settings">The settings for the command.</param>
        /// <returns>Returns an integer value indicating the status of the execution.</returns>
        public override int Execute(CommandContext context, Settings settings)
        {
            using var device = new UbxDevice(settings.Cs);
            Test(device).Wait();
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
            root.Print(await device.GetCfgPort(0),0);
            root.Print(await device.GetCfgPort(1), 1);
            root.Print(await device.GetCfgPort(2), 2);
            root.Print(await device.GetCfgPort(3), 3);
            root.Print(await device.GetCfgPort(4), 4);

            AnsiConsole.Write(root);
        }
    }


    /// This class, `AnsiConsoleHelper`, contains helper methods for formatting and printing console output in ANSI colors and styles.
    /// /
    public static class AnsiConsoleHelper
    {
        /// <summary>
        /// Prints the tree, including the given data and port.
        /// </summary>
        /// <param name="root">The root tree.</param>
        /// <param name="data">The UbxCfgPrt data to print.</param>
        /// <param name="port">The port to print.</param>
        public static void Print(this Tree root, UbxCfgPrt data, byte port)
        {
            var node = root.AddNode($"[green]{data.Name}({port})[/]");
            node.PrintKeyValue(nameof(data.Config.PortType), data.Config.PortType.ToString("G"));
        }

        /// <summary>
        /// Prints the specified tree root with the given UbxMonVer data.
        /// </summary>
        /// <param name="root">The root tree to be printed.</param>
        /// <param name="data">The UbxMonVer data to be printed.</param>
        public static void Print(this Tree root, UbxMonVer data)
        {
            var node = root.AddNode($"[green]{data.Name}[/]");
            node.PrintKeyValue(nameof(data.Software),data.Software);
            node.PrintKeyValue(nameof(data.Hardware), data.Hardware);
            node.PrintKeyValue(nameof(data.Extensions), data.Extensions);
        }

        /// <summary>
        /// Prints a key-value pair in a specific format.
        /// </summary>
        /// <param name="node">The <see cref="TreeNode"/> where the key-value pair will be printed.</param>
        /// <param name="key">The key to be printed.</param>
        /// <param name="value">The value to be printed.</param>
        public static void PrintKeyValue(this TreeNode node, string key,string value)
        {
            node.AddNode($"[yellow]{key,-20}[/]:{value}");
        }

        /// <summary>
        /// Prints the key-value pairs in a tree structure.
        /// </summary>
        /// <param name="node">The root node of the tree.</param>
        /// <param name="key">The key to be printed.</param>
        /// <param name="value">The collection of values associated with the key.</param>
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