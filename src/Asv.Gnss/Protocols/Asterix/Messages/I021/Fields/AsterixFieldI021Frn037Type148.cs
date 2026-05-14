using System;
using System.Buffers.Binary;

namespace Asv.Gnss;

public sealed class AsterixFieldI021Frn037Type148 : AsterixFieldI021SelectedAltitude
{
    public const byte StaticFrn = 33;
    public override string Name => "Final State Selected Altitude";
    public override byte FieldReferenceNumber => StaticFrn;
    public bool ManageVerticalMode { get; set; }
    public bool AltitudeHoldMode { get; set; }
    public bool ApproachMode { get; set; }

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        var raw = BinaryPrimitives.ReadUInt16BigEndian(buffer);
        buffer = buffer[2..];
        ManageVerticalMode = (raw & 0x8000) != 0;
        AltitudeHoldMode = (raw & 0x4000) != 0;
        ApproachMode = (raw & 0x2000) != 0;
        AltitudeFt = AsterixI021Binary.SignExtend(raw & 0x1FFF, 13) * 25.0;
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        var value = (ManageVerticalMode ? 0x8000 : 0) | (AltitudeHoldMode ? 0x4000 : 0) | (ApproachMode ? 0x2000 : 0) |
                    ((int)Math.Round(AltitudeFt / 25.0) & 0x1FFF);
        BinaryPrimitives.WriteUInt16BigEndian(buffer, (ushort)value);
        buffer = buffer[2..];
    }
}