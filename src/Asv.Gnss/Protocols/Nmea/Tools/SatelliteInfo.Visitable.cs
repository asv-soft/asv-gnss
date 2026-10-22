using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class SatelliteInfo : IVisitable
{
    private static readonly Field NrNmeaPrnField = NmeaRinexVisitor.Field(nameof(NmeaPrn), Int32OptionalType.Default, readOnly: true);
    private static readonly Field NrElevationField = NmeaRinexVisitor.Field(nameof(Elevation), Int32OptionalType.Default, readOnly: true);
    private static readonly Field NrAzimuthField = NmeaRinexVisitor.Field(nameof(Azimuth), Int32OptionalType.Default, readOnly: true);
    private static readonly Field NrSnrField = NmeaRinexVisitor.Field(nameof(Snr), Int32OptionalType.Default, readOnly: true);
    private static readonly Field NrExtPrnField = NmeaRinexVisitor.Field(nameof(ExtPrn), Int32Type.Default, readOnly: true);
    private static readonly Field NrExtNavSysField = NmeaRinexVisitor.Field(nameof(ExtNavSys), Int32Type.Default, readOnly: true, sourceType: typeof(NmeaNavigationSystemEnum));

    internal static readonly StructType NrVisitorType = new([NrNmeaPrnField, NrElevationField, NrAzimuthField, NrSnrField, NrExtPrnField, NrExtNavSysField]);

    /// <inheritdoc />
    public virtual void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);

        var valueNmeaPrn = NmeaPrn;
        Int32OptionalType.Accept(visitor, NrNmeaPrnField, NrNmeaPrnField.DataType, ref valueNmeaPrn);

        var valueElevation = Elevation;
        Int32OptionalType.Accept(visitor, NrElevationField, NrElevationField.DataType, ref valueElevation);

        var valueAzimuth = Azimuth;
        Int32OptionalType.Accept(visitor, NrAzimuthField, NrAzimuthField.DataType, ref valueAzimuth);

        var valueSnr = Snr;
        Int32OptionalType.Accept(visitor, NrSnrField, NrSnrField.DataType, ref valueSnr);

        var valueExtPrn = ExtPrn;
        Int32Type.Accept(visitor, NrExtPrnField, NrExtPrnField.DataType, ref valueExtPrn);

        var valueExtNavSys = (int)ExtNavSys;
        Int32Type.Accept(visitor, NrExtNavSysField, NrExtNavSysField.DataType, ref valueExtNavSys);
    }
}
