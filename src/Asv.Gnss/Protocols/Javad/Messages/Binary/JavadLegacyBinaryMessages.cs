using System;

namespace Asv.Gnss;

public partial class JavadLegacyRawMeasurementsMessage
{
    public int Version => Flags >> 13;
    public int HeaderSize => 6 + ((Flags >> 5) & 7);
    public int SlotSize => (Version < 2 ? 10 : 6) + (Flags & 31);

    protected override void DeserializePayload(ref ReadOnlySpan<byte> buffer)
    {
        Sample = JavadBinary.ReadU2(ref buffer, ByteOrder);
        Scale = JavadBinary.ReadU2(ref buffer, ByteOrder);
        ReferenceTime = JavadBinary.ReadU4(ref buffer, ByteOrder);
        Clock = JavadBinary.ReadI2(ref buffer, ByteOrder);
        Flags = JavadBinary.ReadU2(ref buffer, ByteOrder);
        ValidateSlotSize();
        var count = (Flags >> 8) & 31;
        JavadBinaryPayload.RequireAvailable(buffer.Length, count, HeaderSize + SlotSize);
        Satellites = new JavadLegacyRawMeasurementSatellite[count];
        for (var i = 0; i < count; i++)
        {
            var satellite = new JavadLegacyRawMeasurementSatellite
            {
                ReferenceRange = JavadBinary.ReadU4(ref buffer, ByteOrder),
                Usi = JavadBinary.ReadU1(ref buffer),
                SlotFlags = JavadBinary.ReadU1(ref buffer),
                HeaderExtension = JavadBinary.ReadBytes(ref buffer, HeaderSize - 6),
            };
            var slots = (satellite.SlotFlags & 7) + 1;
            JavadBinaryPayload.RequireAvailable(buffer.Length, slots, SlotSize);
            satellite.Slots = new JavadLegacyRawMeasurementSlot[slots];
            for (var j = 0; j < slots; j++)
            {
                var slot = new JavadLegacyRawMeasurementSlot
                {
                    StatusOrDeltaRange = JavadBinary.ReadI2(ref buffer, ByteOrder),
                    Word1 = JavadBinary.ReadU4(ref buffer, ByteOrder),
                };
                if (Version < 2)
                {
                    slot.Flags = JavadBinary.ReadU2(ref buffer, ByteOrder);
                    slot.Lock = JavadBinary.ReadU2(ref buffer, ByteOrder);
                }
                if (Version == 0)
                {
                    slot.Word2 = JavadBinary.ReadU4(ref buffer, ByteOrder);
                }
                slot.ExtensionData = JavadBinary.ReadBytes(ref buffer, SlotSize - KnownSlotSize);
                satellite.Slots[j] = slot;
            }
            Satellites[i] = satellite;
        }
    }

    protected override void SerializePayload(ref Span<byte> buffer)
    {
        JavadBinary.WriteU2(ref buffer, Sample, ByteOrder);
        JavadBinary.WriteU2(ref buffer, Scale, ByteOrder);
        JavadBinary.WriteU4(ref buffer, ReferenceTime, ByteOrder);
        JavadBinary.WriteI2(ref buffer, Clock, ByteOrder);
        var flags = (ushort)((Flags & ~(31 << 8)) | (Satellites.Length << 8));
        JavadBinary.WriteU2(ref buffer, flags, ByteOrder);
        foreach (var satellite in Satellites)
        {
            JavadBinary.WriteU4(ref buffer, satellite.ReferenceRange, ByteOrder);
            JavadBinary.WriteU1(ref buffer, satellite.Usi);
            JavadBinary.WriteU1(ref buffer, (byte)((satellite.SlotFlags & 0xF8) | (satellite.Slots.Length - 1)));
            JavadBinary.WriteBytes(ref buffer, satellite.HeaderExtension);
            foreach (var slot in satellite.Slots)
            {
                JavadBinary.WriteI2(ref buffer, slot.StatusOrDeltaRange, ByteOrder);
                JavadBinary.WriteU4(ref buffer, slot.Word1, ByteOrder);
                if (Version < 2)
                {
                    JavadBinary.WriteU2(ref buffer, slot.Flags!.Value, ByteOrder);
                    JavadBinary.WriteU2(ref buffer, slot.Lock!.Value, ByteOrder);
                }
                if (Version == 0)
                {
                    JavadBinary.WriteU4(ref buffer, slot.Word2!.Value, ByteOrder);
                }
                JavadBinary.WriteBytes(ref buffer, slot.ExtensionData);
            }
        }
    }

