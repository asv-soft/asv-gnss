using Asv.Gnss;
using JetBrains.Annotations;
using Xunit;

namespace Asv.Gnss.Tests.Protocols.Nmea.Messages;

[TestSubject(typeof(NmeaMessageGBS))]
public class NmeaMessageGBSTest
{

    [Fact]
    public void METHOD()
    {
        var data = "$GPGBS,015509.00,-0.031,-0.186,0.219,19,0.000,-0.354,6.972*4D"u8;
        var msg = new NmeaMessageGBS();
        msg.Deserialize(ref data);
    }
}