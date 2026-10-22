using System;
using System.Collections.Generic;
using System.Linq;
using Asv.IO;

namespace Asv.Gnss;

public enum SlotIdEnum : byte
{
    A = 0,
    B = 1,
    C = 2,
    D = 3,
    E = 4,
    F = 5,
    G = 6,
    H = 7
}

public partial class GbasMessage : SpanBitCompactSerializableBase
{

    public const int MaxAppDataSize = 222;
    public const ulong Sync = 0x47DF8C760790; // 48 bit

    /// <summary>
    /// 3 bit, Station slot identifier
    /// </summary>
    public SlotIdEnum SsId { get; set; }

    /// <summary>
    /// TransmissionLength - 25 bit - 48 bit
    /// </summary>
    public List<GbasMessageBlockBase> ApplicationData { get; } = new();

    public static int GetFrameBitSize(ReadOnlySpan<byte> buffer)
    {
        var position = 0;
        ReadTraining(buffer, ref position, out _, out var length, out _);
        return RoundFrameBits(73 + (int)length);
    }

    public static byte[] DecodeApplicationData(ReadOnlySpan<byte> buffer, out SlotIdEnum slot)
    {
        var position = 0;
        return DecodeApplicationData(buffer, ref position, out slot);
    }

    private static byte[] DecodeApplicationData(ReadOnlySpan<byte> buffer, ref int position, out SlotIdEnum slot)
    {
        var start = position;
        ReadTraining(buffer, ref position, out slot, out var length, out var register);
        var frameBits = RoundFrameBits(73 + (int)length);
        EnsureBits(buffer.Length, start, frameBits);
        var appLength = ((int)length - 48) / 8;
        var rsBuffer = new byte[249];
        for (var i = 0; i < appLength; i++)
            rsBuffer[i] = ScrambleByte((byte)SpanBitHelper.GetBitU(buffer, ref position, 8), ref register).Reverse();
        var fec = new byte[6];
        for (var i = 5; i >= 0; i--)
            fec[i] = ScrambleByte((byte)SpanBitHelper.GetBitU(buffer, ref position, 8), ref register);
        var decoded = Decoder.DecodeEx(rsBuffer, fec)
            ?? throw new FormatException("The GBAS application Reed-Solomon code cannot be corrected.");
        // Shortened application data must retain the zero symbols used during encoding.
        for (var i = appLength; i < decoded.Length; i++)
            if (decoded[i] != 0) throw new FormatException("Invalid GBAS application padding.");
        var result = new byte[appLength];
        for (var i = 0; i < result.Length; i++) result[i] = decoded[i].Reverse();
        var padding = start + frameBits - position;
        if (padding > 0 && SpanBitHelper.GetBitU(buffer, ref position, padding) != 0)
            throw new FormatException("Invalid GBAS burst padding.");
        return result;
    }

    private static void ReadTraining(ReadOnlySpan<byte> buffer, ref int position, out SlotIdEnum slot,
        out uint length, out ushort register)
    {
        EnsureBits(buffer.Length, position, 73);
        var sync = SpanBitHelper.GetBitUReverse(buffer, ref position, 24) |
                   ((ulong)SpanBitHelper.GetBitUReverse(buffer, ref position, 24) << 24);
        if (sync != Sync) throw new FormatException("Invalid GBAS synchronization sequence.");
        register = ScrambleRegister;
        var training = new byte[4];
        for (var i = 0; i < 3; i++)
            training[i] = ScrambleByte((byte)SpanBitHelper.GetBitU(buffer, ref position, 8), ref register);
        var finalBit = (byte)(SpanBitHelper.GetBitU(buffer, ref position, 1) << 7);
        training[3] = ScrambleBits(finalBit, 0, 1, ref register);
        if (!TrainingSequenceErrorCorrection(training, 0, out var slotId, out length, out _, out _))
            throw new FormatException("Invalid GBAS training sequence.");
        if (length < 48 || length > MaxAppDataSize * 8 + 48 || (length - 48) % 8 != 0)
            throw new FormatException("Invalid GBAS transmission length.");
        slot = (SlotIdEnum)slotId;
    }

    private static int RoundFrameBits(int bits) => bits + (3 - bits % 3) % 3;