    protected override int GetPayloadByteSize()
    {
        ValidateSlotSize();
        if (Satellites.Length > 31)
        {
            throw new ArgumentException("An integrated GREIS message permits at most 31 satellite records.");
        }
        var size = 12;
        foreach (var satellite in Satellites)
        {
            if (satellite.Slots.Length is < 1 or > 8)
            {
                throw new ArgumentException("An integrated GREIS satellite requires one to eight slots.");
            }
            JavadBinaryPayload.RequireLength(satellite.HeaderExtension, HeaderSize - 6, nameof(satellite.HeaderExtension));
            foreach (var slot in satellite.Slots)
            {
                if (slot.Flags.HasValue != (Version < 2) || slot.Lock.HasValue != (Version < 2)
                    || slot.Word2.HasValue != (Version == 0))
                {
                    throw new InvalidOperationException("Integrated GREIS slot fields must match the message version.");
                }
                JavadBinaryPayload.RequireLength(slot.ExtensionData, SlotSize - KnownSlotSize, nameof(slot.ExtensionData));
            }
            size = checked(size + HeaderSize + SlotSize * satellite.Slots.Length);
        }
        return size;
    }

    private int KnownSlotSize => Version == 0 ? 14 : Version == 1 ? 10 : 6;

    private void ValidateSlotSize()
    {
        if (SlotSize < KnownSlotSize)
        {
            throw new FormatException("Integrated GREIS slot size is shorter than its version requires.");
        }
    }
}

public partial class JavadLegacyClockOffsetsMessage
{
    protected override void DeserializePayload(ref ReadOnlySpan<byte> buffer)
    {
        Sample = JavadBinary.ReadU2(ref buffer, ByteOrder);
        Reserved = JavadBinary.ReadU2(ref buffer, ByteOrder);
        RecordSize = JavadBinary.ReadU1(ref buffer);
        if (RecordSize < 8)
        {
            throw new FormatException("A GREIS clock-offset record must contain at least eight bytes.");
        }
        var count = JavadBinaryPayload.GetRemainingCount(buffer.Length, 0, RecordSize);
        Offsets = new JavadLegacyClockOffset[count];
        for (var i = 0; i < count; i++)
        {
            Offsets[i] = new JavadLegacyClockOffset
            {
                Word1 = JavadBinary.ReadU4(ref buffer, ByteOrder),
                Word2 = JavadBinary.ReadU4(ref buffer, ByteOrder),
                ExtensionData = JavadBinary.ReadBytes(ref buffer, RecordSize - 8),
            };
        }
    }

    protected override void SerializePayload(ref Span<byte> buffer)
    {
        JavadBinary.WriteU2(ref buffer, Sample, ByteOrder);
        JavadBinary.WriteU2(ref buffer, Reserved, ByteOrder);
        JavadBinary.WriteU1(ref buffer, RecordSize);
        foreach (var offset in Offsets)
        {
            JavadBinary.WriteU4(ref buffer, offset.Word1, ByteOrder);
            JavadBinary.WriteU4(ref buffer, offset.Word2, ByteOrder);
            JavadBinary.WriteBytes(ref buffer, offset.ExtensionData);
        }
    }

    protected override int GetPayloadByteSize()
    {
        if (RecordSize < 8)
        {
            throw new ArgumentOutOfRangeException(nameof(RecordSize));
        }
        foreach (var offset in Offsets)
        {
            JavadBinaryPayload.RequireLength(offset.ExtensionData, RecordSize - 8, nameof(offset.ExtensionData));
        }
        return checked(5 + Offsets.Length * RecordSize);
    }
}

