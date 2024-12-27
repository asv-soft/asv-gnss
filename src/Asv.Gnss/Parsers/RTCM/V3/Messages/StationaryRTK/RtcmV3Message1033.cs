using System;
using Asv.IO;

namespace Asv.Gnss
{
    public class RtcmV3Message1033 : RtcmV3Message1008
    {
        public const int RtcmMessageRecAntId = 1033;
        public override ushort MessageId => RtcmMessageRecAntId;
        public override string Name => "Receiver and Antenna Description";

        /// <summary>
        /// Gets or sets number of characters in the name of the receiver type.
        /// </summary>
        public uint ReceiverTypeDescriptorCounterI { get; set; }

        /// <summary>
        /// Gets or sets receiver Type Descriptor.
        /// </summary>
        public string ReceiverTypeDescriptor { get; set; }

        /// <summary>
        /// Gets or sets number of characters in the name of the receiver firmware.
        /// </summary>
        public uint ReceiverFirmwareVersionCounterJ { get; set; }

        /// <summary>
        /// Gets or sets receiver Firmware Version.
        /// </summary>
        public string ReceiverFirmwareVersion { get; set; }

        /// <summary>
        /// Gets or sets number of characters in the name of the receiver serial number.
        /// </summary>
        public uint ReceiverSerialNumberCounterK { get; set; }

        /// <summary>
        /// Gets or sets receiver Serial Number.
        /// </summary>
        public string ReceiverSerialNumber { get; set; }

        protected override void DeserializeContent(
            ReadOnlySpan<byte> buffer,
            ref int bitIndex,
            int messageLength
        )
        {
            base.DeserializeContent(buffer, ref bitIndex, messageLength);
            ReceiverTypeDescriptorCounterI = SpanBitHelper.GetBitU(buffer, ref bitIndex, 8);
            if (ReceiverTypeDescriptorCounterI > 0)
            {
                ReceiverTypeDescriptor = BitToCharHelper.BitArrayToString(
                    buffer,
                    ref bitIndex,
                    (int)ReceiverTypeDescriptorCounterI
                );
            }

            ReceiverFirmwareVersionCounterJ = SpanBitHelper.GetBitU(buffer, ref bitIndex, 8);
            if (ReceiverFirmwareVersionCounterJ > 0)
            {
                ReceiverFirmwareVersion = BitToCharHelper.BitArrayToString(
                    buffer,
                    ref bitIndex,
                    (int)ReceiverFirmwareVersionCounterJ
                );
            }

            ReceiverSerialNumberCounterK = SpanBitHelper.GetBitU(buffer, ref bitIndex, 8);
            if (ReceiverSerialNumberCounterK > 0)
            {
                ReceiverSerialNumber = BitToCharHelper.BitArrayToString(
                    buffer,
                    ref bitIndex,
                    (int)ReceiverSerialNumberCounterK
                );
            }
        }
    }
}
