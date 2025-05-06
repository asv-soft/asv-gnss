using System;

namespace Asv.Gnss
{
    /// <summary>
    /// Represents a binary parser for UBX messages in a GNSS protocol.
    /// </summary>
    public class UbxBinaryParser : GnssMessageParserBase<UbxMessageBase, ushort>
    {
        /// <summary>
        /// The GNSS protocol identifier.
        /// </summary>
        public const string GnssProtocolId = "UBX";

        /// <summary>
        /// Gets the identifier of the protocol.
        /// </summary>
        /// <value>
        /// The protocol identifier.
        /// </value>
        public override string ProtocolId => GnssProtocolId;

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

        // The current index position in the buffer.
        // /
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
            Crc2,
        }

        /// <summary>
        /// Reads a byte of data and processes it according to the current state of the object.
        /// </summary>
        /// <param name="data">The byte of data to be processed.</param>
        /// <returns>True if the data is successfully processed; otherwise, false.</returns>
        public override bool Read(byte data)
        {
            switch (_state)
            {
                case State.Sync1:
                    if (data != UbxHelper.SyncByte1)
                    {
                        break;
                    }

                    _bufferIndex = 0;
                    _buffer[_bufferIndex++] = UbxHelper.SyncByte1;
                    _state = State.Sync2;
                    break;
                case State.Sync2:
                    if (data != UbxHelper.SyncByte2)
                    {
                        _state = State.Sync1;
                    }
                    else
                    {
                        _state = State.IdAndLength;
                        _buffer[_bufferIndex++] = UbxHelper.SyncByte2;
                    }

                    break;
                case State.IdAndLength:
                    _buffer[_bufferIndex++] = data;
                    if (_bufferIndex == 6)
                    {
                        _payloadLength = _buffer[4] | (_buffer[5] << 8);
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
                        {
                            _state = State.Crc1;
                        }
                    }

                    break;
                case State.Crc1:
                    _buffer[_payloadReadBytes + 6] = data;
                    _state = State.Crc2;
                    break;
                case State.Crc2:
                    _buffer[_payloadReadBytes + 6 + 1] = data;
                    var originalCrc = UbxCrc16.Calc(
                        new ReadOnlySpan<byte>(
                            _buffer,
                            2,
                            _payloadLength + 4 /*ID + Length*/
                        )
                    );
                    var sourceCrc = new[] { _buffer[_payloadReadBytes + 6], data };

                    if (originalCrc.Crc1 == sourceCrc[0] && originalCrc.Crc2 == sourceCrc[1])
                    {
                        var msgId = UbxHelper.ReadMessageId(_buffer);
                        var span = new ReadOnlySpan<byte>(_buffer, 0, _payloadReadBytes + 8);
                        ParsePacket(msgId, ref span);
                        Reset();
                        return true;
                    }

                    PublishWhenCrcError();
                    Reset();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return false;
        }

        /// <summary>
        /// Resets the state to Sync1.
        /// </summary>
        public override void Reset()
        {
            _state = State.Sync1;
        }
    }
}
