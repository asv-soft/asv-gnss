using System;
using Xunit;
using Xunit.Abstractions;

namespace Asv.Gnss.Test
{
    public class AsvMessageV2Test
    {
        private readonly ITestOutputHelper _output;

        public AsvMessageV2Test(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void Generate_random_bytes_and_mix_with_messages_for_test_parser()
        {
            var seed = new Random().Next();
            var r = new Random(seed);
            _output.WriteLine("RANDOM SEED:{0}", seed);
            var parser = new AsvMessageParser().RegisterDefaultMessages();
            SpanTestHelper.SerializeDeserializeTestBegin(_output.WriteLine);
            foreach (var func in AsvMessageParserFactory.DefaultMessages)
            {
                var message = func();
                message.Randomize(r);
                SpanTestHelper.TestType(message, func, _output.WriteLine);
                ParserTestHelper.TestParser(parser, message, r, AsvMessageParser.Sync1);
            }
        }

        [Fact]
        public void Heartbeat_message_testing()
        {
            var parser = new AsvMessageParser().RegisterDefaultMessages();
            var buffer = new byte[]
            {
                0xAA,
                0x44,
                0x07,
                0x00,
                0x00,
                0x00,
                0x02,
                0x01,
                0x00,
                0x00,
                0x02,
                0x00,
                0x01,
                0x00,
                0x00,
                0x00,
                0x00,
                0xF7,
                0x1B,
            };
            foreach (var b in buffer)
            {
                parser.Read(b);
            }
        }
    }
}
