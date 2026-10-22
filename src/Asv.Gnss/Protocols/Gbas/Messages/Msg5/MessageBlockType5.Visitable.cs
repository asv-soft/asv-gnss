using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class MessageBlockType5
{
    private static readonly Field PayloadModifiedZCountField = ProtocolPayloadVisitor.Field(nameof(ModifiedZCount), DoubleType.Default);
    private static readonly Field PayloadImpactedSourcesField = ProtocolPayloadVisitor.Field(nameof(ImpactedSources), new ListType(ImpactedSource.PayloadVisitorType, 0, int.MaxValue), readOnly: true);
    private static readonly Field PayloadObstructedApproachesField = ProtocolPayloadVisitor.Field(nameof(ObstructedApproaches), new ListType(Asv.Gnss.ObstructedApproaches.PayloadVisitorType, 0, int.MaxValue), readOnly: true);

    internal new static readonly StructType PayloadVisitorType = new([..GbasMessageBlockBase.PayloadVisitorType.Fields, PayloadModifiedZCountField, PayloadImpactedSourcesField, PayloadObstructedApproachesField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        base.Accept(visitor);

        double valueModifiedZCount = ModifiedZCount;
        DoubleType.Accept(visitor, PayloadModifiedZCountField, PayloadModifiedZCountField.DataType, ref valueModifiedZCount);
        if (!EqualityComparer<double>.Default.Equals(valueModifiedZCount, ModifiedZCount))
        {
            ModifiedZCount = valueModifiedZCount;
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

        List<ObstructedApproaches> valueObstructedApproaches = ObstructedApproaches;
        ProtocolPayloadVisitor.VisitList(visitor, PayloadObstructedApproachesField, PayloadObstructedApproachesField.DataType, ref valueObstructedApproaches,
            static () => new ObstructedApproaches(),
            static (IVisitor v, Field f, IFieldType t, ref ObstructedApproaches item) =>
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
