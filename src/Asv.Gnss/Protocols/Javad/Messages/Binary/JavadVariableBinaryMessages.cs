using System;

namespace Asv.Gnss;

public sealed partial class JavadGpsEphemerisOptionalData
{
    internal void Deserialize(ref ReadOnlySpan<byte> buffer, JavadByteOrder order)
    {
        NavType = JavadBinary.ReadU1(ref buffer);
        LTope = JavadBinary.ReadI4(ref buffer, order);
        LTopc = JavadBinary.ReadI4(ref buffer, order);
        DADot = JavadBinary.ReadF8(ref buffer, order);
        FDelnDot = JavadBinary.ReadF4(ref buffer, order);
        CURAoe = JavadBinary.ReadI1(ref buffer);
        CURAoc = JavadBinary.ReadI1(ref buffer);
        CURAoc1 = JavadBinary.ReadI1(ref buffer);
        CURAoc2 = JavadBinary.ReadI1(ref buffer);
        Cnav = null;
        Cnav2 = null;
        if ((NavType & 8) == 0)
        {
            Cnav = new();
            Cnav.Deserialize(ref buffer, order);
        }
        else
        {
            Cnav2 = new();
            Cnav2.Deserialize(ref buffer, order);
        }
        DAf0 = JavadBinary.ReadF4(ref buffer, order);
    }

    internal void Serialize(ref Span<byte> buffer, JavadByteOrder order)
    {
        JavadBinary.WriteU1(ref buffer, NavType);
        JavadBinary.WriteI4(ref buffer, LTope, order);
        JavadBinary.WriteI4(ref buffer, LTopc, order);
        JavadBinary.WriteF8(ref buffer, DADot, order);
        JavadBinary.WriteF4(ref buffer, FDelnDot, order);
        JavadBinary.WriteI1(ref buffer, CURAoe);
        JavadBinary.WriteI1(ref buffer, CURAoc);
        JavadBinary.WriteI1(ref buffer, CURAoc1);
        JavadBinary.WriteI1(ref buffer, CURAoc2);
        if ((NavType & 8) == 0)
        {
            Cnav!.Serialize(ref buffer, order);
        }
        else
        {
            Cnav2!.Serialize(ref buffer, order);
        }
        JavadBinary.WriteF4(ref buffer, DAf0, order);
    }

    internal int GetByteSize()
    {
        if ((NavType & 8) == 0)
        {
            if (Cnav is null || Cnav2 is not null)
            {
                throw new InvalidOperationException("GPS CNAV ephemeris requires its 16-byte ISC variant.");
            }
            return 45;
        }
        if (Cnav2 is null || Cnav is not null)
        {
            throw new InvalidOperationException("GPS CNAV2 ephemeris requires its 8-byte ISC variant.");
        }
        return 37;
    }
}

public sealed partial class JavadBeiDouModernEphemerisData
{
    internal void Deserialize(ref ReadOnlySpan<byte> buffer, JavadByteOrder order)
    {
        ADot = JavadBinary.ReadF8(ref buffer, order);
        DelNDot = JavadBinary.ReadF4(ref buffer, order);
        Isc_B1C = JavadBinary.ReadF4(ref buffer, order);
        Isc_B2A = JavadBinary.ReadF4(ref buffer, order);
        Tgd_B2Bdata = null;
        Tgd_B2Bpilot = null;
        if (buffer.Length >= 8)
        {
            Tgd_B2Bdata = JavadBinary.ReadF4(ref buffer, order);
            Tgd_B2Bpilot = JavadBinary.ReadF4(ref buffer, order);
        }
    }

    internal void Serialize(ref Span<byte> buffer, JavadByteOrder order)
    {
        JavadBinary.WriteF8(ref buffer, ADot, order);
        JavadBinary.WriteF4(ref buffer, DelNDot, order);
        JavadBinary.WriteF4(ref buffer, Isc_B1C, order);
        JavadBinary.WriteF4(ref buffer, Isc_B2A, order);
        if (Tgd_B2Bdata.HasValue)
        {
            JavadBinary.WriteF4(ref buffer, Tgd_B2Bdata.Value, order);
            JavadBinary.WriteF4(ref buffer, Tgd_B2Bpilot!.Value, order);
        }
    }

