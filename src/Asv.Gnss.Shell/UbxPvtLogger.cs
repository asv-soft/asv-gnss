using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Asv.Common;
using Asv.IO;
using Microsoft.Extensions.Logging;
using ZLogger;

namespace Asv.Gnss.Shell
{
    public class PvtInfo
    {
        public DateTime UtcTime { get; set; }
        public DateTime GpsTime { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double AltitudeEllipse { get; set; }
        public double AltitudeMsl { get; set; }
        public double GroundSpeed2D { get; set; }
        public double HeadingOfMotion2D { get; set; }
    }
    public interface IPvtLogger
    {
        IRxValue<PvtInfo> OnPvtInfo { get; }
        public void Init();
    }
    public class UbxPvtLoggerConfig
    {
        /// <summary>
        /// Connection string for UBX.
        /// </summary>
        public string ConnectionString { get; set; } = "serial:COM10?br=115200";
        
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Pvt logging rate for UBX (Hz)
        /// </summary>
        public byte PvtRate { get; set; } = 1;

        /// <summary>
        /// Gets or sets the timeout value in milliseconds for reconnecting.
        /// </summary>
        /// <value>
        /// The timeout value in milliseconds for reconnecting. The default value is 10,000 milliseconds.
        /// </value>
        public int ReconnectTimeoutMs { get; set; } = 10_000;
    }

    public class UbxPvtLogger : DisposableOnceWithCancel, IPvtLogger
    {
        private readonly ILogger<UbxPvtLogger> _logger;
        private readonly UbxPvtLoggerConfig _cfg;
        private readonly RxValue<PvtInfo> _onPvtInfo;

        /// <summary>
        /// This private variable indicates whether the initialization process has been completed.
        /// </summary>
        private bool _isInit;

        private UbxDevice? _device;
        private int _busy;

        
        public IRxValue<PvtInfo> OnPvtInfo => _onPvtInfo;
        
        public UbxPvtLogger(UbxPvtLoggerConfig cfg)
        {
            using var factory = LoggerFactory.Create(logging =>
            {
                logging.SetMinimumLevel(LogLevel.Trace);

                // Add ZLogger provider to ILoggingBuilder
                logging.AddZLoggerConsole();

                // Output Structured Logging, setup options
                logging.AddZLoggerConsole(options => options.UseJsonFormatter());
            });
            _logger = factory.CreateLogger<UbxPvtLogger>();


            _cfg = cfg ?? throw new ArgumentNullException(nameof(cfg));
            if (_cfg.IsEnabled == false)
            {
                _logger.LogWarning("UBX PVT Logger is disabled and will be ignored");
                return;
            }

            _onPvtInfo = new RxValue<PvtInfo>().DisposeItWith(Disposable);

        }

        /// <summary>
        /// Initializes the method.
        /// </summary>
        public void Init()
        {
            // if disabled => do nothing
            if (_cfg.IsEnabled == false) return;
            Observable.Timer(TimeSpan.FromMilliseconds(1000)).Subscribe(_ => InitUbxDevice(), DisposeCancel);
        }

        /// <summary>
        /// Returns a default instance of <see cref="IGnssConnection"/> using the specified <paramref name="port"/>.
        /// </summary>
        /// <param name="port">The optional port to use for the connection.</param>
        /// <returns>A default instance of <see cref="IGnssConnection"/>.</returns>
        private IGnssConnection GetDefaultUbxConnection(IPort? port)
        {
            return new GnssConnection(port, new UbxBinaryParser().RegisterDefaultMessages());
        }

