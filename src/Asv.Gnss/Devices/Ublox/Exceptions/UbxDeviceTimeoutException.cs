namespace Asv.Gnss
{
    /// <summary>
    /// Represents an exception that is thrown when a timeout occurs while executing a UBX message on a device.
    /// </summary>
    public class UbxDeviceTimeoutException : UbxDeviceException
    {
        /// <summary>
        /// Gets the UbxMessageBase request property.
        /// </summary>
        /// <remarks>
        /// This property represents the UBX message request.
        /// </remarks>
        public UbxMessageBase Request { get; }

        /// <summary>
        /// Gets the timeout value in milliseconds.
        /// </summary>
        /// <value>
        /// The timeout value in milliseconds.
        /// </value>
        public int TimeoutMs { get; }

        public UbxDeviceTimeoutException(string source, UbxMessageBase request, int timeoutMs)
            : base(
                source,
                $"Timeout ({timeoutMs} ms) to execute {request.Name} from source {source}"
            )
        {
            Request = request;
            TimeoutMs = timeoutMs;
        }

        public UbxDeviceTimeoutException(string source, string message)
            : base(source, message) { }

        public UbxDeviceTimeoutException(string source, string message, System.Exception inner)
            : base(source, message, inner) { }

        public UbxDeviceTimeoutException()
            : base() { }

        public UbxDeviceTimeoutException(string message)
            : base(message) { }

        public UbxDeviceTimeoutException(string message, System.Exception innerException)
            : base(message, innerException) { }
    }
}
