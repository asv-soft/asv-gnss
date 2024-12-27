namespace Asv.Gnss.Shell
{
    using System.Collections.Generic;
    using Spectre.Console;

    /// <summary>
    /// This class, `AnsiConsoleHelper`, contains helper methods for formatting and printing console output in ANSI colors and styles.
    /// </summary>
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
            node.PrintKeyValue(nameof(data.Software), data.Software);
            node.PrintKeyValue(nameof(data.Hardware), data.Hardware);
            node.PrintKeyValue(nameof(data.Extensions), data.Extensions);
        }

        /// <summary>
        /// Prints a key-value pair in a specific format.
        /// </summary>
        /// <param name="node">The <see cref="TreeNode"/> where the key-value pair will be printed.</param>
        /// <param name="key">The key to be printed.</param>
        /// <param name="value">The value to be printed.</param>
        public static void PrintKeyValue(this TreeNode node, string key, string value)
        {
            node.AddNode($"[yellow]{key, -20}[/]:{value}");
        }

        /// <summary>
        /// Prints the key-value pairs in a tree structure.
        /// </summary>
        /// <param name="node">The root node of the tree.</param>
        /// <param name="key">The key to be printed.</param>
        /// <param name="value">The collection of values associated with the key.</param>
        public static void PrintKeyValue(this TreeNode node, string key, IEnumerable<string> value)
        {
            var subNode = node.AddNode($"[yellow]{key, -20}[/]");
            foreach (var v in value)
            {
                subNode.AddNode(v);
            }
        }
    }
}
