using System;
using Asv.IO;

namespace Asv.Gnss
{
    public class SbfPacketReceiverStatusRev1 : SbfMessageBase
    {
        public override ushort MessageRevision => 1;
        public override ushort MessageType => 4014;

        public override string Name => "ReceiverStatusV2";


        protected override void DeserializeContent(ref ReadOnlySpan<byte> buffer)
        {
            CPULoad = BinSerialize.ReadByte(ref buffer);
            ExtError = (SbfExtErrorEnum)BinSerialize.ReadByte(ref buffer);
            UpTime = TimeSpan.FromSeconds(BinSerialize.ReadUInt(ref buffer));
            RxState = (SbfRxStateEnum)BinSerialize.ReadUInt(ref buffer);
            RxError = (SbfRxErrorEnum)BinSerialize.ReadUInt(ref buffer);
            var N = BinSerialize.ReadByte(ref buffer); ;
            var sbLength = BinSerialize.ReadByte(ref buffer); ;
            CmdCount = BinSerialize.ReadByte(ref buffer); ;
            Temperature = BinSerialize.ReadByte(ref buffer); ;
            AGCState = new SbfReceiverStatusAGCState[N];
            for (var i = 0; i < N; i++)
            {
                AGCState[i] = new SbfReceiverStatusAGCState();
                AGCState[i].Deserialize(ref buffer, sbLength);
            }
        }

        public SbfReceiverStatusAGCState[] AGCState { get; set; }

        /// <summary>
        /// Receiver temperature with an offset of 100. Remove 100 to get the temperature in degree Celsius.
        /// </summary>
        public int Temperature { get; set; }
        /// <summary>
        /// Command cyclic counter, incremented each time a command is entered
        /// that changes the receiver configuration.After the counter has reached
        /// 255, it resets to 1.
        /// </summary>
        public byte CmdCount { get; set; }

        /// <summary>
        /// Bit field indicating whether an error occurred previously. If this field is not
        /// equal to zero, at least one error has been detected.
        /// </summary>
        public SbfRxErrorEnum RxError { get; set; }

        /// <summary>
        /// Bit field indicating the status of key components of the receiver
        /// </summary>
        public SbfRxStateEnum RxState { get; set; }

        /// <summary>
        /// Number of seconds elapsed since the start-up of the receiver, or since the last reset.
        /// </summary>
        public TimeSpan UpTime { get; set; }

        /// <summary>
        /// Bit field reporting external errors, i.e. errors detected in external data.
        /// Upon detection of an error, the corresponding bit is set for a duration of
        /// one second, and then resets.
        /// </summary>
        public SbfExtErrorEnum ExtError { get; set; }

        /// <summary>
        /// Load on the receiver’s CPU. The load should stay below 80% in normal
        /// operation.Higher loads might result in data loss.
        /// </summary>
        public byte CPULoad { get; set; }
    }

    public class SbfReceiverStatusAGCState
    {
        public void Deserialize(ref ReadOnlySpan<byte> buffer,  byte sbLength)
        {
            var FrontEndID = BinSerialize.ReadByte(ref buffer);
            FrontEndCode = (SbfFrontEndCodeEnum) (FrontEndID & 0b0001_1111);
            Antenna = (AntennaId) (FrontEndID >> 5);
            Gain = (sbyte)BinSerialize.ReadByte(ref buffer);
            SampleVar = BinSerialize.ReadByte(ref buffer);
            BlankingStat = BinSerialize.ReadByte(ref buffer);
        }

        /// <summary>
        /// Current percentage of samples being blanked by the pulse blanking unit.
        /// This field is always 0 for receiver without pulse blanking unit.
        /// </summary>
        public byte BlankingStat { get; set; }

        /// <summary>
        /// Normalized variance of the IF samples. The nominal value for this variance is 100.
        /// </summary>
        public byte SampleVar { get; set; }
        /// <summary>
        /// AGC gain, in dB.
        /// The Do-Not-Use value is used to indicate that the frontend PLL is
        /// not locked.
        /// </summary>
        public sbyte Gain { get; set; }

        public AntennaId Antenna { get; set; }

        public SbfFrontEndCodeEnum FrontEndCode { get; set; }

    }

    public enum SbfFrontEndCodeEnum :byte
    {
        GPSL1_E1 = 0,
        GLOL1 = 1,
        E6 = 2,
        GPSL2 = 3,
        GLOL2 = 4,
        L5_E5a = 5,
        E5b_B2I = 6,
        E5_a_b = 7 ,
        Combined_GPS_GLONASS_SBAS_Galileo_L1 = 8,
        Combined_GPS_GLONASS_L2 = 9,
        MSS_Lband = 10,
        B1I = 11,
        B3I = 12,
        Sband = 13,
    }

    [Flags]
    public enum SbfRxErrorEnum : UInt32
    {
        Reserved1,
        Reserved2,
        Reserved3,
        /// <summary>
        /// SOFTWARE:  set upon detection of a software warning or error. This bit is reset by the command "lif, error".
        /// </summary>
        SOFTWARE,
        /// <summary>
        /// WATCHDOG:  set when the watchdog expired at least once since the last power-on.
        /// </summary>
        WATCHDOG,
        /// <summary>
        /// ANTENNA:  set when antenna overcurrent condition is detected.
        /// </summary>
        ANTENNA,
        /// <summary>
        /// CONGESTION:  set when an output data congestion has been detected on at least one of the communication ports of the receiver during the last second.
        /// </summary>
        CONGESTION,

