﻿using System;
using Newtonsoft.Json;

namespace Asv.Gnss
{
    public abstract class GlonassWordBase
    {
        private byte _wordId;

        public virtual byte WordId
        {
            get => _wordId;
            protected set => _wordId = value;
        }

        public virtual void Deserialize(byte[] data)
        {
            if (data.Length != 11) throw new Exception($"Length of {nameof(data)} array must be 96 bit = 12 bytes  (as Glonass ICD word length )");
            
            var wordId = (byte)GpsRawHelper.GetBitU(data, 4, 4);
            if (wordId > 5) WordId = wordId;
            CheckWordId(wordId);
        }

        protected virtual void CheckWordId(byte wordId)
        {
            if (wordId != WordId) throw new Exception($"Word ID not equals: want {WordId}. Got {wordId}");
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}