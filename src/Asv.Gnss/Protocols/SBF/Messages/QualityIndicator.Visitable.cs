using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class QualityIndicator : IVisitable
{
    private static readonly Field PayloadIndicatorTypeField = SbfComNavVisitor.Field(nameof(IndicatorType), Int32Type.Default, "IndicatorType", enumType: typeof(SbfQualityIndicatorTypeEnum));
    private static readonly Field PayloadValueField = SbfComNavVisitor.Field(nameof(Value), UInt8Type.Default, "Value");

    internal static readonly StructType PayloadVisitorType = new([PayloadIndicatorTypeField, PayloadValueField]);

    /// <inheritdoc />
    public virtual void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);

        var valueIndicatorType = IndicatorType;
        var numericvalueIndicatorType = (int)valueIndicatorType;
        Int32Type.Accept(visitor, PayloadIndicatorTypeField, PayloadIndicatorTypeField.DataType, ref numericvalueIndicatorType);
        valueIndicatorType = (SbfQualityIndicatorTypeEnum)numericvalueIndicatorType;
        if (!EqualityComparer<SbfQualityIndicatorTypeEnum>.Default.Equals(valueIndicatorType, IndicatorType))
        {
            IndicatorType = valueIndicatorType;
        }

        var valueValue = Value;
        UInt8Type.Accept(visitor, PayloadValueField, PayloadValueField.DataType, ref valueValue);
        if (!EqualityComparer<byte>.Default.Equals(valueValue, Value))
        {
            Value = valueValue;
        }
    }
}
