using System;
using System.Reactive.Disposables;
using System.Threading;
using Asv.Common;
using Xunit;
using Xunit.Abstractions;

namespace Asv.Gnss.Tests
{
    // public class UbxDeviceTest
    // {
    //     private readonly ITestOutputHelper _output;
    //     private UbxDevice _device;
    //     private readonly RxValue<UbxNavSvin> _svIn;
    //     private bool _isInit;
    //
    //     public UbxDeviceTest(ITestOutputHelper output)
    //     {
    //         _output = output;
    //         _svIn = new RxValue<UbxNavSvin>().DisposeItWith(Disposable);
    //     }
    //     
    //     [Fact]
    //     public void ConnectToDevice()
    //     {
    //         var msg = new UbxCfgCfg
    //         {
    //             ClearMask = UbxCfgSection.All,
    //             SaveMask = UbxCfgSection.All,
    //             LoadMask = UbxCfgSection.None,
    //             DeviceMask = UbxCfgDeviceMask.DevBbr
    //         };
    //         var b = new byte[1024];
    //         var buffer = new Span<byte>(b);
    //         msg.Serialize(ref buffer);
    //         
    //         _device = new UbxDevice("tcp://172.16.0.1:3307").DisposeItWith(Disposable);
    //         _device.Connection.Filter<UbxNavSvin>().Subscribe(_svIn).DisposeItWith(Disposable);
    //         _device.SetStationaryMode(false, 1).Wait(CancellationToken.None);
    //         _device.TurnOffNmea(CancellationToken.None).Wait(CancellationToken.None);
    //         _device.SetMessageRate<UbxNavSvin>(1).Wait(CancellationToken.None);
    //         _device.SetMessageRate<UbxNavPvt>(1).Wait(CancellationToken.None);
    //         _device.SetMessageRate((byte)UbxHelper.ClassIDs.RTCM3, 0x05, 5).Wait(CancellationToken.None);
    //         
    //         _device.SetupRtcmMSM4Rate(1,CancellationToken.None).Wait(CancellationToken.None);
    //         _device.SetupRtcmMSM7Rate(0,default).Wait(CancellationToken.None);
    //         _device.SetMessageRate((byte)UbxHelper.ClassIDs.RTCM3, 0xE6, 5).Wait(CancellationToken.None);
    //         _device.SetMessageRate((byte)UbxHelper.ClassIDs.NAV, 0x12, 1).Wait(CancellationToken.None);
    //         _device.SetMessageRate((byte)UbxHelper.ClassIDs.RXM, 0x15, 1).Wait(CancellationToken.None);
    //         _device.SetMessageRate((byte)UbxHelper.ClassIDs.RXM, 0x13, 2, default).Wait(CancellationToken.None);
    //         _device.SetMessageRate((byte)UbxHelper.ClassIDs.MON, 0x09, 2, default).Wait(CancellationToken.None);
    //
    //         
    //         
    //         _device.CallCfgSave(CancellationToken.None).Wait(CancellationToken.None);
    //         _device.Connection
    //             .Send(new UbxCfgRst { Bbr = BbrMask.ColdStart, Mode = ResetMode.ControlledSoftwareResetGnssOnly },
    //                 CancellationToken.None).Wait(CancellationToken.None);
    //
    //         while (true)
    //         {
    //             try
    //             {
    //                 var value = _device.GetMonHw(CancellationToken.None).Result;
    //                 break;
    //             }
    //             catch (UbxDeviceTimeoutException)
    //             {
    //                 Thread.Sleep(TimeSpan.FromSeconds(1));
    //             }
    //         }
    //         _device.CallCfgLoad(CancellationToken.None).Wait(CancellationToken.None);
    //         
    //         var ver = _device.GetMonVer().Result;
    //         var cfgTMode3 = _device.GetCfgTMode3(CancellationToken.None).Result;
    //         _isInit = true;
    //     }
    //
    //     public CompositeDisposable Disposable { get; } = new CompositeDisposable();
    // }
}