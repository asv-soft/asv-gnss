using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Asv.IO;

namespace Asv.Gnss;

public class AsterixFieldI020Frn020Type400 : AsterixField
{
    public const byte StaticFrn = 20;
    public override string Name => "Contributing Devices";
    public override int Category => AsterixMessageI020.Category;
    public override byte FieldReferenceNumber => StaticFrn;
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        _devices.Clear();

        if (buffer.IsEmpty)
            return;

        int octetCount =  buffer[0];
        buffer = buffer[1..];

        for (var i = 0; i < octetCount * 8; i++)
        {
            var byteIndex = i / 8;
            var bitIndex = 7 - (i % 8); // ASTERIX: биты нумеруются справа налево

            if (byteIndex >= buffer.Length) continue;
            if ((buffer[byteIndex] & (1 << bitIndex)) != 0)
            {
                _devices.Add((byte)(i + 1)); // Устройство № начинается с 1
            }
        }

        buffer = buffer[octetCount..];
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        if (_devices.Count == 0)
        {
            buffer[0] = 0;
            buffer = buffer[1..];
            return;
        }

        int maxDevice = _devices.Max();
        var octetCount = (maxDevice + 7) / 8;

        buffer[0] = (byte)octetCount;
        buffer = buffer[1..];

        Span<byte> octets = stackalloc byte[octetCount];
        foreach (int device in _devices)
        {
            if (device < 1 || device > octetCount * 8)
                continue;

            var byteIndex = (device - 1) / 8;
            var bitIndex = 7 - ((device - 1) % 8);
            octets[byteIndex] |= (byte)(1 << bitIndex);
        }

        octets.CopyTo(buffer);
        buffer = buffer[octetCount..];
    }

    public override int GetByteSize()
    {
        if (_devices.Count == 0)
        {
            return 1;
        } 

        int maxDevice = _devices.Max(); // максимальный номер устройства
        var octetCount = (maxDevice + 7) / 8; // ceil(max / 8)
        return 1 + octetCount; // 1 байт длины + биты
    }

    public override void Accept(IVisitor visitor)
    {
        ListType.Accept(visitor, DevicesField, DevicesField.DataType, _devices, (index, v, f, t) =>
        {
            var val = _devices[index];
            UInt8Type.Accept(v,f,t, ref val);
            _devices[index] = val;
        });
    }
    
    private static readonly Field DevicesField = new Field.Builder()
        .Name(nameof(Devices))
        .Title(nameof(Devices))
        .Description("Devices")
        .DataType(new ListType(UInt8Type.Default,byte.MinValue,byte.MaxValue))
        .Build();

    private readonly List<byte> _devices = new();

    public List<byte> Devices => _devices;
}