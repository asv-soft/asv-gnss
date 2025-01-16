using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Asv.IO;

namespace Asv.Gnss
{
    /// <summary>
    /// Represents a base class for Communication Navigation ASCII commands.
    /// </summary>
    public abstract class ComNavAsciiCommandBase : GnssMessageBase<string>
    {
        /// <summary>
        /// Serializes the class into an ASCII string.
        /// </summary>
        /// <returns>The serialized ASCII string.</returns>
        protected abstract string SerializeToAsciiString();

        /// <summary>
        /// Gets the name of the message.
        /// </summary>
        public override string Name => MessageId;

        /// <summary>
        /// Gets the protocol identifier.
        /// </summary>
        public override string ProtocolId => "ComNavAscii";

        /// <summary>
        /// Converts the class into its string equivalent.
        /// </summary>
        /// <returns>A string that represents the object.</returns>
        public override string ToString()
        {
            return SerializeToAsciiString();
        }

        /// <summary>
        /// Deserializes the object from a buffer.
        /// </summary>
        /// <param name="buffer">The buffer to deserialize from.</param>
        /// <exception cref="NotImplementedException">Always thrown in this implementation.</exception>
        public override void Deserialize(ref ReadOnlySpan<byte> buffer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Serializes the object to a buffer.
        /// </summary>
        /// <param name="buffer">The buffer to serialize.</param>
        public override void Serialize(ref Span<byte> buffer)
        {
            SerializeToAsciiString().CopyTo(ref buffer, Encoding.ASCII);
            BinSerialize.WriteByte(ref buffer, 0x0D);
            BinSerialize.WriteByte(ref buffer, 0x0A);
        }

        /// <summary>
        /// Gets the byte size of the serialized object.
        /// </summary>
        /// <returns>The byte size of the serialized object.</returns>
        public override int GetByteSize()
        {
            return SerializeToAsciiString().Length
                + 2 /* END of message 0x0D & 0x0A */
            ;
        }
    }

    public static class ComNavAsciiCommandHelper
    {
        /// <summary>
        /// Sends a SaveConfig command to the given GNSS connection.
        /// </summary>
        /// <param name="src">The GNSS connection to which the command is sent.</param>
        /// <param name="cancel">An optional cancellation token.</param>
        /// <returns>A Task that represents the asynchronous operation. The Task's result is a boolean indicating whether the operation was completed successfully.</returns>
        public static Task<bool> SaveConfig(IGnssConnection src, CancellationToken cancel = default)
        {
            return src.Send(ComNavSaveConfigCommand.Default, cancel);
        }

        /// <summary>
        /// Sends an UnlockoutAllSystem command to the given GNSS connection.
        /// </summary>
        /// <param name="src">The GNSS connection to which the command is sent.</param>
        /// <param name="cancel">An optional cancellation token.</param>
        /// <returns>A Task that represents the asynchronous operation.</returns>
        public static Task UnlockoutAllSystem(
            IGnssConnection src,
            CancellationToken cancel = default
        )
        {
            return src.Send(ComNavUnLockoutAllSystemCommand.Default, cancel);
        }

        public static Task UnLogAll(IGnssConnection src, CancellationToken cancel = default)
        {
            return src.Send(ComNavUnLogAllCommand.Default, cancel);
        }

        public static Task SetLockoutSystem(
            IGnssConnection src,
            ComNavSatelliteSystemEnum system,
            CancellationToken cancel = default
        )
        {
            return src.Send(new ComNavSetLockoutSystemCommand { SatelliteSystem = system }, cancel);
        }

        public static Task SetUnLockoutSystem(
            IGnssConnection src,
            ComNavSatelliteSystemEnum system,
            CancellationToken cancel = default
        )
        {
            return src.Send(
                new ComNavSetUnLockoutSystemCommand { SatelliteSystem = system },
                cancel
            );
        }

