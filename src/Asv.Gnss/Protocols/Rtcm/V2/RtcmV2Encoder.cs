using System;
using System.Numerics;
using Asv.IO;

namespace Asv.Gnss;

/// <summary>
/// Encodes normalized RTCM v2 data words into a continuous 6-of-8 transport stream.
/// Create one instance per outgoing stream and serialize access in transmission order.
/// </summary>
public sealed class RtcmV2Encoder : IProtocolEncoder
{
    private uint _previousParity;

    /// <inheritdoc />
    public int GetMaxByteSize(IProtocolMessage message)
    {
        ArgumentNullException.ThrowIfNull(message);
        if (message.Protocol.Id != RtcmV2Protocol.Info.Id)
        {
            throw new ArgumentException("An RTCM v2 message is required.", nameof(message));
        }

        var size = message.GetByteSize();
        if (size is < 6 or > RtcmV2Protocol.MaxMessageSize || size % 3 != 0)
        {
            throw new ArgumentException("Normalized RTCM v2 messages must contain 2 to 33 complete data words.", nameof(message));
        }

        return size / 3 * 5;
    }

    /// <inheritdoc />
    public int Encode(IProtocolMessage message, Span<byte> destination)
    {
        var wireSize = GetMaxByteSize(message);
        if (destination.Length < wireSize)
        {
            throw new ArgumentException("The destination is too small for the encoded RTCM v2 message.", nameof(destination));
        }

        var size = wireSize / 5 * 3;
        Span<byte> storage = stackalloc byte[RtcmV2Protocol.MaxMessageSize];
        var packet = storage[..size];
        var remaining = packet;
        message.Serialize(ref remaining);
        if (!remaining.IsEmpty || packet[0] != RtcmV2Protocol.SyncByte || (packet[5] >> 3) * 3 + 6 != size)
        {
            throw new ArgumentException("The serializer did not produce a complete normalized RTCM v2 message.", nameof(message));
        }

        var zCount = (packet[3] << 5) | (packet[4] >> 3);
        if (zCount >= 6000)
        {
            throw new ArgumentException("The RTCM v2 modified Z-count must be less than one hour.", nameof(message));
        }

        ReadOnlySpan<uint> masks = [0xBB1F3480, 0x5D8F9A40, 0xAEC7CD00, 0x5763E680, 0x6BB1F340, 0x8B7A89C0];
        var previous = _previousParity;
        var output = 0;
        for (var offset = 0; offset < size; offset += 3)
        {
            var data = ((uint)packet[offset] << 16) | ((uint)packet[offset + 1] << 8) | packet[offset + 2];
            var word = (previous << 30) | (data << 6);
            uint parity = 0;
            foreach (var mask in masks)
            {
                parity = (parity << 1) | (uint)(BitOperations.PopCount(word & mask) & 1);
            }

            word |= parity;
            if ((previous & 1) != 0)
            {
                word ^= 0x3FFFFFC0;
            }

            for (var shift = 24; shift >= 0; shift -= 6)
            {
                var chunk = (word >> shift) & 0x3F;
                uint reversed = 0;
                for (var bit = 0; bit < 6; bit++)
                {
                    reversed = (reversed << 1) | (chunk & 1);
                    chunk >>= 1;
                }

                destination[output++] = (byte)(0x40 | reversed);
            }

            previous = word & 3;
        }

        _previousParity = previous;
        return output;
    }
}
