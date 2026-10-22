using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Asv.Gnss.Test;

public sealed class RinexInspectorTest
{
    [Theory]
    [InlineData("demo.10n", 2, "G06", -0.000839701388031)]
    [InlineData("rinex402_examples_MN.rnx", 34, "G04", 1.330170780420e-4)]
    public async Task ReadAsync_PublicRinexNavigationFile_InspectPublishesHeaderAndFinalRecord(
        string fileName, int count, string satellite, double clockBias)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Resources", "Rinex", fileName);
        var capture = await ProtocolFileReader.ReadAsync(path, TestContext.Current.CancellationToken);
        var messages = capture.Messages.Select(item => item.Message).OfType<RinexMessageBase>().ToArray();
        Assert.Equal(new FileInfo(path).Length, capture.ByteCount);
        Assert.Contains(RinexProtocol.Info.Id, capture.Protocols);
        Assert.IsType<RinexHeaderMessage>(messages[0]);
        var navigation = messages.OfType<RinexNavigationMessage>().ToArray();
        Assert.Equal(count, navigation.Length);
        Assert.Equal(satellite, navigation[0].Satellite);
        Assert.Equal(clockBias, navigation[0].Values[0]!.Value, 15);
    }
}
