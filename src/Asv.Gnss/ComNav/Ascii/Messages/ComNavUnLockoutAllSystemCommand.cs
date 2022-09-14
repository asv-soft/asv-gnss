namespace Asv.Gnss
{
    /// Reinstates all previously locked out satellites
    /// This command allows all satellites or systems which have been previously locked out
    /// (LOCKOUT command on page 242 or LOCKOUTSYSTEM command on page 243) to be reinstated in the solution computation.
    /// </summary>
    public class ComNavUnLockoutAllSystemCommand : ComNavAsciiCommandBase
    {
        public static ComNavUnLockoutAllSystemCommand Default { get; } = new();

        public const string MessageContent = "UNLOCKOUTALL";

        protected override string SerializeToAsciiString() => MessageContent;

        public override string MessageId => MessageContent;
    }
}