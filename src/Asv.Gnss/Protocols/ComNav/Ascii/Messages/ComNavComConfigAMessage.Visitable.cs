using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class ComNavComConfigAMessage
{
    private static readonly Field PayloadPortsField = SbfComNavVisitor.Field(nameof(Ports), new ListType(ComNavPortCfg.PayloadVisitorType, 0, ushort.MaxValue), "Ports");

    internal new static readonly StructType PayloadVisitorType = new([..ComNavAsciiMessageBase.PayloadVisitorType.Fields, PayloadPortsField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        base.Accept(visitor);

        var valuePorts = Ports;
        SbfComNavVisitor.VisitArray(visitor, PayloadPortsField, PayloadPortsField.DataType, ref valuePorts, static () => new ComNavPortCfg(),
            static (IVisitor v, Field f, IFieldType t, ref ComNavPortCfg item) =>
            {
                if (item is not null)
                {
                    StructType.Accept(v, f, t, item);
                }
                else
                {
                    v.VisitUnknown(f, t);
                }
            });
        if (!ReferenceEquals(valuePorts, Ports))
        {
            Ports = valuePorts!;
        }
    }
}
