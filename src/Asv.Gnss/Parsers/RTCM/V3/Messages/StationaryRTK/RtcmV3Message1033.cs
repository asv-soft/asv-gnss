using Asv.IO;
using System;

namespace Asv.Gnss
{
    internal class RtcmV3Message1033 : RtcmV3Message1008
    {
        public const int RtcmMessageRecAntId = 1033;
        public override ushort MessageId => RtcmMessageRecAntId;
        public override string Name => "Receiver and Antenna Description";
        /// <summary>
        /// Number of characters in the name of the receiver type
        /// </summary>
        public uint ReceiverTypeDescriptorCounterI { get; set; }
        /// <summary>
        /// Receiver Type Descriptor
        /// </summary>
        public string ReceiverTypeDescriptor { get; set; }

        /// <summary>
        /// Number of characters in the name of the receiver firmware
        /// </summary>
        public uint ReceiverFirmwareVersionCounterJ { get; set; }
        /// <summary>
        /// Receiver Firmware Version
        /// </summary>
        public string ReceiverFirmwareVersion { get; set; }

        /// <summary>
        /// Number of characters in the name of the receiver serial number
        /// </summary>
        public uint ReceiverSerialNumberCounterK { get; set; }
        /// <summary>
        /// Receiver Serial Number 
        /// </summary>
        public string ReceiverSerialNumber { get; set; }
        protected override void DeserializeContent(ReadOnlySpan<byte> buffer, ref int bitIndex, int messageLength)
        {
            base.DeserializeContent(buffer, ref bitIndex, messageLength);
            ReceiverTypeDescriptorCounterI = SpanBitHelper.GetBitU(buffer, ref bitIndex, 8);
            if (ReceiverTypeDescriptorCounterI > 0)
            {
                ReceiverTypeDescriptor = BitToCharHelper.BitArrayToString(buffer, ref bitIndex, (int)ReceiverTypeDescriptorCounterI);
            }

            ReceiverFirmwareVersionCounterJ = SpanBitHelper.GetBitU(buffer, ref bitIndex, 8);
            if (ReceiverFirmwareVersionCounterJ > 0)
            {
                ReceiverFirmwareVersion = BitToCharHelper.BitArrayToString(buffer, ref bitIndex, (int)ReceiverFirmwareVersionCounterJ);
            }

            ReceiverSerialNumberCounterK = SpanBitHelper.GetBitU(buffer, ref bitIndex, 8);
            if (ReceiverSerialNumberCounterK > 0)
            {
                ReceiverSerialNumber = BitToCharHelper.BitArrayToString(buffer, ref bitIndex, (int)ReceiverSerialNumberCounterK);
            }
        }
    }
}