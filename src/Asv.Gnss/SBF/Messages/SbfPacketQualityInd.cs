using System;
using Asv.IO;

namespace Asv.Gnss
{
    public class SbfPacketQualityInd:SbfMessageBase
    {
        public override ushort MessageRevision => 0;
        public override ushort MessageType => 4082;
        public override string Name => "QualityInd";

        protected override void DeserializeContent(ref ReadOnlySpan<byte> buffer)
        {
            var N = BinSerialize.ReadByte(ref buffer);
            Reserved = BinSerialize.ReadByte(ref buffer);
            Indicators = new QualityIndicator[N];
            for (var i = 0; i < N; i++)
            {
                var indicator = BinSerialize.ReadUShort(ref buffer);
                Indicators[i] = new QualityIndicator
                {
                    IndicatorType = (SbfQualityIndicatorTypeEnum)(indicator & 0b0000_0000_0001_1111),
                    Value = (byte)((indicator >> 8) & 0b0000_0000_0000_1111),
                };
            }
        }

        public QualityIndicator[] Indicators { get; set; }

        public byte Reserved { get; set; }
    }

    public enum SbfQualityIndicatorTypeEnum
    {
        /// <summary>
        /// Overall quality
        /// </summary>
        All = 0,
        /// <summary>
        /// GNSS signals from main antenna
        /// </summary>
        SignalMainAntenna = 1,
        /// <summary>
        /// GNSS signals from aux1 antenna
        /// </summary>
        SignalAux1Antenna = 2,
        /// <summary>
        /// RF power level from the main antenna
        /// </summary>
        RfPowerLevelMainAntenna = 11,
        /// <summary>
        /// RF power level from the aux1 antenna
        /// </summary>
        RfPowerLevelAux1Antenna = 12,
        /// <summary>
        /// CPU headroom
        /// </summary>
        CpuHeadroom = 21,
        /// <summary>
        /// OCXO stability (only available on PolaRx5S receivers)
        /// </summary>
        OcxoStability = 25,
        /// <summary>
        /// Base station measurements. This indicator is only
        /// available in RTK mode. A low value could for example hint at severe multipath or interference at the
        /// base station, or also at ionospheric scintillation.
        /// </summary>
        BaseStationMeasurements = 30,
        /// <summary>
        /// RTK post-processing. This indicator is only available when the position mode is not RTK. It indicates the likelihood of getting a cm-accurate RTK
        /// position when post-processing the current data.
        /// </summary>
        RtkPostProcessing = 31,
    }

    public class QualityIndicator
    {
        public SbfQualityIndicatorTypeEnum IndicatorType { get; set; }
        public byte Value { get; set; }
    }
}