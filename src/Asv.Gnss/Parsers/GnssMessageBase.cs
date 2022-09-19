using System;
using System.Diagnostics;
using Geodesy;
using Newtonsoft.Json;

namespace Asv.Gnss
{
    


    public abstract class GnssMessageBase<TMsgId> : IGnssMessageBase
    {

        protected GnssMessageBase()
        {
#if DEBUG
            // ReSharper disable once VirtualMemberCallInConstructor
            Debug.Assert(Name != null, nameof(Name) + " != null");
            // ReSharper disable once VirtualMemberCallInConstructor
            Debug.Assert(MessageId != null, nameof(MessageId) + " != null");
            // ReSharper disable once VirtualMemberCallInConstructor
            // ReSharper disable once VirtualMemberCallInConstructor
#endif
        }

        /// <summary>
        /// This is for custom use (like routing, etc...)
        /// This field not serialize\deserialize
        /// </summary>
        [JsonIgnore]
        public object Tag { get; set; }

        public abstract string ProtocolId { get; }
        public abstract TMsgId MessageId { get; }
        public abstract string Name { get; }
        public string MessageStringId => MessageId.ToString();

        public abstract void Deserialize(ref ReadOnlySpan<byte> buffer);
        public abstract void Serialize(ref Span<byte> buffer);
        public abstract int GetByteSize();

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.None, GlobalPositionConverter.Default, GlobalPositionNullableConverter.Default);
        }
    }
}