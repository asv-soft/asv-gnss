using System;
using Asv.Gnss;
using JetBrains.Annotations;
using Xunit;

namespace Asv.Gnss.Test.Protocols.Messages.I020.Fields;

[TestSubject(typeof(AsterixFieldI020Frn002Type020))]
public class AsterixFieldI020Frn002Type020Test
{

    [Fact]
    public void Should_Initialize_All_Boolean_Properties()
    {
        var field = new AsterixFieldI020Frn002Type020();
    
        // Test all boolean properties
        field.Ot = true;
        Assert.True(field.Ot);
    
        field.Dme = true;
        Assert.True(field.Dme);
    
        field.Uat = true;
        Assert.True(field.Uat);
    
        field.Vdl4 = true;
        Assert.True(field.Vdl4);
    
        field.Hf = true;
        Assert.True(field.Hf);
    
        field.Ms = true;
        Assert.True(field.Ms);
    
        field.Ssr = true;
        Assert.True(field.Ssr);
    
        // Test nullable boolean properties
        field.Tst = true;
        Assert.True(field.Tst);
    
        field.Sim = true;
        Assert.True(field.Sim);
    
        field.Crt = true;
        Assert.True(field.Crt);
    
        field.Gbs = true;
        Assert.True(field.Gbs);
    
        field.Chn = true;
        Assert.True(field.Chn);
    
        
    }
    [Fact]
    public void Should_Serialize_And_Deserialize()
    {
        // Arrange
        var origin = new AsterixFieldI020Frn002Type020
        {
            Ot = false,
            Dme = false,
            Uat = false,
            Vdl4 = false,
            Hf = false,
            Ms = true,
            Ssr = false,
            Tst = false,
            Sim = false,
            Crt = false,
            Gbs = false,
            Chn = false,
            Spi = false,
            Rab = false
        };

        // Act
        var buffer = new byte[4]; // 4 bytes needed for all the flags
        var writeSpan = new Span<byte>(buffer);
        origin.Serialize(ref writeSpan);
    
        var readSpan = new ReadOnlySpan<byte>(buffer);
        var deserialized = new AsterixFieldI020Frn002Type020();
        deserialized.Deserialize(ref readSpan);

        // Assert
        Assert.Equal(origin.Ot, deserialized.Ot);
        Assert.Equal(origin.Dme, deserialized.Dme);
        Assert.Equal(origin.Uat, deserialized.Uat);
        Assert.Equal(origin.Vdl4, deserialized.Vdl4);
        Assert.Equal(origin.Hf, deserialized.Hf);
        Assert.Equal(origin.Ms, deserialized.Ms);
        Assert.Equal(origin.Ssr, deserialized.Ssr);
        Assert.Equal(origin.Tst, deserialized.Tst);
        Assert.Equal(origin.Sim, deserialized.Sim);
        Assert.Equal(origin.Crt, deserialized.Crt);
        Assert.Equal(origin.Gbs, deserialized.Gbs);
        Assert.Equal(origin.Chn, deserialized.Chn);
        Assert.Equal(origin.Spi, deserialized.Spi);
        Assert.Equal(origin.Rab, deserialized.Rab);
    }
}