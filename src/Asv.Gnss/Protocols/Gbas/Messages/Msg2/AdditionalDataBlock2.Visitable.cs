using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AdditionalDataBlock2
{
    private static readonly Field PayloadChannelNumberField = ProtocolPayloadVisitor.Field(nameof(ChannelNumber), UInt16Type.Default);
    private static readonly Field PayloadDeltaLatitudeField = ProtocolPayloadVisitor.Field(nameof(DeltaLatitude), DoubleType.Default);
    private static readonly Field PayloadDeltaLongitudeField = ProtocolPayloadVisitor.Field(nameof(DeltaLongitude), DoubleType.Default);

    internal new static readonly StructType PayloadVisitorType = new([..AdditionalDataBlock.PayloadVisitorType.Fields, PayloadChannelNumberField, PayloadDeltaLatitudeField, PayloadDeltaLongitudeField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        base.Accept(visitor);

        ushort valueChannelNumber = ChannelNumber;
        UInt16Type.Accept(visitor, PayloadChannelNumberField, PayloadChannelNumberField.DataType, ref valueChannelNumber);
        if (!EqualityComparer<ushort>.Default.Equals(valueChannelNumber, ChannelNumber))
        {
            ChannelNumber = valueChannelNumber;
        }

        double valueDeltaLatitude = DeltaLatitude;
        DoubleType.Accept(visitor, PayloadDeltaLatitudeField, PayloadDeltaLatitudeField.DataType, ref valueDeltaLatitude);
        if (!EqualityComparer<double>.Default.Equals(valueDeltaLatitude, DeltaLatitude))
        {
            DeltaLatitude = valueDeltaLatitude;
        }

        double valueDeltaLongitude = DeltaLongitude;
        DoubleType.Accept(visitor, PayloadDeltaLongitudeField, PayloadDeltaLongitudeField.DataType, ref valueDeltaLongitude);
        if (!EqualityComparer<double>.Default.Equals(valueDeltaLongitude, DeltaLongitude))
        {
            DeltaLongitude = valueDeltaLongitude;
        }
    }
}
