using System;
using Asv.IO;

namespace Asv.Gnss
{
    public abstract class SbfPacketGnssRawNavMsgBase : SbfMessageBase
    {
        public SbfNavSysEnum NavSystem { get; set; }

        public uint[] NAVBitsU32 { get; set; }

        /// <summary>
        /// Receiver channel (see 4.1.11)
        /// </summary>
        public byte RxChannel { get; set; }

        /// <summary>
        /// GLONASS frequency number, with an offset of 8.
        /// It ranges from 1 (corresponding to an actual frequency number of -7) to 21 (corresponding to an
        /// actual frequency number of 13).
        /// For non-GLONASS satellites, FreqNr is reserved
        /// and must be ignored by the decoding software.
        /// </summary>
        public byte FreqNr { get; set; }

        /// <summary>
        /// Bit field:
        /// Bits 0-4: Signal type from which the bits have been received, as defined
        /// in 4.1.10
        /// Bits 5-7: Reserved
        /// </summary>
        public byte Source { get; set; }

        /// <summary>
        /// Not applicable
        /// </summary>
        public byte ViterbiCnt { get; set; }

        /// <summary>
        /// Status of the CRC or parity check:
        /// 0: CRC or parity check failed
        /// 1: CRC or parity check passed
        /// </summary>
        public bool CrcPassed { get; set; }

        /// <summary>
        /// Satellite ID, see 4.1.9
        /// </summary>
        public byte SvId { get; set; }

        /// <summary>
        /// RINEX satellite code
        /// </summary>
        public string RinexSatCode { get; set; }

        public SbfSignalTypeEnum SignalType { get; set; }

        public string RindexSignalCode { get; set; }

        public double CarrierFreq { get; set; }

        protected abstract int NavBitsU32Length { get; }

        protected override void DeserializeContent(ref ReadOnlySpan<byte> buffer)
        {
            SvId = BinSerialize.ReadByte(ref buffer);
            RinexSatCode = SbfHelper.GetRinexSatteliteCode(SvId, out var nav);
            SatPrn = SbfHelper.GetSattelitePrn(SvId);
            NavSystem = nav;
            CrcPassed = BinSerialize.ReadByte(ref buffer) != 0;
            ViterbiCnt = BinSerialize.ReadByte(ref buffer);
            Source = BinSerialize.ReadByte(ref buffer);
            FreqNr = BinSerialize.ReadByte(ref buffer);
            SignalType = SbfHelper.GetSignalType(
                Source,
                FreqNr,
                out var constellation,
                out var carrierFreq,
                out var signalRinexCode
            );

            if (constellation != NavSystem)
                throw new Exception("Navigation system code not euqals");
            CarrierFreq = carrierFreq * 1000000.0;
            RindexSignalCode = signalRinexCode;
            RxChannel = BinSerialize.ReadByte(ref buffer);
            NAVBitsU32 = new uint[NavBitsU32Length];
            for (var i = 0; i < NavBitsU32Length; i++)
            {
                NAVBitsU32[i] = BinSerialize.ReadUInt(ref buffer);
            }
            //Padding ignored
        }

        public int SatPrn { get; set; }
    }
}
