using System;
using System.Reactive.Subjects;
using Asv.Common;
using Asv.IO;

namespace Asv.Gnss
{
    /// <summary>
    /// A parser for RTCMv3 messages.
    /// </summary>
    public class RtcmV3Parser : GnssMessageParserBase<RtcmV3MessageBase, ushort>
    {
        /// <summary>
        /// The GNSS protocol identifier.
        /// </summary>
        public const string GnssProtocolId = "RTCMv3";

        /// <summary>
        /// Event raised when raw data is received.
        /// </summary>
        private readonly Subject<RtcmV3RawMessage> _onRawData = new();

        /// <summary>
        /// Private readonly variable representing a buffer of bytes.
        /// </summary>
        /// <remarks>
        /// The buffer has a size of 1030 bytes, consisting of:
        /// - 3 bytes for the preamble
        /// - 0-1023 bytes for data
        /// - 3 bytes for the CRC (Cyclic Redundancy Check).
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
            Crc3,
        }

        /// <summary>
        /// Gets the unique identifier of the GNSS protocol.
        /// </summary>
        /// <value>
        /// The GNSS protocol identifier.
        /// </value>
        public override string ProtocolId => GnssProtocolId;

        /// <summary>
        /// Reads and processes a byte of data for the RtcmV3Decoder.
        /// </summary>
        /// <param name="data">The byte to be processed.</param>
        /// <returns>True if the byte was processed successfully, false otherwise.</returns>
        public override bool Read(byte data)
        {
            switch (_state)
            {
                case State.Sync:
                    if (data == RtcmV3Helper.SyncByte)
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
                    _payloadLength = (ushort)
                        BitHelper.GetBitU(
                            _buffer,
                            14, /* preamble-8bit + reserved-6bit */
                            10 /* length-10bit */
                        );
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

                    var originalCrc = RtcmV3Crc24.Calc(_buffer, _payloadLength + 3, 0);
                    var sourceCrc = BitHelper.GetBitU(
                        _buffer,
                        (uint)((_payloadLength + 3) * 8),
                        24
                    );
                    if (originalCrc == sourceCrc)
                    {
                        var msgNumber = (ushort)
                            BitHelper.GetBitU(
                                _buffer,
                                24, /* preamble-8bit + reserved-6bit + length-10bit */
                                12
                            );
                        var span = new ReadOnlySpan<byte>(
                            _buffer,
                            0,
                            _payloadLength
                                + 6 /* preamble-8bit + reserved-6bit + length-10bit */
                                + 3 /*CRC24*/
                        );
                        if (_onRawData.HasObservers)
                        {
                            _onRawData.OnNext(new RtcmV3RawMessage(msgNumber, span));
                        }

                        ParsePacket(msgNumber, ref span, true);
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
        /// Resets the state of the object to Sync.
        /// </summary>
        public override void Reset()
        {
            _state = State.Sync;
        }

        /// <summary>
        /// Gets event that is raised when a RTCMv3 raw message is received.
        /// </summary>
        /// <value>
        /// An <see cref="IObservable{T}"/> representing the event stream of RTCMv3 raw messages.
        /// </value>
        public IObservable<RtcmV3RawMessage> OnRawMessage => _onRawData;

        /// <summary>
        /// Performs the internal disposal operations once.
        /// </summary>
        protected override void InternalDisposeOnce()
        {
            base.InternalDisposeOnce();
            _onRawData.OnCompleted();
            _onRawData.Dispose();
        }
    }
}
