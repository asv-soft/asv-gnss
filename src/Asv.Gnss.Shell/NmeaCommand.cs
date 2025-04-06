using Asv.IO;
using ConsoleAppFramework;
using Microsoft.Extensions.Logging;
using R3;
using ZLogger;

namespace Asv.Gnss.Shell;

public class NmeaCommand
{
    /// <summary>
    /// Command creates fake diagnostics data from file and opens a mavlink connection.
    /// </summary>
    /// <param name="cs">-cs, connection string</param>
    /// <returns></returns>
    [Command("nmea")]
    public int Run(string cs = "serial:COM53?br=115200&enabled=true")
    {
        var factory = LoggerFactory.Create(logging =>
        {
            logging.SetMinimumLevel(LogLevel.Trace);

            logging.AddZLoggerConsole(options =>
            {
                options.IncludeScopes = true;
                options.UsePlainTextFormatter(formatter =>
                {
                    formatter.SetPrefixFormatter($"{0:HH:mm:ss.fff} | ={1:short}= | {2,-40} ",
                        (in MessageTemplate template, in LogInfo info) =>
                            template.Format(info.Timestamp, info.LogLevel, info.Category));
                    formatter.SetExceptionFormatter((writer, ex) =>
                        Utf8StringInterpolation.Utf8String.Format(writer, $"{ex.Message}"));
                });

            });
        });
        
        var protocol = Protocol.Create(builder =>
        {
            builder.SetLog(factory);
            builder.Protocols.RegisterNmeaProtocol();
            builder.Features.RegisterBroadcastAllFeature();
            builder.Features.RegisterEndpointIdTagFeature();
        });
        var logger = factory.CreateLogger<NmeaCommand>();
        var router = protocol.CreateRouter("Router");
        router.AddPort(cs);
        router.AddPort("tcps://127.0.0.1:7341?enabled=true");

        var browser = DeviceExplorer.Create(router, builder =>
        {
            builder.SetLog(factory);
            builder.Factories.RegisterGnssDevice();
        });

        var index = 0;
        router.OnRxMessage.Subscribe(x =>
        {
            logger.ZLogInformation($"{index++:000}: {x} ");
        });
        
        ConsoleAppHelper.WaitCancelPressOrProcessExit();
        return 0;
    }

}