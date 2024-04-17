using System;
using System.Text;
using Asv.IO;

namespace Asv.Gnss
{
    /// <summary>
    /// Represents the base class for NMEA0183 messages.
    /// </summary>
    /// <typeparam name="TData">The type of data contained in the message.</typeparam>
    public abstract class Nmea0183MessageBase : GnssMessageBase<string>
    {
        /// <summary>
        /// Gets the unique identifier of the GNSS protocol used in NMEA 0183 messages.
        /// </summary>
        /// <value>
        /// The unique identifier of the GNSS protocol.
        /// </value>
        public override string ProtocolId => Nmea0183Parser.GnssProtocolId;

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        /// <value>
        /// The name of the property.
        /// </value>
        /// <remarks>
        /// This property returns the value of the <see cref="MessageId"/> property.
        /// </remarks>
        public override string Name => MessageId;

        /// <summary>
        /// The private field representing the source ID.
        /// </summary>
        private string _sourceId;

        /// <summary>
        /// Gets or sets the title of the source.
        /// </summary>
        /// <value>
        /// The title of the source.
        /// </value>
        public string SourceTitle { get; private set; }

        /// <summary>
        /// Gets or sets the source ID.
        /// </summary>
        /// <value>
        /// The source ID.
        /// </value>
        public string SourceId
        {
            get => _sourceId;
            set
            {
                _sourceId = value;
                SourceTitle = Nmea0183Helper.TryFindSourceTitleById(value);
            }
        }

        /// <summary>
        /// Deserializes a byte buffer into the current instance.
        /// </summary>
        /// <param name="buffer">The byte buffer to deserialize</param>
        public override void Deserialize(ref ReadOnlySpan<byte> buffer)
        {
            if (buffer.Length < 5) throw new Exception("Too small string for NMEA");
            var message = buffer.GetString(Encoding.ASCII);
            SourceId = message.StartsWith('P') ? "P" : message[..2];
            var items = message.Trim().Split(',');
            InternalDeserializeFromStringArray(items);
            buffer = buffer.Slice(buffer.Length);
        }

        /// <summary>
        /// This method is used to internally deserialize an array of strings into the appropriate data structure. </summary> <param name="items">
        /// An array of strings containing the serialized data to be deserialized. </param> <remarks>
        /// This method should be implemented by derived classes to perform the deserialization logic.
        /// It is called internally by other methods within the class. </remarks> <seealso cref="YourDerivedClass.DeserializeFromStringArray(string[])"/>
        /// /
        protected abstract void InternalDeserializeFromStringArray(string[] items);

        /// <summary>
        /// Serializes the object and writes the data to the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer to write the serialized data to.</param>
        public override void Serialize(ref Span<byte> buffer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the byte size of the object.
        /// </summary>
        /// <returns>The byte size of the object.</returns>
        public override int GetByteSize()
        {
            throw new NotImplementedException();
        }
    }
}