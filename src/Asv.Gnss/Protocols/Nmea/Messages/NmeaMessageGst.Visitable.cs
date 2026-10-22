using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class NmeaMessageGst
{
    private static readonly Field NrTimeField = NmeaRinexVisitor.Field(nameof(Time), TimeOnlyOptionalType.Default);
    private static readonly Field NrRmsField = NmeaRinexVisitor.Field(nameof(Rms), DoubleType.Default);
    private static readonly Field NrMajorAxisField = NmeaRinexVisitor.Field(nameof(MajorAxis), DoubleType.Default);
    private static readonly Field NrMinorAxisField = NmeaRinexVisitor.Field(nameof(MinorAxis), DoubleType.Default);
    private static readonly Field NrOrientationField = NmeaRinexVisitor.Field(nameof(Orientation), DoubleType.Default);
    private static readonly Field NrLatStdDevField = NmeaRinexVisitor.Field(nameof(LatStdDev), DoubleType.Default);
    private static readonly Field NrLonStdDevField = NmeaRinexVisitor.Field(nameof(LonStdDev), DoubleType.Default);
    private static readonly Field NrAltStdDevField = NmeaRinexVisitor.Field(nameof(AltStdDev), DoubleType.Default);

    internal new static readonly StructType NrVisitorType = new([..NmeaMessageBase.NrVisitorType.Fields, NrTimeField, NrRmsField, NrMajorAxisField, NrMinorAxisField, NrOrientationField, NrLatStdDevField, NrLonStdDevField, NrAltStdDevField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        base.Accept(visitor);

        var valueTime = Time;
        TimeOnlyOptionalType.Accept(visitor, NrTimeField, NrTimeField.DataType, ref valueTime);
        if (!EqualityComparer<TimeOnly?>.Default.Equals(valueTime, Time))
        {
            Time = valueTime;
        }

        var valueRms = Rms;
        DoubleType.Accept(visitor, NrRmsField, NrRmsField.DataType, ref valueRms);
        if (!EqualityComparer<double>.Default.Equals(valueRms, Rms))
        {
            Rms = valueRms;
        }

        var valueMajorAxis = MajorAxis;
        DoubleType.Accept(visitor, NrMajorAxisField, NrMajorAxisField.DataType, ref valueMajorAxis);
        if (!EqualityComparer<double>.Default.Equals(valueMajorAxis, MajorAxis))
        {
            MajorAxis = valueMajorAxis;
        }

        var valueMinorAxis = MinorAxis;
        DoubleType.Accept(visitor, NrMinorAxisField, NrMinorAxisField.DataType, ref valueMinorAxis);
        if (!EqualityComparer<double>.Default.Equals(valueMinorAxis, MinorAxis))
        {
            MinorAxis = valueMinorAxis;
        }

        var valueOrientation = Orientation;
        DoubleType.Accept(visitor, NrOrientationField, NrOrientationField.DataType, ref valueOrientation);
        if (!EqualityComparer<double>.Default.Equals(valueOrientation, Orientation))
        {
            Orientation = valueOrientation;
        }

        var valueLatStdDev = LatStdDev;
        DoubleType.Accept(visitor, NrLatStdDevField, NrLatStdDevField.DataType, ref valueLatStdDev);
        if (!EqualityComparer<double>.Default.Equals(valueLatStdDev, LatStdDev))
        {
            LatStdDev = valueLatStdDev;
        }

        var valueLonStdDev = LonStdDev;
        DoubleType.Accept(visitor, NrLonStdDevField, NrLonStdDevField.DataType, ref valueLonStdDev);
        if (!EqualityComparer<double>.Default.Equals(valueLonStdDev, LonStdDev))
        {
            LonStdDev = valueLonStdDev;
        }

        var valueAltStdDev = AltStdDev;
        DoubleType.Accept(visitor, NrAltStdDevField, NrAltStdDevField.DataType, ref valueAltStdDev);
        if (!EqualityComparer<double>.Default.Equals(valueAltStdDev, AltStdDev))
        {
            AltStdDev = valueAltStdDev;
        }
    }
}
