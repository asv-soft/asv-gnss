using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class RtcmV3Message1230
{
    private static readonly Field RtcmReferenceStationIDField = RtcmV3Visitor.Field(nameof(ReferenceStationID), UInt32Type.Default, "The Reference Station ID is determined by the service provider. Its primary purpose is to link all message data to their unique sourceName. It is useful in distinguishing between desired and undesired data in cases where more than one service may be using the same data link frequency. It is also useful in accommodating multiple reference stations within a single data link transmission. In reference network applications the Reference Station ID plays an important role, because it is the link between the observation messages of a specific reference station and its auxiliary information contained in other messages for proper operation. Thus the Service Provider should ensure that the Reference Station ID is unique within the whole network, and that ID’s should be reassigned only when absolutely necessary. Service Providers may need to coordinate their Reference Station ID assignments with other Service Providers in their region in order to avoid conflicts. This may be especially critical for equipment accessing multiple services, depending on their services and means of information distribution.");
    private static readonly Field RtcmCodePhaseBiasIndicatorField = RtcmV3Visitor.Field(nameof(CodePhaseBiasIndicator), UInt8Type.Default, "DF421 0 = The GLONASS Pseudorange and PhaseRange observations in the data stream are not aligned to the same measurement epoch. 1 = The GLONASS Pseudorange and PhaseRange observations in the data stream are aligned to the same measurement epoch.");
    private static readonly Field RtcmReservedBiasField = RtcmV3Visitor.Field(nameof(ReservedBias), UInt8Type.Default, "Reserved");
    private static readonly Field RtcmFdmaSignalMaskField = RtcmV3Visitor.Field(nameof(FdmaSignalMask), Int32Type.Default, "DF422 Bitmask, where the MSB corresponds to the presence or absence of DF423, continuing to the LSB or last bit corresponding to the absence or presence of DF426.If the bit is set to zero then the corresponding field is absent from the message.If the bit is set to one then the corresponding field is present in the message", enumType: typeof(FdmaSignalBitmask));
    private static readonly Field RtcmL1CACodePhaseBiasField = RtcmV3Visitor.Field(nameof(L1CACodePhaseBias), DoubleType.Default, "The GLONASS L1 C/A Code-Phase Bias represents the offset between the L1 C/A Pseudorange and L1 PhaseRange measurement epochs in meters. If DF421 is set to 0, the measurement epoch of the GLONASS L1 PhaseRange measurements may be aligned using: Aligned GLONASS L1 PhaseRange = Full GLONASS L1 PhaseRange (see DF042) + GLONASS L1 C/A Code-Phase Bias. If DF421 is set to 1, the measurement epoch of the GLONASS L1 PhaseRange measurements may be unaligned using: Unaligned GLONASS L1 PhaseRange = Full GLONASS L1 PhaseRange(see DF042)  GLONASS L1 C/A Code-Phase Bias. A bit pattern equivalent to 8000h (-655.36 m) in this field indicates invalid value (unknown or falling outside DF-range). DF Resolution = 0.02");
    private static readonly Field RtcmL1PCodePhaseBiasField = RtcmV3Visitor.Field(nameof(L1PCodePhaseBias), DoubleType.Default, "The GLONASS L1 P Code-Phase Bias represents the offset between the L1 P Pseudorange and L1 PhaseRange measurement epochs in meters. If DF421 is set to 0, the measurement epoch of the GLONASS L1 PhaseRange measurements may be aligned using: Aligned GLONASS L1 PhaseRange = Full GLONASS L1 PhaseRange (see DF042) + GLONASS L1 P Code-Phase Bias. If DF421 is set to 1, the measurement epoch of the GLONASS L1 PhaseRange measurements may be unaligned using: Unaligned GLONASS L1 PhaseRange = Full GLONASS L1 PhaseRange(see DF042)  GLONASS L1 P Code-Phase Bias. A bit pattern equivalent to 8000h (-655.36 m) in this field indicates invalid value (unknown or falling outside DF-range). DF Resolution = 0.02");
    private static readonly Field RtcmL2CACodePhaseBiasField = RtcmV3Visitor.Field(nameof(L2CACodePhaseBias), DoubleType.Default, "// The GLONASS L2 C/A Code-Phase Bias represents the offset between // the L2 C/A Pseudorange and L2 PhaseRange measurement epochs in // meters. // If DF421 is set to 0, the measurement epoch of the GLONASS L2 // PhaseRange measurements may be aligned to the Pseudorange // measurements using: // Aligned GLONASS L2 PhaseRange = Full GLONASS L2 PhaseRange // (see DF048) + GLONASS L2 C/A Code-Phase Bias. // If DF421 is set to 1, the measurement epoch of the GLONASS L2 // PhaseRange measurements may be unaligned using: // Unaligned GLONASS L2 PhaseRange = Full GLONASS L2 // PhaseRange(see DF048)  GLONASS L2 C/A Code-Phase Bias. // A bit pattern equivalent to 8000h (-655.36 m) in this field indicates // invalid value (unknown or falling outside DF-range). DF Resolution = 0.02");
    private static readonly Field RtcmL2PCodePhaseBiasField = RtcmV3Visitor.Field(nameof(L2PCodePhaseBias), DoubleType.Default, "The GLONASS L2 P Code-Phase Bias represents the offset between the L2 P Pseudorange and L2 PhaseRange measurement epochs in meters. If DF421 is set to 0, the measurement epoch of the GLONASS L2 PhaseRange measurements may be aligned to the Pseudorange measurements using: Aligned GLONASS L2 PhaseRange = Full GLONASS L2 PhaseRange (see DF048) + GLONASS L2 P Code-Phase Bias. If DF421 is set to 1, the measurement epoch of the GLONASS L2 PhaseRange measurements may be unaligned using: Unaligned GLONASS L2 PhaseRange = Full GLONASS L2 PhaseRange(see DF048)  GLONASS L2 P Code-Phase Bias. A bit pattern equivalent to 8000h (-655.36 m) in this field indicates invalid value (unknown or falling outside DF-range). DF Resolution = 0.02");

    internal new static readonly StructType RtcmVisitorType = new([..RtcmV3MessageBase.RtcmVisitorType.Fields, RtcmReferenceStationIDField, RtcmCodePhaseBiasIndicatorField, RtcmReservedBiasField, RtcmFdmaSignalMaskField, RtcmL1CACodePhaseBiasField, RtcmL1PCodePhaseBiasField, RtcmL2CACodePhaseBiasField, RtcmL2PCodePhaseBiasField]);

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

        var valueCodePhaseBiasIndicator = CodePhaseBiasIndicator;
        UInt8Type.Accept(visitor, RtcmCodePhaseBiasIndicatorField, RtcmCodePhaseBiasIndicatorField.DataType, ref valueCodePhaseBiasIndicator);
        if (!EqualityComparer<byte>.Default.Equals(valueCodePhaseBiasIndicator, CodePhaseBiasIndicator))
        {
            CodePhaseBiasIndicator = valueCodePhaseBiasIndicator;
        }

        var valueReservedBias = ReservedBias;
        UInt8Type.Accept(visitor, RtcmReservedBiasField, RtcmReservedBiasField.DataType, ref valueReservedBias);
        if (!EqualityComparer<byte>.Default.Equals(valueReservedBias, ReservedBias))
        {
            ReservedBias = valueReservedBias;
        }

        var valueFdmaSignalMask = (int)FdmaSignalMask;
        Int32Type.Accept(visitor, RtcmFdmaSignalMaskField, RtcmFdmaSignalMaskField.DataType, ref valueFdmaSignalMask);
        if (!EqualityComparer<int>.Default.Equals(valueFdmaSignalMask, (int)FdmaSignalMask))
        {
            FdmaSignalMask = (FdmaSignalBitmask)valueFdmaSignalMask;
        }

        var valueL1CACodePhaseBias = L1CACodePhaseBias;
        DoubleType.Accept(visitor, RtcmL1CACodePhaseBiasField, RtcmL1CACodePhaseBiasField.DataType, ref valueL1CACodePhaseBias);
        if (!EqualityComparer<double>.Default.Equals(valueL1CACodePhaseBias, L1CACodePhaseBias))
        {
            L1CACodePhaseBias = valueL1CACodePhaseBias;
        }

        var valueL1PCodePhaseBias = L1PCodePhaseBias;
        DoubleType.Accept(visitor, RtcmL1PCodePhaseBiasField, RtcmL1PCodePhaseBiasField.DataType, ref valueL1PCodePhaseBias);
        if (!EqualityComparer<double>.Default.Equals(valueL1PCodePhaseBias, L1PCodePhaseBias))
        {
            L1PCodePhaseBias = valueL1PCodePhaseBias;
        }

        var valueL2CACodePhaseBias = L2CACodePhaseBias;
        DoubleType.Accept(visitor, RtcmL2CACodePhaseBiasField, RtcmL2CACodePhaseBiasField.DataType, ref valueL2CACodePhaseBias);
        if (!EqualityComparer<double>.Default.Equals(valueL2CACodePhaseBias, L2CACodePhaseBias))
        {
            L2CACodePhaseBias = valueL2CACodePhaseBias;
        }

        var valueL2PCodePhaseBias = L2PCodePhaseBias;
        DoubleType.Accept(visitor, RtcmL2PCodePhaseBiasField, RtcmL2PCodePhaseBiasField.DataType, ref valueL2PCodePhaseBias);
        if (!EqualityComparer<double>.Default.Equals(valueL2PCodePhaseBias, L2PCodePhaseBias))
        {
            L2PCodePhaseBias = valueL2PCodePhaseBias;
        }
    }
}
