namespace Asv.Gnss
{
    /// <summary>
    /// This command saves the user’s present configuration, including the current log settings (type, whether output testing data, etc.), FIX settings, baud rate, and so on, refer to Table 14.
    /// </summary>
    public class ComNavSaveConfigCommand : ComNavAsciiCommandBase
    {
        /// <summary>
        /// Gets the default <see cref="ComNavSaveConfigCommand"/> instance.
        /// </summary>
        /// <value>
        /// The default <see cref="ComNavSaveConfigCommand"/> instance.
        /// </value>
        public static ComNavSaveConfigCommand Default { get; } = new();

        /// <summary>
        /// Constant variable representing the message content "SAVECONFIG".
        /// </summary>
        public const string MessageContent = "SAVECONFIG";

        /// <summary>
        /// Serializes the message content to an ASCII string.
        /// </summary>
        /// <returns>
        /// The ASCII string representation of the message content.
        /// </returns>
        protected override string SerializeToAsciiString() => MessageContent;

        /// <summary>
        /// Gets or sets the unique identifier of the message.
        /// </summary>
        /// <remarks>
        /// This property represents the unique identifier associated with the message.
        /// It can be used for various purposes such as tracking, referencing or identification.
        /// </remarks>
        /// <value>The unique identifier of the message.</value>
        public override string MessageId => MessageContent;
    }
}