        /// <summary>
        /// Configures the baud rate and creates a UbxDevice.
        /// </summary>
        /// <param name="currentConfig">The current serial port configuration.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the UbxDevice.</returns>
        private async Task<UbxDevice> ConfigureBaudRateAndCreateDevice(SerialPortConfig currentConfig)
        {
            var availableBr = new[] { currentConfig.BoundRate, 9600, 38400, 57600, 115200, 230400, 460800 }.Distinct()
                .ToArray();
            var requiredBoundRate = currentConfig.BoundRate;
            Exception? lastEx = null;
            foreach (var br in availableBr)
            {
                UbxDevice? device = null;
                CustomSerialPort? port = null;
                try
                {
                    currentConfig.BoundRate = br;
                    port = new CustomSerialPort(currentConfig);
                    port.Enable();
                    device =
                        new UbxDevice(GetDefaultUbxConnection(port), UbxDeviceConfig.Default).DisposeItWith(Disposable);
                    var cfgPort =
                        (UbxCfgPrtConfigUart)(await device.GetCfgPort(1, DisposeCancel).ConfigureAwait(false)).Config;
                    // _svc.Server.StatusText.Info($"GNSS device BoundRate: {cfgPort.BoundRate}");
                    _logger.LogInformation($"GNSS device BoundRate: {cfgPort.BoundRate}");
                    if (cfgPort.BoundRate == requiredBoundRate) return device;

                    // _svc.Server.StatusText.Info($"Change BoundRate {cfgPort.BoundRate} => {requiredBoundRate}");
                    _logger.LogInformation($"Change BoundRate {cfgPort.BoundRate} => {requiredBoundRate}");
                    await device
                        .SetCfgPort(
                            new UbxCfgPrt
                            {
                                Config = new UbxCfgPrtConfigUart { PortId = 1, BoundRate = requiredBoundRate }
                            }, DisposeCancel).ConfigureAwait(false);
                    device.Dispose();
                    port.Disable();
                    port.Dispose();
                    currentConfig.BoundRate = requiredBoundRate;
                    port = new CustomSerialPort(currentConfig);
                    port.Enable();
                    device = new UbxDevice(GetDefaultUbxConnection(port), UbxDeviceConfig.Default)
                        .DisposeItWith(Disposable);

                    cfgPort = (UbxCfgPrtConfigUart)(await device.GetCfgPort(1, DisposeCancel).ConfigureAwait(false))
                        .Config;
                    // _svc.Server.StatusText.Info($"GNSS device BoundRate: {cfgPort.BoundRate}");
                    _logger.LogInformation($"Change BoundRate {cfgPort.BoundRate} => {requiredBoundRate}");
                    if (cfgPort.BoundRate == requiredBoundRate) return device;
                }
                catch (Exception e)
                {
                    device?.Dispose();
                    port?.Disable();
                    port?.Dispose();
                    lastEx = e;
                }
            }

            throw lastEx!;
        }

