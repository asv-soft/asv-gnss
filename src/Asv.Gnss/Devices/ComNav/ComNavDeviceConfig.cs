namespace Asv.Gnss
{
    public class ComNavDeviceConfig
    {
        public static ComNavDeviceConfig Default = new();
        public int AttemptCount { get; set; } = 3;
        public int CommandTimeoutMs { get; set; } = 3000;
        public int ConnectTimeoutMs { get; set; } = 5000;
    }
}
