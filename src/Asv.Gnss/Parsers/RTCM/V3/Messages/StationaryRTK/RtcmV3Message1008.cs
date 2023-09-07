using System;
using Asv.IO;

namespace Asv.Gnss
{
    public class RtcmV3Message1008 : RtcmV3Message1007and1008
    {
        public const int RtcmMessageId = 1008;
        public override ushort MessageId => RtcmMessageId;
        public override string Name => "Antenna Descriptor & Serial Number";
        /// <summary>
        /// The Serial Number Counter defines the number of characters (bytes)
        /// to follow in Antenna Serial Number
        /// </summary>
        public uint SerialNumberCounterM { get; set; }
        /// <summary>
        /// Alphanumeric characters. The Antenna Serial Number is the
        /// individual antenna serial number as issued by the manufacturer of the
        /// antenna.A possible duplication of the Antenna Serial Number is not
        /// possible, because together with the Antenna Descriptor only one
        /// antenna with the particular number will be available.In order to
        /// avoid confusion the Antenna Serial Number should be omitted when
        /// the record is used together with reverse reduction to model type
        /// calibration values, because it cannot be allocated to a real physical
        /// antenna.
        /// </summary>
        public string AntennaSerialNumber { get; set; }
        protected override void DeserializeContent(ReadOnlySpan<byte> buffer, ref int bitIndex, int messageLength)
        {
            base.DeserializeContent(buffer, ref bitIndex, messageLength);
            SerialNumberCounterM = SpanBitHelper.GetBitU(buffer, ref bitIndex, 8);
            if (SerialNumberCounterM > 0)
            {
                AntennaSerialNumber = BitToCharHelper.BitArrayToString(buffer, ref bitIndex, (int)SerialNumberCounterM);
            }
        }
    }
}