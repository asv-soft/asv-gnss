namespace Asv.Gnss
{
    public class ComNavDeviceResponseException : ComNavDeviceException
    {
        public ComNavAsciiCommandBase Request { get; }

        public ComNavDeviceResponseException(string source, ComNavAsciiCommandBase request)
            : base(source, $"Command '{request.Name}' error: source {source} result code Error!")
        {
            Request = request;
        }

        public ComNavDeviceResponseException(string source, string message)
            : base(source, message) { }

        public ComNavDeviceResponseException(string source, string message, System.Exception inner)
            : base(source, message, inner) { }

        public ComNavDeviceResponseException()
            : base() { }

        public ComNavDeviceResponseException(string message)
            : base(message) { }

        public ComNavDeviceResponseException(string message, System.Exception innerException)
            : base(message, innerException) { }
    }
}
