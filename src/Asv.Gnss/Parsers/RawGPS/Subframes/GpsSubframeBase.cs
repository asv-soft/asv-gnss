using System;
using Newtonsoft.Json;

namespace Asv.Gnss
{
    public abstract class GpsSubframeBase
    {
        public uint TOW1_5Epoh { get; set; }
        public abstract byte SubframeId { get; }

        public virtual void Deserialize(byte[] dataWithoutParity)
        {
            if (dataWithoutParity.Length != 30) throw new Exception($"Length of {nameof(dataWithoutParity)} array must be 24 bit x 10 word = 30 bytes  (as GPS ICD subframe length )");
            if (dataWithoutParity[0] != GpsRawHelper.GpsSubframePreamble) throw new Exception($"Preamble error. Want {GpsRawHelper.GpsSubframePreamble}. Got {dataWithoutParity[0]}");
            TOW1_5Epoh = GpsRawHelper.GetBitU(dataWithoutParity, 24, 17); // 2-nd word 1-17 bit
            var subframeId = GpsRawHelper.GetSubframeId((byte)GpsRawHelper.GetBitU(dataWithoutParity, 24 + 19, 3));
            if (subframeId != SubframeId) throw new Exception($"Subframe ID not equals: want {SubframeId}. Got {subframeId}");
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}