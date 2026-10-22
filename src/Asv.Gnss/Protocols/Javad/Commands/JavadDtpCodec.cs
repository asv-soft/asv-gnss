using System;
using System.Buffers.Binary;
using System.Text;

namespace Asv.Gnss;

public enum JavadDtpBlockType : byte
{
    Ordinal = 0x02,
    Last = 0x04,
    Abort = 0x23,
}

/// <summary>A DTP block from GREIS Appendix A.2. Data excludes padding and the error-string terminator.</summary>
public sealed record JavadDtpBlock(
    JavadDtpBlockType Type,
    ushort Number,
    byte[] Data,
    string? Error = null
)
{
    public static JavadDtpBlock Ordinal(ushort number, byte[] data) =>
        new(JavadDtpBlockType.Ordinal, number, data);

    public static JavadDtpBlock Last(byte[] data) => new(JavadDtpBlockType.Last, 0, data);

    public static JavadDtpBlock Failed(string error) => new(JavadDtpBlockType.Last, 0, [], error);

    public static JavadDtpBlock Abort { get; } = new(JavadDtpBlockType.Abort, 0, []);
}

/// <summary>DTP uses negotiated block sizes, little-endian integers and a cumulative CRC independent of MF.</summary>
public static class JavadDtpCodec
{
    public const byte Acknowledge = 0x06;
    public const byte NegativeAcknowledge = 0x15;
    public const byte EndOfBlock = 0x03;
    public const int MaximumBlockSize = 16384;

    public static bool IsAbortRequest(byte request) => request is >= 0x21 and <= 0x2F;

    public static byte[] Encode(
        JavadDtpBlock block,
        int blockSize,
        ushort previousChecksum,
        out ushort checksum
    )
    {
        ArgumentNullException.ThrowIfNull(block);
        ValidateBlockSize(blockSize);
        ArgumentNullException.ThrowIfNull(block.Data);
        checksum = previousChecksum;
        if (block.Type == JavadDtpBlockType.Abort)
            return [(byte)JavadDtpBlockType.Abort];
        if (block.Type is not (JavadDtpBlockType.Ordinal or JavadDtpBlockType.Last))
            throw new ArgumentOutOfRangeException(nameof(block));
        if (
            block.Data.Length > blockSize
            || (
                block.Type == JavadDtpBlockType.Ordinal
                && (block.Data.Length != blockSize || block.Error is not null)
            )
        )
            throw new ArgumentException(
                "An ordinal DTP block must contain exactly the negotiated number of bytes.",
                nameof(block)
            );
        var bytes = new byte[blockSize + 6];
        bytes[0] = (byte)block.Type;
        if (block.Type == JavadDtpBlockType.Ordinal)
            BinaryPrimitives.WriteUInt16LittleEndian(bytes.AsSpan(1), block.Number);
        else if (block.Error is not null)
        {
            foreach (var character in block.Error)
                if (character is '\0' or > '\x7F')
                    throw new ArgumentException(
                        "DTP error descriptions must be ASCII and cannot contain NUL.",
                        nameof(block)
                    );
            if (block.Error.Length >= blockSize)
                throw new ArgumentException(
                    "The DTP error description and terminator must fit in one block.",
                    nameof(block)
                );
            if (block.Data.Length != 0)
                throw new ArgumentException(
                    "An error block cannot also contain file bytes.",
                    nameof(block)
                );
            BinaryPrimitives.WriteInt16LittleEndian(bytes.AsSpan(1), -1);
            Encoding.ASCII.GetBytes(block.Error, bytes.AsSpan(3));
        }
        else
            BinaryPrimitives.WriteInt16LittleEndian(
                bytes.AsSpan(1),
                checked((short)block.Data.Length)
            );
        if (block.Error is null)
            block.Data.CopyTo(bytes.AsSpan(3));
        checksum = ContinueChecksum(previousChecksum, bytes.AsSpan(0, blockSize + 3));
        BinaryPrimitives.WriteUInt16LittleEndian(bytes.AsSpan(blockSize + 3), checksum);
        bytes[^1] = EndOfBlock;
        return bytes;
    }