    internal int GetByteSize()
    {
        if (Tgd_B2Bdata.HasValue != Tgd_B2Bpilot.HasValue)
        {
            throw new InvalidOperationException("Both BeiDou B2B group-delay fields must be supplied together.");
        }
        return Tgd_B2Bdata.HasValue ? 28 : 20;
    }
}

public partial class JavadBeiDouEphemerisMessage
{
    public JavadBeiDouEphemerisMessage()
    {
        LegacyData = new();
    }

    private bool UsesLegacyData => (NavType & 7) != 0;

    protected override void DeserializePayload(ref ReadOnlySpan<byte> buffer)
    {
        RequiredData = new();
        RequiredData.Deserialize(ref buffer, ByteOrder);
        Tgd2 = JavadBinary.ReadF4(ref buffer, ByteOrder);
        NavType = JavadBinary.ReadU1(ref buffer);
        DAf0 = JavadBinary.ReadF4(ref buffer, ByteOrder);
        LegacyData = null;
        ModernData = null;
        if (UsesLegacyData)
        {
            LegacyData = new();
            LegacyData.Deserialize(ref buffer, ByteOrder);
        }
        else if ((NavType & 0x68) != 0)
        {
            ModernData = new();
            ModernData.Deserialize(ref buffer, ByteOrder);
        }
        else
        {
            throw new FormatException("BeiDou ephemeris has no supported navigation signal bit.");
        }
    }

    protected override void SerializePayload(ref Span<byte> buffer)
    {
        RequiredData.Serialize(ref buffer, ByteOrder);
        JavadBinary.WriteF4(ref buffer, Tgd2, ByteOrder);
        JavadBinary.WriteU1(ref buffer, NavType);
        JavadBinary.WriteF4(ref buffer, DAf0, ByteOrder);
        if (UsesLegacyData)
        {
            LegacyData!.Serialize(ref buffer, ByteOrder);
        }
        else
        {
            ModernData!.Serialize(ref buffer, ByteOrder);
        }
    }

    protected override int GetPayloadByteSize()
    {
        if (UsesLegacyData)
        {
            if (LegacyData is null || ModernData is not null)
            {
                throw new InvalidOperationException("BeiDou legacy ephemeris requires AODC and AODE.");
            }
            return 133;
        }
        if ((NavType & 0x68) == 0 || ModernData is null || LegacyData is not null)
        {
            throw new InvalidOperationException("BeiDou modern ephemeris requires its selected ISC variant.");
        }
        return 131 + ModernData.GetByteSize();
    }
}

public partial class JavadIqSamplesMessage
{
    protected override void DeserializePayload(ref ReadOnlySpan<byte> buffer)
    {
        Satellite = new();
        Satellite.Deserialize(ref buffer, ByteOrder);
        SlotId = JavadBinary.ReadU1(ref buffer);
        IsQ = JavadBinary.ReadI1(ref buffer);
        var elementSize = SampleSize();
        var count = JavadBinaryPayload.GetRemainingCount(buffer.Length, 0, elementSize);
        Samples = new JavadIqSample[count];
        for (var i = 0; i < count; i++)
        {
            Samples[i] = new JavadIqSample
            {
                I = JavadBinary.ReadI2(ref buffer, ByteOrder),
                Q = IsQ == 1 ? JavadBinary.ReadI2(ref buffer, ByteOrder) : null,
            };
        }
    }

    protected override void SerializePayload(ref Span<byte> buffer)
    {
        Satellite.Serialize(ref buffer, ByteOrder);
        JavadBinary.WriteU1(ref buffer, SlotId);
        JavadBinary.WriteI1(ref buffer, IsQ);
        foreach (var sample in Samples)
        {
            JavadBinary.WriteI2(ref buffer, sample.I, ByteOrder);
            if (IsQ == 1)
            {
                JavadBinary.WriteI2(ref buffer, sample.Q!.Value, ByteOrder);
            }
        }
    }

    protected override int GetPayloadByteSize()
    {
        var size = SampleSize();
        foreach (var sample in Samples)
        {
            if (sample.Q.HasValue != (IsQ == 1))
            {
                throw new InvalidOperationException("IQ quadrature fields must match IsQ.");
            }
        }
        return checked(4 + Samples.Length * size);
    }

    private int SampleSize() => IsQ switch
    {
        0 => 2,
        1 => 4,
        _ => throw new FormatException("GREIS IQ IsQ must be zero or one."),
    };
}

