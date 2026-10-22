using System;
using System.Linq;
using Asv.IO;
using Xunit;

namespace Asv.Gnss;

public class ProtocolVisitorCoverageTest
{
    [Fact]
    public void EveryImplementedProtocolMessage_ImplementsVisitable()
    {
        var messages = typeof(NmeaMessageGga).Assembly.GetTypes()
            .Where(type => !type.IsAbstract && !type.ContainsGenericParameters && typeof(IProtocolMessage).IsAssignableFrom(type)).ToArray();
        Assert.NotEmpty(messages);
        Assert.Empty(messages.Where(type => !typeof(IVisitable).IsAssignableFrom(type)).Select(type => type.FullName));
    }
}
