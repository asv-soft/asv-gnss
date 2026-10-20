using System;
using System.Buffers.Binary;
using Xunit;

namespace Asv.Gnss;

internal static class UbxTestHelper
{
    public static byte[] CreateFrame(byte messageClass, byte messageId, ReadOnlySpan<byte> payload)
    {
        var frame = new byte[payload.Length + 8];
        frame[0] = 0xB5;
        frame[1] = 0x62;
        frame[2] = messageClass;
        frame[3] = messageId;
        BinaryPrimitives.WriteUInt16LittleEndian(frame.AsSpan(4), checked((ushort)payload.Length));
        payload.CopyTo(frame.AsSpan(6));

        byte checksumA = 0;
        byte checksumB = 0;
        foreach (var value in frame.AsSpan(2, payload.Length + 4))
        {
            checksumA = unchecked((byte)(checksumA + value));
            checksumB = unchecked((byte)(checksumB + checksumA));
        }

        frame[^2] = checksumA;
        frame[^1] = checksumB;
        return frame;
    }

    public static byte[] Serialize(UbxMessageBase message)
    {
        var frame = new byte[message.GetByteSize()];
        var write = frame.AsSpan();
        message.Serialize(ref write);
        Assert.Empty(write.ToArray());
        return frame;
    }

    public static T Deserialize<T>(ReadOnlySpan<byte> frame) where T : UbxMessageBase, new()
    {
        var message = new T();
        Deserialize(message, frame);
        return message;
    }

    public static void Deserialize(UbxMessageBase message, ReadOnlySpan<byte> frame)
    {
        message.Deserialize(ref frame);
        Assert.Empty(frame.ToArray());
    }
}
