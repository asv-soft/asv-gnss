using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Asv.Gnss
{
    public class ComNavSimpleAnswerParser : GnssMessageParserBase<ComNavSimpleAnswerMessageBase, string>
	{
		public const string GnssProtocolId = "ComNavSimpleAnswer";
		public override string ProtocolId => GnssProtocolId;

		private readonly AnswerState[] _states = {
			new(ComNavSimpleOkMessage.ComNavMessageId),
			new(ComNavSimpleErrorMessage.ComNavMessageId),
			new(ComNavSimpleTransmitMessage.ComNavMessageId)
		};
		public override bool Read(byte data)
		{
			foreach (var state in _states)
			{
				var s = state.Read(data);
				if (s != State.Finished) continue;
				
				var span = state.GetMessage();
				ParsePacket(state.MessageId, ref span);
				Reset();
				return true;
			}
			return false;
		}

		public override void Reset()
		{
			foreach (var state in _states)
			{
				state.Reset();
			}
		}

		private enum State
		{
			WaitingNext,
			Finished,
			Error
		}

		private class AnswerState
		{
			private enum AnswerStateEnum
			{
				Header,
				PossibleCarriageReturn,
				PossibleLineFeed,
				Body,
				CarriageReturn
			}

			private readonly byte[][] _header = new byte[2][];
			private readonly byte[] _buffer = new byte[MaxBufferSize];
			private int _bufferIndex = 0;
			private const int MaxBufferSize = 1024;

			private AnswerStateEnum _state = AnswerStateEnum.Header;
			private const byte CarriageReturn = 0xD;
			private const byte LineFeed = 0xA;
			private readonly int _headerLength = 0;

			public AnswerState(string header)
			{
				MessageId = header;
				_header[0] = Encoding.ASCII.GetBytes(header.Trim().ToLower(CultureInfo.InvariantCulture));
				_header[1] = Encoding.ASCII.GetBytes(header.Trim().ToUpper(CultureInfo.InvariantCulture));
				if (_header[0].Length == _header[1].Length) _headerLength = _header[0].Length;
				else throw new Exception($"Protocol {GnssProtocolId}. Header upper case length and header lower case length not equal!");
			}

			public State Read(byte data)
			{
				switch (_state)
				{
					case AnswerStateEnum.Header:
						if (data != _header[0][_bufferIndex] && data != _header[1][_bufferIndex])
						{
							_bufferIndex = 0;
							return State.Error;
						}

						_buffer[_bufferIndex] = data;
						_bufferIndex += 1;
						
						if (_bufferIndex == _headerLength)
							_state = AnswerStateEnum.PossibleCarriageReturn;
						
						return State.WaitingNext;
					case AnswerStateEnum.PossibleCarriageReturn:
						switch (data)
						{
							case CarriageReturn:
								_state = AnswerStateEnum.PossibleLineFeed;
								_buffer[_bufferIndex++] = 0x20;
								return State.WaitingNext;
							case >= 0x20 and <= 0x7E:
								_buffer[_bufferIndex++] = data;
								_state = AnswerStateEnum.Body;
								return State.WaitingNext;
							default:
								Reset();
								return State.Error;
						}

					case AnswerStateEnum.PossibleLineFeed:
						if (data == LineFeed)
						{
							_state = AnswerStateEnum.Body;
							return State.WaitingNext;
						}
						Reset();
						return State.Error;
					case AnswerStateEnum.Body:
						switch (data)
						{
							case >= 0x20 and <= 0x7E:
							{
								_buffer[_bufferIndex++] = data;
								if (_bufferIndex < MaxBufferSize - 2) return State.WaitingNext;
								Reset();
								return State.Error;
							}
							case CarriageReturn:
								_state = AnswerStateEnum.CarriageReturn;
								return State.WaitingNext;
							default:
								Reset();
								return State.Error;
						}
					case AnswerStateEnum.CarriageReturn:

						if (data == LineFeed)
							return State.Finished;
						Reset();
						return State.Error;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			public ReadOnlySpan<byte> GetMessage()
			{
				return new ReadOnlySpan<byte>(_buffer, 0, _bufferIndex);
			}

			public string MessageId { get; }

			public void Reset()
			{
				_bufferIndex = 0;
				_state = AnswerStateEnum.Header;
			}
		}
	}
}