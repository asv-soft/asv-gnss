using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public abstract partial class RtcmV3Msm5
{
    private static readonly Field RtcmSatellitesField = RtcmV3Visitor.Field(nameof(Satellites), new ListType(Satellite.RtcmVisitorType, 0, RtcmV3Visitor.MaximumElements), "Satellites");
    private static readonly Field RtcmRoughRangeIntegerRawField = RtcmV3Visitor.Field(nameof(RoughRangeIntegerRaw), new ListType(Int32Type.Default, 0, RtcmV3Visitor.MaximumElements), "RoughRangeIntegerRaw");
    private static readonly Field RtcmExtendedSatelliteInfoRawField = RtcmV3Visitor.Field(nameof(ExtendedSatelliteInfoRaw), new ListType(Int32Type.Default, 0, RtcmV3Visitor.MaximumElements), "ExtendedSatelliteInfoRaw");
    private static readonly Field RtcmRoughRangeModuloRawField = RtcmV3Visitor.Field(nameof(RoughRangeModuloRaw), new ListType(Int32Type.Default, 0, RtcmV3Visitor.MaximumElements), "RoughRangeModuloRaw");
    private static readonly Field RtcmRoughPhaseRangeRateRawField = RtcmV3Visitor.Field(nameof(RoughPhaseRangeRateRaw), new ListType(Int32Type.Default, 0, RtcmV3Visitor.MaximumElements), "RoughPhaseRangeRateRaw");
    private static readonly Field RtcmFinePseudorangeRawField = RtcmV3Visitor.Field(nameof(FinePseudorangeRaw), new ListType(Int32Type.Default, 0, RtcmV3Visitor.MaximumElements), "FinePseudorangeRaw");
    private static readonly Field RtcmFinePhaseRangeRawField = RtcmV3Visitor.Field(nameof(FinePhaseRangeRaw), new ListType(Int32Type.Default, 0, RtcmV3Visitor.MaximumElements), "FinePhaseRangeRaw");
    private static readonly Field RtcmLockTimeRawField = RtcmV3Visitor.Field(nameof(LockTimeRaw), new ListType(Int32Type.Default, 0, RtcmV3Visitor.MaximumElements), "LockTimeRaw");
    private static readonly Field RtcmHalfCycleRawField = RtcmV3Visitor.Field(nameof(HalfCycleRaw), new ListType(Int32Type.Default, 0, RtcmV3Visitor.MaximumElements), "HalfCycleRaw");
    private static readonly Field RtcmCnrRawField = RtcmV3Visitor.Field(nameof(CnrRaw), new ListType(Int32Type.Default, 0, RtcmV3Visitor.MaximumElements), "CnrRaw");
    private static readonly Field RtcmFinePhaseRangeRateRawField = RtcmV3Visitor.Field(nameof(FinePhaseRangeRateRaw), new ListType(Int32Type.Default, 0, RtcmV3Visitor.MaximumElements), "FinePhaseRangeRateRaw");

    internal new static readonly StructType RtcmVisitorType = new([..RtcmV3MsmBase.RtcmVisitorType.Fields, RtcmSatellitesField, RtcmRoughRangeIntegerRawField, RtcmExtendedSatelliteInfoRawField, RtcmRoughRangeModuloRawField, RtcmRoughPhaseRangeRateRawField, RtcmFinePseudorangeRawField, RtcmFinePhaseRangeRawField, RtcmLockTimeRawField, RtcmHalfCycleRawField, RtcmCnrRawField, RtcmFinePhaseRangeRateRawField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        base.Accept(visitor);

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

        var valueRoughRangeIntegerRaw = RoughRangeIntegerRaw;
        RtcmV3Visitor.VisitArray(visitor, RtcmRoughRangeIntegerRawField, RtcmRoughRangeIntegerRawField.DataType, ref valueRoughRangeIntegerRaw,
            static () => default(int),
            static (IVisitor v, Field f, IFieldType t, ref int item) =>
            {
                Int32Type.Accept(v, f, t, ref item);
            });
        if (!ReferenceEquals(valueRoughRangeIntegerRaw, RoughRangeIntegerRaw))
        {
            RoughRangeIntegerRaw = valueRoughRangeIntegerRaw!;
        }

        var valueExtendedSatelliteInfoRaw = ExtendedSatelliteInfoRaw;
        RtcmV3Visitor.VisitArray(visitor, RtcmExtendedSatelliteInfoRawField, RtcmExtendedSatelliteInfoRawField.DataType, ref valueExtendedSatelliteInfoRaw,
            static () => default(int),
            static (IVisitor v, Field f, IFieldType t, ref int item) =>
            {
                Int32Type.Accept(v, f, t, ref item);
            });
        if (!ReferenceEquals(valueExtendedSatelliteInfoRaw, ExtendedSatelliteInfoRaw))
        {
            ExtendedSatelliteInfoRaw = valueExtendedSatelliteInfoRaw!;
        }

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

        var valueRoughPhaseRangeRateRaw = RoughPhaseRangeRateRaw;
        RtcmV3Visitor.VisitArray(visitor, RtcmRoughPhaseRangeRateRawField, RtcmRoughPhaseRangeRateRawField.DataType, ref valueRoughPhaseRangeRateRaw,
            static () => default(int),
            static (IVisitor v, Field f, IFieldType t, ref int item) =>
            {
                Int32Type.Accept(v, f, t, ref item);
            });
        if (!ReferenceEquals(valueRoughPhaseRangeRateRaw, RoughPhaseRangeRateRaw))
        {
            RoughPhaseRangeRateRaw = valueRoughPhaseRangeRateRaw!;
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

        var valueCnrRaw = CnrRaw;
        RtcmV3Visitor.VisitArray(visitor, RtcmCnrRawField, RtcmCnrRawField.DataType, ref valueCnrRaw,
            static () => default(int),
            static (IVisitor v, Field f, IFieldType t, ref int item) =>
            {
                Int32Type.Accept(v, f, t, ref item);
            });
        if (!ReferenceEquals(valueCnrRaw, CnrRaw))
        {
            CnrRaw = valueCnrRaw!;
        }

        var valueFinePhaseRangeRateRaw = FinePhaseRangeRateRaw;
        RtcmV3Visitor.VisitArray(visitor, RtcmFinePhaseRangeRateRawField, RtcmFinePhaseRangeRateRawField.DataType, ref valueFinePhaseRangeRateRaw,
            static () => default(int),
            static (IVisitor v, Field f, IFieldType t, ref int item) =>
            {
                Int32Type.Accept(v, f, t, ref item);
            });
        if (!ReferenceEquals(valueFinePhaseRangeRateRaw, FinePhaseRangeRateRaw))
        {
            FinePhaseRangeRateRaw = valueFinePhaseRangeRateRaw!;
        }
    }
}
