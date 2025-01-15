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
    }
}
