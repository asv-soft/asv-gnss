namespace Asv.Gnss
{
    public class ComNavUnLogAllCommand : ComNavAsciiCommandBase
    {
        public static ComNavUnLogAllCommand Default = new();

        public const string MessageContent = "UNLOGALL";
        public override string MessageId => MessageContent;

        protected override string SerializeToAsciiString() => MessageContent;
    }
}
