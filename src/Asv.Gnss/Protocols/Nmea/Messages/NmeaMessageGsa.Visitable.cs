using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class NmeaMessageGsa
{
    private static readonly Field NrDopModeField = NmeaRinexVisitor.Field(nameof(DopMode), Int32OptionalType.Default, sourceType: typeof(NmeaDopMode));
    private static readonly Field NrFixModeField = NmeaRinexVisitor.Field(nameof(FixMode), Int32OptionalType.Default, sourceType: typeof(NmeaFixQuality));
    private static readonly Field NrSatellitesField = NmeaRinexVisitor.Field(nameof(Satellites), new ListType(UInt8Type.Default, 0, 32768), readOnly: true);
    private static readonly Field NrPdopField = NmeaRinexVisitor.Field(nameof(Pdop), DoubleType.Default);
    private static readonly Field NrHdopField = NmeaRinexVisitor.Field(nameof(Hdop), DoubleType.Default);
    private static readonly Field NrVdopField = NmeaRinexVisitor.Field(nameof(Vdop), DoubleType.Default);
    private static readonly Field NrSystemIdField = NmeaRinexVisitor.Field(nameof(SystemId), Int32OptionalType.Default);

    internal new static readonly StructType NrVisitorType = new([..NmeaMessageBase.NrVisitorType.Fields, NrDopModeField, NrFixModeField, NrSatellitesField, NrPdopField, NrHdopField, NrVdopField, NrSystemIdField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        base.Accept(visitor);

        var valueDopMode = (int?)DopMode;
        Int32OptionalType.Accept(visitor, NrDopModeField, NrDopModeField.DataType, ref valueDopMode);
        if (!EqualityComparer<int?>.Default.Equals(valueDopMode, (int?)DopMode))
        {
            DopMode = (NmeaDopMode?)valueDopMode;
        }

        var valueFixMode = (int?)FixMode;
        Int32OptionalType.Accept(visitor, NrFixModeField, NrFixModeField.DataType, ref valueFixMode);
        if (!EqualityComparer<int?>.Default.Equals(valueFixMode, (int?)FixMode))
        {
            FixMode = (NmeaFixQuality?)valueFixMode;
        }

        var valueSatellites = Satellites;
        NmeaRinexVisitor.VisitList(visitor, NrSatellitesField, NrSatellitesField.DataType, valueSatellites,
            static (IVisitor visitor, Field field, IFieldType type, ref byte item) =>
            {
                UInt8Type.Accept(visitor, field, type, ref item);
            });

        var valuePdop = Pdop;
        DoubleType.Accept(visitor, NrPdopField, NrPdopField.DataType, ref valuePdop);
        if (!EqualityComparer<double>.Default.Equals(valuePdop, Pdop))
        {
            Pdop = valuePdop;
        }

        var valueHdop = Hdop;
        DoubleType.Accept(visitor, NrHdopField, NrHdopField.DataType, ref valueHdop);
        if (!EqualityComparer<double>.Default.Equals(valueHdop, Hdop))
        {
            Hdop = valueHdop;
        }

        var valueVdop = Vdop;
        DoubleType.Accept(visitor, NrVdopField, NrVdopField.DataType, ref valueVdop);
        if (!EqualityComparer<double>.Default.Equals(valueVdop, Vdop))
        {
            Vdop = valueVdop;
        }

        var valueSystemId = SystemId;
        Int32OptionalType.Accept(visitor, NrSystemIdField, NrSystemIdField.DataType, ref valueSystemId);
        if (!EqualityComparer<int?>.Default.Equals(valueSystemId, SystemId))
        {
            SystemId = valueSystemId;
        }
    }
}
