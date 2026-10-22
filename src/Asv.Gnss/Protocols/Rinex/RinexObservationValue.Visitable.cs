using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial record RinexObservationValue : IVisitable
{
    private static readonly Field NrCodeField = NmeaRinexVisitor.Field(nameof(Code), StringOptionalType.Utf8, readOnly: true);
    private static readonly Field NrValueField = NmeaRinexVisitor.Field(nameof(Value), DoubleOptionalType.Default, readOnly: true);
    private static readonly Field NrLossOfLockIndicatorField = NmeaRinexVisitor.Field(nameof(LossOfLockIndicator), UInt8OptionalType.Default, readOnly: true);
    private static readonly Field NrSignalStrengthIndicatorField = NmeaRinexVisitor.Field(nameof(SignalStrengthIndicator), UInt8OptionalType.Default, readOnly: true);

    internal static readonly StructType NrVisitorType = new([NrCodeField, NrValueField, NrLossOfLockIndicatorField, NrSignalStrengthIndicatorField]);

    /// <inheritdoc />
    public void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);

        string? valueCode = Code;
        StringOptionalType.Accept(visitor, NrCodeField, NrCodeField.DataType, ref valueCode);

        var valueValue = Value;
        DoubleOptionalType.Accept(visitor, NrValueField, NrValueField.DataType, ref valueValue);

        var valueLossOfLockIndicator = LossOfLockIndicator;
        UInt8OptionalType.Accept(visitor, NrLossOfLockIndicatorField, NrLossOfLockIndicatorField.DataType, ref valueLossOfLockIndicator);

        var valueSignalStrengthIndicator = SignalStrengthIndicator;
        UInt8OptionalType.Accept(visitor, NrSignalStrengthIndicatorField, NrSignalStrengthIndicatorField.DataType, ref valueSignalStrengthIndicator);
    }
}
