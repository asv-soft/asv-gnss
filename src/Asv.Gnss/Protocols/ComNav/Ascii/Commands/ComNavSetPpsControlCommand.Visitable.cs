using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class ComNavSetPpsControlCommand
{
    private static readonly Field PayloadPolarityField = SbfComNavVisitor.Field(nameof(Polarity), Int32Type.Default, "Polarity", enumType: typeof(ComNavPpsPolarityEnum));
    private static readonly Field PayloadPeriodField = SbfComNavVisitor.Field(nameof(Period), DoubleType.Default, "Period");
    private static readonly Field PayloadPulseWidthField = SbfComNavVisitor.Field(nameof(PulseWidth), Int32Type.Default, "PulseWidth");

    internal new static readonly StructType PayloadVisitorType = new([..ComNavAsciiCommandBase.PayloadVisitorType.Fields, PayloadPolarityField, PayloadPeriodField, PayloadPulseWidthField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        base.Accept(visitor);

        var valuePolarity = Polarity;
        var numericvaluePolarity = (int)valuePolarity;
        Int32Type.Accept(visitor, PayloadPolarityField, PayloadPolarityField.DataType, ref numericvaluePolarity);
        valuePolarity = (ComNavPpsPolarityEnum)numericvaluePolarity;
        if (!EqualityComparer<ComNavPpsPolarityEnum>.Default.Equals(valuePolarity, Polarity))
        {
            Polarity = valuePolarity;
        }

        var valuePeriod = Period;
        DoubleType.Accept(visitor, PayloadPeriodField, PayloadPeriodField.DataType, ref valuePeriod);
        if (!EqualityComparer<double>.Default.Equals(valuePeriod, Period))
        {
            Period = valuePeriod;
        }

        var valuePulseWidth = PulseWidth;
        Int32Type.Accept(visitor, PayloadPulseWidthField, PayloadPulseWidthField.DataType, ref valuePulseWidth);
        if (!EqualityComparer<int>.Default.Equals(valuePulseWidth, PulseWidth))
        {
            PulseWidth = valuePulseWidth;
        }
    }
}
