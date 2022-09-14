using Asv.IO;

namespace Asv.Gnss
{
    public interface IGnssMessageBase: ISizedSpanSerializable
    {
        /// <summary>
        /// This is for custom use (like routing, etc...)
        /// This field not serialize\deserialize
        /// </summary>
        object Tag { get; set; }
        string ProtocolId { get; }
        string Name { get; }
        string MessageStringId { get; }
    }
}