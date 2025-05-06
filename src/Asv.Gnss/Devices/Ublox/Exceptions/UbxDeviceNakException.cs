namespace Asv.Gnss
{
    /// <summary>
    /// The <see cref="UbxDeviceNakException"/> class represents an exception that is thrown when a UBX message request is rejected with a NAK (Negative Acknowledge) response from the device
    /// .
    /// </summary>
    public class UbxDeviceNakException : UbxDeviceException
    {
        /// <summary>
        /// Gets the request message for communicating with the UBX protocol.
        /// </summary>
        /// <value>
        /// The request message.
        /// </value>
        public UbxMessageBase Request { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UbxDeviceNakException"/> class.
        /// Represents an exception that occurs when a UbxDevice returns a NAK (Negative Acknowledge) response for a command.
        /// </summary>
        /// <remarks>
        /// This exception is thrown when the UbxDevice receives a NAK response from the connected device,
        /// indicating that the requested command was not executed successfully.
        /// </remarks>
        public UbxDeviceNakException(string source, UbxMessageBase request)
            : base(source, $"Command {request.Name} error: source {source} result code NAK")
        {
            Request = request;
        }

        public UbxDeviceNakException(string source, string message)
            : base(source, message) { }

        public UbxDeviceNakException(string source, string message, System.Exception inner)
            : base(source, message, inner) { }

        public UbxDeviceNakException()
            : base() { }

        public UbxDeviceNakException(string message)
            : base(message) { }

        public UbxDeviceNakException(string message, System.Exception innerException)
            : base(message, innerException) { }
    }
}