public abstract partial class JavadLegacyGpsNavigationPayload
{
    protected override void DeserializePayload(ref ReadOnlySpan<byte> buffer)
    {
        RecordSize = JavadBinary.ReadU1(ref buffer);
        if (RecordSize < 42)
        {
            throw new FormatException("A legacy GREIS GPS navigation record requires at least 42 bytes.");
        }
        var count = JavadBinaryPayload.GetRemainingCount(buffer.Length, 0, RecordSize);
        Satellites = new JavadLegacyGpsNavigationSatellite[count];
        for (var i = 0; i < count; i++)
        {
            var satellite = new JavadLegacyGpsNavigationSatellite
            {
                Prn = JavadBinary.ReadI1(ref buffer),
                Counter = JavadBinary.ReadU1(ref buffer),
            };
            for (var j = 0; j < 10; j++)
            {
                satellite.Data[j] = JavadBinary.ReadU4(ref buffer, ByteOrder);
            }
            satellite.ExtensionData = JavadBinary.ReadBytes(ref buffer, RecordSize - 42);
            Satellites[i] = satellite;
        }
    }

    protected override void SerializePayload(ref Span<byte> buffer)
    {
        JavadBinary.WriteU1(ref buffer, RecordSize);
        foreach (var satellite in Satellites)
        {
            JavadBinary.WriteI1(ref buffer, satellite.Prn);
            JavadBinary.WriteU1(ref buffer, satellite.Counter);
            foreach (var word in satellite.Data)
            {
                JavadBinary.WriteU4(ref buffer, word, ByteOrder);
            }
            JavadBinary.WriteBytes(ref buffer, satellite.ExtensionData);
        }
    }

    protected override int GetPayloadByteSize()
    {
        if (RecordSize < 42)
        {
            throw new ArgumentOutOfRangeException(nameof(RecordSize));
        }
        foreach (var satellite in Satellites)
        {
            JavadBinaryPayload.RequireLength(satellite.Data, 10, nameof(satellite.Data));
            JavadBinaryPayload.RequireLength(satellite.ExtensionData, RecordSize - 42, nameof(satellite.ExtensionData));
        }
        return checked(1 + RecordSize * Satellites.Length);
    }
}

public partial class JavadLegacyGlonassNavigationMessage
{
    protected override void DeserializePayload(ref ReadOnlySpan<byte> buffer)
    {
        RecordSize = JavadBinary.ReadU1(ref buffer);
        if (RecordSize < 18)
        {
            throw new FormatException("A legacy GREIS GLONASS navigation record requires at least 18 bytes.");
        }
        var count = JavadBinaryPayload.GetRemainingCount(buffer.Length, 0, RecordSize);
        Satellites = new JavadLegacyGlonassNavigationSatellite[count];
        for (var i = 0; i < count; i++)
        {
            var satellite = new JavadLegacyGlonassNavigationSatellite
            {
                FrequencyChannelPlusOne = JavadBinary.ReadI1(ref buffer),
                Counter = JavadBinary.ReadU1(ref buffer),
            };
            for (var j = 0; j < 4; j++)
            {
                satellite.Data[j] = JavadBinary.ReadU4(ref buffer, ByteOrder);
            }
            satellite.ExtensionData = JavadBinary.ReadBytes(ref buffer, RecordSize - 18);
            Satellites[i] = satellite;
        }
    }

    protected override void SerializePayload(ref Span<byte> buffer)
    {
        JavadBinary.WriteU1(ref buffer, RecordSize);
        foreach (var satellite in Satellites)
        {
            JavadBinary.WriteI1(ref buffer, satellite.FrequencyChannelPlusOne);
            JavadBinary.WriteU1(ref buffer, satellite.Counter);
            foreach (var word in satellite.Data)
            {
                JavadBinary.WriteU4(ref buffer, word, ByteOrder);
            }
            JavadBinary.WriteBytes(ref buffer, satellite.ExtensionData);
        }
    }

    protected override int GetPayloadByteSize()
    {
        if (RecordSize < 18)
        {
            throw new ArgumentOutOfRangeException(nameof(RecordSize));
        }
        foreach (var satellite in Satellites)
        {
            JavadBinaryPayload.RequireLength(satellite.Data, 4, nameof(satellite.Data));
            JavadBinaryPayload.RequireLength(satellite.ExtensionData, RecordSize - 18, nameof(satellite.ExtensionData));
        }
        return checked(1 + RecordSize * Satellites.Length);
    }
}
