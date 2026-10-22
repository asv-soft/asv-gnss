using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class SbfReceiverStatusAGCState : IVisitable
{
    private static readonly Field PayloadBlankingStatField = SbfComNavVisitor.Field(nameof(BlankingStat), UInt8Type.Default, "Current percentage of samples being blanked by the pulse blanking unit. This field is always 0 for receiver without pulse blanking unit.");
    private static readonly Field PayloadSampleVarField = SbfComNavVisitor.Field(nameof(SampleVar), UInt8Type.Default, "Normalized variance of the IF samples. The nominal value for this variance is 100.");
    private static readonly Field PayloadGainField = SbfComNavVisitor.Field(nameof(Gain), Int8Type.Default, "AGC gain, in dB. The Do-Not-Use value is used to indicate that the frontend PLL is not locked.");
    private static readonly Field PayloadAntennaField = SbfComNavVisitor.Field(nameof(Antenna), Int32Type.Default, "Antenna", enumType: typeof(AntennaId));
    private static readonly Field PayloadFrontEndCodeField = SbfComNavVisitor.Field(nameof(FrontEndCode), UInt8Type.Default, "FrontEndCode", enumType: typeof(SbfFrontEndCodeEnum));

    internal static readonly StructType PayloadVisitorType = new([PayloadBlankingStatField, PayloadSampleVarField, PayloadGainField, PayloadAntennaField, PayloadFrontEndCodeField]);

    /// <inheritdoc />
    public virtual void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);

        var valueBlankingStat = BlankingStat;
        UInt8Type.Accept(visitor, PayloadBlankingStatField, PayloadBlankingStatField.DataType, ref valueBlankingStat);
        if (!EqualityComparer<byte>.Default.Equals(valueBlankingStat, BlankingStat))
        {
            BlankingStat = valueBlankingStat;
        }

        var valueSampleVar = SampleVar;
        UInt8Type.Accept(visitor, PayloadSampleVarField, PayloadSampleVarField.DataType, ref valueSampleVar);
        if (!EqualityComparer<byte>.Default.Equals(valueSampleVar, SampleVar))
        {
            SampleVar = valueSampleVar;
        }

        var valueGain = Gain;
        Int8Type.Accept(visitor, PayloadGainField, PayloadGainField.DataType, ref valueGain);
        if (!EqualityComparer<sbyte>.Default.Equals(valueGain, Gain))
        {
            Gain = valueGain;
        }

        var valueAntenna = Antenna;
        var numericvalueAntenna = (int)valueAntenna;
        Int32Type.Accept(visitor, PayloadAntennaField, PayloadAntennaField.DataType, ref numericvalueAntenna);
        valueAntenna = (AntennaId)numericvalueAntenna;
        if (!EqualityComparer<AntennaId>.Default.Equals(valueAntenna, Antenna))
        {
            Antenna = valueAntenna;
        }

        var valueFrontEndCode = FrontEndCode;
        var numericvalueFrontEndCode = (byte)valueFrontEndCode;
        UInt8Type.Accept(visitor, PayloadFrontEndCodeField, PayloadFrontEndCodeField.DataType, ref numericvalueFrontEndCode);
        valueFrontEndCode = (SbfFrontEndCodeEnum)numericvalueFrontEndCode;
        if (!EqualityComparer<SbfFrontEndCodeEnum>.Default.Equals(valueFrontEndCode, FrontEndCode))
        {
            FrontEndCode = valueFrontEndCode;
        }
    }
}
