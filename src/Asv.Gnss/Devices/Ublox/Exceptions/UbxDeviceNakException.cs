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
        /// Represents an exception that occurs when a UbxDevice returns a NAK (Negative Acknowledge) response for a command.
        /// </summary>
        /// <remarks>
        /// This exception is thrown when the UbxDevice receives a NAK response from the connected device,
        /// indicating that the requested command was not executed successfully.
        /// </remarks>
        public UbxDeviceNakException(string source, UbxMessageBase request) : base(source,$"Command {request.Name} error: source {source} result code NAK")
        {
            Request = request;
        }
    }
}