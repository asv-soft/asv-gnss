using System;
using System.Threading;
using System.Threading.Tasks;
using Asv.Common;

namespace Asv.Gnss
{
    /// <summary>
    /// Gets the software and hardware version information of the device.
    /// </summary>
    /// <returns>A Task that represents the asynchronous operation. The task result contains the UbxMonVer object.</returns>
    public static class UbxDeviceHelper
    {
        #region MonVer

        /// <summary>
        /// Retrieves the MON-VER message from the given IUbxDevice asynchronously.
        /// </summary>
        /// <param name="src">The IUbxDevice to retrieve the MON-VER message from.</param>
        /// <param name="cancel">A CancellationToken to cancel the operation (optional, default is CancellationToken.None).</param>
        /// <returns>A Task of type UbxMonVer representing the MON-VER message.</returns>
        public static Task<UbxMonVer> GetMonVer(
            this IUbxDevice src,
            CancellationToken cancel = default
        )
        {
            return src.Pool<UbxMonVer, UbxMonVerPool>(new UbxMonVerPool(), cancel);
        }

        #endregion

        #region CfgPort

        /// <summary>
        /// Retrieves the configuration port for the specified port ID.
        /// </summary>
        /// <param name="src">The IUbxDevice instance.</param>
        /// <param name="portId">The port ID to retrieve the configuration port for.</param>
        /// <param name="cancel">A CancellationToken to cancel the operation (optional).</param>
        /// <returns>A Task representing the operation, which will return an instance of UbxCfgPrt.</returns>
        public static Task<UbxCfgPrt> GetCfgPort(
            this IUbxDevice src,
            byte portId,
            CancellationToken cancel = default
        )
        {
            return src.Pool<UbxCfgPrt, UbxCfgPrtPool>(
                new UbxCfgPrtPool { PortId = portId },
                cancel
            );
        }

        /// <summary>
        /// Sets the configuration port of the IUbxDevice object.
        /// </summary>
        /// <param name="src">The IUbxDevice object for which the configuration port is to be set.</param>
        /// <param name="value">The configuration port value to set.</param>
        /// <param name="cancel">The cancellation token (optional).</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        public static Task SetCfgPort(
            this IUbxDevice src,
            UbxCfgPrt value,
            CancellationToken cancel = default
        )
        {
            return src.Push(value, cancel);
        }

        #endregion

        #region CfgAntenna

        /// <summary>
        /// Gets the configuration of the antenna.
        /// </summary>
        /// <param name="src">The UBX device.</param>
        /// <param name="cancel">Cancellation token to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the configuration of the antenna.</returns>
        public static Task<UbxCfgAnt> GetCfgAntenna(
            this IUbxDevice src,
            CancellationToken cancel = default
        )
        {
            return src.Pool<UbxCfgAnt, UbxCfgAntPool>(new UbxCfgAntPool(), cancel);
        }

