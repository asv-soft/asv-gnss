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
    }
}
