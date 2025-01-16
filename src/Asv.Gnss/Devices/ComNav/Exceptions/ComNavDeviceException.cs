using System;

namespace Asv.Gnss
{
    public class ComNavDeviceException : Exception
    {
        public string SourceName { get; }

        public ComNavDeviceException(string source, string message)
            : base(message)
        {
            SourceName = source;
        }

        public ComNavDeviceException(string source, string message, Exception inner)
            : base(message, inner)
        {
            SourceName = source;
        }

        public ComNavDeviceException()
            : base() { }

        public ComNavDeviceException(string message)
            : base(message) { }

        public ComNavDeviceException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
