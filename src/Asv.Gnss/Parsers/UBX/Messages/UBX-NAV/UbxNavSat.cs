using System;
using Asv.IO;
using Newtonsoft.Json;

namespace Asv.Gnss
{
    public class UbxNavSatPool : UbxMessageBase
    {
        public override string Name => "UBX-NAV-SAT-POOL";
        public override byte Class => 0x01;
        public override byte SubClass => 0x35;

        protected override void SerializeContent(ref Span<byte> buffer) { }

        protected override void DeserializeContent(ref ReadOnlySpan<byte> buffer) { }

        protected override int GetContentByteSize() => 0;

        public override void Randomize(Random random) { }
    }

    [SerializationNotSupported]
    public class UbxNavSat : UbxMessageBase
    {
        public override string Name => "UBX-NAV-SAT";
        public override byte Class => 0x01;
        public override byte SubClass => 0x35;

        /// <summary>
        /// Gets or sets gPS time of week of the navigation epoch. See the description of iTOW for details.
        /// </summary>
        public ulong ITOW { get; set; }

        /// <summary>
        /// Gets or sets message version (0x01 for this version).
        /// </summary>
        public byte Version { get; set; }

        /// <summary>
        /// Gets or sets number of satellites.
        /// </summary>
        public byte NumSvs { get; set; }
        public UbxNavSatelliteItem[] Items { get; set; }

        protected override void SerializeContent(ref Span<byte> buffer)
        {
            throw new NotImplementedException();
        }

        protected override void DeserializeContent(ref ReadOnlySpan<byte> buffer)
        {
            ITOW = BinSerialize.ReadUInt(ref buffer);
            Version = BinSerialize.ReadByte(ref buffer);
            NumSvs = BinSerialize.ReadByte(ref buffer);
            var reserved1 = BinSerialize.ReadByte(ref buffer);
            var reserved2 = BinSerialize.ReadByte(ref buffer);
            Items = new UbxNavSatelliteItem[NumSvs];
            for (var i = 0; i < NumSvs; i++)
            {
                Items[i] = new UbxNavSatelliteItem();
                Items[i].Deserialize(ref buffer);
            }
        }

        protected override int GetContentByteSize() => (Items == null ? 0 : Items.Length * 12) + 8;

        public override void Randomize(Random random) { }
    }

    public class UbxNavSatelliteItem : ISizedSpanSerializable
    {
        public UbxNavSatGnssId GnssType { get; set; }

        /// <summary>
        /// Gets or sets gNSS identifier (see Satellite Numbering) for assignment.
        /// </summary>
        public byte GnssId { get; set; }

        /// <summary>
        /// Gets or sets satellite identifier (see Satellite Numbering) for assignment.
        /// </summary>
        public byte SvId { get; set; }

        /// <summary>
        /// Gets or sets carrier to noise ratio (signal strength).
        /// </summary>
        public byte CnobBHz { get; set; }

        /// <summary>
        /// Gets or sets elevation (range: +/-90), unknown if out of range.
        /// </summary>
        public sbyte ElevDeg { get; set; }

        /// <summary>
        /// Gets or sets azimuth (range 0-360), unknown if elevation is out of range.
        /// </summary>
        public short AzimDeg { get; set; }

        /// <summary>
        /// Gets or sets pseudorange residual.
        /// </summary>
        public double PrResM { get; set; }

