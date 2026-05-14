using System;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Asv.Gnss
{
    public class ComNavLogListAMessage : ComNavAsciiMessageBase
	{
		public const string ComNavMessageId = "LOGLISTA";
		public override string MessageId => ComNavMessageId;
		public override string Name => ComNavMessageId;
		protected override void InternalContentDeserialize(string[] msg)
		{
			var msgCnt = msg.Length / 6;
			Messages = new LogMessage[msgCnt];

			for (var i = 0; i < msgCnt; i++)
			{
				var logMsg = new LogMessage();

#if NETFRAMEWORK
				logMsg.Direction = Enum.TryParse(msg[0 + i * 6], true, out ComNavPortEnum dir) ? dir : ComNavPortEnum.NO_PORTS;
				logMsg.Message = Enum.TryParse(msg[1 + i * 6], true, out ComNavMessageEnum msgType) ? msgType : ComNavMessageEnum.UNKNOWN;
				logMsg.Format = Enum.TryParse(msg[2 + i * 6], true, out ComNavFormat format) ? format : ComNavFormat.None;
				logMsg.Trigger = Enum.TryParse(msg[3 + i * 6], true, out ComNavTriggerEnum trigger) ? trigger : ComNavTriggerEnum.NONE;
#else
				if (Enum.TryParse(typeof(ComNavPortEnum), msg[0 + i * 6], true, out var dir))
					 logMsg.Direction = (ComNavPortEnum)dir;
				else logMsg.Direction = ComNavPortEnum.NO_PORTS;

				if (Enum.TryParse(typeof(ComNavMessageEnum), msg[1 + i * 6], true, out var msgType))
					logMsg.Message = (ComNavMessageEnum)msgType;
				else logMsg.Message = ComNavMessageEnum.UNKNOWN;

				if (Enum.TryParse(typeof(ComNavFormat), msg[2 + i * 6], true, out var format))
					logMsg.Format = (ComNavFormat)format;
				else logMsg.Format = ComNavFormat.None;

				if (Enum.TryParse(typeof(ComNavTriggerEnum), msg[3 + i * 6], true, out var trigger))
					logMsg.Trigger = (ComNavTriggerEnum)trigger;
				else logMsg.Trigger = ComNavTriggerEnum.NONE;
#endif

				logMsg.Period =
					!double.TryParse(msg[4 + i * 6], NumberStyles.Any, NumberFormatInfo.InvariantInfo, out var period)
						? 0.0
						: period;

				logMsg.Offset =
					!double.TryParse(msg[5 + i * 6], NumberStyles.Any, NumberFormatInfo.InvariantInfo, out var offset)
						? 0.0
						: offset;

				Messages[i] = logMsg;

			}
		}

		public LogMessage[] Messages { get; private set; }
	}

	public class LogMessage
	{
		[JsonConverter(typeof(StringEnumConverter))]
		public ComNavPortEnum Direction { get; set; }

		[JsonConverter(typeof(StringEnumConverter))]
		public ComNavMessageEnum Message { get; set; }

		[JsonConverter(typeof(StringEnumConverter))]
		public ComNavFormat Format { get; set; }

		[JsonConverter(typeof(StringEnumConverter))]
		public ComNavTriggerEnum Trigger { get; set; }
		public double Period { get; set; }
		public double Offset { get; set; }
	}
}