public partial class JavadSpectrumMessage
{
    protected override void DeserializePayload(ref ReadOnlySpan<byte> buffer)
    {
        CurrFrq = JavadBinary.ReadI2(ref buffer, ByteOrder);
        FinalFrq = JavadBinary.ReadI2(ref buffer, ByteOrder);
        SpectrumCount = JavadBinary.ReadU1(ref buffer);
        BlockCount = JavadBinary.ReadU1(ref buffer);
        JavadBinaryPayload.RequireAvailable(buffer.Length, SpectrumCount * BlockCount, 2);
        Blocks = new JavadSpectrumBlock[BlockCount];
        for (var i = 0; i < Blocks.Length; i++)
        {
            var block = new JavadSpectrumBlock { Values = new short[SpectrumCount] };
            for (var j = 0; j < block.Values.Length; j++)
            {
                block.Values[j] = JavadBinary.ReadI2(ref buffer, ByteOrder);
            }
            Blocks[i] = block;
        }
    }

    protected override void SerializePayload(ref Span<byte> buffer)
    {
        JavadBinary.WriteI2(ref buffer, CurrFrq, ByteOrder);
        JavadBinary.WriteI2(ref buffer, FinalFrq, ByteOrder);
        JavadBinary.WriteU1(ref buffer, SpectrumCount);
        JavadBinary.WriteU1(ref buffer, BlockCount);
        foreach (var block in Blocks)
        {
            foreach (var value in block.Values)
            {
                JavadBinary.WriteI2(ref buffer, value, ByteOrder);
            }
        }
    }

    protected override int GetPayloadByteSize()
    {
        JavadBinaryPayload.RequireLength(Blocks, BlockCount, nameof(Blocks));
        foreach (var block in Blocks)
        {
            JavadBinaryPayload.RequireLength(block.Values, SpectrumCount, nameof(block.Values));
        }
        return 6 + BlockCount * SpectrumCount * 2;
    }
}

public partial class JavadExtendedSpectrumMessage
{
    protected override void DeserializePayload(ref ReadOnlySpan<byte> buffer)
    {
        CurrFrq = JavadBinary.ReadI2(ref buffer, ByteOrder);
        FinalFrq = JavadBinary.ReadI2(ref buffer, ByteOrder);
        SpectrumCount = JavadBinary.ReadU1(ref buffer);
        BlockCount = JavadBinary.ReadU1(ref buffer);
        JavadBinaryPayload.RequireAvailable(buffer.Length, SpectrumCount * BlockCount, 4);
        Blocks = new JavadExtendedSpectrumBlock[BlockCount];
        for (var i = 0; i < Blocks.Length; i++)
        {
            var block = new JavadExtendedSpectrumBlock { Values = new short[SpectrumCount] };
            for (var j = 0; j < block.Values.Length; j++)
            {
                block.Values[j] = JavadBinary.ReadI2(ref buffer, ByteOrder);
            }
            block.AgcMinimum = JavadBinary.ReadBytes(ref buffer, SpectrumCount);
            block.AgcMaximum = JavadBinary.ReadBytes(ref buffer, SpectrumCount);
            Blocks[i] = block;
        }
    }

    protected override void SerializePayload(ref Span<byte> buffer)
    {
        JavadBinary.WriteI2(ref buffer, CurrFrq, ByteOrder);
        JavadBinary.WriteI2(ref buffer, FinalFrq, ByteOrder);
        JavadBinary.WriteU1(ref buffer, SpectrumCount);
        JavadBinary.WriteU1(ref buffer, BlockCount);
        foreach (var block in Blocks)
        {
            foreach (var value in block.Values)
            {
                JavadBinary.WriteI2(ref buffer, value, ByteOrder);
            }
            JavadBinary.WriteBytes(ref buffer, block.AgcMinimum);
            JavadBinary.WriteBytes(ref buffer, block.AgcMaximum);
        }
    }

    protected override int GetPayloadByteSize()
    {
        JavadBinaryPayload.RequireLength(Blocks, BlockCount, nameof(Blocks));
        foreach (var block in Blocks)
        {
            JavadBinaryPayload.RequireLength(block.Values, SpectrumCount, nameof(block.Values));
            JavadBinaryPayload.RequireLength(block.AgcMinimum, SpectrumCount, nameof(block.AgcMinimum));
            JavadBinaryPayload.RequireLength(block.AgcMaximum, SpectrumCount, nameof(block.AgcMaximum));
        }
        return 6 + BlockCount * SpectrumCount * 4;
    }
}