        public static Task SendDgpsStationId(
            IGnssConnection src,
            DgpsTxIdEnum type,
            byte id,
            CancellationToken cancel = default
        )
        {
            return src.Send(new ComNavDgpsTxIdCommand { Type = type, Id = id }, cancel);
        }

        /// <summary>
        /// Logs a command to the given GNSS connection.
        /// </summary>
        /// <param name="src">The GNSS connection on which the command will be logged.</param>
        /// <param name="message">The command message.</param>
        /// <param name="portName">The name of the port. If not specified, the default port will be used.</param>
        /// <param name="format">The format of the log. If not specified, the default format will be used.</param>
        /// <param name="trigger">The trigger for the log. If not specified, the default trigger will be used.</param>
        /// <param name="period">The period for the log. If not specified, the default period will be used.</param>
        /// <param name="cancel">An optional cancellation token.</param>
        /// <returns>A Task that represents the asynchronous operation.</returns>
        public static Task LogCommand(
            IGnssConnection src,
            ComNavMessageEnum message,
            string portName = default,
            ComNavFormat? format = default,
            ComNavTriggerEnum? trigger = default,
            uint? period = default,
            CancellationToken cancel = default
        )
        {
            return src.Send(
                new ComNavAsciiLogCommand
                {
                    Type = message,
                    Format = format,
                    PortName = portName,
                    Trigger = trigger,
                    Period = period,
                },
                cancel
            );
        }

        /// <summary>
        /// Configures the receiver to fix the height at the last calculated value if the number of
        /// satellites available is insufficient for a 3-D solution. This provides a 2-D solution.
        /// Height calculation resumes when the number of satellites available allows a 3-D solution.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static Task FixAuto(IGnssConnection src, CancellationToken cancel = default)
        {
            return src.Send(new ComNavFixCommand { FixType = ComNavFixType.Auto }, cancel);
        }

        /// <summary>
        /// Unfix. Clears any previous FIX commands.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static Task SendFixNone(IGnssConnection src, CancellationToken cancel = default)
        {
            return src.Send(new ComNavFixCommand { FixType = ComNavFixType.None }, cancel);
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
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static Task SendFixHeight(
            IGnssConnection src,
            double altitude,
            CancellationToken cancel = default
        )
        {
            return src.Send(
                new ComNavFixCommand { FixType = ComNavFixType.Height, Alt = altitude },
                cancel
            );
        }

        /// <summary>
        /// <para>
        /// Configures the receiver with its position fixed. This command is used when it is
        /// necessary to generate differential corrections.
        /// For both pseudorange and differential corrections, this command must be properly
        /// initialized before the receiver can operate as a GNSS base station. Once initialized,
        /// the receiver computes differential corrections for each satellite being tracked. The
        /// computed differential corrections can then be output to rover stations using the
        /// RTCMV3 differential corrections data log format.S
        /// The values entered into the fix position command should reflect the precise position
        /// of the base station antenna phase center. Any errors in the fix position coordinates
        /// directly bias the corrections calculated by the base receiver.
        /// </para>
        /// <para>
        /// The receiver performs all internal computations based on WGS84 and the DATUM
        /// command (see page 131) is defaulted as such. The datum in which you choose to
        /// operate (by changing the DATUM command (see page 131)) is internally converted
        /// to and from WGS84. Therefore, all differential corrections are based on WGS84,
        /// regardless of your operating datum.
        /// </para>
        /// <para>
        /// The FIX POSITION command overrides any previous FIX HEIGHT or FIX
        /// POSITION command settings.
        /// </para>
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static Task FixPosition(
            IGnssConnection src,
            double latitude,
            double longitude,
            double altitude,
            CancellationToken cancel = default
        )
        {
            return src.Send(
                new ComNavFixCommand
                {
                    FixType = ComNavFixType.Position,
                    Lat = latitude,
                    Lon = longitude,
                    Alt = altitude,
                },
                cancel
            );
        }
    }
}
