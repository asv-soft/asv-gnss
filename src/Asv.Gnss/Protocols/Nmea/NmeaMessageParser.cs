using System;
using System.Diagnostics;
using Asv.IO;
using Microsoft.Extensions.Logging;
using ZLogger;

namespace Asv.Gnss;


/// <summary>
/// The Nmea0183Parser class is responsible for parsing NMEA 0183 messages.
/// </summary>
public class NmeaMessageParser : ProtocolParser<NmeaMessage, NmeaMessageId>
{
    
    /// <summary>
    /// The current state of the object.
    /// </summary>
    private State _state;

    /// <summary>
    /// Represents a byte array buffer with a fixed size of 1024 bytes.
    /// </summary>
    private readonly byte[] _buffer = new byte[1024];

    /// <summary>
    /// The cyclic redundancy check (CRC) buffer.
    /// </summary>
    private readonly byte[] _crcBuffer = new byte[2];

    /// <summary>
    /// Represents the number of read messages.
    /// </summary>
    private int _byteRead;

    private readonly ILogger<NmeaMessageParser> _logger;

    public NmeaMessageParser(
        IProtocolMessageFactory<NmeaMessage,NmeaMessageId> messageFactory, 
        IProtocolContext context, 
        IStatisticHandler? statisticHandler) 
        : base(messageFactory, context, statisticHandler)
    {
        _logger = context.LoggerFactory.CreateLogger<NmeaMessageParser>();
    }

    /// <summary>
    /// Represents the state of the system.
    /// </summary>
    private enum State
    {
        /// <summary>
        /// Represents the synchronization state of a process.
        /// </summary>
        Sync,

        /// <summary>
        /// Represents the state of the messaging process.
        /// </summary>
        Msg,

        /// <summary>
        /// Represents the Crc1 state of the State enum.
        /// </summary>
        Crc1,

        /// <summary>
        /// Represents the Crc2 state in the State enum.
        /// </summary>
        Crc2,

        /// <summary>
        /// Represents the End1 state of the State enum.
        /// </summary>
        End1,

        /// <summary>
        /// Represents the 'End2' state of the State enum.
        /// </summary>
        End2,
            
        /// <summary>
        /// Represents the NoCheckSum state of the State enum
        /// </summary>
        NoChecksum,
    }
    
    public override bool Push(byte data)
    {
        try
        {
            switch (_state)
            {
                case State.Sync:
                    if (data is NmeaProtocol.StartMessageByte1 or NmeaProtocol.StartMessageByte2)
                    {
                        _byteRead = 0;
                        _state = State.Msg;
                    }

                    break;
                case State.Msg:
                    switch (data)
                    {
                        case NmeaProtocol.StartCrcByte:
                            _state = State.Crc1;
                            break;
                        case NmeaProtocol.EndMessageByte:
                            _state = State.NoChecksum;
                            break;
                        default:
                        {
                            if (_byteRead >= (_buffer.Length - 2))
                            {
                                // oversize
                                _state = State.Sync;
                            }
                            else
                            {
                                _buffer[_byteRead] = data;
                                ++_byteRead;
                            }

                            break;
                        }
                    }

                    break;
                case State.Crc1:
                    _crcBuffer[0] = data;
                    _state = State.Crc2;
                    break;
                case State.Crc2:
                    _crcBuffer[1] = data;
                    _state = State.End1;
                    break;
                case State.End1:
                    if (data != 0x0D)
                    {
                        Reset();
                        return false;
                    }

                    _state = State.End2;
                    break;
                case State.End2:
                    if (data != 0x0A)
                    {
                        Reset();
                        return false;
                    }

                    var spanMsg = new ReadOnlySpan<byte>(_buffer, 0, _byteRead);
                    var readCrc = NmeaProtocol.Encoding.GetString(_crcBuffer);
                    var calcCrc = NmeaProtocol.CalcCrc(spanMsg);
                    if (readCrc == calcCrc && NmeaProtocol.TryGetMessageId(spanMsg, out var msgId, out _))
                    {
                        InternalParsePacket(msgId, ref spanMsg);
                        Reset();
                        return true;
                    }

                    Reset();
                    return false;

                case State.NoChecksum:
                    if (data != 0x0A)
                    {
                        Reset();
                        return false;
                    }
                    var spanMsg1 = new ReadOnlySpan<byte>(_buffer, 0, _byteRead);
                    if (NmeaProtocol.TryGetMessageId(spanMsg1, out var msgId2, out _))
                    {
                        InternalParsePacket(msgId2, ref spanMsg1);
                        Reset();
                        return true;
                    }
                    Reset();
                    return false;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return false;
        }
        catch (Exception e)
        {
            _logger.ZLogCritical(e,$"Fatal error to decode packet:{e.Message}");
            _state = State.Sync;
            Debug.Assert(false, e.Message);
            return false;
        }

    }

    

    /// <summary>
    /// Resets the state of the object to the initial state.
    /// </summary>
    public override void Reset()
    {
        _state = State.Sync;
    }

    public override ProtocolInfo Info => NmeaProtocol.ProtocolInfo;
}