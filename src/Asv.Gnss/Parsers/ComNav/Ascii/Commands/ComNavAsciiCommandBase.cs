using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Asv.IO;

namespace Asv.Gnss
{
    public abstract class ComNavAsciiCommandBase : GnssMessageBase<string>
    {
        protected abstract string SerializeToAsciiString();

        public override string Name => MessageId;

        public override string ProtocolId => "ComNavAscii";

        public override string ToString()
        {
            return SerializeToAsciiString();
        }

        public override void Deserialize(ref ReadOnlySpan<byte> buffer)
        {
            throw new NotImplementedException();
        }

        public override void Serialize(ref Span<byte> buffer)
        {
            SerializeToAsciiString().CopyTo(ref buffer,Encoding.ASCII);
            BinSerialize.WriteByte(ref buffer, 0x0D);
            BinSerialize.WriteByte(ref buffer, 0x0A);
        }

        public override int GetByteSize()
        {
            return SerializeToAsciiString().Length + 2 /* END of message 0x0D & 0x0A */;
        }
    }



    public static class ComNavAsciiCommandHelper
    {
        public static Task<bool> SaveConfig(IGnssConnection src, CancellationToken cancel = default)
        {
            return src.Send(ComNavSaveConfigCommand.Default, cancel);
        }

        public static Task UnlockoutAllSystem(IGnssConnection src, CancellationToken cancel = default)
        {
            return src.Send(ComNavUnLockoutAllSystemCommand.Default, cancel);
        }

        public static Task UnLogAll(IGnssConnection src, CancellationToken cancel = default)
        {
	        return src.Send(ComNavUnLogAllCommand.Default, cancel);
        }

		public static Task SetLockoutSystem(IGnssConnection src, ComNavSatelliteSystemEnum system, CancellationToken cancel = default)
        {
            return src.Send(new ComNavSetLockoutSystemCommand
            {
                SatelliteSystem = system
            }, cancel);
        }

		public static Task SetUnLockoutSystem(IGnssConnection src, ComNavSatelliteSystemEnum system, CancellationToken cancel = default)
		{
			return src.Send(new ComNavSetUnLockoutSystemCommand
			{
				SatelliteSystem = system
			}, cancel);
		}
		public static Task SendDgpsStationId(IGnssConnection src, DgpsTxIdEnum type, byte id, CancellationToken cancel = default)
        {
            return src.Send(new ComNavDgpsTxIdCommand
            {
                Type = type,
                Id = id,
            }, cancel);
        }


        public static Task LogCommand(IGnssConnection src, ComNavMessageEnum message, string portName = default, ComNavFormat? format = default,
            ComNavTriggerEnum? trigger = default,
            uint? period = default,
            CancellationToken cancel = default)
        {
            return src.Send(new ComNavAsciiLogCommand
            {
                Type = message,
                Format = format,
                PortName = portName,
                Trigger = trigger,
                Period = period,

            }, cancel);
        }
        /// <summary>
        /// Configures the receiver to fix the height at the last calculated value if the number of
        /// satellites available is insufficient for a 3-D solution. This provides a 2-D solution.
        /// Height calculation resumes when the number of satellites available allows a 3-D solution
        /// </summary>
        public static Task FixAuto(IGnssConnection src, CancellationToken cancel = default)
        {
            return src.Send(new ComNavFixCommand
            {
                FixType = ComNavFixType.Auto,
            }, cancel);
        }
        /// <summary>
        /// Unfix. Clears any previous FIX commands
        /// </summary>
        /// <param name="src"></param>
        /// <param name="cancel"></param>
        /// <returns></returns>
        public static Task SendFixNone(IGnssConnection src, CancellationToken cancel = default)
        {
            return src.Send(new ComNavFixCommand
            {
                FixType = ComNavFixType.None,
            }, cancel);
        }
        /// <summary>
        /// Configures the receiver in 2-D mode with its height constrained to a given value. This
        /// command is used mainly in marine applications where height in relation to mean sea
        /// level may be considered to be approximately constant. The height entered using this
        /// command is referenced to the mean sea level, see the BESTPOS log on page 497
        /// (is in metres). The receiver is capable of receiving and applying differential
        /// corrections from a base station while fix height is in effect. The fix height command
        /// overrides any previous FIX HEIGHT or FIX POSITION command.
        /// Note: This command only affects pseudorange corrections and solutions.
        /// </summary>
        public static Task SendFixHeight(IGnssConnection src, double altitude, CancellationToken cancel = default)
        {
            return src.Send(new ComNavFixCommand
            {
                FixType = ComNavFixType.Height,
                Alt = altitude,
            }, cancel);
        }
        /// <summary>
        /// Configures the receiver with its position fixed. This command is used when it is
        /// necessary to generate differential corrections.
        /// For both pseudorange and differential corrections, this command must be properly
        /// initialized before the receiver can operate as a GNSS base station. Once initialized,
        /// the receiver computes differential corrections for each satellite being tracked. The
        /// computed differential corrections can then be output to rover stations using the
        /// RTCMV3 differential corrections data log format.S
        /// The values entered into the fix position command should reflect the precise position
        /// of the base station antenna phase center. Any errors in the fix position coordinates
        /// directly bias the corrections calculated by the base receiver
        /// 
        /// The receiver performs all internal computations based on WGS84 and the DATUM
        /// command (see page 131) is defaulted as such. The datum in which you choose to
        /// operate (by changing the DATUM command (see page 131)) is internally converted
        /// to and from WGS84. Therefore, all differential corrections are based on WGS84,
        /// regardless of your operating datum.
        ///
        /// The FIX POSITION command overrides any previous FIX HEIGHT or FIX
        /// POSITION command settings.
        /// </summary>
        public static Task FixPosition(IGnssConnection src, double latitude, double longitude, double altitude, CancellationToken cancel = default)
        {
            return src.Send(new ComNavFixCommand
            {
                FixType = ComNavFixType.Position,
                Lat = latitude,
                Lon = longitude,
                Alt = altitude,
            }, cancel);
        }
    }
}