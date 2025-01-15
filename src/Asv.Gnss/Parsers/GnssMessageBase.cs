using System;
using System.Diagnostics;
using Geodesy;
using Newtonsoft.Json;

namespace Asv.Gnss
{
    /// <summary>
    /// Base class for GNSS messages.
    /// </summary>
    /// <typeparam name="TMsgId">The type of the message ID.</typeparam>
    public abstract class GnssMessageBase<TMsgId> : IGnssMessageBase
    {
        /// <summary>
        /// Base class for GNSS message types.
        /// </summary>
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
        /// Gets or sets the custom tag for this object.
        /// </summary>
        /// <remarks>
        /// This tag is meant for custom use, such as for routing purposes.
        /// It is not serialized or deserialized when using JSON.
        /// </remarks>
        [JsonIgnore]
        public object Tag { get; set; }

        /// <summary>
        /// Gets the protocol ID associated with the object.
        /// </summary>
        /// <remarks>
        /// This property represents the unique identifier of the protocol.
        /// </remarks>
        /// <returns>The protocol ID.</returns>
        public abstract string ProtocolId { get; }

        /// <summary>
        /// Gets the unique identifier of the message.
        /// </summary>
        /// <typeparam name="TMsgId">The type of the message identifier.</typeparam>
        /// <returns>The unique identifier of the message.</returns>
        public abstract TMsgId MessageId { get; }

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        /// <returns>
        /// The name of the property as a string.
        /// </returns>
        public abstract string Name { get; }

        /// <summary>
        /// Gets the string representation of the message ID.
        /// </summary>
        /// <remarks>
        /// This property returns the string representation of the <see cref="MessageId"/> property.
        /// </remarks>
        /// <value>
        /// A string representing the message ID.
        /// </value>
        public string MessageStringId => MessageId.ToString();

        /// <summary>
        /// Deserializes the provided buffer and populates the object.
        /// </summary>
        /// <param name="buffer">The buffer to deserialize.</param>
        /// <remarks>
        /// The Deserialize method is used to deserialize the buffer and populate the object with its values.
        /// The reference to the buffer is passed by reference to allow the method to modify the buffer
        /// and update the object accordingly.
        /// </remarks>
        public abstract void Deserialize(ref ReadOnlySpan<byte> buffer);

        /// <summary>
        /// Serializes the object into a byte buffer. </summary> <param name="buffer">The memory span to store the serialized data.</param> <remarks>
        /// This method is implemented by derived classes to serialize the object into a byte buffer.
        /// The serialized data is stored in the provided memory span. </remarks>
        /// /
        public abstract void Serialize(ref Span<byte> buffer);

        /// <summary>
        /// Gets the byte size of the object.
        /// </summary>
        /// <returns>The byte size of the object.</returns>
        public abstract int GetByteSize();

        /// <summary>
        /// Returns a string representation of the object.
        /// </summary>
        /// <returns>A JSON string representing the current object.</returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(
                this,
                Formatting.None,
                GlobalPositionConverter.Default,
                GlobalPositionNullableConverter.Default
            );
        }
    }
}
