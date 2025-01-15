using System;
using System.Text;
using Asv.IO;

namespace Asv.Gnss
{
    public abstract class ComNavSimpleAnswerMessageBase : GnssMessageBase<string>
    {
        public override string ProtocolId => ComNavSimpleAnswerParser.GnssProtocolId;

        private const char Separator = ' ';
        private const byte CarriageReturn = 0xD;
        private const byte LineFeed = 0xA;

        public override void Deserialize(ref ReadOnlySpan<byte> buffer)
        {
#if NETFRAMEWORK
            var message = Encoding.ASCII.GetString(buffer.ToArray()).Split(Separator);
#else
            var message = Encoding.ASCII.GetString(buffer).Split(Separator);
#endif
            var msgIdLength = MessageId.Split(Separator).Length;
            var msgId = string.Join(Separator.ToString(), message, 0, msgIdLength);

            if (!string.Equals(MessageId, msgId, StringComparison.InvariantCultureIgnoreCase))
                throw new GnssParserException(
                    ProtocolId,
                    $"Error to deserialize {ProtocolId} packet: message id not equal (want [{MessageId}] got [{msgId}])"
                );

            var bodyBuilder = new StringBuilder();

            for (var i = msgIdLength; i < message.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(message[i]))
                    bodyBuilder.Append($"{message[i]} ");
            }

            Body = bodyBuilder.ToString().Trim();
            buffer = buffer.Slice(buffer.Length);
        }

        public string Body { get; set; }

        public override void Serialize(ref Span<byte> buffer)
        {
            MessageId.CopyTo(ref buffer, Encoding.ASCII);
            BinSerialize.WriteByte(ref buffer, CarriageReturn);
            BinSerialize.WriteByte(ref buffer, LineFeed);
            Body.CopyTo(ref buffer, Encoding.ASCII);
            BinSerialize.WriteByte(ref buffer, CarriageReturn);
            BinSerialize.WriteByte(ref buffer, LineFeed);
        }

        public override int GetByteSize()
        {
            return MessageId.Length + 2 + Body.Length + 2;
        }
    }
}