        /// <summary>
        /// Sets the configuration for the antenna on a UBX device.
        /// </summary>
        /// <param name="src">The UBX device from which to send the configuration.</param>
        /// <param name="value">The configuration value to be set for the antenna.</param>
        /// <param name="cancel">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static Task SetCfgAntenna(
            this IUbxDevice src,
            UbxCfgAnt value,
            CancellationToken cancel = default
        )
        {
            return src.Push(value, cancel);
        }

        #endregion

        #region CfgRate

        /// <summary>
        /// Retrieves the configuration rate from the specified IUbxDevice.
        /// </summary>
        /// <param name="src">The IUbxDevice to retrieve the rate from.</param>
        /// <param name="cancel">Cancellation token to cancel the operation (optional).</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an instance of UbxCfgRate.</returns>
        public static Task<UbxCfgRate> GetCfgRate(
            this IUbxDevice src,
            CancellationToken cancel = default
        )
        {
            return src.Pool<UbxCfgRate, UbxCfgRatePool>(new UbxCfgRatePool(), cancel);
        }

        /// <summary>
        /// Sets the configuration rate for a UBX device.
        /// </summary>
        /// <param name="src">The UBX device.</param>
        /// <param name="value">The configuration rate to set.</param>
        /// <param name="cancel">The cancellation token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static Task SetCfgRate(
            this IUbxDevice src,
            UbxCfgRate value,
            CancellationToken cancel = default
        )
        {
            return src.Push(value, cancel);
        }

        #endregion

        #region CfgMsg

        /// <summary>
        /// Retrieves a configuration message from the UBX device.
        /// </summary>
        /// <param name="src">The UBX device.</param>
        /// <param name="msgClass">The message class.</param>
        /// <param name="msgSubClass">The message subclass.</param>
        /// <param name="cancel">The optional cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation, returning the retrieved configuration message.</returns>
        public static Task<UbxCfgMsg> GetCfgMsg(
            this IUbxDevice src,
            byte msgClass,
            byte msgSubClass,
            CancellationToken cancel = default
        )
        {
            return src.Pool<UbxCfgMsg, UbxCfgMsgPool>(
                new UbxCfgMsgPool { MsgClass = msgClass, MsgId = msgSubClass },
                cancel
            );
        }

        /// <summary>
        /// Sets the configuration message on the given IUbxDevice.
        /// </summary>
        /// <param name="src">The IUbxDevice to set the configuration message on.</param>
        /// <param name="msg">The configuration message to set.</param>
        /// <param name="cancel">Optional cancellation token.</param>
        /// <returns>A Task that represents the asynchronous operation of setting the configuration message.</returns>
        public static Task SetCfgMsg(
            this IUbxDevice src,
            UbxCfgMsg msg,
            CancellationToken cancel = default
        )
        {
            return src.Push(msg, cancel);
        }

        /// <summary>
        /// Sets the message rate for a specific message class and subclass.
        /// </summary>
        /// <param name="src">The IUbxDevice interface implementation.</param>
        /// <param name="msgClass">The message class.</param>
        /// <param name="msgSubClass">The message subclass.</param>
        /// <param name="msgRate">The desired message rate.</param>
        /// <param name="cancel">The CancellationToken for cancellation support. Defaults to default(CancellationToken).</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static Task SetMessageRate(
            this IUbxDevice src,
            byte msgClass,
            byte msgSubClass,
            byte msgRate,
            CancellationToken cancel = default
        )
        {
            return src.SetCfgMsg(
                new UbxCfgMsg
                {
                    MsgClass = msgClass,
                    MsgId = msgSubClass,
                    CurrentPortRate = msgRate,
                },
                cancel
            );
        }

        /// <summary>
        /// Sets the message rate for the specified message type on the given UBX device.
        /// </summary>
        /// <typeparam name="TMsg">The type of UBX message.</typeparam>
        /// <param name="src">The UBX device.</param>
        /// <param name="msgRate">The desired message rate.</param>
        /// <param name="cancel">The cancellation token (optional).</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static Task SetMessageRate<TMsg>(
            this IUbxDevice src,
            byte msgRate,
            CancellationToken cancel = default
        )
            where TMsg : UbxMessageBase, new()
        {
            var msg = new TMsg();
            return src.SetCfgMsg(
                new UbxCfgMsg
                {
                    MsgClass = msg.Class,
                    MsgId = msg.SubClass,
                    CurrentPortRate = msgRate,
                },
                cancel
            );
        }

        #endregion

        #region CfgNav5

        /// <summary>
        /// Gets the CfgNav5 configuration from the UBX device.
        /// </summary>
        /// <param name="src">The UBX device.</param>
        /// <param name="cancel">Cancellation token (optional).</param>
        /// <returns>The CfgNav5 configuration.</returns>
        public static Task<UbxCfgNav5> GetCfgNav5(
            this IUbxDevice src,
            CancellationToken cancel = default
        )
        {
            return src.Pool<UbxCfgNav5, UbxCfgNav5Pool>(new UbxCfgNav5Pool(), cancel);
        }

        /// <summary>Sets the configuration for the navigation engine 5 (CFG-NAV5) for the specified IUbxDevice.</summary> <param name="src">The IUbxDevice instance that the configuration should be set for.</param> <param name="value">The UbxCfgNav5 value representing the configuration settings.</param> <param name="cancel">The CancellationToken used to cancel the task (optional).</param> <returns>A Task representing the asynchronous operation.</returns>
        /// /
        public static Task SetCfgNav5(
            this IUbxDevice src,
            UbxCfgNav5 value,
            CancellationToken cancel = default
        )
        {
            return src.Push(value, cancel);
        }

        #endregion

        #region RTCM rate

        /// <summary>
        /// Set up the RTCM MSM4 rate for the given IUbxDevice.
        /// </summary>
        /// <param name="src">The IUbxDevice to set up RTCM MSM4 rate for.</param>
        /// <param name="msgRate">The message rate to set.</param>
        /// <param name="cancel">A cancellation token to cancel the operation.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public static async Task SetupRtcmMSM4Rate(
            this IUbxDevice src,
            byte msgRate,
            CancellationToken cancel
        )
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

        /// <summary>
        /// Sets up the rate of MSM7 messages for the specified UBX device.
        /// </summary>
        /// <param name="src">The UBX device.</param>
        /// <param name="msgRate">The rate of MSM7 messages to be set.</param>
        /// <param name="cancel">The cancellation token.</param>
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        public static async Task SetupRtcmMSM7Rate(
            this IUbxDevice src,
            byte msgRate,
            CancellationToken cancel
        )
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

        /// <summary>
        /// Clears the configuration of the device.
        /// </summary>
        /// <param name="src">The device on which to clear the configuration.</param>
        /// <param name="cancel">The cancellation token.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public static Task CallCfgClear(this IUbxDevice src, CancellationToken cancel = default)
        {
            return src.Push(
                new UbxCfgCfg
                {
                    ClearMask = UbxCfgSection.All,
                    SaveMask = UbxCfgSection.None,
                    LoadMask = UbxCfgSection.None,
                    DeviceMask = UbxCfgDeviceMask.DevBbr,
                },
                cancel
            );
        }

        /// <summary>
        /// Saves the call configuration to the device.
        /// </summary>
        /// <param name="src">The IUbxDevice object representing the device.</param>
        /// <param name="cancel">Optional cancellation token to cancel the operation.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        /// <remarks>
        /// This method saves the call configuration by pushing a <see cref="UbxCfgCfg"/> object to the device.
        /// The <see cref="UbxCfgCfg"/> object is initialized with the following parameters:
        /// - ClearMask: <see cref="UbxCfgSection.All"/> (indicating to clear all sections before saving)
        /// - SaveMask: <see cref="UbxCfgSection.All"/> (indicating to save all sections)
        /// - LoadMask: <see cref="UbxCfgSection.None"/> (indicating to not load any sections)
        /// - DeviceMask: <see cref="UbxCfgDeviceMask.DevBbr"/> (indicating to save the configuration to the BBR memory).
        /// </remarks>
        public static Task CallCfgSave(this IUbxDevice src, CancellationToken cancel = default)
        {
            return src.Push(
                new UbxCfgCfg
                {
                    ClearMask = UbxCfgSection.All,
                    SaveMask = UbxCfgSection.All,
                    LoadMask = UbxCfgSection.None,
                    DeviceMask = UbxCfgDeviceMask.DevBbr,
                },
                cancel
            );
        }

        /// <summary>
        /// Loads the configuration of the specified device.
        /// </summary>
        /// <param name="src">The source device to load the configuration.</param>
        /// <param name="cancel">The cancellation token to cancel the operation (optional).</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the result of the configuration load operation.
        /// </returns>
        public static Task CallCfgLoad(this IUbxDevice src, CancellationToken cancel = default)
        {
            return src.Push(
                new UbxCfgCfg
                {
                    ClearMask = UbxCfgSection.None,
                    SaveMask = UbxCfgSection.None,
                    LoadMask = UbxCfgSection.All,
                    DeviceMask = UbxCfgDeviceMask.DevBbr,
                },
                cancel
            );
        }

        #endregion

        #region Reset

        /// <summary>
        /// Resets the configuration of the device.
        /// </summary>
        /// <param name="src">The IUbxDevice instance.</param>
        /// <param name="navBbrMask">The BBR mask to use for resetting the navigation data.</param>
        /// <param name="resetMode">The reset mode to use.</param>
        /// <param name="cancel">The cancellation token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static Task CallCfgReset(
            this IUbxDevice src,
            BbrMask navBbrMask = BbrMask.HotStart,
            ResetMode resetMode = ResetMode.HardwareResetImmediately,
            CancellationToken cancel = default
        )
        {
            return src.Push(new UbxCfgRst { Bbr = navBbrMask, Mode = resetMode }, cancel);
        }

        /// <summary>
        /// HardwareReset method.
        /// </summary>
        /// <param name="src">The IUbxDevice object.</param>
        /// <param name="cancel">Optional cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static Task HardwareReset(this IUbxDevice src, CancellationToken cancel = default)
        {
            return src.CallCfgReset(BbrMask.HotStart, ResetMode.HardwareResetImmediately, cancel);
        }

        /// <summary>
        /// Resets the GNSS module using a controlled software reset.
        /// </summary>
        /// <param name="src">The IUbxDevice instance representing the GNSS module.</param>
        /// <param name="cancel">The CancellationToken used to cancel the operation.</param>
        /// <returns>A Task representing the asynchronous reset operation.</returns>
        public static Task SoftwareGnssReset(
            this IUbxDevice src,
            CancellationToken cancel = default
        )
        {
            // return src.CallCfgReset(BbrMask.ColdStart, ResetMode.ControlledSoftwareResetGnssOnly, cancel);
            return src.Connection.Send(
                new UbxCfgRst
                {
                    Bbr = BbrMask.ColdStart,
                    Mode = ResetMode.ControlledSoftwareResetGnssOnly,
                },
                cancel
            );
        }

        #endregion

        #region Pool HW

        /// <summary>
        /// Retrieves the UbxMonHw object from the specified IUbxDevice instance.
        /// </summary>
        /// <param name="src">The IUbxDevice instance.</param>
        /// <param name="cancel">Optional cancellation token.</param>
        /// <returns>The UbxMonHw object.</returns>
        public static Task<UbxMonHw> GetMonHw(
            this IUbxDevice src,
            CancellationToken cancel = default
        )
        {
            return src.Pool<UbxMonHw, UbxMonHwPool>(new UbxMonHwPool(), cancel);
        }

        #endregion

        #region GetCfgTMode

        /// <summary>
        /// Retrieves the configuration of time mode 3 from the UBX device.
        /// </summary>
        /// <param name="src">The UBX device.</param>
        /// <param name="cancel">Optional cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an instance of <see cref="UbxCfgTMode3"/> which represents the configuration of time mode 3.</returns>
        /// <remarks>
        /// The method uses the <see cref="IUbxDevice.Pool{TRequest,TResponse}(IUbxPool{TRequest,TResponse}, CancellationToken)"/> method internally to retrieve the configured time mode 3 from
        /// the UBX device.
        /// </remarks>
        public static Task<UbxCfgTMode3> GetCfgTMode3(
            this IUbxDevice src,
            CancellationToken cancel = default
        )
        {
            return src.Pool<UbxCfgTMode3, UbxCfgTMode3Pool>(new UbxCfgTMode3Pool(), cancel);
        }

        /// <summary>
        /// Sets the configuration and time mode 3 for a given IUbxDevice.
        /// </summary>
        /// <param name="src">The IUbxDevice instance.</param>
        /// <param name="value">The UbxCfgTMode3 value to set.</param>
        /// <param name="cancel">The cancellation token (optional).</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static Task SetCfgTMode3(
            this IUbxDevice src,
            UbxCfgTMode3 value,
            CancellationToken cancel = default
        )
        {
            return src.Push(value, cancel);
        }

        /// <summary>
        /// Sets the survey-in mode of the device.
        /// </summary>
        /// <param name="src">The IUbxDevice object.</param>
        /// <param name="minDuration">The minimum duration of the survey-in process in seconds. Default value is 60 seconds.</param>
        /// <param name="positionAccuracyLimit">The position accuracy limit for the survey-in process in meters. Default value is 10 meters.</param>
        /// <param name="cancel">The cancellation token to cancel the operation. Default value is CancellationToken.None.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public static Task SetSurveyInMode(
            this IUbxDevice src,
            uint minDuration = 60,
            double positionAccuracyLimit = 10,
            CancellationToken cancel = default
        )
        {
            return src.SetCfgTMode3(
                new UbxCfgTMode3
                {
                    Mode = TMode3Enum.SurveyIn,
                    FixedPosition3DAccuracy = 0.0,
                    SurveyInMinDuration = minDuration,
                    SurveyInPositionAccuracyLimit = positionAccuracyLimit,
                },
                cancel
            );
        }

        /// <summary>
        /// Sets the device into Fixed Base mode with the specified position and parameters.
        /// </summary>
        /// <param name="src">The IUbxDevice instance to invoke the method on.</param>
        /// <param name="position">The fixed base position in <see cref="GeoPoint"/> format.</param>
        /// <param name="position3DAccuracy">The accuracy of the fixed base position in meters. Default is 0.0001 meters.</param>
        /// <param name="cancel">A cancellation token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static Task SetFixedBaseMode(
            this IUbxDevice src,
            GeoPoint position,
            double position3DAccuracy = 0.0001,
            CancellationToken cancel = default
        )
        {
            return src.SetCfgTMode3(
                new UbxCfgTMode3
                {
                    Mode = TMode3Enum.FixedMode,
                    IsGivenInLLA = true,
                    FixedPosition3DAccuracy = position3DAccuracy,
                    SurveyInPositionAccuracyLimit = 0.2,
                    Location = position,
                },
                cancel
            );
        }

        #endregion

        /// <summary>
        /// Retrieves the Navigation Satellite (NavSat) data from the specified UBX device.
        /// </summary>
        /// <param name="src">The UBX device to retrieve the NavSat data from.</param>
        /// <param name="cancel">Optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. The task result is an instance of <see cref="UbxNavSat"/> containing the NavSat data.</returns>
        public static Task<UbxNavSat> GetNavSat(
            this IUbxDevice src,
            CancellationToken cancel = default
        )
        {
            return src.Pool<UbxNavSat, UbxNavSatPool>(new UbxNavSatPool(), cancel);
        }

        /// <summary>
        /// Retrieves the navigation position velocity time solution (NAV-PVT) from the UBX device.
        /// </summary>
        /// <param name="src">The UBX device to retrieve the NAV-PVT from.</param>
        /// <param name="cancel">A cancellation token to cancel the operation if needed.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the NAV-PVT data.</returns>
        public static Task<UbxNavPvt> GetNavPvt(
            this IUbxDevice src,
            CancellationToken cancel = default
        )
        {
            return src.Pool<UbxNavPvt, UbxNavPvtPool>(new UbxNavPvtPool(), cancel);
        }

        /// <summary>
        /// Sets the stationary mode of the device.
        /// </summary>
        /// <param name="src">The IUbxDevice object.</param>
        /// <param name="movingBase">True if the device is in moving base mode, false otherwise.</param>
        /// <param name="msgRate">The message rate for RTCM3 messages.</param>
        /// <param name="cancel">The cancellation token.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public static async Task SetStationaryMode(
            this IUbxDevice src,
            bool movingBase,
            byte msgRate,
            CancellationToken cancel = default
        )
        {
            if (!movingBase)
            {
                await src.SetCfgNav5(
                    new UbxCfgNav5
                    {
                        PlatformModel = UbxCfgNav5.ModelEnum.Stationary,
                        PositionMode = UbxCfgNav5.PositionModeEnum.Auto,
                    },
                    cancel
                );

                // 4072
                await src.SetMessageRate((byte)UbxHelper.ClassIDs.RTCM3, 0xFE, 0, cancel);
            }
            else
            {
                await src.SetCfgNav5(
                    new UbxCfgNav5
                    {
                        PlatformModel = UbxCfgNav5.ModelEnum.Portable,
                        PositionMode = UbxCfgNav5.PositionModeEnum.Auto,
                    },
                    cancel
                );

                // 4072
                await src.SetMessageRate((byte)UbxHelper.ClassIDs.RTCM3, 0xFE, msgRate, cancel);
            }
        }

        /// <summary>
        /// Turns off all NMEA messages on the given IUbxDevice.
        /// </summary>
        /// <param name="src">The instance of IUbxDevice to turn off NMEA messages for.</param>
        /// <param name="cancel">The cancellation token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static async Task TurnOffNmea(
            this IUbxDevice src,
            CancellationToken cancel = default
        )
        {
            // turn off all nmea
            for (var a = 0; a <= 0xf; a++)
            {
                if (a is 0x0B or 0x0C or 0x0E)
                {
                    continue;
                }

                await src.SetMessageRate((byte)UbxHelper.ClassIDs.NMEA, (byte)a, 0, cancel);
            }
        }

        /// <summary>
        /// Reboots the receiver by calling various methods in a specific order.
        /// </summary>
        /// <param name="src">The IUbxDevice instance representing the receiver.</param>
        /// <param name="cancel">The cancellation token to cancel the operation if needed. Default is <see cref="CancellationToken.None"/>.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="UbxDeviceTimeoutException">Thrown when there is a timeout while calling <see cref="IUbxDevice.GetMonHw"/>.</exception>
        public static async Task RebootReceiver(
            this IUbxDevice src,
            CancellationToken cancel = default
        )
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

        /// <summary>
        /// Sets up the device with default configurations.
        /// </summary>
        /// <param name="src">The IUbxDevice instance.</param>
        /// <param name="movingBase">Indicates whether the device is in moving base mode.</param>
        /// <param name="messageRate">The message rate.</param>
        /// <param name="cancel">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static async Task SetupByDefault(
            this IUbxDevice src,
            bool movingBase = false,
            byte messageRate = 1,
            CancellationToken cancel = default
        )
        {
            await src.SetCfgPort(
                new UbxCfgPrt
                {
                    Config = new UbxCfgPrtConfigUart { PortId = 1, BoundRate = 115200 },
                },
                cancel: cancel
            );
            await src.SetCfgRate(new UbxCfgRate { RateHz = 1.0 }, cancel);

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

            // await SetMessageRate((byte)UbxHelper.ClassIDs.RXM, 0x10, 1, cancel);

            // rxm-sfrb/sfrb - 2s
            await src.SetMessageRate((byte)UbxHelper.ClassIDs.RXM, 0x13, 2, cancel);

            // await SetMessageRate((byte)UbxHelper.ClassIDs.RXM, 0x11, 2, cancel);

            // mon-hw - 2s
            await src.SetMessageRate((byte)UbxHelper.ClassIDs.MON, 0x09, 2, cancel);
        }
    }
}
