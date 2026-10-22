namespace Asv.Gnss
{
    public partial class ComNavSimpleOkMessage : ComNavSimpleAnswerMessageBase
    {
        public const string ComNavMessageId = "OK!";
        public override string MessageId => ComNavMessageId;
        public override string Name => ComNavMessageId;
    }
}