using Asv.Gnss;
using JetBrains.Annotations;
using Xunit;

namespace Asv.Gnss.Test.Protocols.Messages.I020;

[TestSubject(typeof(AsterixMessageI020))]
public class AsterixMessageI020Test
{

    [Fact]
    public void METHOD()
    {
        var message = new AsterixMessageI020
        {
            new AsterixRecordI020
            {
                DataSourceIdentifier = new AsterixFieldI020_010(),
            }
        };
    }
}