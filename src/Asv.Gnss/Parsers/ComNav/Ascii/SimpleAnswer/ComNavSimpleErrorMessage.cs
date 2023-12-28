namespace Asv.Gnss
{
    public class ComNavSimpleErrorMessage : ComNavSimpleAnswerMessageBase
    {
        public const string ComNavMessageId = "Error!";
        public override string MessageId => ComNavMessageId;
        public override string Name => ComNavMessageId;
    }
}