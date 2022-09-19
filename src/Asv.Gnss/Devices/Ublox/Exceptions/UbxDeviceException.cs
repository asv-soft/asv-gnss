using System;

namespace Asv.Gnss
{
    public class UbxDeviceException : Exception
    {
        public string SourceName { get; }

        public UbxDeviceException(string source, string message) : base(message)
        {
            SourceName = source;
        }

        public UbxDeviceException(string source, string message, Exception inner) : base(message, inner)
        {
            SourceName = source;
        }
    }
}