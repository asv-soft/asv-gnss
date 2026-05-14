using System;
using System.IO;
using System.Linq;
using Asv.Gnss;
using JetBrains.Annotations;
using Xunit;

namespace Asv.Gnss.Test;

[TestSubject(typeof(AsterixMessageI034))]
public class AsterixMessageI034Test
{
    private const string JasterixCat034Ed126FileName = "cat034ed1.26.bin";
    private static readonly byte[] JasterixCat034ExpectedFields = [1, 2, 3, 4, 6, 7];

    [Fact]
    public void Deserialize_JasterixCat034Ed126Resource_ShouldMatchReferenceValues()
    {
        // Source: https://github.com/OpenATSGmbH/jASTERIX/blob/master/src/test/cat034ed1.26.bin
        // Reference assertions: https://github.com/OpenATSGmbH/jASTERIX/blob/master/src/test/test_cat034_1.26.cpp
        var data = ReadJasterixCat034Ed126Data();

        Assert.Equal(20, data.Length);

        var message = new AsterixMessageI034();
        var buffer = new ReadOnlySpan<byte>(data);
        message.Deserialize(ref buffer);

        Assert.Equal(0, buffer.Length);

        var record = Assert.Single(message);
        Assert.Equal(JasterixCat034ExpectedFields, record.Select(x => x.FieldReferenceNumber).ToArray());

        var source = Assert.IsType<AsterixFieldI034Frn001Type010>(record.DataSourceIdentifier);
        Assert.Equal(SystemAreaCode.LocalAirport, source.Sac);
        Assert.Equal(2, source.Sic);

        var messageType = Assert.IsType<AsterixFieldI034Frn002Type000>(record.MessageType);
        Assert.Equal(2, messageType.MessageType);

        var timeOfDay = Assert.IsType<AsterixFieldI034Frn003Type030>(record.TimeOfDay);
        Assert.Equal(33499.84375, timeOfDay.Seconds, 10);

        var sector = Assert.IsType<AsterixFieldI034Frn004Type020>(record.SectorNumber);
        Assert.Equal(90.0, sector.SectorNumberDeg, 10);

        var configuration = Assert.IsType<AsterixFieldI034Frn006Type050>(record.SystemConfigurationStatus);
        Assert.Equal(0x94, configuration.Selector.Raw);
        Assert.True(configuration.Selector.HasCommonPart);
        Assert.True(configuration.Selector.HasPsrPart);
        Assert.True(configuration.Selector.HasModeSPart);
        Assert.False(configuration.Selector.HasExtension);

        var configurationCommon = Assert.IsType<AsterixI034SystemConfigurationCommon>(configuration.Common);
        Assert.False(configurationCommon.Nogo);
        Assert.True(configurationCommon.Rdpc);
        Assert.False(configurationCommon.Rdpr);
        Assert.False(configurationCommon.OverloadRdp);
        Assert.False(configurationCommon.OverloadTransmission);
        Assert.False(configurationCommon.MonitoringSystemConnected);
        Assert.False(configurationCommon.TimeSourceValid);

        var configurationPsr = Assert.IsType<AsterixI034SystemConfigurationPsr>(configuration.Psr);
        Assert.False(configurationPsr.Antenna);
        Assert.Equal(3, configurationPsr.ChannelAB);
        Assert.False(configurationPsr.Overload);
        Assert.False(configurationPsr.MonitoringSystemConnected);

        var configurationModeS = Assert.IsType<AsterixI034SystemConfigurationModeS>(configuration.ModeS);
        Assert.False(configurationModeS.Antenna);
        Assert.Equal(2, configurationModeS.ChannelAB);
        Assert.False(configurationModeS.OverloadSurveillance);
        Assert.False(configurationModeS.MonitoringSystemConnected);
        Assert.True(configurationModeS.SurveillanceCoordinationFunction);
        Assert.False(configurationModeS.DataLinkFunction);
        Assert.False(configurationModeS.OverloadSurveillanceCoordinationFunction);
        Assert.False(configurationModeS.OverloadDataLinkFunction);

        var processing = Assert.IsType<AsterixFieldI034Frn007Type060>(record.SystemProcessingMode);
        Assert.Equal(0x94, processing.Selector.Raw);
        Assert.True(processing.Selector.HasCommonPart);
        Assert.True(processing.Selector.HasPsrPart);
        Assert.True(processing.Selector.HasModeSPart);
        Assert.False(processing.Selector.HasExtension);

        var processingCommon = Assert.IsType<AsterixI034SystemProcessingCommon>(processing.Common);
        Assert.False(processingCommon.ReducedRdp);
        Assert.False(processingCommon.ReducedTransmission);

        var processingPsr = Assert.IsType<AsterixI034SystemProcessingPsr>(processing.Psr);
        Assert.False(processingPsr.Polarization);
        Assert.False(processingPsr.ReducedRadar);
        Assert.False(processingPsr.SensitivityTimeControl);

        var processingModeS = Assert.IsType<AsterixI034SystemProcessingModeS>(processing.ModeS);
        Assert.False(processingModeS.ReducedRadar);
        Assert.True(processingModeS.ClusterState);
    }

    [Fact]
    public void Serialize_JasterixCat034Ed126Resource_ShouldRoundtripBytes()
    {
        var data = ReadJasterixCat034Ed126Data();
        var message = new AsterixMessageI034();
        var readBuffer = new ReadOnlySpan<byte>(data);
        message.Deserialize(ref readBuffer);

        var serialized = new byte[message.GetByteSize()];
        var writeBuffer = new Span<byte>(serialized);
        message.Serialize(ref writeBuffer);

        Assert.Equal(0, writeBuffer.Length);
        Assert.Equal(data, serialized);
    }

    private static byte[] ReadJasterixCat034Ed126Data()
    {
        return File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "Resources", "Asterix", JasterixCat034Ed126FileName));
    }
}
