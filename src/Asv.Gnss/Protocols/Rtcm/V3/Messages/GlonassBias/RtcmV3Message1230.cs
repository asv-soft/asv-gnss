using System;
using System.Linq;
using Asv.IO;

namespace Asv.Gnss;

public class RtcmV3Message1230 : RtcmV3MessageBase
{
    private const double DfResol = 0.02;
    private short _l1CACodePhaseBiasDf;
    private short _l1PCodePhaseBiasDf;
    private short _l2CACodePhaseBiasDf;
    private short _l2PCodePhaseBiasDf;

    protected override void InternalDeserialize(ReadOnlySpan<byte> buffer, ref int bitIndex)
    {
        ReferenceStationID = SpanBitHelper.GetBitU(buffer, ref bitIndex, 12);
        CodePhaseBiasIndicator = (byte)SpanBitHelper.GetBitU(buffer, ref bitIndex, 1);
        ReservedBias = (byte)SpanBitHelper.GetBitU(buffer, ref bitIndex, 3);
        FdmaSignalMask = (FdmaSignalBitmask)SpanBitHelper.GetBitU(buffer, ref bitIndex, 4);
        
        var biasLength = (buffer.Length - bitIndex / 8 - 3) / 2;
        if (biasLength == 0) return;
        if ((FdmaSignalMask & FdmaSignalBitmask.L2PMask) == FdmaSignalBitmask.L2PMask)
        {
            _l2PCodePhaseBiasDf = (short)SpanBitHelper.GetBitU(buffer, ref bitIndex, 16);
            --biasLength;
        }
        if (biasLength == 0) return;
        if ((FdmaSignalMask & FdmaSignalBitmask.L2CAMask) == FdmaSignalBitmask.L2CAMask)
        {
            _l2CACodePhaseBiasDf = (short)SpanBitHelper.GetBitU(buffer, ref bitIndex, 16);
            --biasLength;
        }
        if (biasLength == 0) return;
        if ((FdmaSignalMask & FdmaSignalBitmask.L1PMask) == FdmaSignalBitmask.L1PMask)
        {
            _l1PCodePhaseBiasDf = (short)SpanBitHelper.GetBitU(buffer, ref bitIndex, 16);
            --biasLength;
        }
        if (biasLength == 0) return;
        
        if ((FdmaSignalMask & FdmaSignalBitmask.CAL1Mask) != FdmaSignalBitmask.CAL1Mask) return;
        _l1CACodePhaseBiasDf = (short)SpanBitHelper.GetBitU(buffer, ref bitIndex, 16);
    }

    protected override void InternalSerialize(Span<byte> buffer, ref int bitIndex)
    {
        SpanBitHelper.SetBitU(buffer, ref bitIndex, 12, ReferenceStationID);
        SpanBitHelper.SetBitU(buffer, ref bitIndex, 1, CodePhaseBiasIndicator);
        SpanBitHelper.SetBitU(buffer, ref bitIndex, 3, ReservedBias);
        SpanBitHelper.SetBitU(buffer, ref bitIndex, 4, (uint)FdmaSignalMask);

        if ((FdmaSignalMask & FdmaSignalBitmask.L2PMask) == FdmaSignalBitmask.L2PMask)
        {
            SpanBitHelper.SetBitU(buffer, ref bitIndex, 16, _l2PCodePhaseBiasDf);
        }
        if ((FdmaSignalMask & FdmaSignalBitmask.L2CAMask) == FdmaSignalBitmask.L2CAMask)
        {
            SpanBitHelper.SetBitU(buffer, ref bitIndex, 16, _l2CACodePhaseBiasDf);
        }
        if ((FdmaSignalMask & FdmaSignalBitmask.L1PMask) == FdmaSignalBitmask.L1PMask)
        {
            SpanBitHelper.SetBitU(buffer, ref bitIndex, 16, _l1PCodePhaseBiasDf);
        }
        if ((FdmaSignalMask & FdmaSignalBitmask.CAL1Mask) == FdmaSignalBitmask.CAL1Mask)
        {
            SpanBitHelper.SetBitU(buffer, ref bitIndex, 16, _l1CACodePhaseBiasDf);
        }
    }

