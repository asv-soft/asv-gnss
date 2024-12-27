namespace Asv.Gnss
{
    public class ComNavPortCfg
    {
        public ComNavPortEnum Port { get; set; }
        public uint BaudRate { get; set; }
        public uint[] OtherParams { get; } = new uint[8];
    }
}
