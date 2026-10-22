using System;
using Asv.IO;

namespace Asv.Gnss
{
    

    public class SbfPacketMeasEpochRev1 : SbfMessageBase
    {
        public override ushort MessageType => 4027;
        
        public override ushort MessageRevision => 1;
        protected override void DeserializeContent(ref ReadOnlySpan<byte> buffer)
        {
            var n1 = BinSerialize.ReadByte(ref buffer);
            SB1Length = BinSerialize.ReadByte(ref buffer);
            SB2Length = BinSerialize.ReadByte(ref buffer);
            CommonFlags = (MeasEpochCommonFlags)BinSerialize.ReadByte(ref buffer);
            if (MessageRevision == 1)
            {
                CumClkJumps = BinSerialize.ReadByte(ref buffer) * 0.001;
            }
            Reserved = BinSerialize.ReadByte(ref buffer);
            SubBlocks = new MeasEpochChannelType1[n1];
            for (var index = 0; index < SubBlocks.Length; index++)
            {
                SubBlocks[index] = new MeasEpochChannelType1();
                SubBlocks[index].Deserialize(ref buffer, SB1Length, out var n2);
                if (n2 != 0) buffer = buffer.Slice(n2 * SB2Length);
            }
        }

        public override string Name => "MeasEpochV2";

        
        /// <summary>
        /// Cumulative millisecond clock jumps since start-up, with an ambiguity of
        /// k*256 ms.For example, if two clock jumps of -1 ms have occurred since
        /// startup, this field contains the value 254.
        /// </summary>
        public double CumClkJumps { get; set; }

        public MeasEpochChannelType1[] SubBlocks { get; set; }

        /// <summary>
        /// Reserved for future use, to be ignored by decoding software
        /// </summary>
        public byte Reserved { get; set; }


        public MeasEpochCommonFlags CommonFlags { get; set; }
        /// <summary>
        /// Length of a MeasEpochChannelType2 sub-block
        /// </summary>
        public byte SB2Length { get; set; }
        /// <summary>
        /// Length of a MeasEpochChannelType1 sub-block, excluding the nested MeasEpochChannelType2 sub-blocks
        /// </summary>
        public byte SB1Length { get; set; }
    }

    public enum AntennaId
    {
        Main = 0,
        Aux1 = 1,
        Aux2 = 2
    }

    /// <summary>
    /// Bit field containing flags common to all measurements.
    /// </summary>
    [Flags]
    public enum MeasEpochCommonFlags : byte
    {
        /// <summary>
        /// Bit 0: Multipath mitigation: if this bit is set, multipath mitigation is enabled. (see the setMultipathMitigation command).
        /// </summary>
        MultipathMitigationEnabled,
        /// <summary>
        /// Bit 1: Smoothing of code: if this bit is set, at least one of the code measurements are smoothed values (see setSmoothingInterval command).
        /// </summary>
        MeasurementsAreSmoothed,
        /// <summary>
        /// Bit 2: Carrier phase align: if this bit is set, the fractional part of the carrier phase measurements from different modulations on the same
        /// carrier frequency (e.g. GPS L2C and L2P) are aligned, i.e. multiplexing biases (0.25 or 0.5 cycles) are corrected. Aligned carrier phase
        /// measurements can be directly included in RINEX files. If this bit is
        /// unset, this block contains raw carrier phase measurements. This bit
        /// is always set in the current firmware version.
        /// </summary>
        CarrierPhaseAlign,
        /// <summary>
        /// Bit 3: Clock steering: this bit is set if clock steering is active (seesetClockSyncThreshold command).
        /// </summary>
        ClockSteering,
        /// <summary>
        /// Bit 4: Not applicable.
        /// </summary>
        NotApplicable,
        /// <summary>
        /// Bit 5: High dynamics: this bit is set when the receiver is in high-dynamics mode (see the setReceiverDynamics command).
        /// </summary>
        HighDynamics,
        /// <summary>
        /// Bit 6: Reserved
        /// </summary>
        Reserved,
        /// <summary>
        /// Bit 7: Scrambling: bit set when the measurements are scrambled. Scrambling is applied when the "Measurement Availability" permission is
        /// not granted (see the lif, Permissions command)
        /// </summary>
        Scrambling,
    }

