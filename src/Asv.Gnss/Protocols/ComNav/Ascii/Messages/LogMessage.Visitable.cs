using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class LogMessage : IVisitable
{
    private static readonly Field PayloadDirectionField = SbfComNavVisitor.Field(nameof(Direction), UInt32Type.Default, "Direction", enumType: typeof(ComNavPortEnum));
    private static readonly Field PayloadMessageField = SbfComNavVisitor.Field(nameof(Message), Int32Type.Default, "Message", enumType: typeof(ComNavMessageEnum));
    private static readonly Field PayloadFormatField = SbfComNavVisitor.Field(nameof(Format), Int32Type.Default, "Format", enumType: typeof(ComNavFormat));
    private static readonly Field PayloadTriggerField = SbfComNavVisitor.Field(nameof(Trigger), Int32Type.Default, "Trigger", enumType: typeof(ComNavTriggerEnum));
    private static readonly Field PayloadPeriodField = SbfComNavVisitor.Field(nameof(Period), DoubleType.Default, "Period");
    private static readonly Field PayloadOffsetField = SbfComNavVisitor.Field(nameof(Offset), DoubleType.Default, "Offset");

    internal static readonly StructType PayloadVisitorType = new([PayloadDirectionField, PayloadMessageField, PayloadFormatField, PayloadTriggerField, PayloadPeriodField, PayloadOffsetField]);

    /// <inheritdoc />
    public virtual void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);

        var valueDirection = Direction;
        var numericvalueDirection = (uint)valueDirection;
        UInt32Type.Accept(visitor, PayloadDirectionField, PayloadDirectionField.DataType, ref numericvalueDirection);
        valueDirection = (ComNavPortEnum)numericvalueDirection;
        if (!EqualityComparer<ComNavPortEnum>.Default.Equals(valueDirection, Direction))
        {
            Direction = valueDirection;
        }

        var valueMessage = Message;
        var numericvalueMessage = (int)valueMessage;
        Int32Type.Accept(visitor, PayloadMessageField, PayloadMessageField.DataType, ref numericvalueMessage);
        valueMessage = (ComNavMessageEnum)numericvalueMessage;
        if (!EqualityComparer<ComNavMessageEnum>.Default.Equals(valueMessage, Message))
        {
            Message = valueMessage;
        }

        var valueFormat = Format;
        var numericvalueFormat = (int)valueFormat;
        Int32Type.Accept(visitor, PayloadFormatField, PayloadFormatField.DataType, ref numericvalueFormat);
        valueFormat = (ComNavFormat)numericvalueFormat;
        if (!EqualityComparer<ComNavFormat>.Default.Equals(valueFormat, Format))
        {
            Format = valueFormat;
        }

        var valueTrigger = Trigger;
        var numericvalueTrigger = (int)valueTrigger;
        Int32Type.Accept(visitor, PayloadTriggerField, PayloadTriggerField.DataType, ref numericvalueTrigger);
        valueTrigger = (ComNavTriggerEnum)numericvalueTrigger;
        if (!EqualityComparer<ComNavTriggerEnum>.Default.Equals(valueTrigger, Trigger))
        {
            Trigger = valueTrigger;
        }

        var valuePeriod = Period;
        DoubleType.Accept(visitor, PayloadPeriodField, PayloadPeriodField.DataType, ref valuePeriod);
        if (!EqualityComparer<double>.Default.Equals(valuePeriod, Period))
        {
            Period = valuePeriod;
        }

        var valueOffset = Offset;
        DoubleType.Accept(visitor, PayloadOffsetField, PayloadOffsetField.DataType, ref valueOffset);
        if (!EqualityComparer<double>.Default.Equals(valueOffset, Offset))
        {
            Offset = valueOffset;
        }
    }
}
