using System;
using System.Collections.Generic;
using System.Linq;
namespace Asv.Gnss;

public class MessageBlockType4 : GbasMessageBlockBase
{
    public const byte GbasMessageTypeId = 4;

    public override byte MessageTypeId => GbasMessageTypeId;

    public List<FasDataSet> FasDataSet { get; } = new();

    public override int GetBitSize()
    {
        return base.GetBitSize() + FasDataSet.Sum(_ => _.GetBitSize());
    }

    protected override void InternalSerialize(Span<byte> buffer, ref int bitPosition)
    {
        foreach (var fasDataSet in FasDataSet)
        {
            fasDataSet.Serialize(buffer, ref bitPosition);
        }
    }

    protected override void InternalDeserialize(ReadOnlySpan<byte> buffer, ref int bitPosition, int dataBitSize)
    {
        var origin = bitPosition;
        FasDataSet.Clear();
        while (dataBitSize - bitPosition + origin > 0)
        {
            var fast = new FasDataSet();
            fast.Deserialize(buffer, ref bitPosition);
            FasDataSet.Add(fast);
        }
    }
}

public enum RunwayLetterEnum : byte
{
    Non = 0,

    R = 1,

    C = 2,

    L = 3
}

public enum ApproachPerformanceDesignatorEnum : byte
{
    GastAB = 0,

    /// <summary>
    /// GAST C
    /// </summary>
    GastC = 1,

    GastCD = 2,

    GastCD1AddService = 3,

    GastCD2AddService = 4,

    NotReserved1 = 5,
    NotReserved2 = 6,
    NotReserved3 = 7,
}

public enum TCHUnitsEnum : byte
{
    Ft = 0,
    M = 1
}

public enum RouteIndicatorEnum
{
    Space = 0,
    A = 1,
    B = 2,
    C = 3,
    D = 4,
    E = 5,
    F = 6,
    G = 7,
    H = 8,
    J = 10,
    K = 11,
    L = 12,
    M = 13,
    N = 14,
    P = 16,
    Q = 17,
    R = 18,
    S = 19,
    T = 20,
    U = 21,
    V = 22,
    W = 23,
    X = 24,
    Y = 25,
    Z = 26
}