    public class MeasEpochChannelType1
    {
        private const byte SB1Length = 20;
        public void Deserialize(ref ReadOnlySpan<byte> buffer, byte length, out byte n2)
        {
            if (length != SB1Length) throw new GnssParserException(SbfBinaryParser.GnssProtocolId, $"Error to deserialize SBF type-1 sub-block. Length type-1 sub-block should be [{SB1Length}], but it is [{length}]");
            RxChannel = BinSerialize.ReadByte(ref buffer);
            var typeBitfield1 = BinSerialize.ReadByte(ref buffer);
            var signalNumber = (typeBitfield1 & 0b00011111);
            Antenna = (AntennaId)(typeBitfield1 >> 5);
            SVID = BinSerialize.ReadByte(ref buffer);
            var misc = BinSerialize.ReadByte(ref buffer);
            var codeLSB = BinSerialize.ReadUInt(ref buffer);
            PR = ((misc & 0b00001111) * 4294967296.0 + codeLSB) * 0.001;
            DopplerHz = BinSerialize.ReadInt(ref buffer) * 0.0001;
            var carrierLSB = BinSerialize.ReadUShort(ref buffer);
            var carrierMSB = BinSerialize.ReadSByte(ref buffer);
            CN0 = BinSerialize.ReadByte(ref buffer) * 0.25;
            LockTime = BinSerialize.ReadUShort(ref buffer);
            var typeBitfield2 = BinSerialize.ReadByte(ref buffer);
            n2 = BinSerialize.ReadByte(ref buffer);
            IsSmoothed = (typeBitfield2 & 0x1) == 1;
            IsHalfCycleAmbiguity = ((typeBitfield2 >> 2) & 0x1) == 1;
            var freqNum = typeBitfield2 >> 3;
            SbfHelper.GetSignalType((byte)signalNumber, (byte)freqNum, out var sys, out var freq, out var rinexCode);
            Frequency = freq * 1000000;
            RinexCode = rinexCode;
            SatSys = sys;
            // var lambda = 299792458 / Frequency; 
            CarrierPhase = (carrierMSB * 65536.0 + carrierLSB) * 0.001;
        }

        public bool IsSmoothed { get; set; }
        public bool IsHalfCycleAmbiguity { get; set; }
        public SbfNavSysEnum SatSys { get; set; }
        public string RinexCode { get; set; }
        public double Frequency { get; set; }


        /// <summary>
        /// Duration of continuous carrier phase. The lock-time is reset at the initial
        /// lock of the phase-locked-loop, and whenever a loss of lock condition occurs.
        /// </summary>
        public ushort LockTime { get; set; }

        /// <summary>
        /// The C/N0 in dB-Hz
        /// </summary>
        public double CN0 { get; set; }


        public AntennaId Antenna { get; set; }

        /// <summary>
        /// MSB of the carrier phase relative to the pseudorange. The full carrier phase can be computed by:
        /// L[cycles] = PRtype1[m]/λ +(CarrierMSB*65536+CarrierLSB)*0.001
        /// where λ is the carrier wavelength corresponding to the
        /// frequency of the signal type in the Type field above:
        /// λ=299792458/fL m, with fL the carrier frequency as listed
        /// in section 4.1.10.
        /// </summary>
        public double CarrierPhase { get; set; }

        public double DopplerHz { get; set; }
        /// <summary>
        /// SB of the pseudorange. The pseudorange expressed in meters
        /// is computed as follows: PRtype1[m] = (CodeMSB*4294967296+CodeLSB)*0.001
        /// where CodeMSB is part of the Misc field
        /// </summary>
        public double PR { get; set; }

        public byte SVID { get; set; }

        public byte RxChannel { get; set; }
    }
}