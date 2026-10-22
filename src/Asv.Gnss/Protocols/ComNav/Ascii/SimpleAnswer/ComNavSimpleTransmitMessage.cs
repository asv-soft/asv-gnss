namespace Asv.Gnss
{
    public partial class ComNavSimpleTransmitMessage : ComNavSimpleAnswerMessageBase
    {
        public const string ComNavMessageId = "Command transmited";
        public override string MessageId => ComNavMessageId;
        public override string Name => ComNavMessageId;
    }
}