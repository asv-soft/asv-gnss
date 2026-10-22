using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Asv.IO;
using R3;
using Xunit;

namespace Asv.Gnss.Test;

public class RinexValidationTest
{
    [Theory]
    [InlineData('X')]
    [InlineData('O')]
    [InlineData('M')]
    public void HeaderWrite_UnsupportedOrMissingDescriptors_RejectsUnreadableHeader(char fileType)
    {
        var message = new RinexHeaderMessage { Header = new RinexHeader { FileType = fileType } };
        if (fileType == 'X') Assert.Throws<NotSupportedException>(() => message.GetByteSize());
        else Assert.Throws<FormatException>(() => message.GetByteSize());
    }

    [Fact]
    public async Task FileWrite_IncompatibleBodyContext_IsRejectedSynchronouslyAndAsynchronously()
    {
        var records = RinexFile.Read(Path.Combine(AppContext.BaseDirectory, "Resources", "Rinex", "demo.10n"));
        var body = Assert.IsType<RinexNavigationMessage>(records[1]);
        body.Header = new RinexHeader { Version = 3.04m, FileType = 'N' };
        using var output = new MemoryStream();
        Assert.Throws<ArgumentException>(() => RinexFile.Write(output, records));
        await Assert.ThrowsAsync<ArgumentException>(() => RinexFile.WriteAsync(output, records, TestContext.Current.CancellationToken));
    }

    [Fact]
    public void Push_MalformedNav4Record_KeepsNextDescriptorAndRecoversEveryLaterRecord()
    {
        var text = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Resources", "Rinex", "rinex402_examples_MN.rnx"));
        var lines = text.Split('\n');
        var descriptor = Array.FindIndex(lines, line => line.StartsWith("> EPH", StringComparison.Ordinal));
        Assert.True(descriptor >= 0);
        lines[descriptor + 1] = lines[descriptor + 1][..23] + "NOT_A_NUMBER" + lines[descriptor + 1][35..];
        using var parser = (RinexMessageParser)Protocol.Create(builder => builder.Protocols.RegisterRinexProtocol())
            .CreateParser(RinexProtocol.Info.Id);
        var messages = new List<IProtocolMessage>();
        var errors = new List<Exception>();
        using var subscription = parser.OnMessage.Subscribe(messages.Add);
        using var errorSubscription = parser.OnError.Subscribe(errors.Add, errors.Add, _ => { });
        foreach (var value in Encoding.ASCII.GetBytes(string.Join("\n", lines))) parser.Push(value);
        parser.Complete();
        var navigation = messages.OfType<RinexNavigationMessage>().ToArray();
        Assert.Equal(33, navigation.Length);
        Assert.Equal(8, navigation.Count(message => message.RecordType == "EPH"));
        Assert.Equal(13, navigation.Count(message => message.RecordType == "STO"));
        Assert.Equal(5, navigation.Count(message => message.RecordType == "EOP"));
        Assert.Equal(7, navigation.Count(message => message.RecordType == "ION"));
        Assert.NotEmpty(errors);
    }
}