        /// <summary>
        /// Initializes the UBX GNSS device.
        /// </summary>
        private async void InitUbxDevice()
        {
            try
            {
                if (_device == null)
                {
                    // start to init device
                    // _svc.Server.StatusText.Info("Connecting to GNSS device...");
                    _logger.LogInformation("Connecting to GNSS device...");
                    var port = PortFactory.Create(_cfg.ConnectionString, true);
                    if (port.PortType == PortType.Serial)
                    {
                        port.Disable();
                        port.Dispose();
                        var uri = new Uri(_cfg.ConnectionString);
                        SerialPortConfig.TryParseFromUri(uri, out var portConf);
                        _device = await ConfigureBaudRateAndCreateDevice(portConf).ConfigureAwait(false);
                    }
                    else
                    {
                        _device =
                            new UbxDevice(GetDefaultUbxConnection(port), UbxDeviceConfig.Default)
                                .DisposeItWith(Disposable);
                    }

                    _device?.Connection.Filter<UbxNavPvt>().Select(_ => new PvtInfo
                        {
                            UtcTime = _.UtcTime,
                            GpsTime = _.GpsTime,
                            Latitude = _.Latitude,
                            Longitude = _.Longitude,
                            AltitudeEllipse = _.AltElipsoid,
                            AltitudeMsl = _.AltMsl,
                            GroundSpeed2D = _.GroundSpeed2D,
                            HeadingOfMotion2D = _.HeadingOfMotion2D
                        })
                        .Subscribe(_onPvtInfo)
                        .DisposeItWith(Disposable);
                }

                var ver = await _device.GetMonVer(DisposeCancel);
                // _svc.Server.StatusText.Debug($"Found GNSS HW:{ver.Hardware.Trim('\0')}");
                _logger.LogInformation($"Found GNSS HW:{ver.Hardware.Trim('\0')}");
                // _svc.Server.StatusText.Debug($"GNSS SW:{ver.Software.Trim('\0')}");
                _logger.LogInformation($"GNSS SW:{ver.Software.Trim('\0')}");
                var ext = ver.Extensions.Select(_ => _.Trim('\0')).Distinct().ToArray();
                // _svc.Server.StatusText.Debug($"GNSS EXT:{string.Join(",", ext)}");
                _logger.LogInformation($"GNSS EXT:{string.Join(",", ext)}");
                await _device.SetCfgNav5(
                    new UbxCfgNav5
                    {
                        PlatformModel = UbxCfgNav5.ModelEnum.AirborneWithLess1gAcceleration,
                        PositionMode = UbxCfgNav5.PositionModeEnum.Auto
                    }, DisposeCancel);

                //Turn Off RtcmV3 and NMEA
                await _device.SetMessageRate((byte)UbxHelper.ClassIDs.RTCM3, 0xFE, 0, DisposeCancel);
                await _device.SetMessageRate((byte)UbxHelper.ClassIDs.RTCM3, 0x05, 0, DisposeCancel);
                await _device.SetupRtcmMSM4Rate(0, DisposeCancel);
                await _device.SetMessageRate((byte)UbxHelper.ClassIDs.RTCM3, 0xE6, 0, DisposeCancel);
                await _device.SetMessageRate((byte)UbxHelper.ClassIDs.NAV, 0x12, 0, DisposeCancel);
                await _device.SetMessageRate((byte)UbxHelper.ClassIDs.RXM, 0x15, 0, DisposeCancel);
                await _device.SetMessageRate((byte)UbxHelper.ClassIDs.RXM, 0x13, 0, DisposeCancel);
                await _device.TurnOffNmea(DisposeCancel);


                await _device.SetCfgRate(
                    new UbxCfgRate { NavRate = 1, RateHz = 10, TimeSystem = UbxCfgRate.TimeSystemEnum.Utc },
                    DisposeCancel);
                // surveyin msg - for feedback
                await _device.SetMessageRate<UbxNavSvin>(2, DisposeCancel);
                // pvt msg - for feedback
                await _device.SetMessageRate<UbxNavPvt>(_cfg.PvtRate, DisposeCancel);
                // mon-hw - 2s
                await _device.SetMessageRate((byte)UbxHelper.ClassIDs.MON, 0x09, 2, DisposeCancel);

                _isInit = true;
                if (!await StartIdleMode(DisposeCancel)) throw new Exception();
                _isInit = true;
            }
            catch (Exception e)
            {
                _logger.LogError("Error to init GNSS");
                _logger.LogError(
                    $"Reconnect after {TimeSpan.FromMilliseconds(_cfg.ReconnectTimeoutMs).TotalSeconds:F0} sec...");
                Observable.Timer(TimeSpan.FromMilliseconds(_cfg.ReconnectTimeoutMs))
                    .Subscribe(_ => InitUbxDevice(), DisposeCancel);
            }
        }
        
        /// <summary>
        /// Starts the idle mode of the receiver.
        /// </summary>
        /// <param name="cancel">The cancellation token to cancel the operation.</param>
        /// <returns>The result of the operation.</returns>
        public async Task<bool> StartIdleMode(CancellationToken cancel)
        {
            if (CheckInitAndBeginCall() == false) return false;
            try
            {
                var mode = await _device.GetCfgTMode3(cancel);
                if (mode.Mode == TMode3Enum.Disabled) return true;
                await _device.Push(new UbxCfgTMode3 { Mode = TMode3Enum.Disabled, IsGivenInLLA = false }, cancel)
                    .ConfigureAwait(false);
                await _device.RebootReceiver(cancel).ConfigureAwait(false);
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError("GNSS IDLE mode error");
                _logger.LogError(e.Message);
                return false;
            }
            finally
            {
                EndCall();
            }
        }
        
        /// <summary>
        /// Ends the call and updates the '_busy' flag to indicate that the call has ended.
        /// </summary>
        private void EndCall()
        {
            Interlocked.Exchange(ref _busy, 0);
        }

        /// <summary>
        /// Checks if the initialization is complete and begins the method call.
        /// </summary>
        /// <returns>Returns true if the initialization is complete and the method call can proceed.
        /// Returns false if the initialization is not complete or if there is an ongoing method call.</returns>
        private bool CheckInitAndBeginCall()
        {
            // this is for reject duplicate requests
            if (Interlocked.CompareExchange(ref _busy, 1, 0) != 0)
            {
                _logger.LogWarning("Temporarily rejected: now is busy");
                return false;
            }

            if (_isInit) return true;
            _logger.LogWarning("Temporarily rejected: now is busy");
            return false;

        }
    }
}