    /// <summary>Consumes exactly one complete block; an invalid frame leaves the input span unchanged.</summary>
    public static JavadDtpBlock Decode(
        ref ReadOnlySpan<byte> buffer,
        int blockSize,
        ushort previousChecksum,
        out ushort checksum
    )
    {
        ValidateBlockSize(blockSize);
        checksum = previousChecksum;
        if (buffer.IsEmpty)
            throw new FormatException("A DTP block is missing its type byte.");
        if (buffer[0] == (byte)JavadDtpBlockType.Abort)
        {
            buffer = buffer[1..];
            return JavadDtpBlock.Abort;
        }
        if (buffer[0] is not ((byte)JavadDtpBlockType.Ordinal or (byte)JavadDtpBlockType.Last))
            throw new FormatException("Invalid DTP block type.");
        if (buffer.Length < blockSize + 6)
            throw new FormatException("Truncated DTP block.");
        var frame = buffer[..(blockSize + 6)];
        if (frame[^1] != EndOfBlock)
            throw new FormatException("Invalid DTP end-of-block marker.");
        var computed = ContinueChecksum(previousChecksum, frame[..(blockSize + 3)]);
        if (computed != BinaryPrimitives.ReadUInt16LittleEndian(frame[(blockSize + 3)..]))
            throw new FormatException("DTP cumulative CRC mismatch.");
        JavadDtpBlock result;
        if (frame[0] == (byte)JavadDtpBlockType.Ordinal)
            result = JavadDtpBlock.Ordinal(
                BinaryPrimitives.ReadUInt16LittleEndian(frame[1..]),
                frame.Slice(3, blockSize).ToArray()
            );
        else
        {
            var count = BinaryPrimitives.ReadInt16LittleEndian(frame[1..]);
            if (count < -1 || count > blockSize)
                throw new FormatException("Invalid DTP last-block count.");
            var data = frame.Slice(3, blockSize);
            if (count == -1)
            {
                var terminator = data.IndexOf((byte)0);
                if (terminator < 0)
                    throw new FormatException("A DTP error string must be zero-terminated.");
                foreach (var value in data[..terminator])
                    if (value > 127)
                        throw new FormatException("DTP errors use ASCII text.");
                result = JavadDtpBlock.Failed(Encoding.ASCII.GetString(data[..terminator]));
            }
            else
            {
                foreach (var value in data[count..])
                    if (value != 0)
                        throw new FormatException(
                            "The unused part of the last DTP block must be zero-filled."
                        );
                result = JavadDtpBlock.Last(data[..count].ToArray());
            }
        }
        checksum = computed;
        buffer = buffer[(blockSize + 6)..];
        return result;
    }

    public static ushort ContinueChecksum(ushort previousChecksum, ReadOnlySpan<byte> data) =>
        JavadChecksum.Compute16(data, previousChecksum);

    internal static void ValidateBlockSize(int blockSize)
    {
        if (blockSize is < 1 or > MaximumBlockSize)
            throw new ArgumentOutOfRangeException(nameof(blockSize));
    }
}

public enum JavadDtpRequestAction
{
    Ignore,
    SendCurrentBlock,
    PrepareNextBlock,
    Finished,
    Aborted,
}

/// <summary>Request-driven upload state. The caller supplies one block at a time and owns transport I/O and deadlines.</summary>
public sealed class JavadDtpTransmitter
{
    private ushort _pendingChecksum;

    public JavadDtpTransmitter(int blockSize = 512)
    {
        JavadDtpCodec.ValidateBlockSize(blockSize);
        BlockSize = blockSize;
    }

    public int BlockSize { get; }
    public ushort Checksum { get; private set; }
    public ushort NextBlockNumber { get; private set; }
    public ReadOnlyMemory<byte> CurrentBlock { get; private set; }
    public bool IsLastBlock { get; private set; }
    public bool IsAborted { get; private set; }
    public bool IsComplete { get; private set; }

    /// <summary>Prepares bytes once. NACK retransmits the same bytes without advancing the checksum or block number.</summary>
    public ReadOnlyMemory<byte> Prepare(ReadOnlySpan<byte> data, bool isLast = false)
    {
        if (IsAborted || IsComplete || !CurrentBlock.IsEmpty || IsLastBlock)
            throw new InvalidOperationException(
                "The transfer cannot accept another block before its current block is acknowledged."
            );
        CurrentBlock = JavadDtpCodec.Encode(
            isLast
                ? JavadDtpBlock.Last(data.ToArray())
                : JavadDtpBlock.Ordinal(NextBlockNumber, data.ToArray()),
            BlockSize,
            Checksum,
            out _pendingChecksum
        );
        IsLastBlock = isLast;
        return CurrentBlock;
    }

    public JavadDtpRequestAction ProcessRequest(byte request)
    {
        if (IsComplete)
            return JavadDtpRequestAction.Ignore;
        if (JavadDtpCodec.IsAbortRequest(request))
        {
            IsAborted = true;
            CurrentBlock = default;
            return JavadDtpRequestAction.Aborted;
        }
        if (IsAborted)
            return JavadDtpRequestAction.Ignore;
        if (request == JavadDtpCodec.NegativeAcknowledge)
            return !CurrentBlock.IsEmpty ? JavadDtpRequestAction.SendCurrentBlock
                : IsLastBlock ? JavadDtpRequestAction.Finished
                : JavadDtpRequestAction.PrepareNextBlock;
        if (request != JavadDtpCodec.Acknowledge || CurrentBlock.IsEmpty)
            return JavadDtpRequestAction.Ignore;
        Checksum = _pendingChecksum;
        CurrentBlock = default;
        if (IsLastBlock)
        {
            IsComplete = true;
            return JavadDtpRequestAction.Finished;
        }
        NextBlockNumber = unchecked((ushort)(NextBlockNumber + 1));
        return JavadDtpRequestAction.PrepareNextBlock;
    }

