using System;
using System.Reactive.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Asv.Gnss.Test
{
    public class UbxTest
    {
        private readonly ITestOutputHelper _output;

        public UbxTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void MessagesAndParserTest()
        {
            var seed = new Random().Next();
            var r = new Random(seed);
            _output.WriteLine("RANDOM SEED:{0}", seed);
            var parser = new UbxBinaryParser().RegisterDefaultMessages();
            SpanTestHelper.SerializeDeserializeTestBegin(_output.WriteLine);
            foreach (var func in UbxFactory.DefaultMessages)
            {
                var message = func();
                message.Randomize(r);
                SpanTestHelper.TestType(message, func, _output.WriteLine);
                if (message.GetType().GetCustomAttributes(typeof(SerializationNotSupportedAttribute), true).Length == 0)
                {
                    ParserTestHelper.TestParser(parser, message, r);
                }
            }
            // this is test for POOL messages (only send to receiver) 
            foreach (var func in UbxFactory.DefaultPoolMessages)
            {
                var message = func();
                message.Randomize(r);
                SpanTestHelper.TestType(message, func, _output.WriteLine);
            }
        }


        
    }
}