using System;
using Asv.IO;

namespace Asv.Gnss
{
    /// <summary>
    /// Represents the type of an ASV device.
    /// </summary>
    public enum AsvDeviceType : ushort
    {
        /// <summary>
        /// Represents an unknown device type.
        /// </summary>
        /// <remarks>
        /// This value indicates that the device type is unknown or not specified.
        /// </remarks>
        Unknown = 0,

        /// <summary>
        /// Represents the GbasServer device type.
        /// </summary>
        GbasServer = 1,

        /// <summary>
        /// Represents the device types used in the ASV system.
        /// </summary>
        GbasModulator = 2,

        /// <summary>
        /// Represents a Gbas monitoring device.
        /// </summary>
        GbasMonDev = 3,
    }

    /// <summary>
    /// Represents the state of an ASV device.
    /// </summary>
    public enum AsvDeviceState : byte
    {
        /// <summary>
        /// Represents the unknown state of an ASV device.
        /// </summary>
        /// <remarks>
        /// This state indicates that the current state of the ASV device is unknown.
        /// </remarks>
        /// <example>
        /// The ASV device state can be set to Unknown in the following way:
        /// <code>
        /// AsvDeviceState state = AsvDeviceState.Unknown;
        /// </code>
        /// </example>
        Unknown = 0,

        /// <summary>
        /// Represents the state of the ASV device as active.
        /// </summary>
        Active = 1,

        /// <summary>
        /// Represents the error state of the AsvDeviceState enumeration.
        /// </summary>
        /// <remarks>
        /// The Error state indicates that there is an error or malfunction in the associated device.
        /// </remarks>
        Error = 2,
    }

    /// <summary>
    /// Represents a HeartBeat message in the AsvMessage protocol.
    /// </summary>
    public class AsvMessageHeartBeat : AsvMessageBase
    {
        /// <summary>
        /// The unique identifier of a packet message.
        /// </summary>
        public static ushort PacketMessageId = 0x0000;

        /// <summary>
        /// Gets the unique identifier for the message.
        /// </summary>
        /// <value>
        /// The message identifier.
        /// </value>
        public override ushort MessageId => PacketMessageId;

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        /// <value>
        /// The name "HeartBeat".
        /// </value>
        public override string Name => "HeartBeat";

        /// <summary>
        /// Gets or sets the type of the device.
        /// </summary>
        /// <value>
        /// The type of the device.
        /// </value>
        public AsvDeviceType DeviceType { get; set; }

        /// <summary>
        /// Represents the state of a device.
        /// </summary>
        /// <value>
        /// The device state.
        /// </value>
        public AsvDeviceState DeviceState { get; set; }

        /// <summary>
        /// Gets or sets the Reserved1 property.
        /// </summary>
        /// <value>
        /// The Reserved1 property.
        /// </value>
        public byte Reserved1 { get; set; }

        /// <summary>
        /// Gets or sets the Reserved2 property.
        /// </summary>
        /// <value>
        /// A byte representing the value of Reserved2.
        /// </value>
        public byte Reserved2 { get; set; }

        /// <summary>
        /// Gets or sets the value of the Reserved3 property.
        /// </summary>
        /// <remarks>
        /// This property represents a reserved byte value.
        /// </remarks>
        /// <value>
        /// The value of the Reserved3 property.
        /// </value>
        public byte Reserved3 { get; set; }

        /// <summary>
        /// Gets or sets the value of the Reserved4 property.
        /// </summary>
        /// <remarks>
        /// This property represents a byte value that is reserved for future use.
        /// </remarks>
        /// <value>
        /// The value of the Reserved4 property.
        /// </value>
        public byte Reserved4 { get; set; }

        /// <summary>
        /// Deserializes the internal content of the object from a byte buffer.
        /// </summary>
        /// <param name="buffer">A reference to the byte buffer containing the serialized data.</param>
        protected override void InternalContentDeserialize(ref ReadOnlySpan<byte> buffer)
        {
            DeviceType = (AsvDeviceType)BinSerialize.ReadUShort(ref buffer);
            DeviceState = (AsvDeviceState)BinSerialize.ReadByte(ref buffer);
            Reserved1 = BinSerialize.ReadByte(ref buffer);
            Reserved2 = BinSerialize.ReadByte(ref buffer);
            Reserved3 = BinSerialize.ReadByte(ref buffer);
            Reserved4 = BinSerialize.ReadByte(ref buffer);
        }

        /// <summary>
        /// Serializes the internal content of the object into a byte buffer.
        /// </summary>
        /// <param name="buffer">
        /// A reference to the byte buffer where the serialized content will be written.
        /// </param>
        protected override void InternalContentSerialize(ref Span<byte> buffer)
        {
            BinSerialize.WriteUShort(ref buffer, (ushort)DeviceType);
            BinSerialize.WriteByte(ref buffer,(byte)DeviceState);
            BinSerialize.WriteByte(ref buffer, Reserved1);
            BinSerialize.WriteByte(ref buffer, Reserved2);
            BinSerialize.WriteByte(ref buffer, Reserved3);
            BinSerialize.WriteByte(ref buffer, Reserved4);
        }

        /// <summary>
        /// Retrieves the size of the content in bytes.
        /// </summary>
        /// <returns>
        /// The size of the content in bytes.
        /// </returns>
        protected override int InternalGetContentByteSize() => 7;

        /// Randomizes the properties of the object using the provided random number generator.
        /// @param random The random number generator to use for generating random values.
        /// /
        public override void Randomize(Random random)
        {
            Tag = null;
            Sequence = (ushort)random.Next(0, ushort.MaxValue);
            TargetId = (byte)random.Next(0, byte.MaxValue);
            SenderId = (byte)random.Next(0, byte.MaxValue);
            DeviceType = (AsvDeviceType)random.Next(0,Enum.GetValues(typeof(AsvDeviceType)).Length -1);
            DeviceState = (AsvDeviceState)random.Next(0, Enum.GetValues(typeof(AsvDeviceState)).Length - 1);
            Reserved1 = (byte)random.Next(0, byte.MaxValue);
            Reserved2 = (byte)random.Next(0, byte.MaxValue);
            Reserved3 = (byte)random.Next(0, byte.MaxValue);
            Reserved4 = (byte)random.Next(0, byte.MaxValue);
        }
    }
}