        public UbxQualityIndEnum QualityInd { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether signal in the subset specified in Signal Identifiers is currently being used for navigation.
        /// </summary>
        public bool SvUsed { get; set; }
        public UbxHealthInd Health { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether differential correction data is available for this SV.
        /// </summary>
        public bool DiffCorr { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether carrier smoothed pseudorange used.
        /// </summary>
        public bool Smoothed { get; set; }

        /// <summary>
        /// Gets or sets orbit source.
        /// </summary>
        public UbxOrbitSource OrbitSource { get; set; }

        /// <summary>
        ///  Gets or sets a value indicating whether ephemeris is available for this SV.
        /// </summary>
        public bool IsephAvail { get; set; }

        /// <summary>
        ///  Gets or sets a value indicating whether almanac is available for this SV.
        /// </summary>
        public bool IsalmAvail { get; set; }

        /// <summary>
        ///  Gets or sets a value indicating whether assistNow Offline data is available for this SV.
        /// </summary>
        public bool IsanoAvail { get; set; }

        /// <summary>
        ///  Gets or sets a value indicating whether assistNow Autonomous data is available for this SV.
        /// </summary>
        public bool IsaopAvail { get; set; }

        /// <summary>
        ///  Gets or sets a value indicating whether sBAS corrections have been used for a signal in the subset specified in Signal Identifiers.
        /// </summary>
        public bool IssbasCorrUsed { get; set; }

        /// <summary>
        ///  Gets or sets a value indicating whether rTCM corrections have been used for a signal in the subset specified in Signal Identifiers.
        /// </summary>
        public bool IsrtcmCorrUsed { get; set; }

        /// <summary>
        ///  Gets or sets a value indicating whether qZSS SLAS corrections have been used for a signal in the subset specified in Signal Identifiers.
        /// </summary>
        public bool IsslasCorrUsed { get; set; }

        /// <summary>
        ///  Gets or sets a value indicating whether sPARTN corrections have been used for a signal in the subset specified in Signal Identifiers.
        /// </summary>
        public bool IsspartnCorrUsed { get; set; }

        /// <summary>
        ///  Gets or sets a value indicating whether pseudorange corrections have been used for a signal in the subset specified in Signal Identifiers.
        /// </summary>
        public bool IsprCorrUsed { get; set; }

        /// <summary>
        ///  Gets or sets a value indicating whether carrier range corrections have been used for a signal in the subset specified in Signal Identifiers.
        /// </summary>
        public bool IscrCorrUsed { get; set; }

        /// <summary>
        ///  Gets or sets a value indicating whether range rate (Doppler) corrections have been used for a signal in the subset specified in Signal.
        /// </summary>
        public bool IsdoCorrUsed { get; set; }

        public void Deserialize(ref ReadOnlySpan<byte> buffer)
        {
            GnssId = BinSerialize.ReadByte(ref buffer);
            GnssType = (UbxNavSatGnssId)GnssId;
            SvId = BinSerialize.ReadByte(ref buffer);
            CnobBHz = BinSerialize.ReadByte(ref buffer);
            ElevDeg = (sbyte)BinSerialize.ReadSByte(ref buffer);
            AzimDeg = BinSerialize.ReadShort(ref buffer);
            PrResM = BinSerialize.ReadShort(ref buffer) * 0.1;
            var flags = BinSerialize.ReadUInt(ref buffer);
            QualityInd = (UbxQualityIndEnum)(flags & 0b0000_0111);
            SvUsed = (flags & 0b000_1000) != 0;
            Health = (UbxHealthInd)((flags >> 4) & 0b0011);
            DiffCorr = (flags & 0b0100_0000) != 0;
            Smoothed = (flags & 0b1000_0000) != 0;
            OrbitSource = (UbxOrbitSource)((flags >> 8) & 0b0111);

            IsephAvail = (flags & 0b0000_0000_0000_1000_0000_0000) != 0;
            IsalmAvail = (flags & 0b0000_0000_0001_0000_0000_0000) != 0;
            IsanoAvail = (flags & 0b0000_0000_0010_0000_0000_0000) != 0;
            IsaopAvail = (flags & 0b0000_0000_0100_0000_0000_0000) != 0;

            IssbasCorrUsed = (flags & 0b0000_0001_0000_0000_0000_0000) != 0;
            IsrtcmCorrUsed = (flags & 0b0000_0010_0000_0000_0000_0000) != 0;
            IsslasCorrUsed = (flags & 0b0000_0100_0000_0000_0000_0000) != 0;
            IsspartnCorrUsed = (flags & 0b0000_1000_0000_0000_0000_0000) != 0;
            IsprCorrUsed = (flags & 0b0001_0000_0000_0000_0000_0000) != 0;
            IscrCorrUsed = (flags & 0b0010_0000_0000_0000_0000_0000) != 0;
            IsdoCorrUsed = (flags & 0b0100_0000_0000_0000_0000_0000) != 0;
        }

        public void Serialize(ref Span<byte> buffer)
        {
            throw new NotImplementedException();
        }

        public int GetByteSize() => 12;

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public enum UbxNavSatGnssId : byte
    {
        GPS = 0,
        SBAS = 1,
        Galileo = 2,
        BeiDou = 3,
        IMES = 4,
        QZSS = 5,
        GLONASS = 6,
    }

    public enum UbxOrbitSource
    {
        NoOrbitInformationIsAvailableForThisSV = 0,
        EphemerisIsUsed = 1,
        AlmanacIsUsed = 2,
        AssistNowOfflineOrbitIsUsed = 3,
        AssistNowAutonomousOrbitIsUsed = 4,
        OtherOrbitInformationIsUsed1 = 5,
        OtherOrbitInformationIsUsed2 = 6,
        OtherOrbitInformationIsUsed3 = 7,
    }

    public enum UbxHealthInd : byte
    {
        Unknown = 0,
        Healthy = 1,
        Unhealthy = 2,
    }

    public enum UbxQualityIndEnum
    {
        NoSignal = 0,
        SearchingSignal = 1,
        SignalAcquired = 2,
        SignalDetectedButUnusable = 3,
        CodeLockedAndTimeSynchronized = 4,
        CodeAndCarrierLockedAndTimeSynchronized1 = 5,
        CodeAndCarrierLockedAndTimeSynchronized2 = 6,
        CodeAndCarrierLockedAndTimeSynchronized3 = 7,
    }
}
