using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class SignalRaw : IVisitable
{
    private static readonly Field RtcmRinexCodeField = RtcmV3Visitor.Field(nameof(RinexCode), StringOptionalType.Utf8, "RinexCode");
    private static readonly Field RtcmObservationCodeField = RtcmV3Visitor.Field(nameof(ObservationCode), UInt8Type.Default, "ObservationCode");
    private static readonly Field RtcmObservationIndexField = RtcmV3Visitor.Field(nameof(ObservationIndex), Int32Type.Default, "ObservationIndex");

    internal static readonly StructType RtcmVisitorType = new([RtcmRinexCodeField, RtcmObservationCodeField, RtcmObservationIndexField]);

    /// <inheritdoc />
    public virtual void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);

        string? valueRinexCode = RinexCode;
        StringOptionalType.Accept(visitor, RtcmRinexCodeField, RtcmRinexCodeField.DataType, ref valueRinexCode);
        if (!EqualityComparer<string>.Default.Equals(valueRinexCode, RinexCode))
        {
            RinexCode = valueRinexCode!;
        }

        var valueObservationCode = ObservationCode;
        UInt8Type.Accept(visitor, RtcmObservationCodeField, RtcmObservationCodeField.DataType, ref valueObservationCode);
        if (!EqualityComparer<byte>.Default.Equals(valueObservationCode, ObservationCode))
        {
            ObservationCode = valueObservationCode;
        }

        var valueObservationIndex = ObservationIndex;
        Int32Type.Accept(visitor, RtcmObservationIndexField, RtcmObservationIndexField.DataType, ref valueObservationIndex);
        if (!EqualityComparer<int>.Default.Equals(valueObservationIndex, ObservationIndex))
        {
            ObservationIndex = valueObservationIndex;
        }
    }
}
