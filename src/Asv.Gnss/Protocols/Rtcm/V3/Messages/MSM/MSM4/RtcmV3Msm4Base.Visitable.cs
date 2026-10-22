using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public abstract partial class RtcmV3Msm4Base
{
    private static readonly Field RtcmRoughRangesRawField = RtcmV3Visitor.Field(nameof(RoughRangesRaw), new ListType(DoubleType.Default, 0, RtcmV3Visitor.MaximumElements), "RoughRangesRaw");
    private static readonly Field RtcmPseudoRangeRawField = RtcmV3Visitor.Field(nameof(PseudoRangeRaw), new ListType(DoubleType.Default, 0, RtcmV3Visitor.MaximumElements), "PseudoRangeRaw");
    private static readonly Field RtcmPhaseRangeRawField = RtcmV3Visitor.Field(nameof(PhaseRangeRaw), new ListType(DoubleType.Default, 0, RtcmV3Visitor.MaximumElements), "PhaseRangeRaw");
    private static readonly Field RtcmCnrRawField = RtcmV3Visitor.Field(nameof(CnrRaw), new ListType(DoubleType.Default, 0, RtcmV3Visitor.MaximumElements), "CnrRaw");
    private static readonly Field RtcmLockRawField = RtcmV3Visitor.Field(nameof(LockRaw), new ListType(UInt8Type.Default, 0, RtcmV3Visitor.MaximumElements), "LockRaw");
    private static readonly Field RtcmHalfCycleRawField = RtcmV3Visitor.Field(nameof(HalfCycleRaw), new ListType(UInt8Type.Default, 0, RtcmV3Visitor.MaximumElements), "HalfCycleRaw");
    private static readonly Field RtcmSatellitesField = RtcmV3Visitor.Field(nameof(Satellites), new ListType(Satellite.RtcmVisitorType, 0, RtcmV3Visitor.MaximumElements), "Satellites");

    internal new static readonly StructType RtcmVisitorType = new([..RtcmV3MsmBase.RtcmVisitorType.Fields, RtcmRoughRangesRawField, RtcmPseudoRangeRawField, RtcmPhaseRangeRawField, RtcmCnrRawField, RtcmLockRawField, RtcmHalfCycleRawField, RtcmSatellitesField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        base.Accept(visitor);

        var valueRoughRangesRaw = RoughRangesRaw;
        RtcmV3Visitor.VisitArray(visitor, RtcmRoughRangesRawField, RtcmRoughRangesRawField.DataType, ref valueRoughRangesRaw,
            static () => default(double),
            static (IVisitor v, Field f, IFieldType t, ref double item) =>
            {
                DoubleType.Accept(v, f, t, ref item);
            });
        if (!ReferenceEquals(valueRoughRangesRaw, RoughRangesRaw))
        {
            RoughRangesRaw = valueRoughRangesRaw!;
        }

        var valuePseudoRangeRaw = PseudoRangeRaw;
        RtcmV3Visitor.VisitArray(visitor, RtcmPseudoRangeRawField, RtcmPseudoRangeRawField.DataType, ref valuePseudoRangeRaw,
            static () => default(double),
            static (IVisitor v, Field f, IFieldType t, ref double item) =>
            {
                DoubleType.Accept(v, f, t, ref item);
            });
        if (!ReferenceEquals(valuePseudoRangeRaw, PseudoRangeRaw))
        {
            PseudoRangeRaw = valuePseudoRangeRaw!;
        }

        var valuePhaseRangeRaw = PhaseRangeRaw;
        RtcmV3Visitor.VisitArray(visitor, RtcmPhaseRangeRawField, RtcmPhaseRangeRawField.DataType, ref valuePhaseRangeRaw,
            static () => default(double),
            static (IVisitor v, Field f, IFieldType t, ref double item) =>
            {
                DoubleType.Accept(v, f, t, ref item);
            });
        if (!ReferenceEquals(valuePhaseRangeRaw, PhaseRangeRaw))
        {
            PhaseRangeRaw = valuePhaseRangeRaw!;
        }

        var valueCnrRaw = CnrRaw;
        RtcmV3Visitor.VisitArray(visitor, RtcmCnrRawField, RtcmCnrRawField.DataType, ref valueCnrRaw,
            static () => default(double),
            static (IVisitor v, Field f, IFieldType t, ref double item) =>
            {
                DoubleType.Accept(v, f, t, ref item);
            });
        if (!ReferenceEquals(valueCnrRaw, CnrRaw))
        {
            CnrRaw = valueCnrRaw!;
        }

        var valueLockRaw = LockRaw;
        RtcmV3Visitor.VisitArray(visitor, RtcmLockRawField, RtcmLockRawField.DataType, ref valueLockRaw,
            static () => default(byte),
            static (IVisitor v, Field f, IFieldType t, ref byte item) =>
            {
                UInt8Type.Accept(v, f, t, ref item);
            });
        if (!ReferenceEquals(valueLockRaw, LockRaw))
        {
            LockRaw = valueLockRaw!;
        }

        var valueHalfCycleRaw = HalfCycleRaw;
        RtcmV3Visitor.VisitArray(visitor, RtcmHalfCycleRawField, RtcmHalfCycleRawField.DataType, ref valueHalfCycleRaw,
            static () => default(byte),
            static (IVisitor v, Field f, IFieldType t, ref byte item) =>
            {
                UInt8Type.Accept(v, f, t, ref item);
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
