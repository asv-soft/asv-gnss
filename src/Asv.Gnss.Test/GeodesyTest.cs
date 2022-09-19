using System.Collections.Generic;
using Geodesy;
using Newtonsoft.Json;
using Xunit;

namespace Asv.Gnss.Test
{
    public class GeodesyTest
    {
        [Fact]
        public void GlobalPositionConverterTest()
        {
            for (int i = 0; i < 1000; i++)
            {
                var a = new GlobalPosition(new GlobalCoordinates(-90, 180), 1000);
                var src = JsonConvert.SerializeObject(a, GlobalPositionConverter.Default);
                var obj = JsonConvert.DeserializeObject<GlobalPosition>(src,
                    new JsonSerializerSettings
                        { Converters = new List<JsonConverter> { GlobalPositionConverter.Default } });
                Assert.Equal(a, obj);
            }
        }
    }
}