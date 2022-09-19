namespace Asv.Gnss
{
    public class UbxDeviceNakException : UbxDeviceException
    {
        public UbxMessageBase Request { get; }

        public UbxDeviceNakException(string source, UbxMessageBase request) : base(source,$"Command {request.Name} error: source {source} result code NAK")
        {
            Request = request;
        }
    }
}