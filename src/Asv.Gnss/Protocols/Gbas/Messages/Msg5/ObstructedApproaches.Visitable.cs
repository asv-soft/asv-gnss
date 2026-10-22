using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class ObstructedApproaches : IVisitable
{
    private static readonly Field PayloadRPDSField = ProtocolPayloadVisitor.Field(nameof(RPDS), UInt8Type.Default);
    private static readonly Field PayloadImpactedSourcesField = ProtocolPayloadVisitor.Field(nameof(ImpactedSources), new ListType(ImpactedSource.PayloadVisitorType, 0, int.MaxValue), readOnly: true);

    internal static readonly StructType PayloadVisitorType = new([PayloadRPDSField, PayloadImpactedSourcesField]);

    /// <inheritdoc />
    public virtual void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);

        byte valueRPDS = RPDS;
        UInt8Type.Accept(visitor, PayloadRPDSField, PayloadRPDSField.DataType, ref valueRPDS);
        if (!EqualityComparer<byte>.Default.Equals(valueRPDS, RPDS))
        {
            RPDS = valueRPDS;
        }

        List<ImpactedSource> valueImpactedSources = ImpactedSources;
        ProtocolPayloadVisitor.VisitList(visitor, PayloadImpactedSourcesField, PayloadImpactedSourcesField.DataType, ref valueImpactedSources,
            static () => new ImpactedSource(),
            static (IVisitor v, Field f, IFieldType t, ref ImpactedSource item) =>
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
    }
}
