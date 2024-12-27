namespace Asv.Gnss.Shell
{
    public class UbxTrackLoggerConfig
    {
        /// <summary>
        /// Gets or sets connection string for UBX.
        /// </summary>
        public string ConnectionString { get; set; } = "tcp://10.10.5.16:30"; // "serial:COM10?br=115200";

        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets pvt logging rate for UBX (Hz).
        /// </summary>
        public byte PvtRate { get; set; } = 1;

        /// <summary>
        /// Gets or sets the timeout value in milliseconds for reconnecting.
        /// </summary>
        /// <value>
        /// The timeout value in milliseconds for reconnecting. The default value is 10,000 milliseconds.
        /// </value>
        public int ReconnectTimeoutMs { get; set; } = 10_000;
    }
}
