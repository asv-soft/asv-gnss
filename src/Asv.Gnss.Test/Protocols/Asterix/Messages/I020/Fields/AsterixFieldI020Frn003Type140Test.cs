using System;
using Asv.Gnss;
using JetBrains.Annotations;
using Xunit;

namespace Asv.Gnss.Test.Protocols.Messages.I020.Fields;

[TestSubject(typeof(AsterixFieldI020Frn003Type140))]
public class AsterixFieldI020Frn003Type140Test
{
    [Fact]
    public void Should_Serialize_And_Deserialize_TimeOfDay()
    {
        var origin = new AsterixFieldI020Frn003Type140
        {
            Time = TimeOnly.FromTimeSpan(TimeSpan.FromSeconds(33502.7109375))
        };
    
        var buffer = new byte[origin.GetByteSize()];
        var writeSpan = new Span<byte>(buffer);
        origin.Serialize(ref writeSpan);
    
        var readSpan = new ReadOnlySpan<byte>(buffer);
        var deserialized = new AsterixFieldI020Frn003Type140();
        deserialized.Deserialize(ref readSpan);
    
        Assert.Equal(origin.Time, deserialized.Time);
    }

    
}