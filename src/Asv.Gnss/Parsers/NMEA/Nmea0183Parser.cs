using System;
using System.Text;

namespace Asv.Gnss
{
    public class Nmea0183Parser:GnssMessageParserBase<Nmea0183MessageBase,string>
    {
        public const string GnssProtocolId = "NMEA0183";
        private State _state;
        private readonly byte[] _buffer = new byte[1024];
        private readonly byte[] crcBuffer = new byte[2];
        private int _msgReaded;

       

        private enum State
        {
            Sync,
            Msg,
            Crc1,
            Crc2,
            End1,
            End2,
        }


        public override string ProtocolId => GnssProtocolId;

        public override bool Read(byte data)
        {
             switch (_state)
            {
                case State.Sync:
                     if (data == 0x24 /*'$'*/ || data == 0x21/*'!'*/)
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
                    if (readCrc == calcCrc)
                    {
                        var msgId = strMessage.Substring(2, 3).ToUpper();
                        var span = new ReadOnlySpan<byte>(_buffer, 0, _msgReaded);
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

       

        public override void Reset()
        {
            _state = State.Sync;
        }

        
    }
}