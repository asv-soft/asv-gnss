using Asv.IO;
using JetBrains.Annotations;
using Xunit;

namespace Asv.Gnss.Test;

[TestSubject(typeof(NmeaMessageParser))]
public class NmeaMessageParserTest
{

    [Fact]
    public void METHOD()
    {
        var protocol = Protocol.Create(builder =>
        {
            builder.Protocols.RegisterNmeaProtocol();
        });
        
    }
}