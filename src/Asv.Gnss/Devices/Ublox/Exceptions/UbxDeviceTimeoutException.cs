namespace Asv.Gnss
{
    public class UbxDeviceTimeoutException : UbxDeviceException
    {
        public UbxMessageBase Request { get; }
        public int TimeoutMs { get; }

        public UbxDeviceTimeoutException(string source, UbxMessageBase request, int timeoutMs) 
            : base(source, $"Timeout ({timeoutMs} ms) to execute {request.Name} from source {source}")
        {
            Request = request;
            TimeoutMs = timeoutMs;
        }
    }
}