using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public abstract partial class RtcmV3Message1007and1008
{
    private static readonly Field RtcmReferenceStationIDField = RtcmV3Visitor.Field(nameof(ReferenceStationID), UInt32Type.Default, "The Reference Station ID is determined by the service provider. Its primary purpose is to link all message data to their unique sourceName. It is useful in distinguishing between desired and undesired data in cases where more than one service may be using the same data link frequency. It is also useful in accommodating multiple reference stations within a single data link transmission. In reference network applications the Reference Station ID plays an important role, because it is the link between the observation messages of a specific reference station and its auxiliary information contained in other messages for proper operation. Thus the Service Provider should ensure that the Reference Station ID is unique within the whole network, and that ID’s should be reassigned only when absolutely necessary. Service Providers may need to coordinate their Reference Station ID assignments with other Service Providers in their region in order to avoid conflicts. This may be especially critical for equipment accessing multiple services, depending on their services and means of information distribution.");
    private static readonly Field RtcmDescriptorCounterNField = RtcmV3Visitor.Field(nameof(DescriptorCounterN), UInt32Type.Default, "The Descriptor Counter defines the number of characters (bytes) to follow in DF030, Antenna Descriptor");
    private static readonly Field RtcmAntennaDescriptorField = RtcmV3Visitor.Field(nameof(AntennaDescriptor), StringOptionalType.Utf8, "Alphanumeric characters. IGS limits the number of characters to 20 at this time, but this DF allows more characters for future extension.");
    private static readonly Field RtcmAntennaSetupIDField = RtcmV3Visitor.Field(nameof(AntennaSetupID), UInt32Type.Default, "0=Use standard IGS Model 1-255=Specific Antenna Setup ID# The Antenna Setup ID is a parameter for use by the service provider to indicate the particular reference station-antenna combination.The number should be increased whenever a change occurs at the station that affects the antenna phase center variations. While the Antenna Descriptor and the Antenna Serial Number give an indication of when the installed antenna has been changed, it is envisioned that other changes could occur. For instance the antenna might been repaired, or the surrounding of the antenna might have been changed and the provider of the service may want to make the user station aware of the change.Depending on the change of the phase center variations due to a setup change, a change in the Antenna Setup ID would mean that the user should check with the service provider to see if the antenna phase center variation in use is still valid. Of course, the provider must make appropriate information available to the users.");

    internal new static readonly StructType RtcmVisitorType = new([..RtcmV3MessageBase.RtcmVisitorType.Fields, RtcmReferenceStationIDField, RtcmDescriptorCounterNField, RtcmAntennaDescriptorField, RtcmAntennaSetupIDField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        base.Accept(visitor);

        var valueReferenceStationID = ReferenceStationID;
        UInt32Type.Accept(visitor, RtcmReferenceStationIDField, RtcmReferenceStationIDField.DataType, ref valueReferenceStationID);
        if (!EqualityComparer<uint>.Default.Equals(valueReferenceStationID, ReferenceStationID))
        {
            ReferenceStationID = valueReferenceStationID;
        }

        var valueDescriptorCounterN = DescriptorCounterN;
        UInt32Type.Accept(visitor, RtcmDescriptorCounterNField, RtcmDescriptorCounterNField.DataType, ref valueDescriptorCounterN);
        if (!EqualityComparer<uint>.Default.Equals(valueDescriptorCounterN, DescriptorCounterN))
        {
            DescriptorCounterN = valueDescriptorCounterN;
        }

        string? valueAntennaDescriptor = AntennaDescriptor;
        StringOptionalType.Accept(visitor, RtcmAntennaDescriptorField, RtcmAntennaDescriptorField.DataType, ref valueAntennaDescriptor);
        if (!EqualityComparer<string>.Default.Equals(valueAntennaDescriptor, AntennaDescriptor))
        {
            AntennaDescriptor = valueAntennaDescriptor!;
        }

        var valueAntennaSetupID = AntennaSetupID;
        UInt32Type.Accept(visitor, RtcmAntennaSetupIDField, RtcmAntennaSetupIDField.DataType, ref valueAntennaSetupID);
        if (!EqualityComparer<uint>.Default.Equals(valueAntennaSetupID, AntennaSetupID))
        {
            AntennaSetupID = valueAntennaSetupID;
        }
    }
}
