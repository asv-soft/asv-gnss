using System;
using Asv.IO;

namespace Asv.Gnss;

public partial class AdditionalDataBlock3 : AdditionalDataBlock
{
    private const double P10_7 = 0.0000001;

    public const byte BlockNumber = 3;

    public AdditionalDataBlock3()
    {

    }

    protected override void InternalSerialize(Span<byte> buffer, ref int bitPosition)
    {
        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 8, (byte)Math.Round(Kmd_e_D_GPS / 0.05, 0));
        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 8, (byte)Math.Round(Kmd_e_D_Glonass / 0.05, 0));

        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 8, (byte)Math.Round(Sigma_vert_iono_gradient_D / P10_7, 0));

        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 5, (byte)Math.Round(Y_EIG / 0.1, 0));
        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 3, (byte)Math.Round(M_EIG / 0.1, 0));
    }

    protected override void InternalDeserialize(ReadOnlySpan<byte> buffer, ref int bitPosition, byte byteSize)
    {
        Kmd_e_D_GPS = SpanBitHelper.GetBitUReverse(buffer, ref bitPosition, 8) * 0.05;
        Kmd_e_D_Glonass = SpanBitHelper.GetBitUReverse(buffer, ref bitPosition, 8) * 0.05;

        Sigma_vert_iono_gradient_D = SpanBitHelper.GetBitUReverse(buffer, ref bitPosition, 8) * P10_7;

        Y_EIG = SpanBitHelper.GetBitUReverse(buffer, ref bitPosition, 5) * 0.1;
        M_EIG = SpanBitHelper.GetBitUReverse(buffer, ref bitPosition, 3) * 0.1;
    }

    public override int GetBitSize()
    {
        return base.GetBitSize() + 4 * 8;
    }

    public override byte TypeNumber => BlockNumber;

    public double Kmd_e_D_GPS { get; set; }

    public double Kmd_e_D_Glonass { get; set; }

    public double Sigma_vert_iono_gradient_D { get; set; }

    public double Y_EIG { get; set; }

    public double M_EIG { get; set; }
}