    protected override int InternalGetBitSize()
    {
        var biasLength = Enum.GetValues<FdmaSignalBitmask>().Count(bitmask => (FdmaSignalMask & bitmask) == bitmask);

        return 12 + 1 + 3 + 4 + 16 * biasLength;
    }

    public static readonly ushort MessageId = 1230;

    public override string Name => "Glonass bias information";
    public override ushort Id => MessageId;

    /// <summary>
    /// The Reference Station ID is determined by the service provider. Its 
    /// primary purpose is to link all message data to their unique sourceName. It is 
    /// useful in distinguishing between desired and undesired data in cases 
    /// where more than one service may be using the same data link 
    /// frequency. It is also useful in accommodating multiple reference 
    /// stations within a single data link transmission. 
    /// In reference network applications the Reference Station ID plays an 
    /// important role, because it is the link between the observation messages 
    /// of a specific reference station and its auxiliary information contained in 
    /// other messages for proper operation. Thus the Service Provider should 
    /// ensure that the Reference Station ID is unique within the whole 
    /// network, and that ID’s should be reassigned only when absolutely 
    /// necessary. 
    /// Service Providers may need to coordinate their Reference Station ID
    /// assignments with other Service Providers in their region in order to 
    /// avoid conflicts. This may be especially critical for equipment 
    /// accessing multiple services, depending on their services and means of 
    /// information distribution.
    /// </summary>
    public uint ReferenceStationID { get; set; }

    /// <summary>
    /// DF421 
    /// 0 = The GLONASS Pseudorange and PhaseRange observations in the
    /// data stream are not aligned to the same measurement epoch.
    /// 1 = The GLONASS Pseudorange and PhaseRange observations in the
    /// data stream are aligned to the same measurement epoch.
    /// </summary>
    public byte CodePhaseBiasIndicator { get; set; }

    /// <summary>
    /// Reserved
    /// </summary>
    public byte ReservedBias { get; set; }

    /// <summary>
    /// DF422
    /// Bitmask, where the MSB corresponds to the presence or absence of
    /// DF423, continuing to the LSB or last bit corresponding to the absence
    /// or presence of DF426.If the bit is set to zero then the corresponding
    /// field is absent from the message.If the bit is set to one then the
    /// corresponding field is present in the message
    /// </summary>
    public FdmaSignalBitmask FdmaSignalMask { get; set; }

    /// <summary>
    /// The GLONASS L1 C/A Code-Phase Bias represents the offset between
    /// the L1 C/A Pseudorange and L1 PhaseRange measurement epochs in
    /// meters.
    /// If DF421 is set to 0, the measurement epoch of the GLONASS L1
    /// PhaseRange measurements may be aligned using:
    /// Aligned GLONASS L1 PhaseRange = Full GLONASS L1 PhaseRange
    /// (see DF042) + GLONASS L1 C/A Code-Phase Bias.
    /// If DF421 is set to 1, the measurement epoch of the GLONASS L1
    /// PhaseRange measurements may be unaligned using:
    /// Unaligned GLONASS L1 PhaseRange = Full GLONASS L1
    /// PhaseRange(see DF042)  GLONASS L1 C/A Code-Phase Bias.
    /// A bit pattern equivalent to 8000h (-655.36 m) in this field indicates
    /// invalid value (unknown or falling outside DF-range). DF Resolution = 0.02
    /// </summary>
    public double L1CACodePhaseBias
    {
        get => _l1CACodePhaseBiasDf * DfResol;
        set => _l1CACodePhaseBiasDf = (short)Math.Round(value / DfResol);
    }

