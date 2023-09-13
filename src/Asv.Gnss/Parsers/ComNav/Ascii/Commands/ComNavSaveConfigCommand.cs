namespace Asv.Gnss
{
    /// <summary>
    /// This command saves the user’s present configuration, including the current log settings (type, whether output testing data, etc.), FIX settings, baud rate, and so on, refer to Table 14.
    /// </summary>
    public class ComNavSaveConfigCommand : ComNavAsciiCommandBase
    {
        public static ComNavSaveConfigCommand Default { get; } = new();

        public const string MessageContent = "SAVECONFIG";

        protected override string SerializeToAsciiString() => MessageContent;

        public override string MessageId => MessageContent;
    }
}