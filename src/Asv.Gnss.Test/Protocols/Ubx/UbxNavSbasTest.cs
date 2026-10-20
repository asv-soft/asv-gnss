using System;
using System.IO;
using Xunit;

namespace Asv.Gnss;

public class UbxNavSbasTest
{
    [Fact]
    public void Deserialize_RecordedSbas_ShouldInitializeAllSatellitesAndPreserveTwelveByteRecordAlignment()
    {
        var capture = File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "Resources", "Ubx", "pygpsdata-NAV.bin"));

        var message = UbxTestHelper.Deserialize<UbxNavSbas>(capture.AsSpan(2228, 392));

        Assert.Equal(560117000U, message.iTOW);
        Assert.Equal((byte)136, message.Geo);
        Assert.Equal(UbxSbasMode.EnabledIntegrity, message.Mode);
        Assert.Equal(UbxSbasSystem.EGNOS, message.Sys);
        Assert.True(message.IsCorrections);
        Assert.True(message.IsIntegrity);
        Assert.False(message.IsRanging);
        Assert.Equal((byte)31, message.Cnt);
        Assert.Equal(31, message.SvDatas.Length);
        Assert.All(message.SvDatas, satellite => Assert.NotNull(satellite));
        Assert.Equal(392, message.GetByteSize());

        var first = message.SvDatas[0];
        Assert.Equal((byte)243, first.SvId);
        Assert.Equal((byte)8, first.Flags);
        Assert.Equal((byte)14, first.Udre);
        Assert.Equal(byte.MaxValue, first.SvSys);
        Assert.Equal((short)0, first.Prc);
        Assert.Equal((short)274, first.Ic);

        var second = message.SvDatas[1];
        Assert.Equal((byte)5, second.SvId);
        Assert.Equal((byte)139, second.Flags);
        Assert.Equal((byte)16, second.SvSys);
        Assert.True(second.IsRanging);
        Assert.Equal((short)-113, second.Prc);
        Assert.Equal((short)140, second.Ic);

        var middle = message.SvDatas[14];
        Assert.Equal((byte)18, middle.SvId);
        Assert.Equal((short)13, middle.Prc);
        Assert.Equal((short)321, middle.Ic);

        var last = message.SvDatas[^1];
        Assert.Equal((byte)136, last.SvId);
        Assert.Equal((byte)1, last.SvSys);
        Assert.Equal((byte)6, last.SvService);
        Assert.Equal((byte)243, last.Reserved2);
        Assert.True(last.IsCorrections);
        Assert.True(last.IsIntegrity);
        Assert.Equal((short)0, last.Prc);
        Assert.Equal((short)0, last.Ic);
    }
}