        Reserved4,
        /// <summary>
        /// MISSEDEVENT:  set when an external event congestion has been detected during the last second. It indicates that the receiver is receiving too many events on its EVENTx pins.
        /// </summary>
        MISSEDEVENT,
        /// <summary>
        /// CPUOVERLOAD:  set when the CPU load is larger than 90%.
        /// </summary>
        CPUOVERLOAD,
        /// <summary>
        /// INVALIDCONFIG:  set if one or more configuration file (e.g. permissions) is invalid or absent.
        /// </summary>
        INVALIDCONFIG,
        /// <summary>
        /// OUTOFGEOFENCE:  set if the receiver is currently out of its permitted region of operation (geofencing).
        /// </summary>
        OUTOFGEOFENCE,
    }

    [Flags]
    public enum SbfRxStateEnum:UInt32
    {
        Reserved,
        /// <summary>
        /// ACTIVEANTENNA:  this bit is set when an active antenna is sensed at the main antenna connector. This functionality is only available on certain receiver models.
        /// </summary>
        ACTIVEANTENNA,
        /// <summary>
        /// EXT_FREQ:  this bit is set if an external frequency reference is detected at the 10 MHz input, and cleared if the receiver uses its own internal clock.
        /// </summary>
        EXT_FREQ,
        /// <summary>
        /// EXT_TIME:  this bit is set if a pulse has been detected on the TimeSync input.
        /// </summary>
        EXT_TIME,
        /// <summary>
        /// WNSET:  see corresponding bit in the SyncLevel field of the ReceiverTime block.
        /// </summary>
        WNSET,
        /// <summary>
        /// TOWSET:  see corresponding bit in the SyncLevel field of the ReceiverTime block.
        /// </summary>
        TOWSET,
        /// <summary>
        /// FINETIME:  see corresponding bit in the SyncLevel field of the ReceiverTime block.
        /// </summary>
        FINETIME,
        /// <summary>
        /// INTERNALDISK_ACTIVITY:  this bit is set for one second each time data is logged to the internal disk (DSK1). If the logging rate is larger than 1 Hz, set continuously.
        /// </summary>
        INTERNALDISK_ACTIVITY,
        /// <summary>
        /// INTERNALDISK_FULL:  this bit is set when the internal disk (DSK1) is full. A disk is full when it is filled to 95% of its total capacity.
        /// </summary>
        INTERNALDISK_FULL,
        /// <summary>
        /// INTERNALDISK_MOUNTED:  this bit is set when the internal disk (DSK1) is mounted.
        /// </summary>
        INTERNALDISK_MOUNTED,
        /// <summary>
        /// INT_ANT:  this bit is set when the GNSS RF signal is taken from the internal antenna input, and cleared when it comes from the external antenna input (only applicable on receiver models featuring an internal antenna input).
        /// </summary>
        INT_ANT,
        /// <summary>
        /// REFOUT_LOCKED:  if set, the 10-MHz frequency provided at the REF OUT connector is locked to GNSS time. Otherwise it is freerunning.
        /// </summary>
        REFOUT_LOCKED,
        /// <summary>
        /// LBAND_ANT:  this bit is set when the L-band signal is tracked from the dedicated L-band antenna, and cleared when it is tracked from the same antenna as the GNSS signals, or when the receiver does not support L-band tracking.
        /// </summary>
        LBAND_ANT,
        /// <summary>
        /// EXTERNALDISK_ACTIVITY:  this bit is set for one second each time data is logged to the external disk (DSK2). If the logging rate is larger than 1 Hz, set continuously.
        /// </summary>
        EXTERNALDISK_ACTIVITY,
        /// <summary>
        /// EXTERNALDISK_FULL:  this bit is set when the external disk (DSK2) is full. A disk is full when it is filled to 95% of its total capacity.
        /// </summary>
        EXTERNALDISK_FULL,
        /// <summary>
        /// EXTERNALDISK_MOUNTED:  this bit is set when the external disk (DSK2) is mounted.
        /// </summary>
        EXTERNALDISK_MOUNTED,
        /// <summary>
        /// PPS_IN_CAL:  this bit is set when PPS IN delay calibration is ongoing. Only applicable to PolaRx5TR receivers.
        /// </summary>
        PPS_IN_CAL,
        /// <summary>
        /// DIFFCORR_IN:  this bit is set for one second each time differential corrections are decoded. If the input rate is larger than 1 Hz, set continuously.
        /// </summary>
        DIFFCORR_IN,
        /// <summary>
        /// INTERNET:  this bit is set when the receiver has internet access. If not set, there is either no internet access, or the receiver could not reliably determine the status.
        /// </summary>
        INTERNET,

    }

    [Flags]
    public enum SbfExtErrorEnum :byte
    {
        /// <summary>
        /// SISERROR: set if a violation of the signal-in-space ICD has been
        /// detected for at least one satellite while that satellite is reported
        /// as healthy. Use the command "lif,SisError" for details
        /// </summary>
        SISERROR,
        /// <summary>
        /// DIFFCORRERROR: set when an anomaly has been detected
        /// in an incoming differential correction stream, causing the receiver to fail to decode the corrections. Use the command
        /// "lif,DiffCorrError" for details
        /// </summary>
        DIFFCORRERROR,
        /// <summary>
        /// EXTSENSORERROR: set when a malfunction has been detected
        /// on at least one of the external sensors connected to the receiver. Use the command "lif, ExtSensorError" for details.
        /// </summary>
        EXTSENSORERROR,
        /// <summary>
        /// SETUPERROR: set when a configuration/setup error has been
        /// detected. An example of such error is when a remote
        /// NTRIP Caster is not reachable. Use the command "lif,
        /// SetupError" for details.
        /// </summary>
        SETUPERROR,
        Reserved1,
        Reserved2,
        Reserved3,
        Reserved4,

    }
}