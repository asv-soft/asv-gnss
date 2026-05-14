namespace Asv.Gnss
{
    public enum ComNavPpsPolarityEnum
    {
        Negative,
        Positive
    }
    public class ComNavSetPpsControlCommand : ComNavAsciiCommandBase
    {
        public ComNavPpsPolarityEnum Polarity { get; set; } = ComNavPpsPolarityEnum.Negative;

        public double Period { get; set; } = 1.0;

        public int PulseWidth { get; set; } = 1000;

        public override string MessageId => "PPSCONTROL";
        protected override string SerializeToAsciiString()
        {
            var pol = Polarity == ComNavPpsPolarityEnum.Negative ? "NEGATIVE" : "POSITIVE";
            return $"PPSCONTROL ENABLE {pol} {Period:F1} {PulseWidth}";
        }
    }
}