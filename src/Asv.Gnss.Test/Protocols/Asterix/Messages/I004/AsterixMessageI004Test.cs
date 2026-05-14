using System;
using System.Buffers.Binary;
using System.IO;
using System.Linq;
using Asv.Gnss;
using JetBrains.Annotations;
using Xunit;

namespace Asv.Gnss.Test.Protocols.Asterix.Messages.I004;

[TestSubject(typeof(AsterixMessageI004))]
public class AsterixMessageI004Test
{
    private const string JasterixCat004Ed14FileName = "cat004ed1.4.bin";

    [Fact]
    public void Deserialize_JasterixCat004Ed14Resource_ShouldMatchReferenceValues()
    {
        // Source: https://github.com/OpenATSGmbH/jASTERIX/blob/master/src/test/cat004ed1.4.bin
        // Reference assertions: https://github.com/OpenATSGmbH/jASTERIX/blob/master/src/test/test_cat004_1.4.cpp
        var messages = DeserializeAllMessages(ReadJasterixCat004Ed14Data());

        Assert.Equal(3, messages.Length);

        var alive = Assert.Single(messages[0]);
        Assert.Equal([1, 2, 4, 7], alive.Select(x => x.FieldReferenceNumber).ToArray());
        var aliveSource = Assert.IsType<AsterixFieldI004Frn001Type010>(alive.DataSourceIdentifier);
        Assert.Equal(SystemAreaCode.LocalAirport, aliveSource.Sac);
        Assert.Equal(1, aliveSource.Sic);
        var aliveMessageType = Assert.IsType<AsterixFieldI004Frn002Type000>(alive.MessageType);
        Assert.Equal(1, aliveMessageType.MessageType);
        var aliveTime = Assert.IsType<AsterixFieldI004Frn004Type020>(alive.TimeOfMessage);
        Assert.Equal(2829.0, aliveTime.Seconds);
        var aliveStatus = Assert.IsType<AsterixFieldI004Frn007Type060>(alive.SafetyNetFunctionStatus);
        Assert.False(aliveStatus.Mrva);
        Assert.False(aliveStatus.Ramld);
        Assert.False(aliveStatus.Ramhd);
        Assert.False(aliveStatus.Msaw);
        Assert.True(aliveStatus.Apw);
        Assert.True(aliveStatus.Clam);
        Assert.True(aliveStatus.Stca);

        var clam = Assert.Single(messages[1]);
        Assert.Equal([1, 2, 4, 5, 8], clam.Select(x => x.FieldReferenceNumber).ToArray());
        var clamSource = Assert.IsType<AsterixFieldI004Frn001Type010>(clam.DataSourceIdentifier);
        Assert.Equal(SystemAreaCode.LocalAirport, clamSource.Sac);
        Assert.Equal(1, clamSource.Sic);
        var clamMessageType = Assert.IsType<AsterixFieldI004Frn002Type000>(clam.MessageType);
        Assert.Equal(6, clamMessageType.MessageType);
        var clamTime = Assert.IsType<AsterixFieldI004Frn004Type020>(clam.TimeOfMessage);
        Assert.Equal(70666.0, clamTime.Seconds);
        var clamAlertIdentifier = Assert.IsType<AsterixFieldI004Frn005Type040>(clam.AlertIdentifier);
        Assert.Equal(15496, clamAlertIdentifier.AlertIdentifier);
        var clamTrackNumber1 = Assert.IsType<AsterixFieldI004Frn008Type030>(clam.TrackNumber1);
        Assert.Equal(3027, clamTrackNumber1.TrackNumber);

        var stca = Assert.Single(messages[2]);
        Assert.Equal([1, 2, 4, 5, 6, 8, 10, 16], stca.Select(x => x.FieldReferenceNumber).ToArray());
        var stcaSource = Assert.IsType<AsterixFieldI004Frn001Type010>(stca.DataSourceIdentifier);
        Assert.Equal(SystemAreaCode.LocalAirport, stcaSource.Sac);
        Assert.Equal(1, stcaSource.Sic);
        var stcaMessageType = Assert.IsType<AsterixFieldI004Frn002Type000>(stca.MessageType);
        Assert.Equal(7, stcaMessageType.MessageType);
        var stcaTime = Assert.IsType<AsterixFieldI004Frn004Type020>(stca.TimeOfMessage);
        Assert.Equal(70658.0, stcaTime.Seconds);
        var stcaAlertIdentifier = Assert.IsType<AsterixFieldI004Frn005Type040>(stca.AlertIdentifier);
        Assert.Equal(15497, stcaAlertIdentifier.AlertIdentifier);
        var stcaAlertStatus = Assert.IsType<AsterixFieldI004Frn006Type045>(stca.AlertStatus);
        Assert.Equal(3, stcaAlertStatus.AlertStatus);
        var stcaTrackNumber1 = Assert.IsType<AsterixFieldI004Frn008Type030>(stca.TrackNumber1);
        Assert.Equal(3130, stcaTrackNumber1.TrackNumber);
        var stcaConflictCharacteristics = Assert.IsType<AsterixFieldI004Frn010Type120>(stca.ConflictCharacteristics);
        Assert.Equal([0x00], stcaConflictCharacteristics.Data);
        var stcaTrackNumber2 = Assert.IsType<AsterixFieldI004Frn016Type035>(stca.TrackNumber2);
        Assert.Equal(2417, stcaTrackNumber2.TrackNumber);
    }

    [Fact]
    public void Serialize_JasterixCat004Ed14Resource_ShouldRoundtripBytes()
    {
        var data = ReadJasterixCat004Ed14Data();
        var messages = DeserializeAllMessages(data);
        var serialized = new byte[messages.Sum(x => x.GetByteSize())];
        var writeBuffer = new Span<byte>(serialized);

        foreach (var message in messages)
        {
            message.Serialize(ref writeBuffer);
        }

        Assert.Equal(0, writeBuffer.Length);
        Assert.Equal(data, serialized);
    }

    private static AsterixMessageI004[] DeserializeAllMessages(byte[] data)
    {
        var buffer = new ReadOnlySpan<byte>(data);
        var messages = new AsterixMessageI004[3];
        for (var i = 0; i < messages.Length; i++)
        {
            var length = BinaryPrimitives.ReadUInt16BigEndian(buffer[1..]);
            var block = buffer[..length];
            var message = new AsterixMessageI004();
            message.Deserialize(ref block);
            Assert.Equal(0, block.Length);
            messages[i] = message;
            buffer = buffer[length..];
        }

        Assert.Equal(0, buffer.Length);
        return messages;
    }

    private static byte[] ReadJasterixCat004Ed14Data()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Resources", "Asterix", JasterixCat004Ed14FileName);
        return File.ReadAllBytes(path);
    }
}
