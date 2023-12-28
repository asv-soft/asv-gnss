using System;
using Asv.IO;

namespace Asv.Gnss
{
    /// <summary>
    /// Represents a parser for RTCM version 2 messages.
    /// </summary>
    public class RtcmV2Parser : GnssMessageParserBase<RtcmV2MessageBase, ushort>
    {
        /// <summary>
        /// Constant variable representing the GNSS protocol ID used for communication.
        /// </summary>
        /// <remarks>
        /// This GNSS protocol ID indicates that the communication is using the RtcmV2 protocol.
        /// </remarks>
        public const string GnssProtocolId = "RtcmV2";

        /// <summary>
        /// The synchronization byte used for communication.
        /// </summary>
        public const byte SyncByte = 0x66;

        /// <summary>
        /// Private readonly variable representing a message buffer.
        /// </summary>
        private readonly byte[] _buffer = new byte[33 * 3]; /* message buffer   */

        /// <summary>
        /// Private variable to hold the word buffer for rtcm 2.
        /// </summary>
        private uint _word;                /* word buffer for rtcm 2            */

        /// <summary>
        /// Number of bytes in the message buffer.
        /// </summary>
        private int _readedBytes;          /* number of bytes in message buffer */

        /// <summary>
        /// The number of bits in the word buffer.
        /// </summary>
        private int _readedBits;           /* number of bits in word buffer     */

        /// <summary>
        /// Represents the length of a message in bytes.
        /// </summary>
        private int _len;                  /* message length (bytes)            */

        /// <summary>
        /// Gets the identifier of the protocol.
        /// </summary>
        /// <value>
        /// The identifier of the protocol.
        /// </value>
        public override string ProtocolId => GnssProtocolId;

        /// <summary>
        /// Reads the specified data byte.
        /// </summary>
        /// <param name="data">The data byte to be read.</param>
        /// <returns>True if the read operation is successful; otherwise, false.</returns>
        public override bool Read(byte data)
        {
            if ((data & 0xC0) != 0x40)
            {
                return false; /* ignore if upper 2bit != 01 */
            }

            for (var i = 0; i < 6; i++, data >>= 1)
            {
                /* decode 6-of-8 form */
                _word = (uint)((_word << 1) + (data & 1));

                /* synchronize frame */
                if (_readedBytes == 0)
                {
                    var preamb = (byte)(_word >> 22);
                    if ((_word & 0x40000000) != 0) preamb ^= 0xFF; /* decode preamble */
                    if (preamb != SyncByte) continue;

                    /* check parity */
                    if (!DecodeWord(_word, _buffer, 0)) continue;
                    _readedBytes = 3; _readedBits = 0;
                    continue;
                }

                if (++_readedBits < 30)
                {
                    continue;
                }

                _readedBits = 0;

                /* check parity */
                if (!DecodeWord(_word, _buffer, _readedBytes))
                {
                    PublishWhenCrcError();
                    _readedBytes = 0; _word &= 0x3;
                    continue;
                }
                _readedBytes += 3;
                if (_readedBytes == 6) _len = (_buffer[5] >> 3) * 3 + 6;
                if (_readedBytes < _len) continue;
                _readedBytes = 0;
                _word &= 0x3;


                /* decode rtcm2 message */
                var pos = 8;
                var msgType = (ushort)SpanBitHelper.GetBitU(_buffer,ref pos, 6);
                var span = new ReadOnlySpan<byte>(_buffer);
                ParsePacket(msgType, ref span,true);
                Reset();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Decodes a word using a hamming code and populates a byte array with the decoded data.
        /// </summary>
        /// <param name="word">The word to be decoded.</param>
        /// <param name="data">The byte array to populate with the decoded data.</param>
        /// <param name="offset">The starting index in the byte array to populate with the decoded data.</param>
        /// <returns>
        /// True if the word is successfully decoded and the byte array is populated with the decoded data.
        /// False otherwise.
        /// </returns>
        private static bool DecodeWord(uint word, byte[] data, int offset)
        {
            var hamming = new uint[] { 0xBB1F3480, 0x5D8F9A40, 0xAEC7CD00, 0x5763E680, 0x6BB1F340, 0x8B7A89C0 };
            uint parity = 0;

            if ((word & 0x40000000) != 0) word ^= 0x3FFFFFC0;

            for (var i = 0; i < 6; i++)
            {
                parity <<= 1;
                for (var w = (word & hamming[i]) >> 6; w != 0; w >>= 1)
                    parity ^= w & 0x1;
            }
            if (parity != (word & 0x3F)) return false;

            for (var i = 0; i < 3; i++) data[i + offset] = (byte)(word >> (22 - i * 8));
            return true;
        }

        /// <summary>
        /// Resets the object to its initial state.
        /// </summary>
        public override void Reset()
        {
            //_word = 0;
            _readedBytes = 0;
            _readedBits = 0;
            _len = 0;
        }
    }
}