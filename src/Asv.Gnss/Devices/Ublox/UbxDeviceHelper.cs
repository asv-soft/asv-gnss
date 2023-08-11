using System;
using System.Threading;
using System.Threading.Tasks;
using Asv.Common;
using Geodesy;

namespace Asv.Gnss
{
    public static class UbxDeviceHelper
    {
        #region MonVer

        public static Task<UbxMonVer> GetMonVer(this IUbxDevice src, CancellationToken cancel = default)
        {
            return src.Pool<UbxMonVer, UbxMonVerPool>(new UbxMonVerPool(), cancel);
        }

        #endregion

        #region CfgPort

        public static Task<UbxCfgPrt> GetCfgPort(this IUbxDevice src, byte portId, CancellationToken cancel = default)
        {
            return src.Pool<UbxCfgPrt, UbxCfgPrtPool>(new UbxCfgPrtPool { PortId = portId }, cancel);
        }
        public static Task SetCfgPort(this IUbxDevice src, UbxCfgPrt value, CancellationToken cancel = default)
        {
            return src.Push(value, cancel);
        }

        #endregion

        #region CfgAntenna

        public static Task<UbxCfgAnt> GetCfgAntenna(this IUbxDevice src, CancellationToken cancel = default)
        {
            return src.Pool<UbxCfgAnt, UbxCfgAntPool>(new UbxCfgAntPool(), cancel);
        }
        public static Task SetCfgAntenna(this IUbxDevice src, UbxCfgAnt value, CancellationToken cancel = default)
        {
            return src.Push(value, cancel);
        }

        #endregion

        #region CfgRate

        public static Task<UbxCfgRate> GetCfgRate(this IUbxDevice src, CancellationToken cancel = default)
        {
            return src.Pool<UbxCfgRate, UbxCfgRatePool>(new UbxCfgRatePool(), cancel);
        }
        public static Task SetCfgRate(this IUbxDevice src, UbxCfgRate value, CancellationToken cancel = default)
        {
            return src.Push(value, cancel);
        }

        #endregion

        #region CfgMsg

        public static Task<UbxCfgMsg> GetCfgMsg(this IUbxDevice src, byte msgClass, byte msgSubClass, CancellationToken cancel = default)
        {
            return src.Pool<UbxCfgMsg, UbxCfgMsgPool>(new UbxCfgMsgPool{MsgClass = msgClass , MsgId = msgSubClass}, cancel);
        }

        public static Task SetCfgMsg(this IUbxDevice src, UbxCfgMsg msg, CancellationToken cancel = default)
        {
            return src.Push(msg, cancel);
        }

        public static Task SetMessageRate(this IUbxDevice src, byte msgClass, byte msgSubClass, byte msgRate, CancellationToken cancel = default)
        {
            return src.SetCfgMsg(new UbxCfgMsg{MsgClass = msgClass,MsgId = msgSubClass,CurrentPortRate = msgRate}, cancel);
        }
        
        public static Task SetMessageRate<TMsg>(this IUbxDevice src, byte msgRate, CancellationToken cancel = default) 
            where TMsg : UbxMessageBase, new()
        {
            var msg = new TMsg();
            return src.SetCfgMsg(new UbxCfgMsg{MsgClass = msg.Class,MsgId = msg.SubClass,CurrentPortRate = msgRate}, cancel);
        }

        #endregion

        #region CfgNav5

        public static Task<UbxCfgNav5> GetCfgNav5(this IUbxDevice src, CancellationToken cancel = default)
        {
            return src.Pool<UbxCfgNav5, UbxCfgNav5Pool>(new UbxCfgNav5Pool(), cancel);
        }
        public static Task SetCfgNav5(this IUbxDevice src, UbxCfgNav5 value, CancellationToken cancel = default)
        {
            return src.Push(value, cancel);
        }

        #endregion

        #region RTCM rate

        public static async Task SetupRtcmMSM4Rate(this IUbxDevice src, byte msgRate, CancellationToken cancel)
        {
            // 1074
            await src.SetMessageRate((byte)UbxHelper.ClassIDs.RTCM3, 0x4A, msgRate, cancel);

            // 1084
            await src.SetMessageRate((byte)UbxHelper.ClassIDs.RTCM3, 0x54, msgRate, cancel);

            // 1094
            await src.SetMessageRate((byte)UbxHelper.ClassIDs.RTCM3, 0x5E, msgRate, cancel);

            // 1124
            await src.SetMessageRate((byte)UbxHelper.ClassIDs.RTCM3, 0x7C, msgRate, cancel);
        }

