using System;
using System.Buffers.Binary;
using Asv.IO;

namespace Asv.Gnss;

public class AsterixFieldI020Frn010Type090 : AsterixField
{
    private int _rawValue;
    public const byte StaticFrn = 10;
    public const string StaticName = "Flight Level and Vertical Rate";
    public override string Name => StaticName;
    
    
    public override int Category => AsterixMessageI020.Category;
    public override byte FieldReferenceNumber => StaticFrn;
    
    private static readonly Field VField = new Field.Builder()
        .Name(nameof(V))
        .Title("V")
        .Description("0 Code validated, 1 Code not validated")
        .DataType(BoolType.Default)
        .Build();

    public bool V
    {
        get => (_rawValue & (1 << 15)) != 0;
        set
        {
            if (value)
            {
                _rawValue |= (1 << 15);
            }
            else
            {
                _rawValue &= unchecked((ushort)~(1 << 15));
            }
        }
    }

    private static readonly Field GField = new Field.Builder()
        .Name(nameof(G))
        .Title("G")
        .Description("0 Default, 1 Garbled code")
        .DataType(BoolType.Default)
        .Build();

    
    public bool G
    {
        get => (_rawValue & (1 << 14)) != 0;
        set
        {
            if (value)
            {
                _rawValue |= (1 << 14);
            }
            else
            {
                _rawValue &= unchecked((ushort)~(1 << 14));
            }
        }
    }
    
   private static readonly Field FlightLevelField = new Field.Builder()
        .Name(nameof(FlightLevelFt))
        .Title("Flight Level")
        .Description("Flight Level in feet (positive or negative)")
        .DataType(DoubleType.Default)
        .Build();

   /// <summary>
   /// Gets or sets the Flight Level in feet.
   /// Represents the flight level as a positive or negative value, calculated from
   /// a 14-bit two’s complement format and scaled by 25 feet.
   /// </summary>
   public double FlightLevelFt
   {
        get
        {
            // Извлекаем 14-битное значение
            var raw = _rawValue & 0x3FFF;

            // Преобразование из two's complement
            if ((raw & 0x2000) != 0) // если установлен бит знака (бит 13)
            {
                raw |= unchecked((int)0xFFFFC000); // расширяем знак
            }

            return raw * 25.0;
        }
        set
        {
            var scaled = (int)Math.Round(value / 25.0);
            ushort raw;
            if (scaled < 0)
            {
                raw = (ushort)(scaled & 0x3FFF); // two’s complement в 14 битах
            }
            else
            {
                raw = (ushort)scaled;
            }

            var vgBits = (ushort)(_rawValue & 0xC000); // биты 15–16
            _rawValue = (ushort)(vgBits | raw);
        }
    }

   /// <summary>
   /// Gets or sets the "FlightLevelMsl" property, representing the flight level
   /// above mean sea level (MSL) in meters.
   /// Conversion between feet and meters is handled internally, where 1 foot equals 0.3048 meters.
   /// </summary>
   public double FlightLevelMsl
   {
        get => FlightLevelFt * 0.3048; // Convert feet to meters
        set => FlightLevelFt = value / 0.3048; // Convert meters to feet
    }

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        _rawValue = BinaryPrimitives.ReadInt16BigEndian(buffer);
        buffer = buffer[2..];
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        BinaryPrimitives.WriteUInt16BigEndian(buffer, (ushort)_rawValue);
        buffer = buffer[2..];
    }

    public override int GetByteSize() => 2;

    
    public override void Accept(IVisitor visitor)
    {
        var vValue = V;
        BoolType.Accept(visitor, VField, VField.DataType, ref vValue);
        V = vValue;

        var gValue = G;
        BoolType.Accept(visitor, GField, GField.DataType, ref gValue);
        G = gValue;

        var flightLevelValue = FlightLevelFt;
        DoubleType.Accept(visitor, FlightLevelField, FlightLevelField.DataType, ref flightLevelValue);
        FlightLevelFt = flightLevelValue;
    }
}