namespace Asv.Gnss
{
    // Reinstates all previously locked out satellites
    // This command allows all satellites or systems which have been previously locked out
    // (LOCKOUT command on page 242 or LOCKOUTSYSTEM command on page 243) to be reinstated in the solution computation.
    // </summary>
    public class ComNavUnLockoutAllSystemCommand : ComNavAsciiCommandBase
    {
        /// <summary>
        /// Gets the default instance of the ComNavUnLockoutAllSystemCommand class.
        /// </summary>
        /// <remarks>
        /// This property represents the default instance of the ComNavUnLockoutAllSystemCommand class.
        /// It is used to access the ComNavUnLockoutAllSystemCommand without having to instantiate a new object.
        /// </remarks>
        public static ComNavUnLockoutAllSystemCommand Default { get; } = new();

        /// <summary>
        /// Represents the content of a message.
        /// </summary>
        public const string MessageContent = "UNLOCKOUTALL";

        /// <summary>
        /// This method serializes the message content to an ASCII string.
        /// </summary>
        /// <returns>
        /// The serialized ASCII string representation of the message content.
        /// </returns>
        protected override string SerializeToAsciiString() => MessageContent;

        /// <summary>
        /// Gets the identifier of the message.
        /// </summary>
        /// <value>
        /// The identifier of the message.
        /// </value>
        public override string MessageId => MessageContent;
    }
}