        public static async Task SetupRtcmMSM7Rate(this IUbxDevice src, byte msgRate, CancellationToken cancel)
        {
            // 1077
            await src.SetMessageRate((byte)UbxHelper.ClassIDs.RTCM3, 0x4D, msgRate, cancel);

            // 1087
            await src.SetMessageRate((byte)UbxHelper.ClassIDs.RTCM3, 0x57, msgRate, cancel);

            // 1097
            await src.SetMessageRate((byte)UbxHelper.ClassIDs.RTCM3, 0x61, msgRate, cancel);

            // 1127
            await src.SetMessageRate((byte)UbxHelper.ClassIDs.RTCM3, 0x7F, msgRate, cancel);
        }

        #endregion

        #region CfgCfg

        public static Task CallCfgClear(this IUbxDevice src,CancellationToken cancel = default)
        {
            return src.Push(new UbxCfgCfg
            {
                ClearMask = UbxCfgSection.All,
                SaveMask = UbxCfgSection.None,
                LoadMask = UbxCfgSection.None,
                DeviceMask = UbxCfgDeviceMask.DevBbr
            },cancel);
        }

        public static Task CallCfgSave(this IUbxDevice src, CancellationToken cancel = default)
        {
            return src.Push(new UbxCfgCfg
            {
                ClearMask = UbxCfgSection.All,
                SaveMask = UbxCfgSection.All,
                LoadMask = UbxCfgSection.None,
                DeviceMask = UbxCfgDeviceMask.DevBbr
            }, cancel);
        }
        public static Task CallCfgLoad(this IUbxDevice src, CancellationToken cancel = default)
        {
            return src.Push(new UbxCfgCfg
            {
                ClearMask = UbxCfgSection.None,
                SaveMask = UbxCfgSection.None,
                LoadMask = UbxCfgSection.All,
                DeviceMask = UbxCfgDeviceMask.DevBbr
            }, cancel);
        }

        #endregion

        #region Reset 

        public static Task CallCfgReset(this IUbxDevice src, BbrMask navBbrMask = BbrMask.HotStart,
            ResetMode resetMode = ResetMode.HardwareResetImmediately, CancellationToken cancel = default)
        {
            return src.Push(new UbxCfgRst { Bbr = navBbrMask, Mode = resetMode }, cancel);
        }

        public static Task HardwareReset(this IUbxDevice src, CancellationToken cancel = default)
        {
            return src.CallCfgReset(BbrMask.HotStart, ResetMode.HardwareResetImmediately, cancel);
        }

        public static Task SoftwareGnssReset(this IUbxDevice src, CancellationToken cancel = default)
        {
            // return src.CallCfgReset(BbrMask.ColdStart, ResetMode.ControlledSoftwareResetGnssOnly, cancel);
            return src.Connection.Send(
                new UbxCfgRst { Bbr = BbrMask.ColdStart, Mode = ResetMode.ControlledSoftwareResetGnssOnly }, cancel);
        }

        #endregion

        #region Pool HW

        public static Task<UbxMonHw> GetMonHw(this IUbxDevice src, CancellationToken cancel = default)
        {
            return src.Pool<UbxMonHw, UbxMonHwPool>(new UbxMonHwPool(),cancel);
        }

        #endregion

        #region GetCfgTMode

        public static Task<UbxCfgTMode3> GetCfgTMode3(this IUbxDevice src,CancellationToken cancel = default)
        {
            return src.Pool<UbxCfgTMode3, UbxCfgTMode3Pool>(new UbxCfgTMode3Pool(),cancel);
        }

        public static Task SetCfgTMode3(this IUbxDevice src, UbxCfgTMode3 value, CancellationToken cancel = default)
        {
            return src.Push(value, cancel);
        }

        public static Task SetSurveyInMode(this IUbxDevice src, uint minDuration = 60, double positionAccuracyLimit = 10, CancellationToken cancel = default)
        {
            return src.SetCfgTMode3(new UbxCfgTMode3
            {
                Mode = TMode3Enum.SurveyIn, 
                FixedPosition3DAccuracy = 0.0,
                SurveyInMinDuration = minDuration,
                SurveyInPositionAccuracyLimit = positionAccuracyLimit,
            },cancel);
        }

        public static Task SetFixedBaseMode(this IUbxDevice src, GeoPoint position, double position3DAccuracy = 0.0001, CancellationToken cancel = default)
        {
            return src.SetCfgTMode3(new UbxCfgTMode3
            {
                Mode = TMode3Enum.FixedMode,
                IsGivenInLLA = true,
                FixedPosition3DAccuracy = position3DAccuracy,
                SurveyInPositionAccuracyLimit = 0.2,
                Location = position,
            }, cancel);
        }

