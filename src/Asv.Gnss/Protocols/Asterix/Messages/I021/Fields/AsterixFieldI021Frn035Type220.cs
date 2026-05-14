using System;
using System.Buffers.Binary;

namespace Asv.Gnss;

public sealed class AsterixFieldI021Frn035Type220 : AsterixFieldI021Fixed
{
    public const byte StaticFrn = 31;
    public override string Name => "Met Information";
    public override byte FieldReferenceNumber => StaticFrn;
    public double? WindSpeed { get; set; }
    public double? WindDirection { get; set; }
    public double? Temperature { get; set; }
    public byte? Turbulence { get; set; }

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        var flags = new VariableLengthValue();
        flags.Deserialize(ref buffer);
        WindSpeed = flags[0] == true ? BinaryPrimitives.ReadUInt16BigEndian(buffer) : null;
        if (flags[0] == true) buffer = buffer[2..];
        WindDirection = flags[1] == true ? BinaryPrimitives.ReadUInt16BigEndian(buffer) : null;
        if (flags[1] == true) buffer = buffer[2..];
        Temperature = flags[2] == true ? BinaryPrimitives.ReadInt16BigEndian(buffer) * 0.25 : null;
        if (flags[2] == true) buffer = buffer[2..];
        Turbulence = flags[3] == true ? buffer[0] : null;
        if (flags[3] == true) buffer = buffer[1..];
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        var flags = new VariableLengthValue();
        if (WindSpeed.HasValue) flags[0] = true;
        if (WindDirection.HasValue) flags[1] = true;
        if (Temperature.HasValue) flags[2] = true;
        if (Turbulence.HasValue) flags[3] = true;
        flags.Serialize(ref buffer);
        if (WindSpeed.HasValue)
        {
            BinaryPrimitives.WriteUInt16BigEndian(buffer, (ushort)Math.Round(WindSpeed.Value));
            buffer = buffer[2..];
        }

        if (WindDirection.HasValue)
        {
            BinaryPrimitives.WriteUInt16BigEndian(buffer, (ushort)Math.Round(WindDirection.Value));
            buffer = buffer[2..];
        }

        if (Temperature.HasValue)
        {
            BinaryPrimitives.WriteInt16BigEndian(buffer, (short)Math.Round(Temperature.Value / 0.25));
            buffer = buffer[2..];
        }

        if (Turbulence.HasValue)
        {
            buffer[0] = Turbulence.Value;
            buffer = buffer[1..];
        }
    }

    public override int GetByteSize() => 1 + (WindSpeed.HasValue ? 2 : 0) + (WindDirection.HasValue ? 2 : 0) +
                                         (Temperature.HasValue ? 2 : 0) + (Turbulence.HasValue ? 1 : 0);
}