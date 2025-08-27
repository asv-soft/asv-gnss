using System;
using Asv.IO;
using Microsoft.Extensions.Logging;
using R3;
using ZLogger;

namespace Asv.Gnss;

public class RtcmV3MessageParser : ProtocolParser<RtcmV3MessageBase, ushort>
{
    /// <summary>
    /// Private readonly variable representing a buffer of bytes.
    /// </summary>
    /// <remarks>
    /// The buffer has a size of 1030 bytes, consisting of:
    /// - 3 bytes for the preamble
    /// - 0-1023 bytes for data
    /// - 3 bytes for the CRC (Cyclic Redundancy Check)
    /// </remarks>
    private readonly byte[] _buffer = new byte[1030]; // 3 (preamb.) + 0-1023 bytes data + 3 (CRC)

    /// <summary>
    /// Represents the state of an object.
    /// </summary>
    private State _state;

    /// <summary>
    /// The number of payload bytes read.
    /// </summary>
    private ushort _payloadReadedBytes;

    /// <summary>
    /// Represents the length of the payload in bytes.
    /// </summary>
    private int _payloadLength;

    private readonly ILogger<RtcmV3MessageParser> _logger;


    public RtcmV3MessageParser(IProtocolMessageFactory<RtcmV3MessageBase, ushort> messageFactory,
        IProtocolContext context, IStatisticHandler? statisticHandler) : base(messageFactory, context, statisticHandler)
    {
        _logger = context.LoggerFactory.CreateLogger<RtcmV3MessageParser>();
    }

    public override bool Push(byte data)
    {
        switch (_state)
            {
                case State.Sync:
                    if (data == RtcmV3Protocol.SyncByte)
                    {
                        _state = State.Preamb1;
                        _buffer[0] = data;
                    }
                    break;
                case State.Preamb1:
                    _buffer[1] = data;
                    _state = State.Preamb2;
                    break;
                case State.Preamb2:
                    _buffer[2] = data;
                    _state = State.Payload;
                    _payloadLength = (ushort)BitHelper.GetBitU(_buffer, 14 /* preamble-8bit + reserved-6bit */, 10 /* length-10bit */);
                    _payloadReadedBytes = 0;
                    if (_payloadLength > _buffer.Length)
                    {
                        // buffer oversize
                        Reset();
                    }
                    break;
                case State.Payload:
                    // read payload
                    if (_payloadReadedBytes < _payloadLength)
                    {
                        _buffer[_payloadReadedBytes + 3] = data;
                        ++_payloadReadedBytes;
                    }
                    else
                    {
                        _state = State.Crc1;
                        goto case State.Crc1;
                    }
                    break;
                case State.Crc1:
                    _buffer[_payloadLength + 3] = data;
                    _state = State.Crc2;
                    break;
                case State.Crc2:
                    _buffer[_payloadLength + 3 + 1] = data;
                    _state = State.Crc3;
                    break;
                case State.Crc3:
                    _buffer[_payloadLength + 3 + 2] = data;
                    var msgId = RtcmV3Protocol.GetMessageId(_buffer);
                    var spanMsg = new ReadOnlySpan<byte>(_buffer, 0, 3 /* preamble-8bit + reserved-6bit + length-10bit */ + _payloadLength + 3 /* CRC24 */);
                    try
                    {
                        InternalParsePacket(msgId, ref spanMsg);
                        Reset();
                        return true;
                    }
                    catch (ProtocolParserException ex)
                    {
                        _logger.ZLogTrace($"{ex.Message}[RtcmV3Message{msgId}]");
                        InternalOnError(ex);
                        return false;
                    }
                    catch (Exception ex)
                    {
                        _logger.ZLogTrace($"{ex.Message}[RtcmV3Message{msgId}]");
                        InternalOnError(new ProtocolParserException(Info,"Parser ",ex));
                        return false;
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return false;
    }

    public override void Reset()
    {
        _state = State.Sync;
    }
    
    /// <summary>
    /// Represents the state of a system.
    /// </summary>
    private enum State
    {
        /// <summary>
        /// Represents the state of synchronization.
        /// </summary>
        Sync,

        /// <summary>
        /// Represents the state where the first preamble is being processed.
        /// </summary>
        Preamb1,

        /// <summary>
        /// Represents the second preamble state of a communication protocol.
        /// </summary>
        Preamb2,

        /// <summary>
        /// Represents the possible states of a transmission.
        /// </summary>
        Payload,

        /// <summary>
        /// Represents the state of the system when calculating the Crc2.
        /// </summary>
        Crc2,

        /// <summary>
        /// Represents the Crc1 state of a system.
        /// </summary>
        Crc1,

        /// <summary>
        /// Represents the possible state of a system.
        /// </summary>
        Crc3
    }

    public override ProtocolInfo Info => RtcmV3Protocol.Info;
}