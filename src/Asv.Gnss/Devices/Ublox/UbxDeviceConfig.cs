namespace Asv.Gnss
{
    /// <summary>
    /// Represents the configuration settings for a UBX device.
    /// </summary>
    public class UbxDeviceConfig
    {
        /// <summary>
        /// Gets or sets the default UbxDeviceConfig instance.
        /// </summary>
        public static UbxDeviceConfig Default = new();

        /// <summary>
        /// Gets or sets the number of attempts.
        /// </summary>
        /// <value>
        /// The number of attempts.
        /// </value>
        public int AttemptCount { get; set; } = 3;

        /// <summary>
        /// Gets or sets the timeout for executing a command in milliseconds.
        /// </summary>
        /// <value>
        /// The command timeout in milliseconds.
        /// </value>
        public int CommandTimeoutMs { get; set; } = 3000;
    }
}