    private static void EnsureBits(int byteLength, int position, int count)
    {
        if (position < 0 || count < 0 || (long)position + count > (long)byteLength * 8)
            throw new FormatException("Truncated GBAS burst.");
    }

    public override void Deserialize(ReadOnlySpan<byte> buffer, ref int bitPosition)
    {
        ApplicationData.Clear();
        var decoded = DecodeApplicationData(buffer, ref bitPosition, out var slot);
        SsId = slot;
        ReadOnlySpan<byte> data = decoded;
        var position = 0;
        while (position < decoded.Length * 8)
        {
            EnsureBits(decoded.Length, position, 48);
            var typePosition = position + 32;
            var type = GetMessageTypeId(ref data, ref typePosition);
            var block = GbasMessageBlockFactory.CreateGbasMessageBlock(type);
            var previous = position;
            block.Deserialize(data, ref position);
            if (position <= previous || position > decoded.Length * 8)
                throw new FormatException("Invalid GBAS application block length.");
            ApplicationData.Add(block);
        }
    }

    public override void Serialize(Span<byte> buffer, ref int bitPosition)
    {
        var originBitPosition = bitPosition;
        var appSize = ApplicationData.Sum(block => block.GetBitSize());
        if ((byte)SsId > 7 || appSize < 0 || appSize % 8 != 0 || appSize > MaxAppDataSize * 8)
            throw new ArgumentException("Invalid GBAS slot or application data size.");
        EnsureBits(buffer.Length, bitPosition, GetBitSize());

        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 24, (uint)(Sync & 0xFFFFFF));
        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 24, (uint)(Sync >> 24));
        // 48 bit

        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 3, (uint)SsId);
        // 3 bit

        var appDataBitSize = (uint)ApplicationData.Sum(_ => _.GetBitSize());
        var transmissionLength = appDataBitSize /*App data */ + 48 /*ApplicationFec: Reed-Solomon*/;
        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 17, transmissionLength);

        // 5 bit, [P1, ..., P5] = [SSID1, ..., SSID3, TL1, ..., TL17] * HT
        var trainingSeqFec = GetTrainingSequenceFec((byte)SsId, transmissionLength);
        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 5, trainingSeqFec);

        var startAppBit = bitPosition;
        foreach (var block in ApplicationData)
        {
            block.Serialize(buffer, ref bitPosition);
        }

        var appBitSize = (bitPosition - startAppBit);
        var appByteSize = appBitSize % 8 == 0 ? appBitSize / 8 : (appBitSize / 8 + 1);

        var rsBuffer = new byte[249]; /*max app data size*/
        for (var i = 0; i < appByteSize; i++)
        {
            rsBuffer[i] = (byte)SpanBitHelper.GetBitUReverse(buffer, ref startAppBit, 8);
        }

        var appFec = Encoder.EncodeEx(rsBuffer, 6);
        for (var i = 5; i >= 0; i--)
        {
            SpanBitHelper.SetBitU(buffer, ref bitPosition, 8, appFec[i]);
        }

        // scramble: 3 (SsId) + 17 (transmissionLength) + 5 (trainingSeqFec) + N bits of APP DATA + 48 (ApplicationFec)
        var register = ScrambleRegister;

        var scrambleStartBitPosition = 48 + (int)originBitPosition;
        var scrambleStopBitPosition = transmissionLength + 3 /*(SsId)*/ + 17 /*(transmissionLength)*/ +
                                      5 /*(trainingSeqFec)*/ + originBitPosition + 48 /*Sync*/;
        var scrambleStartBytePosition = scrambleStartBitPosition / 8;
        var scrambleStopBytePosition = scrambleStopBitPosition / 8;

        if (scrambleStartBitPosition % 8 != 0)
        {
            var bitsWithoutScramble = (byte)(scrambleStartBitPosition % 8);
            buffer[scrambleStartBytePosition] = ScrambleBits(buffer[scrambleStartBytePosition], bitsWithoutScramble,
                (byte)(8 - bitsWithoutScramble), ref register);
            scrambleStartBytePosition++;
        }

        for (var i = scrambleStartBytePosition; i < scrambleStopBytePosition; i++)
        {
            buffer[i] = ScrambleByte(buffer[i], ref register);
        }

        if (scrambleStopBitPosition % 8 != 0)
        {
            var bitWithScramble = (byte)(scrambleStopBitPosition % 8);
            buffer[(int)scrambleStopBytePosition] =
                ScrambleBits(buffer[(int)scrambleStopBytePosition], 0, bitWithScramble, ref register);
        }

        var messageBitSize = bitPosition - originBitPosition;
        var fillBits = 0;
        if (messageBitSize % 3 != 0)
        {
            fillBits += 3 - messageBitSize % 3;
        }

        if (fillBits != 0)
        {
            SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, fillBits, 0);
        }
    }

    public void Scramble(Span<byte> buffer, ref int startBitPosition, int stopBitPosition)
    {
        var originBitPosition = startBitPosition;

        // scramble: 3 (SsId) + 17 (transmissionLength) + 5 (trainingSeqFec) + N bits of APP DATA + 48 (ApplicationFec)
        var register = ScrambleRegister;

        var scrambleStartBitPosition = 48 + originBitPosition;
        startBitPosition += 48;
        var scrambleStopBitPosition = stopBitPosition;
        var scrambleStartBytePosition = scrambleStartBitPosition / 8;
        var scrambleStopBytePosition = scrambleStopBitPosition / 8;

        if (scrambleStartBitPosition % 8 != 0)
        {
            var bitsWithoutScramble = (byte)(scrambleStartBitPosition % 8);
            var length = (byte)(8 - bitsWithoutScramble);
            buffer[scrambleStartBytePosition] = ScrambleBits(buffer[scrambleStartBytePosition], bitsWithoutScramble,
                length, ref register);
            startBitPosition += length;
            scrambleStartBytePosition++;
        }

        for (var i = scrambleStartBytePosition; i < scrambleStopBytePosition; i++)
        {
            buffer[i] = ScrambleByte(buffer[i], ref register);
            startBitPosition += 8;
        }

        if (scrambleStopBitPosition % 8 != 0)
        {
            var bitWithScramble = (byte)(scrambleStopBitPosition % 8);
            buffer[(int)scrambleStopBytePosition] =
                ScrambleBits(buffer[(int)scrambleStopBytePosition], 0, bitWithScramble, ref register);
            startBitPosition += bitWithScramble;
        }
    }
    public override int GetBitSize()
    {
        var cnt = 48 /*Sync*/ + 3 /*(SsId)*/ + 17 /*(transmissionLength)*/ + 5 /*(trainingSeqFec)*/ +
                  ApplicationData.Sum(_ => _.GetBitSize()) + 48 /*ApplicationFec*/;
        if (cnt % 3 != 0)
        {
            cnt += 3 - cnt % 3;
        }

        return cnt;
    }

    #region Factory



    public static byte GetMessageTypeId(ref ReadOnlySpan<byte> buffer, ref int bitPosition)
    {
        return (byte)SpanBitHelper.GetBitUReverse(buffer, ref bitPosition, 8);
    }

    #endregion

    #region ReedSolomon


    private static ReedSolomonEncoder Encoder => new(GenericGF.GBAS_CODE_FIELD_256);
    private static readonly ReedSolomonDecoder Decoder = new(GenericGF.GBAS_CODE_FIELD_256);


    #endregion

    #region Scramble

    private const ushort ScrambleRegister = 0x4D4B;

    private static byte CyclicShift(ref ushort register)
    {
        var result = (byte)((byte)(register >> 14) ^ (byte)(register & 0x1));

        register = (ushort)(((register << 0x1) & 0x7FFF) + result);

        return result;
    }

    private static byte ScrambleByte(byte src, ref ushort register)
    {
        byte result = 0;

        for (var i = 0; i < 8; i++)
        {
            var m = CyclicShift(ref register);
            result = (byte)((result << 0x1) | (((src >> (7 - i)) & 0x1) ^ m));
        }

        return result;
    }

    private static byte ScrambleBits(byte src, byte offsetBits, byte length, ref ushort register)
    {
        byte result = 0;

        var mask = (byte)(((0xFF >> offsetBits) & (0xFF << (8 - (length + offsetBits)))) ^ 0xFF);

        for (var i = offsetBits; i < length + offsetBits; i++)
        {
            var m = CyclicShift(ref register);
            result |= (byte)((((src >> (7 - i)) & 0x1) ^ m) << (7 - i));
        }

        return (byte)(result | (src & mask));

    }

    #endregion

    #region Training FEC

    /// <summary>
    /// Checks the first 25 bits and tries to correct errors
    /// </summary>
    /// <param name="buffer">Descrambled data</param>
    /// <param name="offsetBits">Starting bits</param>
    /// <param name="ssId">3 bit, Station slot identifier</param>
    /// <param name="trainingSequenceFec">FEC</param>
    /// <param name="isErrorAny">Were there any error</param>
    /// <returns></returns>
    public static bool TrainingSequenceErrorCorrection(byte[] buffer, int offsetBits, out byte ssId,
        out uint transmissionLength,
        out byte trainingSequenceFec, out bool isErrorAny)
    {
        var trainingSequence = (SpanBitHelper.GetBitU(buffer, ref offsetBits, 3) << 17) +
                               SpanBitHelper.GetBitU(buffer, ref offsetBits, 17);
        var fec = (byte)SpanBitHelper.GetBitU(buffer, ref offsetBits, 5);

        var u = (uint)((fec << 20) + trainingSequence);
        if (CheckU(u))
        {
            ssId = (byte)(((byte)(trainingSequence >> 17)).Reverse() >> 5);
            transmissionLength = (trainingSequence & 0x1FFFF).Reverse() >> 15;
            trainingSequenceFec = (byte)(fec.Reverse() >> 3);
            isErrorAny = false;
            return true;
        }

        var results = new List<uint>();

        uint m = 0x1000000;
        for (uint k = 0; k < 25; k++, m >>= 1)
        {
            var e = u ^ m;
            if (CheckU(e)) results.Add(e);
        }

        foreach (var result in results)
        {
            var newFec = (byte)(result >> 20);
            trainingSequence = result & 0xFFFFF;
            ssId = (byte)(((byte)(trainingSequence >> 17)).Reverse() >> 5);
            transmissionLength = (trainingSequence & 0x1FFFF).Reverse() >> 15;
            trainingSequenceFec = (byte)(newFec.Reverse() >> 3);
            isErrorAny = true;
            return true;
        }

        ssId = 0;
        transmissionLength = 0;
        trainingSequenceFec = 0;
        isErrorAny = false;
        return false;

    }

    /// <summary>
    /// Check U * IH == 0
    /// </summary>
    /// <param name="u">25 bit = FEC(5 bit) + SlotId(3 bit) + Length(17 bit)</param>
    /// <returns></returns>
    private static bool CheckU(uint u)
    {
        uint s = 0;
        for (var i = 0; i < 5; i++)
        {
            uint bitItem = 0;
            var vector = u & IH[i];

            uint mask = 0x1000000;
            for (var j = 0; j < 25; j++, mask >>= 1)
            {
                bitItem ^= (vector & mask) >> (24 - j);
            }

            s = s + (bitItem << (4 - i));
        }

        return s == 0;
    }

    private static readonly uint[] IH = { 0x1000FFF, 0x083F0FF, 0x04C730F, 0x02DB533, 0x0169E55 };
    private static readonly uint[] H = { 0x00FFF, 0x3F0FF, 0xC730F, 0xDB533, 0x69E55 };

    /// <summary>
    /// </summary>
    /// <param name="ssId">Time slot Id (3 bit)</param>
    /// <param name="transmissionLength">Transmission Length (17 bit)</param>
    /// <returns>FEC</returns>
    public static byte GetTrainingSequenceFec(byte ssId, uint transmissionLength)
    {
        ssId = (byte)(ssId.Reverse() >> 5);
        transmissionLength = transmissionLength.Reverse() >> 15;
        var trainingSequence = ((uint)ssId << 17) + transmissionLength;

        byte fec = 0;
        for (var i = 0; i < 5; i++)
        {
            uint mask = 0x80000;
            uint bitItem = 0;
            var vector = trainingSequence & H[i];
            for (var j = 0; j < 20; j++, mask >>= 1)
            {
                bitItem ^= (vector & mask) >> (19 - j);
            }

            fec = (byte)(fec + (bitItem << (4 - i)));
        }

        return (byte)(fec.Reverse() >> 3);
    }

    #endregion





}
