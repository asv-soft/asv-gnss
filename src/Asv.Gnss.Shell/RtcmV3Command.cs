using Asv.IO;
using ConsoleAppFramework;
using Microsoft.Extensions.Logging;
using R3;
using ZLogger;

namespace Asv.Gnss.Shell;

public class RtcmV3Command
{
    /// <summary>
    /// Command creates fake diagnostics data from file and opens a mavlink connection.
    /// </summary>
    /// <param name="cs">-cs, connection string</param>
    /// <returns></returns>
    [Command("rtcm")]
    public int Run(string cs = "tcp://192.168.137.71:30")
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
            builder.Protocols.RegisterRtcmV3Protocol();
            builder.Features.RegisterBroadcastAllFeature();
            builder.Features.RegisterEndpointIdTagFeature();
        });
        var logger = factory.CreateLogger<RtcmV3Command>();
        var router = protocol.CreateRouter("Router");
        router.AddPort(cs);
        router.AddPort("tcps://127.0.0.1:7341?enabled=true");

        var browser = DeviceExplorer.Create(router, builder =>
        {
            builder.SetLog(factory);
            builder.Factories.RegisterGnssDevice();
        });

        var index = 0;
        // router.OnRxMessage.Subscribe(x =>
        // {
        //     logger.ZLogInformation($"{index++:000}: {x} ");
        // });

        var rtcmV3Factory = new RtcmV3MessageFactory();
        router.OnRxMessage.FilterByType<RtcmV3MessageBase>().Subscribe(x =>
        {
            var newBuffer = new byte[x.GetByteSize()];
            var buffer = new Span<byte>(newBuffer);
            x.Serialize(ref buffer);
        });
        
        ConsoleAppHelper.WaitCancelPressOrProcessExit();
        return 0;
    }
}