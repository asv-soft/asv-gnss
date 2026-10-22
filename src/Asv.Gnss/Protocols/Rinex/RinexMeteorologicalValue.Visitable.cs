using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial record RinexMeteorologicalValue : IVisitable
{
    private static readonly Field NrCodeField = NmeaRinexVisitor.Field(nameof(Code), StringOptionalType.Utf8, readOnly: true);
    private static readonly Field NrValueField = NmeaRinexVisitor.Field(nameof(Value), DoubleOptionalType.Default, readOnly: true);

    internal static readonly StructType NrVisitorType = new([NrCodeField, NrValueField]);

    /// <inheritdoc />
    public void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);

        string? valueCode = Code;
        StringOptionalType.Accept(visitor, NrCodeField, NrCodeField.DataType, ref valueCode);

        var valueValue = Value;
        DoubleOptionalType.Accept(visitor, NrValueField, NrValueField.DataType, ref valueValue);
    }
}
