using System;
using Asv.IO;
using Microsoft.Extensions.Logging;
using ZLogger;

namespace Asv.Gnss;

public class UbxMessageParser : ProtocolParser<UbxMessageBase, ushort>
{
    /// <summary>
    /// Represents the maximum size of a packet in bytes. </summary> <remarks>
    /// This constant represents the maximum size of a packet in bytes. It is defined as 1024 times 8,
    /// resulting in a maximum size of 8192 bytes. </remarks>
    /// /
    public const int MaxPacketSize = 1024 * 8;

    /// <summary>
    /// Represents a fixed-size byte buffer.
    /// </summary>
    private readonly byte[] _buffer = new byte[MaxPacketSize];

    /// The current index position in the buffer.
    /// /
    private int _bufferIndex = 0;

    /// <summary>
    /// Represents the state of the system.
    /// </summary>
    private State _state = State.Sync1;

    /// <summary>
    /// Represents the length in bytes of the payload.
    /// </summary>
    private int _payloadLength = 0;

    /// <summary>
    /// Variable that stores the total number of payload bytes read.
    /// </summary>
    private int _payloadReadBytes = 0;


    /// <summary>
    /// Represents the states of a synchronization process.
    /// </summary>
    private enum State
    {
        /// <summary>
        /// Represents the state Sync1 of the communication protocol.
        /// </summary>
        /// <remarks>
        /// This state is responsible for synchronizing the sender and receiver during data transmission.
        /// </remarks>
        Sync1,

        /// <summary>
        /// Represents the synchronization state Sync2.
        /// </summary>
        /// <remarks>
        /// This state indicates that the synchronization process is in the second stage.
        /// </remarks>
        Sync2,

        /// <summary>
        /// Represents the state of the communication process.
        /// </summary>
        IdAndLength,

        /// <summary>
        /// Represents the Payload state of the State enum.
        /// </summary>
        /// <remarks>
        /// This state indicates that the payload of a data packet is being processed.
        /// </remarks>
        Payload,

        /// <summary>
        /// Represents the Crc1 state of the State enum.
        /// </summary>
        Crc1,

        /// <summary>
        /// Represents the CRC2 state of the system.
        /// </summary>
        Crc2
    }
    
    private readonly ILogger<UbxMessageParser> _logger;

    public UbxMessageParser(IProtocolMessageFactory<UbxMessageBase, ushort> messageFactory, IProtocolContext context,
        IStatisticHandler? statisticHandler) : base(messageFactory, context, statisticHandler)
    {
        _logger = context.LoggerFactory.CreateLogger<UbxMessageParser>();
    }


    public override bool Push(byte data)
    {
        switch (_state)
        {
            case State.Sync1:
                if (data != UbxProtocol.SyncByte1) break;
                _buffer[_bufferIndex++] = UbxProtocol.SyncByte1;
                _state = State.Sync2;
                break;
            case State.Sync2:
                if (data != UbxProtocol.SyncByte2)
                {
                    Reset();
                }
                else
                {
                    _state = State.IdAndLength;
                    _buffer[_bufferIndex++] = UbxProtocol.SyncByte2;
                }
                break;
            case State.IdAndLength:
                _buffer[_bufferIndex++] = data;
                if (_bufferIndex == 6)
                {
                    _payloadLength = UbxProtocol.ReadMessageLength(_buffer);
                    _state = State.Payload;
                    _payloadReadBytes = 0;
                    // reset on oversize packet
                    if (_payloadLength > _buffer.Length)
                    {
                        // buffer oversize
                        Reset();
                    }
                }
                break;
            case State.Payload:
                // read payload
                if (_payloadReadBytes < _payloadLength)
                {
                    _buffer[_payloadReadBytes + 6] = data;
                    _payloadReadBytes++;

                    if (_payloadReadBytes == _payloadLength)
                        _state = State.Crc1;
                }
                break;
            case State.Crc1:
                _buffer[_payloadLength + 6] = data;
                _state = State.Crc2;
                break;
            case State.Crc2:
                _buffer[_payloadLength + 6 + 1] = data;
                var msgId = UbxProtocol.ReadMessageId(_buffer);
                var spanMsg = new ReadOnlySpan<byte>(_buffer, 0, _payloadReadBytes + 8);
                try
                {
                    InternalParsePacket(msgId, ref spanMsg);
                    Reset();
                    return true;
                }
                catch (ProtocolParserException ex)
                {
                    _logger.ZLogTrace($"{ex.Message}[UbxMessage{msgId}]");
                    InternalOnError(ex);
                    Reset();
                    return false;
                }
                catch (Exception ex)
                {
                    _logger.ZLogTrace($"{ex.Message}[UbxMessage{msgId}]");
                    InternalOnError(new ProtocolParserException(Info, "Parser ", ex));
                    Reset();
                    return false;
                }
            default:
                throw new ArgumentOutOfRangeException();
        }
        return false;
    }

    public override void Reset()
    {
        _bufferIndex = 0;
        _state = State.Sync1;
    }

    public override ProtocolInfo Info => UbxProtocol.Info;
}