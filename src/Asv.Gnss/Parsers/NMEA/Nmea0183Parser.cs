using System;
using System.Collections.Generic;
using System.Text;

namespace Asv.Gnss
{
    /// <summary>
    /// The Nmea0183Parser class is responsible for parsing NMEA 0183 messages.
    /// </summary>
    public class Nmea0183Parser:GnssMessageParserBase<Nmea0183MessageBase,string>
    {
        /// <summary>
        /// The constant variable representing the GNSS protocol ID.
        /// </summary>
        public const string GnssProtocolId = "NMEA0183";

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
        private readonly byte[] crcBuffer = new byte[2];

        /// <summary>
        /// Represents the number of read messages.
        /// </summary>
        private int _msgReaded;

        private readonly List<Nmea0183GetMessageIdDelegate> _proprietaryMessageIdGetterList = new(2);


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

        /// <summary>
        /// Gets the ProtocolId property.
        /// </summary>
        /// <value>
        /// The identifier of the GNSS protocol.
        /// </value>
        public override string ProtocolId => GnssProtocolId;

        /// <summary>
        /// Reads a byte of data and processes it according to the current state.
        /// </summary>
        /// <param name="data">The data byte to be read and processed.</param>
        /// <returns>
        /// <c>true</c> if the byte is successfully processed and the message is valid;
        /// otherwise, <c>false</c> if the byte is invalid or the message fails the CRC check.
        /// </returns>
        public override bool Read(byte data)
        {
             switch (_state)
            {
                case State.Sync:
                    if (data == 0x24 /*'$'*/ || data == 0x21 /*'!'*/)
                    {
                        _msgReaded = 0;
                        _state = State.Msg;
                    }
                    break;
                case State.Msg:
                    if (data == '*')
                    {
                        _state = State.Crc1;
                    }
                    else if (data == 0x0D)
                    {
                        _state = State.NoChecksum;
                    }
                    else
                    {
                        if (_msgReaded >= (_buffer.Length - 2))
                        {
                            // oversize
                            _state = State.Sync;
                        }
                        else
                        {
                            _buffer[_msgReaded] = data;
                            ++_msgReaded;
                        }
                    }
                    break;
                case State.Crc1:
                    crcBuffer[0] = data;
                    _state = State.Crc2;
                    break;
                case State.Crc2:
                    crcBuffer[1] = data;
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
                    var strMessage = Encoding.ASCII.GetString(_buffer, 0, _msgReaded);
                    var readCrc = Encoding.ASCII.GetString(crcBuffer);
                    var calcCrc = NmeaCrc.Calc(new ReadOnlySpan<byte>(_buffer,0,_msgReaded));
                    string msgId = null;
                    if (readCrc == calcCrc)
                    {
                        if (strMessage.StartsWith('P')) // proprietary message
                        {
                            foreach (var idDelegate in _proprietaryMessageIdGetterList)
                            {
                                if (!idDelegate(strMessage, out var parsedMessageId)) continue;
                                msgId = parsedMessageId;
                                break;
                            }
                        }
                        else // standard message
                        {
                            msgId = strMessage.Substring(2, 3).ToUpper();
                        }
                        if (msgId == null)
                        {
                            Reset();
                            return false;
                        }
                        var span = new ReadOnlySpan<byte>(_buffer, 0, _msgReaded);
                        ParsePacket(msgId, ref span);
                        Reset();
                        return true;
                        
                    }
                    PublishWhenCrcError();
                    Reset();
                    break;
                case State.NoChecksum:
                    if (data != 0x0A)
                    {
                        Reset();
                        return false;
                    }
                    var strMessage1 = Encoding.ASCII.GetString(_buffer, 0, _msgReaded);
                    string msgId1 = null;
                    if (strMessage1.StartsWith('P')) // proprietary message
                    {
                        foreach (var idDelegate in _proprietaryMessageIdGetterList)
                        {
                            if (!idDelegate(strMessage1, out var parsedMessageId)) continue;
                            msgId1 = parsedMessageId;
                            break;
                        }
                    }
                    else // standard message
                    {
                        msgId1 = strMessage1.Substring(2, 3).ToUpper();
                    }
                    if (msgId1 == null)
                    {
                        Reset();
                        return false;
                    }
                    var span1 = new ReadOnlySpan<byte>(_buffer, 0, _msgReaded);
                    ParsePacket(msgId1, ref span1);
                    Reset();
                    return true;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return false;
        }

        public void RegisterProprietary(Nmea0183GetMessageIdDelegate idGetter,Func<Nmea0183MessageBase> factory)
        {
            _proprietaryMessageIdGetterList.Add(idGetter);
            base.Register(factory);
        }

        /// <summary>
        /// Resets the state of the object to the initial state.
        /// </summary>
        public override void Reset()
        {
            _state = State.Sync;
        }

        
    }
    
    public delegate bool Nmea0183GetMessageIdDelegate(string rawNmeaSentence, out string messageId);
}