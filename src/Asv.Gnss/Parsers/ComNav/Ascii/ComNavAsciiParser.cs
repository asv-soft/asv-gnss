using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Asv.Gnss
{
    public class ComNavAsciiParser : GnssMessageParserBase<ComNavAsciiMessageBase, string>
	{
		public const string GnssProtocolId = "ComNavAscii";

		public const byte FirstSyncByte = 0x23;
		public const byte Separator = 0x2C;
		public const byte HeaderSeparator = 0x3B;
		public const byte MessageSeparator = 0x2A;
		public const byte CarriageReturn = 0xD;
		public const byte LineFeed = 0xA;

		private const int MaxPacketSize = 1024 * 4;

		private State _state;
		private readonly byte[] _buffer = new byte[MaxPacketSize];
		private readonly StringBuilder _crc = new();
		private int _bufferIndex = 0;

		private enum State
		{
			Sync,
			Header,
			Message,
			Crc,
			CarriageReturn
		}
		public override string ProtocolId => GnssProtocolId;
		public override bool Read(byte data)
		{
			switch (_state)
			{
				case State.Sync:
					if (data != FirstSyncByte) return false;
					_bufferIndex = 0;
					_buffer[_bufferIndex++] = FirstSyncByte;
					_state = State.Header;
					break;
				case State.Header:
					if (data == FirstSyncByte)
					{
						Reset();
						_buffer[_bufferIndex++] = FirstSyncByte;
						_state = State.Header;
					}
					if (_bufferIndex >= MaxPacketSize - 6  // ;(1 byte) + *(1 byte) + CRC32
					    || data is MessageSeparator or CarriageReturn or LineFeed)
					{
						Reset();
						break;
					}
					_buffer[_bufferIndex++] = data;
					if (data == HeaderSeparator) _state = State.Message;
					break;
				case State.Message:
					if (data == FirstSyncByte)
					{
						Reset();
						_buffer[_bufferIndex++] = FirstSyncByte;
						_state = State.Header;
					}
					if (_bufferIndex >= MaxPacketSize - 5  // *(1 byte) + CRC32
					    || data is HeaderSeparator or CarriageReturn or LineFeed)
					{
						Reset();
						break;
					}
					_buffer[_bufferIndex++] = data;
					if (data == MessageSeparator) _state = State.Crc;
					break;
				case State.Crc:
					if (data == FirstSyncByte)
					{
						Reset();
						_buffer[_bufferIndex++] = FirstSyncByte;
						_state = State.Header;
					}
					if (_bufferIndex >= MaxPacketSize - 4  /* CRC32 */
					    || data is HeaderSeparator or MessageSeparator or LineFeed)
					{
						Reset();
						break;
					}

					if (data == CarriageReturn)
					{
						var crcStr = _crc.ToString().Trim();
						if (uint.TryParse(crcStr, NumberStyles.AllowHexSpecifier, NumberFormatInfo.InvariantInfo,
							    out var crc))
						{
							_buffer[_bufferIndex++] = (byte)(crc & 0xFF);
							_buffer[_bufferIndex++] = (byte)((crc >> 8) & 0xFF);
							_buffer[_bufferIndex++] = (byte)((crc >> 16) & 0xFF);
							_buffer[_bufferIndex++] = (byte)(crc >> 24);
						}
						else
						{
							Reset();
							break;
						}
						_state = State.CarriageReturn;
					}
					else
					{
						_crc.Append(Encoding.ASCII.GetString(new[] { data }));
					}
					break;
				case State.CarriageReturn:
					if (data != LineFeed)
					{
						Reset();
						break;
					}

					var msgSpan = new ReadOnlySpan<byte>(_buffer, 1, _bufferIndex - 6); /* -(# + * + CRC32) */
					var crc32Index = _bufferIndex - 4 /* CRC32 */;
					var calculatedHash = ComNavCrc32.Calc(msgSpan, _bufferIndex - 6);  /* -(# + * + CRC32) */
					var readedHash = BitConverter.ToUInt32(_buffer, crc32Index);
					if (calculatedHash != readedHash)
					{
						PublishWhenCrcError();
						Reset();
					}
					else
					{
						msgSpan = new ReadOnlySpan<byte>(_buffer, 1, _bufferIndex - 6); /* -(# + * + CRC32) */
						var msg = Encoding.ASCII.GetString(msgSpan.ToArray())
							.Split(Encoding.ASCII.GetChars(new[] { HeaderSeparator }))
							.SelectMany(_ =>
								_.Split(Encoding.ASCII.GetChars(new[] { Separator })))
							.ToArray();
						if (!msg.Any())
						{
							Reset();
							break;
						}

						var msgId = msg[0].ToUpper();
						var span = new ReadOnlySpan<byte>(_buffer, 0, _bufferIndex);
						ParsePacket(msgId, ref span);
						Reset();
						return true;
					}
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			return false;
		}

		public override void Reset()
		{
			_bufferIndex = 0;
			_state = State.Sync;
			_crc.Clear();
		}
	}
}