        #endregion

        public static Task<UbxNavSat> GetNavSat(this IUbxDevice src, CancellationToken cancel = default)
        {
            return src.Pool<UbxNavSat, UbxNavSatPool>(new UbxNavSatPool(), cancel);
        }

        public static Task<UbxNavPvt> GetNavPvt(this IUbxDevice src, CancellationToken cancel = default)
        {
            return src.Pool<UbxNavPvt, UbxNavPvtPool>(new UbxNavPvtPool(), cancel);
        }

        public static async Task SetStationaryMode(this IUbxDevice src, bool movingBase, byte msgRate, CancellationToken cancel = default)
        {
            if (!movingBase)
            {
                await src.SetCfgNav5(new UbxCfgNav5
                {
                    PlatformModel = UbxCfgNav5.ModelEnum.Stationary,
                    PositionMode = UbxCfgNav5.PositionModeEnum.Auto
                }, cancel);

                // 4072
                await src.SetMessageRate((byte)UbxHelper.ClassIDs.RTCM3, 0xFE, msgRate, cancel);
            }
            else
            {
                await src.SetCfgNav5(new UbxCfgNav5
                {
                    PlatformModel = UbxCfgNav5.ModelEnum.Portable,
                    PositionMode = UbxCfgNav5.PositionModeEnum.Auto
                }, cancel);

                // 4072
                await src.SetMessageRate((byte)UbxHelper.ClassIDs.RTCM3, 0xFE, 0, cancel);
            }
        }

        public static async Task TurnOffNmea(this IUbxDevice src, CancellationToken cancel = default)
        {
            // turn off all nmea
            for (var a = 0; a <= 0xf; a++)
            {
                if (a is 0x0B or 0x0C or 0x0E)
                    continue;
                await src.SetMessageRate((byte)UbxHelper.ClassIDs.NMEA, (byte)a, 0, cancel);
            }
        }

        public static async Task RebootReceiver(this IUbxDevice src, CancellationToken cancel = default)
        {
            await src.CallCfgSave(cancel);
            await src.SoftwareGnssReset(cancel);
            while (true)
            {
                try
                {
                    var value = await src.GetMonHw(cancel);
                    break;
                }
                catch (UbxDeviceTimeoutException)
                {
                    await Task.Delay(TimeSpan.FromSeconds(1), cancel);
                }
            }
            await src.CallCfgLoad(cancel);
        }

        public static async Task SetupByDefault(this IUbxDevice src, bool movingBase = false, byte messageRate = 1, CancellationToken cancel = default)
        {
            await src.SetCfgPort(new UbxCfgPrt
            {
                Config = new UbxCfgPrtConfigUart
                {
                    PortId = 1, 
                    BoundRate = 115200
                }
            }, cancel: cancel);
            await src.SetCfgRate(new UbxCfgRate
            {
                RateHz = 1.0
            }, cancel);

            await src.SetStationaryMode(movingBase, messageRate, cancel);

            await src.TurnOffNmea(cancel);
            
            // surveyin msg - for feedback
            await src.SetMessageRate((byte)UbxHelper.ClassIDs.NAV, 0x3B, 1, cancel);
            
            // pvt msg - for feedback
            await src.SetMessageRate((byte)UbxHelper.ClassIDs.NAV, 0x07, 1, cancel);
            
            // 1005 - 5s
            await src.SetMessageRate((byte)UbxHelper.ClassIDs.RTCM3, 0x05, 5, cancel);
            
            await src.SetupRtcmMSM4Rate(messageRate, cancel);
            await src.SetupRtcmMSM7Rate(0, cancel);
            
            // 1230 - 5s
            await src.SetMessageRate((byte)UbxHelper.ClassIDs.RTCM3, 0xE6, 5, cancel);
            
            // NAV-VELNED - 1s
            await src.SetMessageRate((byte)UbxHelper.ClassIDs.NAV, 0x12, 1, cancel);
            
            // rxm-raw/rawx - 1s
            await src.SetMessageRate((byte)UbxHelper.ClassIDs.RXM, 0x15, 1, cancel);
            //await SetMessageRate((byte)UbxHelper.ClassIDs.RXM, 0x10, 1, cancel);
            
            // rxm-sfrb/sfrb - 2s
            await src.SetMessageRate((byte)UbxHelper.ClassIDs.RXM, 0x13, 2, cancel);
            //await SetMessageRate((byte)UbxHelper.ClassIDs.RXM, 0x11, 2, cancel);
            
            // mon-hw - 2s
            await src.SetMessageRate((byte)UbxHelper.ClassIDs.MON, 0x09, 2, cancel);
        }
    }
}