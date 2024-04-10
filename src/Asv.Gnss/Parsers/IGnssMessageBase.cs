using Asv.IO;

namespace Asv.Gnss
{
    /// <summary>
    /// Represents a base interface for GNSS messages.
    /// </summary>
    public interface IGnssMessageBase: ISizedSpanSerializable
    {
        /// <summary>
        /// Gets or sets the custom use property (like routing, etc...)
        /// This field is not serialized or deserialized.
        /// </summary>
        /// <value>
        /// The custom use property.
        /// </value>
        object Tag { get; set; }

        /// <summary>
        /// Gets the unique identifier of the protocol.
        /// </summary>
        /// <remarks>
        /// The ProtocolId property is a string that represents the unique identifier
        /// assigned to the protocol.
        /// </remarks>
        string ProtocolId { get; }

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        /// <value>
        /// The name of the property.
        /// </value>
        string Name { get; }

        /// <summary>
        /// Gets the string ID of the message.
        /// </summary>
        /// <remarks>
        /// This property is used to identify the message string. It represents the unique identifier of the message in the system.
        /// </remarks>
        /// <value>
        /// A <see cref="string"/> representing the string ID of the message.
        /// </value>
        string MessageStringId { get; }
    }
}