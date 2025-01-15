using System;
using System.Reactive.Linq;
using DeepEqual;
using DeepEqual.Syntax;
using Xunit;

namespace Asv.Gnss.Test
{
    public static class ParserTestHelper
    {
        public static void TestParser<TMessage>(
            IGnssMessageParser parser,
            TMessage message,
            Random r,
            byte syncByteForParser
        )
            where TMessage : IGnssMessageBase
        {
            var arr = new byte[message.GetByteSize()];
            var span = new Span<byte>(arr);
            message.Serialize(ref span);

            var randomBegin = new byte[r.Next(0, 256)];
            r.NextBytes(randomBegin);

            var parsedMessage = default(TMessage);
            parser
                .OnMessage.Where(_ => _.ProtocolId == message.ProtocolId)
                .Cast<TMessage>()
                .Subscribe(_ => parsedMessage = _);

            parser.Reset();
            foreach (var b in randomBegin)
            {
                // it is necessary to check random bytes at the beginning, as it may cause parser synchronization and message skipping
                if (b == syncByteForParser)
                    continue;
                parser.Read(b);
            }

            foreach (var b in arr)
            {
                parser.Read(b);
            }

            Assert.NotNull(parsedMessage);
            message
                .WithDeepEqual(parsedMessage)
                .WithCustomComparison(new FloatComparison(0.5, 0.5f))
                .Assert();
        }
    }
}
