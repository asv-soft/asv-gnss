using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class SbfPacketMeasEpochRev1
{
    private static readonly Field PayloadCumClkJumpsField = SbfComNavVisitor.Field(nameof(CumClkJumps), DoubleType.Default, "Cumulative millisecond clock jumps since start-up, with an ambiguity of k*256 ms.For example, if two clock jumps of -1 ms have occurred since startup, this field contains the value 254.");
    private static readonly Field PayloadSubBlocksField = SbfComNavVisitor.Field(nameof(SubBlocks), new ListType(MeasEpochChannelType1.PayloadVisitorType, 0, ushort.MaxValue), "SubBlocks");
    private static readonly Field PayloadReservedField = SbfComNavVisitor.Field(nameof(Reserved), UInt8Type.Default, "Reserved for future use, to be ignored by decoding software");
    private static readonly Field PayloadCommonFlagsField = SbfComNavVisitor.Field(nameof(CommonFlags), UInt8Type.Default, "CommonFlags", enumType: typeof(MeasEpochCommonFlags));
    private static readonly Field PayloadSB2LengthField = SbfComNavVisitor.Field(nameof(SB2Length), UInt8Type.Default, "Length of a MeasEpochChannelType2 sub-block");
    private static readonly Field PayloadSB1LengthField = SbfComNavVisitor.Field(nameof(SB1Length), UInt8Type.Default, "Length of a MeasEpochChannelType1 sub-block, excluding the nested MeasEpochChannelType2 sub-blocks");

    internal new static readonly StructType PayloadVisitorType = new([..SbfMessageBase.PayloadVisitorType.Fields, PayloadCumClkJumpsField, PayloadSubBlocksField, PayloadReservedField, PayloadCommonFlagsField, PayloadSB2LengthField, PayloadSB1LengthField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        base.Accept(visitor);

        var valueCumClkJumps = CumClkJumps;
        DoubleType.Accept(visitor, PayloadCumClkJumpsField, PayloadCumClkJumpsField.DataType, ref valueCumClkJumps);
        if (!EqualityComparer<double>.Default.Equals(valueCumClkJumps, CumClkJumps))
        {
            CumClkJumps = valueCumClkJumps;
        }

        var valueSubBlocks = SubBlocks;
        SbfComNavVisitor.VisitArray(visitor, PayloadSubBlocksField, PayloadSubBlocksField.DataType, ref valueSubBlocks, static () => new MeasEpochChannelType1(),
            static (IVisitor v, Field f, IFieldType t, ref MeasEpochChannelType1 item) =>
            {
                if (item is not null)
                {
                    StructType.Accept(v, f, t, item);
                }
                else
                {
                    v.VisitUnknown(f, t);
                }
            });
        if (!ReferenceEquals(valueSubBlocks, SubBlocks))
        {
            SubBlocks = valueSubBlocks!;
        }

        var valueReserved = Reserved;
        UInt8Type.Accept(visitor, PayloadReservedField, PayloadReservedField.DataType, ref valueReserved);
        if (!EqualityComparer<byte>.Default.Equals(valueReserved, Reserved))
        {
            Reserved = valueReserved;
        }

        var valueCommonFlags = CommonFlags;
        var numericvalueCommonFlags = (byte)valueCommonFlags;
        UInt8Type.Accept(visitor, PayloadCommonFlagsField, PayloadCommonFlagsField.DataType, ref numericvalueCommonFlags);
        valueCommonFlags = (MeasEpochCommonFlags)numericvalueCommonFlags;
        if (!EqualityComparer<MeasEpochCommonFlags>.Default.Equals(valueCommonFlags, CommonFlags))
        {
            CommonFlags = valueCommonFlags;
        }

        var valueSB2Length = SB2Length;
        UInt8Type.Accept(visitor, PayloadSB2LengthField, PayloadSB2LengthField.DataType, ref valueSB2Length);
        if (!EqualityComparer<byte>.Default.Equals(valueSB2Length, SB2Length))
        {
            SB2Length = valueSB2Length;
        }

        var valueSB1Length = SB1Length;
        UInt8Type.Accept(visitor, PayloadSB1LengthField, PayloadSB1LengthField.DataType, ref valueSB1Length);
        if (!EqualityComparer<byte>.Default.Equals(valueSB1Length, SB1Length))
        {
            SB1Length = valueSB1Length;
        }
    }
}