    /// <summary>
    /// The GLONASS L1 P Code-Phase Bias represents the offset between the
    /// L1 P Pseudorange and L1 PhaseRange measurement epochs in meters.
    /// If DF421 is set to 0, the measurement epoch of the GLONASS L1
    /// PhaseRange measurements may be aligned using:
    /// Aligned GLONASS L1 PhaseRange = Full GLONASS L1 PhaseRange
    /// (see DF042) + GLONASS L1 P Code-Phase Bias.
    /// If DF421 is set to 1, the measurement epoch of the GLONASS L1
    /// PhaseRange measurements may be unaligned using:
    /// Unaligned GLONASS L1 PhaseRange = Full GLONASS L1
    /// PhaseRange(see DF042)  GLONASS L1 P Code-Phase Bias.
    /// A bit pattern equivalent to 8000h (-655.36 m) in this field indicates
    /// invalid value (unknown or falling outside DF-range). DF Resolution = 0.02
    /// </summary>
    public double L1PCodePhaseBias
    {
        get => _l1PCodePhaseBiasDf * DfResol;
        set => _l1PCodePhaseBiasDf = (short)Math.Round(value / DfResol);
    }

    /// <summary>
    //  The GLONASS L2 C/A Code-Phase Bias represents the offset between
    //  the L2 C/A Pseudorange and L2 PhaseRange measurement epochs in
    //  meters.
    //  If DF421 is set to 0, the measurement epoch of the GLONASS L2
    //  PhaseRange measurements may be aligned to the Pseudorange
    //  measurements using:
    //  Aligned GLONASS L2 PhaseRange = Full GLONASS L2 PhaseRange
    //  (see DF048) + GLONASS L2 C/A Code-Phase Bias.
    //  If DF421 is set to 1, the measurement epoch of the GLONASS L2
    //  PhaseRange measurements may be unaligned using:
    //  Unaligned GLONASS L2 PhaseRange = Full GLONASS L2
    //  PhaseRange(see DF048)  GLONASS L2 C/A Code-Phase Bias.
    //  A bit pattern equivalent to 8000h (-655.36 m) in this field indicates
    //  invalid value (unknown or falling outside DF-range). DF Resolution = 0.02
    /// </summary>
    public double L2CACodePhaseBias
    {
        get => _l2CACodePhaseBiasDf * DfResol;
        set => _l2CACodePhaseBiasDf = (short)Math.Round(value / DfResol);
    }

    /// <summary>
    /// The GLONASS L2 P Code-Phase Bias represents the offset between the
    /// L2 P Pseudorange and L2 PhaseRange measurement epochs in meters.
    /// If DF421 is set to 0, the measurement epoch of the GLONASS L2
    /// PhaseRange measurements may be aligned to the Pseudorange
    /// measurements using:
    /// Aligned GLONASS L2 PhaseRange = Full GLONASS L2 PhaseRange
    /// (see DF048) + GLONASS L2 P Code-Phase Bias.
    /// If DF421 is set to 1, the measurement epoch of the GLONASS L2
    /// PhaseRange measurements may be unaligned using:
    /// Unaligned GLONASS L2 PhaseRange = Full GLONASS L2
    /// PhaseRange(see DF048)  GLONASS L2 P Code-Phase Bias.
    /// A bit pattern equivalent to 8000h (-655.36 m) in this field indicates
    /// invalid value (unknown or falling outside DF-range). DF Resolution = 0.02
    /// </summary>
    public double L2PCodePhaseBias
    {
        get => _l2PCodePhaseBiasDf * DfResol;
        set => _l2PCodePhaseBiasDf = (short)Math.Round(value / DfResol);
    }

    [Flags]
    public enum FdmaSignalBitmask
    {
        CAL1Mask = 0x1,
        L1PMask = 0x2,
        L2CAMask = 0x4,
        L2PMask = 0x8,
    }
}