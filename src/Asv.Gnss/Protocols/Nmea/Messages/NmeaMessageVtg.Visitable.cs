using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class NmeaMessageVtg
{
    private static readonly Field NrTrueTrackField = NmeaRinexVisitor.Field(nameof(TrueTrack), DoubleType.Default);
    private static readonly Field NrTrueTrackUnitsField = NmeaRinexVisitor.Field(nameof(TrueTrackUnits), UInt8OptionalType.Default, sourceType: typeof(TrueTrackUnit));
    private static readonly Field NrMagneticTrackField = NmeaRinexVisitor.Field(nameof(MagneticTrack), DoubleType.Default);
    private static readonly Field NrMagneticTrackUnitField = NmeaRinexVisitor.Field(nameof(MagneticTrackUnit), UInt8OptionalType.Default, sourceType: typeof(MagneticTrackUnit));
    private static readonly Field NrGroundSpeedKnotsField = NmeaRinexVisitor.Field(nameof(GroundSpeedKnots), DoubleType.Default);
    private static readonly Field NrGroundSpeedKnotsUnitField = NmeaRinexVisitor.Field(nameof(GroundSpeedKnotsUnit), UInt8OptionalType.Default, sourceType: typeof(GroundSpeedKnotsUnit));
    private static readonly Field NrGroundSpeedKmhField = NmeaRinexVisitor.Field(nameof(GroundSpeedKmh), DoubleType.Default);
    private static readonly Field NrGroundSpeedKmhUnitsField = NmeaRinexVisitor.Field(nameof(GroundSpeedKmhUnits), Int32OptionalType.Default, sourceType: typeof(GroundSpeedKmhUnit));
    private static readonly Field NrModeField = NmeaRinexVisitor.Field(nameof(Mode), Int32OptionalType.Default, sourceType: typeof(NmeaPositioningSystemMode));

    internal new static readonly StructType NrVisitorType = new([..NmeaMessageBase.NrVisitorType.Fields, NrTrueTrackField, NrTrueTrackUnitsField, NrMagneticTrackField, NrMagneticTrackUnitField, NrGroundSpeedKnotsField, NrGroundSpeedKnotsUnitField, NrGroundSpeedKmhField, NrGroundSpeedKmhUnitsField, NrModeField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        base.Accept(visitor);

        var valueTrueTrack = TrueTrack;
        DoubleType.Accept(visitor, NrTrueTrackField, NrTrueTrackField.DataType, ref valueTrueTrack);
        if (!EqualityComparer<double>.Default.Equals(valueTrueTrack, TrueTrack))
        {
            TrueTrack = valueTrueTrack;
        }

        var valueTrueTrackUnits = (byte?)TrueTrackUnits;
        UInt8OptionalType.Accept(visitor, NrTrueTrackUnitsField, NrTrueTrackUnitsField.DataType, ref valueTrueTrackUnits);
        if (!EqualityComparer<byte?>.Default.Equals(valueTrueTrackUnits, (byte?)TrueTrackUnits))
        {
            TrueTrackUnits = (TrueTrackUnit?)valueTrueTrackUnits;
        }

        var valueMagneticTrack = MagneticTrack;
        DoubleType.Accept(visitor, NrMagneticTrackField, NrMagneticTrackField.DataType, ref valueMagneticTrack);
        if (!EqualityComparer<double>.Default.Equals(valueMagneticTrack, MagneticTrack))
        {
            MagneticTrack = valueMagneticTrack;
        }

        var valueMagneticTrackUnit = (byte?)MagneticTrackUnit;
        UInt8OptionalType.Accept(visitor, NrMagneticTrackUnitField, NrMagneticTrackUnitField.DataType, ref valueMagneticTrackUnit);
        if (!EqualityComparer<byte?>.Default.Equals(valueMagneticTrackUnit, (byte?)MagneticTrackUnit))
        {
            MagneticTrackUnit = (MagneticTrackUnit?)valueMagneticTrackUnit;
        }

        var valueGroundSpeedKnots = GroundSpeedKnots;
        DoubleType.Accept(visitor, NrGroundSpeedKnotsField, NrGroundSpeedKnotsField.DataType, ref valueGroundSpeedKnots);
        if (!EqualityComparer<double>.Default.Equals(valueGroundSpeedKnots, GroundSpeedKnots))
        {
            GroundSpeedKnots = valueGroundSpeedKnots;
        }

        var valueGroundSpeedKnotsUnit = (byte?)GroundSpeedKnotsUnit;
        UInt8OptionalType.Accept(visitor, NrGroundSpeedKnotsUnitField, NrGroundSpeedKnotsUnitField.DataType, ref valueGroundSpeedKnotsUnit);
        if (!EqualityComparer<byte?>.Default.Equals(valueGroundSpeedKnotsUnit, (byte?)GroundSpeedKnotsUnit))
        {
            GroundSpeedKnotsUnit = (GroundSpeedKnotsUnit?)valueGroundSpeedKnotsUnit;
        }

        var valueGroundSpeedKmh = GroundSpeedKmh;
        DoubleType.Accept(visitor, NrGroundSpeedKmhField, NrGroundSpeedKmhField.DataType, ref valueGroundSpeedKmh);
        if (!EqualityComparer<double>.Default.Equals(valueGroundSpeedKmh, GroundSpeedKmh))
        {
            GroundSpeedKmh = valueGroundSpeedKmh;
        }

        var valueGroundSpeedKmhUnits = (int?)GroundSpeedKmhUnits;
        Int32OptionalType.Accept(visitor, NrGroundSpeedKmhUnitsField, NrGroundSpeedKmhUnitsField.DataType, ref valueGroundSpeedKmhUnits);
        if (!EqualityComparer<int?>.Default.Equals(valueGroundSpeedKmhUnits, (int?)GroundSpeedKmhUnits))
        {
            GroundSpeedKmhUnits = (GroundSpeedKmhUnit?)valueGroundSpeedKmhUnits;
        }

        var valueMode = (int?)Mode;
        Int32OptionalType.Accept(visitor, NrModeField, NrModeField.DataType, ref valueMode);
        if (!EqualityComparer<int?>.Default.Equals(valueMode, (int?)Mode))
        {
            Mode = (NmeaPositioningSystemMode?)valueMode;
        }
    }
}
