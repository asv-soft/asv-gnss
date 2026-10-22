using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class ComNavAsciiLogCommand
{
    private static readonly Field PayloadTypeField = SbfComNavVisitor.Field(nameof(Type), Int32Type.Default, "Gets or sets the Type of the ComNavMessageEnum.", enumType: typeof(ComNavMessageEnum));
    private static readonly Field PayloadTriggerField = SbfComNavVisitor.Field(nameof(Trigger), Int32OptionalType.Default, "Gets or sets the trigger for a ComNav Trigger.", enumType: typeof(ComNavTriggerEnum));
    private static readonly Field PayloadPeriodField = SbfComNavVisitor.Field(nameof(Period), UInt32OptionalType.Default, "Gets or sets the period.");
    private static readonly Field PayloadOffsetTimeField = SbfComNavVisitor.Field(nameof(OffsetTime), UInt32OptionalType.Default, "Gets or sets the offset time in milliseconds.");
    private static readonly Field PayloadFormatField = SbfComNavVisitor.Field(nameof(Format), Int32OptionalType.Default, "Gets or sets the format of the property.", enumType: typeof(ComNavFormat));
    private static readonly Field PayloadPortNameField = SbfComNavVisitor.Field(nameof(PortName), StringOptionalType.Utf8, "Gets or sets the name of the port.");

    internal new static readonly StructType PayloadVisitorType = new([..ComNavAsciiCommandBase.PayloadVisitorType.Fields, PayloadTypeField, PayloadTriggerField, PayloadPeriodField, PayloadOffsetTimeField, PayloadFormatField, PayloadPortNameField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        base.Accept(visitor);

        var valueType = Type;
        var numericvalueType = (int)valueType;
        Int32Type.Accept(visitor, PayloadTypeField, PayloadTypeField.DataType, ref numericvalueType);
        valueType = (ComNavMessageEnum)numericvalueType;
        if (!EqualityComparer<ComNavMessageEnum>.Default.Equals(valueType, Type))
        {
            Type = valueType;
        }

        var valueTrigger = Trigger;
        var numericvalueTrigger = (int?)valueTrigger;
        Int32OptionalType.Accept(visitor, PayloadTriggerField, PayloadTriggerField.DataType, ref numericvalueTrigger);
        valueTrigger = (ComNavTriggerEnum?)numericvalueTrigger;
        if (!EqualityComparer<ComNavTriggerEnum?>.Default.Equals(valueTrigger, Trigger))
        {
            Trigger = valueTrigger;
        }

        var valuePeriod = Period;
        UInt32OptionalType.Accept(visitor, PayloadPeriodField, PayloadPeriodField.DataType, ref valuePeriod);
        if (!EqualityComparer<uint?>.Default.Equals(valuePeriod, Period))
        {
            Period = valuePeriod;
        }

        var valueOffsetTime = OffsetTime;
        UInt32OptionalType.Accept(visitor, PayloadOffsetTimeField, PayloadOffsetTimeField.DataType, ref valueOffsetTime);
        if (!EqualityComparer<uint?>.Default.Equals(valueOffsetTime, OffsetTime))
        {
            OffsetTime = valueOffsetTime;
        }

        var valueFormat = Format;
        var numericvalueFormat = (int?)valueFormat;
        Int32OptionalType.Accept(visitor, PayloadFormatField, PayloadFormatField.DataType, ref numericvalueFormat);
        valueFormat = (ComNavFormat?)numericvalueFormat;
        if (!EqualityComparer<ComNavFormat?>.Default.Equals(valueFormat, Format))
        {
            Format = valueFormat;
        }

        string? valuePortName = PortName;
        StringOptionalType.Accept(visitor, PayloadPortNameField, PayloadPortNameField.DataType, ref valuePortName);
        if (!EqualityComparer<string>.Default.Equals(valuePortName, PortName))
        {
            PortName = valuePortName!;
        }
    }
}