    /// <summary>Call after transport completion. Sending LAST ends the transfer without waiting for a final ACK.</summary>
    public void MarkCurrentBlockSent()
    {
        if (CurrentBlock.IsEmpty)
            throw new InvalidOperationException("No DTP block is prepared.");
        if (!IsLastBlock)
            return;
        Checksum = _pendingChecksum;
        CurrentBlock = default;
        IsComplete = true;
    }
}

/// <summary>One completed block or recovery request. A null Request means no ACK/NACK should be sent.</summary>
public sealed record JavadDtpReceiveResult(
    JavadDtpBlock? Block,
    byte? Request,
    bool IsComplete,
    bool IsDuplicate = false
);

/// <summary>Incremental DTP download state, including cumulative CRC, retransmissions and block-number wrap.</summary>
public sealed class JavadDtpReceiver
{
    private readonly byte[] _buffer;
    private int _count;
    private ushort[]? _checksumSeeds;
    private ulong _successfulBlocks;

    public JavadDtpReceiver(int blockSize = 512)
    {
        JavadDtpCodec.ValidateBlockSize(blockSize);
        BlockSize = blockSize;
        _buffer = new byte[blockSize + 6];
    }

    public int BlockSize { get; }
    public ushort Checksum { get; private set; }
    public ushort ExpectedBlockNumber { get; private set; }
    public bool IsComplete { get; private set; }
    public byte InitialRequest => JavadDtpCodec.NegativeAcknowledge;

    public JavadDtpReceiveResult? Push(byte value)
    {
        if (IsComplete)
            throw new InvalidOperationException("The DTP transfer has already ended.");
        if (_count == 0)
        {
            if (value == (byte)JavadDtpBlockType.Abort)
            {
                IsComplete = true;
                return new(JavadDtpBlock.Abort, null, true);
            }
            if (value is not ((byte)JavadDtpBlockType.Ordinal or (byte)JavadDtpBlockType.Last))
                return null;
        }
        _buffer[_count++] = value;
        if (_count != _buffer.Length)
            return null;
        _count = 0;
        ReadOnlySpan<byte> frame = _buffer;
        var number = BinaryPrimitives.ReadUInt16LittleEndian(frame[1..]);
        var ordinal = frame[0] == (byte)JavadDtpBlockType.Ordinal;
        var historical = ordinal && _checksumSeeds is not null && _successfulBlocks > number;
        if (historical && number != ExpectedBlockNumber)
        {
            var historicalFrame = frame;
            try
            {
                JavadDtpCodec.Decode(
                    ref historicalFrame,
                    BlockSize,
                    _checksumSeeds![number],
                    out _
                );
                return new(null, null, false, true);
            }
            catch (FormatException) { }
        }
        JavadDtpBlock block;
        ushort checksum;
        try
        {
            block = JavadDtpCodec.Decode(ref frame, BlockSize, Checksum, out checksum);
        }
        catch (FormatException)
        {
            return new(null, JavadDtpCodec.NegativeAcknowledge, false);
        }
        if (block.Type == JavadDtpBlockType.Ordinal && number != ExpectedBlockNumber)
        {
            IsComplete = true;
            return new(null, (byte)'#', true);
        }
        if (ordinal)
        {
            // At most one CRC seed per 16-bit sequence number, independent of file size.
            if (_checksumSeeds is null || number >= _checksumSeeds.Length)
            {
                var size = _checksumSeeds?.Length ?? 16;
                while (size <= number)
                    size *= 2;
                Array.Resize(ref _checksumSeeds, size);
            }
            _checksumSeeds[number] = Checksum;
            _successfulBlocks++;
        }
        Checksum = checksum;
        if (block.Type == JavadDtpBlockType.Last)
        {
            IsComplete = true;
            return new(block, null, true);
        }
        ExpectedBlockNumber = unchecked((ushort)(ExpectedBlockNumber + 1));
        return new(block, JavadDtpCodec.Acknowledge, false);
    }

    /// <summary>Discards a partial block after the caller's deadline and requests the unchanged current block.</summary>
    public byte RequestRetry()
    {
        if (IsComplete)
            throw new InvalidOperationException("The DTP transfer has already ended.");
        _count = 0;
        return JavadDtpCodec.NegativeAcknowledge;
    }
}
