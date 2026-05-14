using System;
using System.Collections.Generic;

namespace Asv.Gnss;

/// <summary>
/// I048/250 Mode S MB Data.
/// </summary>
public sealed class AsterixFieldI048Frn010Type250 : AsterixFieldI048
{
    /// <summary>
    /// Field reference number.
    /// </summary>
    public const byte StaticFrn = 10;

    /// <inheritdoc />
    public override string Name => "Mode S MB Data";

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    /// <summary>
    /// Repetitive Mode S MB data blocks.
    /// </summary>
    public List<AsterixI048ModeSMbData> Data { get; } = [];

    /// <inheritdoc />
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        var count = buffer[0];
        buffer = buffer[1..];
        Data.Clear();
        for (var i = 0; i < count; i++)
        {
            var item = new AsterixI048ModeSMbData();
            buffer[..7].CopyTo(item.MbData);
            buffer = buffer[7..];
            item.Bds1 = (byte)(buffer[0] >> 4);
            item.Bds2 = (byte)(buffer[0] & 0x0F);
            buffer = buffer[1..];
            Data.Add(item);
        }
    }

    /// <inheritdoc />
    public override void Serialize(ref Span<byte> buffer)
    {
        buffer[0] = (byte)Data.Count;
        buffer = buffer[1..];
        foreach (var item in Data)
        {
            item.MbData.CopyTo(buffer);
            buffer = buffer[7..];
            buffer[0] = (byte)((item.Bds1 << 4) | (item.Bds2 & 0x0F));
            buffer = buffer[1..];
        }
    }

    /// <inheritdoc />
    public override int GetByteSize() => 1 + Data.Count * 8;
}