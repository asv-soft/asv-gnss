namespace Asv.Gnss
{
    public class ComNavDeviceTimeoutException : ComNavDeviceException
    {
        public ComNavAsciiCommandBase Request { get; }
        public int TimeoutMs { get; }

        public ComNavDeviceTimeoutException(
            string source,
            ComNavAsciiCommandBase request,
            int timeoutMs
        )
            : base(
                source,
                $"Timeout ({timeoutMs} ms) to execute {request.Name} from source {source}"
            )
        {
            Request = request;
            TimeoutMs = timeoutMs;
        }

        public ComNavDeviceTimeoutException(string source, string message)
            : base(source, message) { }

        public ComNavDeviceTimeoutException(string source, string message, System.Exception inner)
            : base(source, message, inner) { }

        public ComNavDeviceTimeoutException()
            : base() { }

        public ComNavDeviceTimeoutException(string message)
            : base(message) { }

        public ComNavDeviceTimeoutException(string message, System.Exception innerException)
            : base(message, innerException) { }
    }
}
