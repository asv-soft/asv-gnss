// See https://aka.ms/new-console-template for more information

using System.Text;
using Asv.Gnss.Shell;
using ConsoleAppFramework;

Console.InputEncoding = Encoding.UTF8;
Console.OutputEncoding = Encoding.UTF8;
Console.BackgroundColor = ConsoleColor.Black;
        
var app = ConsoleApp.Create();

app.Add<NmeaCommand>();
await app.RunAsync(args);