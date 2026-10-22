using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public abstract partial class RtcmV3Msm3
{
    private static readonly Field RtcmRoughRangeModuloRawField = RtcmV3Visitor.Field(nameof(RoughRangeModuloRaw), new ListType(Int32Type.Default, 0, RtcmV3Visitor.MaximumElements), "RoughRangeModuloRaw");
    private static readonly Field RtcmFinePseudorangeRawField = RtcmV3Visitor.Field(nameof(FinePseudorangeRaw), new ListType(Int32Type.Default, 0, RtcmV3Visitor.MaximumElements), "FinePseudorangeRaw");
    private static readonly Field RtcmFinePhaseRangeRawField = RtcmV3Visitor.Field(nameof(FinePhaseRangeRaw), new ListType(Int32Type.Default, 0, RtcmV3Visitor.MaximumElements), "FinePhaseRangeRaw");
    private static readonly Field RtcmLockTimeRawField = RtcmV3Visitor.Field(nameof(LockTimeRaw), new ListType(Int32Type.Default, 0, RtcmV3Visitor.MaximumElements), "LockTimeRaw");
    private static readonly Field RtcmHalfCycleRawField = RtcmV3Visitor.Field(nameof(HalfCycleRaw), new ListType(Int32Type.Default, 0, RtcmV3Visitor.MaximumElements), "HalfCycleRaw");
    private static readonly Field RtcmSatellitesField = RtcmV3Visitor.Field(nameof(Satellites), new ListType(Satellite.RtcmVisitorType, 0, RtcmV3Visitor.MaximumElements), "Satellites");

    internal new static readonly StructType RtcmVisitorType = new([..RtcmV3MsmBase.RtcmVisitorType.Fields, RtcmRoughRangeModuloRawField, RtcmFinePseudorangeRawField, RtcmFinePhaseRangeRawField, RtcmLockTimeRawField, RtcmHalfCycleRawField, RtcmSatellitesField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        base.Accept(visitor);

        var valueRoughRangeModuloRaw = RoughRangeModuloRaw;
        RtcmV3Visitor.VisitArray(visitor, RtcmRoughRangeModuloRawField, RtcmRoughRangeModuloRawField.DataType, ref valueRoughRangeModuloRaw,
            static () => default(int),
            static (IVisitor v, Field f, IFieldType t, ref int item) =>
            {
                Int32Type.Accept(v, f, t, ref item);
            });
        if (!ReferenceEquals(valueRoughRangeModuloRaw, RoughRangeModuloRaw))
        {
            RoughRangeModuloRaw = valueRoughRangeModuloRaw!;
        }

        var valueFinePseudorangeRaw = FinePseudorangeRaw;
        RtcmV3Visitor.VisitArray(visitor, RtcmFinePseudorangeRawField, RtcmFinePseudorangeRawField.DataType, ref valueFinePseudorangeRaw,
            static () => default(int),
            static (IVisitor v, Field f, IFieldType t, ref int item) =>
            {
                Int32Type.Accept(v, f, t, ref item);
            });
        if (!ReferenceEquals(valueFinePseudorangeRaw, FinePseudorangeRaw))
        {
            FinePseudorangeRaw = valueFinePseudorangeRaw!;
        }

        var valueFinePhaseRangeRaw = FinePhaseRangeRaw;
        RtcmV3Visitor.VisitArray(visitor, RtcmFinePhaseRangeRawField, RtcmFinePhaseRangeRawField.DataType, ref valueFinePhaseRangeRaw,
            static () => default(int),
            static (IVisitor v, Field f, IFieldType t, ref int item) =>
            {
                Int32Type.Accept(v, f, t, ref item);
            });
        if (!ReferenceEquals(valueFinePhaseRangeRaw, FinePhaseRangeRaw))
        {
            FinePhaseRangeRaw = valueFinePhaseRangeRaw!;
        }

        var valueLockTimeRaw = LockTimeRaw;
        RtcmV3Visitor.VisitArray(visitor, RtcmLockTimeRawField, RtcmLockTimeRawField.DataType, ref valueLockTimeRaw,
            static () => default(int),
            static (IVisitor v, Field f, IFieldType t, ref int item) =>
            {
                Int32Type.Accept(v, f, t, ref item);
            });
        if (!ReferenceEquals(valueLockTimeRaw, LockTimeRaw))
        {
            LockTimeRaw = valueLockTimeRaw!;
        }

        var valueHalfCycleRaw = HalfCycleRaw;
        RtcmV3Visitor.VisitArray(visitor, RtcmHalfCycleRawField, RtcmHalfCycleRawField.DataType, ref valueHalfCycleRaw,
            static () => default(int),
            static (IVisitor v, Field f, IFieldType t, ref int item) =>
            {
                Int32Type.Accept(v, f, t, ref item);
            });
        if (!ReferenceEquals(valueHalfCycleRaw, HalfCycleRaw))
        {
            HalfCycleRaw = valueHalfCycleRaw!;
        }

        var valueSatellites = Satellites;
        RtcmV3Visitor.VisitArray(visitor, RtcmSatellitesField, RtcmSatellitesField.DataType, ref valueSatellites,
            static () => new Satellite(),
            static (IVisitor v, Field f, IFieldType t, ref Satellite item) =>
            {
                if (item != null)
                {
                    StructType.Accept(v, f, t, item);
                }
                else
                {
                    v.VisitUnknown(f, t);
                }
            });
        if (!ReferenceEquals(valueSatellites, Satellites))
        {
            Satellites = valueSatellites!;
        }
    }
}
