using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Asv.IO;
using R3;
using Xunit;

namespace Asv.Gnss;

public class JavadPortTest
{
    [Fact]
    public void Receive_FormatSurvivesDispatcherResetsAndInterleavedNmea()
    {
        var protocol = Protocol.Create(builder =>
        {
            builder.Protocols.RegisterJavadProtocol();
            builder.Protocols.RegisterNmeaProtocol();
        });
        using var port = new VirtualPort(
            "greis-mixed",
            (IProtocolContext)protocol,
            static _ => true,
            new Statistic()
        );
        var messages = new List<IProtocolMessage>();
        var errors = new List<Exception>();
        using var rx = port.OnRxMessage.Subscribe(messages.Add);
        using var rxError = port.OnRxError.Subscribe(errors.Add);
        var parser = port.Parsers.OfType<JavadMessageParser>().Single();
        using var parserError = parser.OnError.Subscribe(_ => { }, errors.Add, _ => { });
        var format = Convert.FromHexString("4D463030394A5030313030313842");
        var time = Convert.FromHexString("7E7E30303501020304FD");
        var nmea =
            "$GPGGA,123519,4807.038,N,01131.000,E,1,08,0.9,545.4,M,46.9,M,,*47\r\n"u8.ToArray();

        // The production dispatcher calls Reset on every parser after any complete message.
        foreach (var value in format.Concat(time).Concat(nmea).Concat(time))
            port.Rx.OnNext([value]);

        Assert.Empty(errors);
        Assert.Equal(4, messages.Count);
        Assert.IsType<JavadMessagesFormatMessage>(messages[0]);
        Assert.Equal(0x01020304U, Assert.IsType<JavadReceiverTimeMessage>(messages[1]).TimeOfDay);
        Assert.Equal(48.1173, Assert.IsType<NmeaMessageGga>(messages[2]).Latitude, 7);
        Assert.Equal(0x01020304U, Assert.IsType<JavadReceiverTimeMessage>(messages[3]).TimeOfDay);
        Assert.Equal(JavadByteOrder.BigEndian, parser.ByteOrder);
    }

    [Fact]
    public async Task Send_CommandAndReceiveLabelledReply_UseExistingPortContracts()
    {
        var protocol = Protocol.Create(builder => builder.Protocols.RegisterJavadProtocol());
        using var port = new VirtualPort(
            "greis-command",
            (IProtocolContext)protocol,
            static _ => true,
            new Statistic()
        );
        var wire = new List<byte[]>();
        var messages = new List<IProtocolMessage>();
        var errors = new List<Exception>();
        using var tx = port.Tx.Subscribe(wire.Add);
        using var rx = port.OnRxMessage.Subscribe(messages.Add);
        using var txError = port.OnTxError.Subscribe(errors.Add);
        using var rxError = port.OnRxError.Subscribe(errors.Add);
        var command = JavadCommand.Print(new JavadObjectPath("/par/rcv/id")).WithLabel("id");

        await port.Send(command, TestContext.Current.CancellationToken);
        port.Rx.OnNext("RE00A%id%ABC123"u8.ToArray());

        Assert.Empty(errors);
        Assert.Equal("%id%print,/par/rcv/id\r\n", Encoding.ASCII.GetString(Assert.Single(wire)));
        var reply = Assert.IsType<JavadReplyMessage>(Assert.Single(messages));
        Assert.True(reply.Response.Matches(command));
        Assert.Equal("ABC123", reply.Response.Text);
    }
}
