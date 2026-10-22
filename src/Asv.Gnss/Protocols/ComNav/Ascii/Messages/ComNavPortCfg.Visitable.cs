using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class ComNavPortCfg : IVisitable
{
    private static readonly Field PayloadPortField = SbfComNavVisitor.Field(nameof(Port), UInt32Type.Default, "Port", enumType: typeof(ComNavPortEnum));
    private static readonly Field PayloadBaudRateField = SbfComNavVisitor.Field(nameof(BaudRate), UInt32Type.Default, "BaudRate");
    private static readonly Field PayloadOtherParamsField = SbfComNavVisitor.Field(nameof(OtherParams), new ListType(UInt32Type.Default, 8, 8), "OtherParams", readOnly: true);

    internal static readonly StructType PayloadVisitorType = new([PayloadPortField, PayloadBaudRateField, PayloadOtherParamsField]);

    /// <inheritdoc />
    public virtual void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);

        var valuePort = Port;
        var numericvaluePort = (uint)valuePort;
        UInt32Type.Accept(visitor, PayloadPortField, PayloadPortField.DataType, ref numericvaluePort);
        valuePort = (ComNavPortEnum)numericvaluePort;
        if (!EqualityComparer<ComNavPortEnum>.Default.Equals(valuePort, Port))
        {
            Port = valuePort;
        }

        var valueBaudRate = BaudRate;
        UInt32Type.Accept(visitor, PayloadBaudRateField, PayloadBaudRateField.DataType, ref valueBaudRate);
        if (!EqualityComparer<uint>.Default.Equals(valueBaudRate, BaudRate))
        {
            BaudRate = valueBaudRate;
        }

        var valueOtherParams = OtherParams;
        SbfComNavVisitor.VisitFixedArray(visitor, PayloadOtherParamsField, valueOtherParams,
            static (IVisitor v, Field f, IFieldType t, ref uint item) =>
            {
                UInt32Type.Accept(v, f, t, ref item);
            });
    }
}
