using System;
using Asv.IO;
using Microsoft.Extensions.Logging;
using ZLogger;

namespace Asv.Gnss
{
    /// <summary>
    /// This class represents a binary parser for ComNav GNSS messages.
    /// It extends the GnssMessageParserBase class, which parses GNSS messages.
    /// </summary>
    public class ComNavBinaryParser : ProtocolParser<ComNavBinaryMessageBase, ushort>
    {
        /// <summary>
        /// Represents the GNSS protocol ID.
        /// </summary>
        public const string GnssProtocolId = "ComNavBinary";
        
        /// <summary>
        /// The maximum size of a packet.
        /// </summary>
        public const int MaxPacketSize = 1024 * 10;
        
        /// <summary>
        /// The byte value representing the first synchronization byte.
        /// </summary>
        public const byte FirstSyncByte = 0xAA;

        /// <summary>
        /// The second synchronization byte used in the code.
        /// </summary>
        /// <remarks>
        /// This constant variable stores the value of the second synchronization byte in the code.
        /// The value is represented as a byte and is equal to 0x44.
        /// </remarks>
        public const byte SecondSyncByte = 0x44;

        /// <summary>
        /// Represents the third synchronization byte.
        /// </summary>
        public const byte ThirdSyncByte = 0x12;


        /// <summary>
        /// Represents the current state.
        /// </summary>
        private State _state;

        /// <summary>
        /// Represents a buffer used for storing byte data.
        /// </summary>
        private readonly byte[] _buffer = new byte[MaxPacketSize];

        /// <summary>
        /// Represents the index of the buffer.
        /// </summary>
        private int _bufferIndex = 0;

        /// <summary>
        /// Represents the length of the header in bytes.
        /// </summary>
        private byte _headerLength;

        /// Represents the length of a message.
        /// This variable is a private member of a class.
        /// </summary>
        private ushort _messageLength;

        /// <summary>
        /// The index at which the stop message should be displayed.
        /// </summary>
        private int _stopMessageIndex;
        private readonly ILogger<ComNavBinaryParser> _logger;


        /// <summary>
        /// Represents the possible states for a synchronization process.
        /// </summary>
        private enum State
        {
            /// <summary>
            /// Represents the first synchronization state in the state machine.
            /// </summary>
            Sync1,

            /// <summary>
            /// Represents the second state of synchronization.
            /// </summary>
            Sync2,

            /// <summary>
            /// Represents the third synchronization state.
            /// </summary>
            Sync3,

            /// <summary>
            /// Represents the enumeration of possible states.
            /// </summary>
            HeaderLength,

            /// <summary>
            /// Enum representing the different states of synchronization and message processing.
            /// </summary>
            Header,

            /// <summary>
            /// Represents a state in the message processing workflow.
            /// </summary>
            Message
        }

        /// <summary>
        /// Gets the protocol ID for the GNSS protocol.
        /// </summary>
        /// <remarks>
        /// The protocol ID uniquely identifies the GNSS protocol.
        /// </remarks>
        /// <value>
        /// The protocol ID for the GNSS protocol.
        /// </value>
        public ComNavBinaryParser(
            IProtocolMessageFactory<ComNavBinaryMessageBase, ushort> messageFactory,
            IProtocolContext context,
            IStatisticHandler? statisticHandler) : base(messageFactory, context, statisticHandler)
        {
            _logger = context.LoggerFactory.CreateLogger<ComNavBinaryParser>();
        }

        public override ProtocolInfo Info => ComNavProtocol.BinaryInfo;


        /// <summary>
        /// Read method reads a byte of data and processes it according to the current state of the parser.
        /// </summary>
        /// <param name="data">The byte of data to be read and processed.</param>
        /// <returns>Returns a boolean value indicating if the read and processing was successful.</returns>
        public override bool Push(byte data)
        {
            switch (_state)
            {
                case State.Sync1:
                    if (data != FirstSyncByte) return false;
                    _bufferIndex = 0;
                    _buffer[_bufferIndex++] = FirstSyncByte;
                    _state = State.Sync2;
                    break;
                case State.Sync2:
                    if (data != SecondSyncByte)
                    {
                        _state = State.Sync1;
                    }
                    else
                    {
                        _state = State.Sync3;
                        _buffer[_bufferIndex++] = SecondSyncByte;
                    }
                    break;
                case State.Sync3:
                    if (data != ThirdSyncByte)
                    {
                        _state = State.Sync1;
                    }
                    else
                    {
                        _state = State.HeaderLength;
                        _buffer[_bufferIndex++] = ThirdSyncByte;
                    }
                    break;
                case State.HeaderLength:
                    _headerLength = data;
                    _buffer[_bufferIndex++] = data;
                    _state = State.Header;
                    break;
                case State.Header:
                    _buffer[_bufferIndex++] = data;
                    if (_bufferIndex == _headerLength)
                    {
                        _messageLength = BitConverter.ToUInt16(_buffer, 8);
                        _stopMessageIndex = _headerLength + _messageLength + 4 /* CRC 32 bit*/;
                        _state = State.Message;
                    }
                    break;
                case State.Message:
                    _buffer[_bufferIndex++] = data;
                    if (_bufferIndex == _stopMessageIndex)
                    {
                        /* step back to last byte */
                        var crc32Index = _bufferIndex - 4 /* CRC32 */;
                        var calculatedHash = ComNavCrc32.Calc(_buffer, 0, crc32Index);
                        var readedHash = BitConverter.ToUInt32(_buffer, crc32Index);
                        if (calculatedHash == readedHash)
                        {
                            var msgId = BitConverter.ToUInt16(_buffer, 4);
                            var span = new ReadOnlySpan<byte>(_buffer, 0, _stopMessageIndex);
                            try
                            {
                                InternalParsePacket(msgId, ref span);
                            }
                            catch (Exception ex)
                            {
                                _logger.ZLogTrace($"{ex.Message}[ComNavBinaryMessage{msgId}]");
                                InternalOnError(new ProtocolParserException(Info, "Parser ", ex));
                                Reset();
                                return false;
                            }
                            Reset();
                            return true;
                        }
                        PublishWhenCrcError();
                        Reset();
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();

            }
            return false;
        }

        /// <summary>
        /// Resets the state of the object to its initial value.
        /// </summary>
        public override void Reset()
        {
            _state = State.Sync1;
            _bufferIndex = 0;
            _headerLength = 0;
            _messageLength = 0;
            _stopMessageIndex = 0;
        }

        private void PublishWhenCrcError()
        {
            InternalOnError(new ProtocolParserException(Info, "CRC error"));
        }
    }
}
