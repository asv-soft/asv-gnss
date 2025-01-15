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
    }
}