public partial class JavadSingleSpectrumMessage
{
    protected override void DeserializePayload(ref ReadOnlySpan<byte> buffer)
    {
        PointNumber = JavadBinary.ReadI4(ref buffer, ByteOrder);
        NameId = JavadBinary.ReadU1(ref buffer);
        ValuesPerPoint = JavadBinary.ReadU1(ref buffer);
        PointCount = JavadBinary.ReadU2(ref buffer, ByteOrder);
        var count = ValuesPerPoint * PointCount;
        JavadBinaryPayload.RequireAvailable(buffer.Length, count, 2);
        Values = new short[count];
        for (var i = 0; i < count; i++)
        {
            Values[i] = JavadBinary.ReadI2(ref buffer, ByteOrder);
        }
        SpectrumInfo = null;
        if (buffer.Length >= 33)
        {
            SpectrumInfo = new();
            SpectrumInfo.Deserialize(ref buffer, ByteOrder);
        }
    }

    protected override void SerializePayload(ref Span<byte> buffer)
    {
        JavadBinary.WriteI4(ref buffer, PointNumber, ByteOrder);
        JavadBinary.WriteU1(ref buffer, NameId);
        JavadBinary.WriteU1(ref buffer, ValuesPerPoint);
        JavadBinary.WriteU2(ref buffer, PointCount, ByteOrder);
        foreach (var value in Values)
        {
            JavadBinary.WriteI2(ref buffer, value, ByteOrder);
        }
        SpectrumInfo?.Serialize(ref buffer, ByteOrder);
    }

    protected override int GetPayloadByteSize()
    {
        JavadBinaryPayload.RequireLength(Values, ValuesPerPoint * PointCount, nameof(Values));
        return checked(8 + Values.Length * 2 + (SpectrumInfo?.GetByteSize() ?? 0));
    }
}

public partial class JavadLoggingHistoryMessage
{
    protected override void DeserializePayload(ref ReadOnlySpan<byte> buffer)
    {
        SatelliteCount = JavadBinary.ReadU1(ref buffer);
        TargetStream = JavadBinary.ReadU1(ref buffer);
        Issue = JavadBinary.ReadU2(ref buffer, ByteOrder);
        BitsCount = JavadBinary.ReadU2(ref buffer, ByteOrder);
        LastBitTime = JavadBinary.ReadU4(ref buffer, ByteOrder);
        Uids = JavadBinary.ReadBytes(ref buffer, SatelliteCount);
        Pad = JavadBinary.ReadBytes(ref buffer, PaddingCount);
        var count = HistoryWordCount;
        JavadBinaryPayload.RequireAvailable(buffer.Length, count, 4);
        Hist = new uint[count];
        for (var i = 0; i < count; i++)
        {
            Hist[i] = JavadBinary.ReadU4(ref buffer, ByteOrder);
        }
    }

    protected override void SerializePayload(ref Span<byte> buffer)
    {
        JavadBinary.WriteU1(ref buffer, SatelliteCount);
        JavadBinary.WriteU1(ref buffer, TargetStream);
        JavadBinary.WriteU2(ref buffer, Issue, ByteOrder);
        JavadBinary.WriteU2(ref buffer, BitsCount, ByteOrder);
        JavadBinary.WriteU4(ref buffer, LastBitTime, ByteOrder);
        JavadBinary.WriteBytes(ref buffer, Uids);
        JavadBinary.WriteBytes(ref buffer, Pad);
        foreach (var word in Hist)
        {
            JavadBinary.WriteU4(ref buffer, word, ByteOrder);
        }
    }

    protected override int GetPayloadByteSize()
    {
        JavadBinaryPayload.RequireLength(Uids, SatelliteCount, nameof(Uids));
        JavadBinaryPayload.RequireLength(Pad, PaddingCount, nameof(Pad));
        JavadBinaryPayload.RequireLength(Hist, HistoryWordCount, nameof(Hist));
        return 10 + SatelliteCount + PaddingCount + HistoryWordCount * 4;
    }

    private int PaddingCount => (4 - SatelliteCount % 4) % 4;
    private int HistoryWordCount => ((BitsCount + 31) / 32) * SatelliteCount;
}
