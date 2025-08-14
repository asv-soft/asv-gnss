using System;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using Spectre.Console.Cli;

namespace Asv.Gnss.Shell;


internal class AsvObserver : IObserver<AsvMessageBase>
{
    private readonly StringBuilder _gpsBuilder = new();
    private readonly StringBuilder _gloBuilder = new();

    public void OnCompleted()
    {
    }

    public void OnError(Exception error)
    {
    }

    public void OnNext(AsvMessageBase value)
    {
        if (value == null) return;
        if (value.MessageId != 0x110 && value.MessageId != 0x111) return;
        
        if (value.MessageId == 0x110)
        {
            var gpsEpoch = (AsvMessageGpsObservations)value;
            _gpsBuilder.Clear();
            _gpsBuilder.Append($"GPS epoch: {gpsEpoch.Tow:dd.MM.yyyy HH:mm:ss}");
            _gpsBuilder.Append(Environment.NewLine);
            foreach (var item in gpsEpoch.Observations)
            {
                var phase = item.L1CarrierPhase * AsvHelper.CLIGHT / 1.57542E9;
                _gpsBuilder.Append($"{item.SatelliteCode} [{(item.ParticipationIndicator ? "+" : "-")}] PR={item.L1PseudoRange,7:00000.0} RR={phase,9:000.0000} SNR={item.L1CNR,5:00.00}");
                // if (!item.ParticipationIndicator) 
                _gpsBuilder.Append($" {item.ReasonForException:G}");
                _gpsBuilder.Append(Environment.NewLine);
            }
        }
        if (value.MessageId == 0x111)
        {
            var gloEpoch = (AsvMessageGloObservations)value;
            _gloBuilder.Clear();
            _gloBuilder.Append($"Glonass epoch: {gloEpoch.Tod:dd.MM.yyyy HH:mm:ss}");
            _gloBuilder.Append(Environment.NewLine);
            foreach (var item in gloEpoch.Observations)
            {
                var phase = item.L1CarrierPhase * AsvHelper.CLIGHT / item.Frequency;
                _gloBuilder.Append($"{item.SatelliteCode}[{(item.ParticipationIndicator ? "+" : "-")}] PR={item.L1PseudoRange,7:00000.0} RR={phase,9:000.0000} SNR={item.L1CNR,5:00.00}");
                // if (!item.ParticipationIndicator)
                _gloBuilder.Append($" {item.ReasonForException:G}");
                _gloBuilder.Append(Environment.NewLine);
            }
        }
        Console.Clear();
        Console.WriteLine(_gpsBuilder.ToString());
        Console.WriteLine(_gloBuilder.ToString());
    }
}

internal class AsvCommand : Command<AsvCommand.Settings>
{
    /// <summary>
    /// Represents the settings for the application.
    /// </summary>
    public sealed class Settings : CommandSettings
    {
        /// <summary>
        /// Connection string for Asv.
        /// </summary>
        [Description("Connection string for Asv")]
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

        var obs = new AsvObserver();
        using var conn = new GnssConnection(settings.Cs, new AsvMessageParser().RegisterDefaultMessages());
        using var sub = conn.OnMessage.Where(m => m is AsvMessageBase).Select(m => (AsvMessageBase)m).Subscribe(obs);

        // Wait for shutdown to start
        waitForProcessShutdownStart.Wait();
        
        
        return 0;
    }
    
}


