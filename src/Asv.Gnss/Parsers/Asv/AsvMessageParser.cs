using System;
using System.Collections.Generic;

namespace Asv.Gnss
{
    /// <summary>
    /// Represents a message parser for the Asv protocol.
    /// </summary>
    /// <typeparam name="AsvMessageBase">The base type for Asv messages.</typeparam>
    /// <typeparam name="ushort">The type used for message IDs.</typeparam>
    public class AsvMessageParser : GnssMessageParserBase<AsvMessageBase, ushort>
    {
        /// <summary>
        /// The GNSS protocol identifier.
        /// </summary>
        /// <value>
        /// A constant string representing the GNSS protocol identifier.
        /// </value>
        public const string GnssProtocolId = "Asv";

        /// <summary>
        /// The size of the header in bytes.
        /// </summary>
        public const ushort HeaderSize = 10;

        /// <summary>
        /// The size of the CRC (Cyclic Redundancy Check) in bytes.
        /// </summary>
        /// <remarks>
        /// This variable is defined as the size of the CRC16, which is 2 bytes (ushort).
        /// </remarks>
        public const ushort CrcSize = sizeof(ushort); /*CRC16*/

        /// <summary>
        /// Represents the size of data, in bytes.
        /// </summary>
        public const ushort DataSize = 1012; /*DATA*/

        /// <summary>
        /// The maximum size of a message, including data, header, and CRC.
        /// </summary>
        public const ushort MaxMessageSize =
            DataSize
            + HeaderSize
            + CrcSize /*CRC16*/
        ;

        /// <summary>
        /// Constant variable representing the Sync1 value.
        /// </summary>
        public const byte Sync1 = 0xAA;

        /// <summary>
        /// Represents a constant value for Sync2.
        /// </summary>
        /// <remarks>
        /// Sync2 is a byte constant with a value of 0x44.
        /// </remarks>
        public const byte Sync2 = 0x44;

        /// <summary>
        /// Represents the identifier of the GNSS protocol used.
        /// </summary>
        /// <value>
        /// The GNSS protocol identifier.
        /// </value>
        public override string ProtocolId => GnssProtocolId;

        /// <summary>
        /// The buffer used to store a byte array.
        /// </summary>
        private readonly byte[] _buffer = new byte[MaxMessageSize];

        /// Represents the current state of an object.
        /// /
        private State _state;

        /// <summary>
        /// The index of the buffer.
        /// </summary>
        private int _bufferIndex;

        /// <summary>
        /// Represents the stop index of a certain operation.
        /// </summary>
        private int _stopIndex;

        /// <summary>
        /// Reads a byte of data and processes it according to the protocol.
        /// </summary>
        /// <param name="data">The byte of data to be processed.</param>
        /// <returns>
        /// Returns true if the byte of data is successfully processed and a complete message is parsed.
        /// Returns false if the byte of data does not contribute to a complete message.
        /// </returns>
        public override bool Read(byte data)
        {
            switch (_state)
            {
                case State.Sync1:
                    if (data != Sync1)
                        return false;
                    _bufferIndex = 0;
                    _buffer[_bufferIndex++] = Sync1;
                    _state = State.Sync2;
                    break;
                case State.Sync2:
                    if (data != Sync2)
                    {
                        _state = State.Sync1;
                    }
                    else
                    {
                        _state = State.MessageLength;
                        _buffer[_bufferIndex++] = Sync2;
                    }
                    break;
                case State.MessageLength:
                    _buffer[_bufferIndex++] = data;
                    if (_bufferIndex == 4)
                    {
                        _stopIndex = BitConverter.ToUInt16(_buffer, 2) + 12; // 10 header + 2 crc = 12
                        _state = _stopIndex >= _buffer.Length ? State.Sync1 : State.Message;
                    }
                    break;
                case State.Message:
                    _buffer[_bufferIndex++] = data;
                    if (_bufferIndex == _stopIndex)
                    {
                        var crc = BitConverter.ToUInt16(_buffer, _stopIndex - 2);
                        var calcCrc = AsvCrc16.Calc(_buffer, 0, _stopIndex - 2);
                        if (calcCrc != crc)
                        {
                            PublishWhenCrcError();
                            Reset();
                        }
                        else
                        {
                            var msgId = BitConverter.ToUInt16(_buffer, 8);
                            var span = new ReadOnlySpan<byte>(_buffer, 0, _stopIndex);
                            ParsePacket(msgId, ref span);
                            Reset();
                            return true;
                        }
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return false;
        }

        /// <summary>
        /// Resets the state of the object to the initial state.
        /// </summary>
        public override void Reset()
        {
            _state = State.Sync1;
        }

        /// <summary>
        /// Represents the different states of a process.
        /// </summary>
        private enum State
        {
            /// <summary>
            /// Represents the first synchronization state.
            /// </summary>
            /// <remarks>
            /// This state occurs during the synchronization process where the system is in its initial state and is waiting for the first synchronization signal.
            /// </remarks>
            Sync1,

            /// <summary>
            /// Represents the second sync state.
            /// </summary>
            Sync2,

            /// <summary>
            /// Represents a member of the <see cref="State"/> enumeration that indicates the state of the message length.
            /// </summary>
            MessageLength,

            /// <summary>
            /// Represents the state of the process.
            /// </summary>
            Message,
        }
    }
}
