using System;

namespace Asv.Gnss
{
    /// <summary>
    /// Represents an exception that is thrown by the UbxDevice class.
    /// </summary>
    public class UbxDeviceException : Exception
    {
        /// <summary>
        /// Gets the name of the source.
        /// </summary>
        /// <value>
        /// The name of the source.
        /// </value>
        public string SourceName { get; }

        /// <summary>
        /// Represents an exception specific to UbxDevice.
        /// </summary>
        /// <remarks>
        /// It is thrown when an error occurs in UbxDevice class.
        /// </remarks>
        /// <param name="source">The source of the exception.</param>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public UbxDeviceException(string source, string message)
            : base(message)
        {
            SourceName = source;
        }

        /// <summary>
        /// Initializes a new instance of the UbxDeviceException class with a specified source, message, and inner exception.
        /// </summary>
        /// <param name="source">The source of the exception.</param>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="inner">The exception that is the cause of the current exception.</param>
        public UbxDeviceException(string source, string message, Exception inner)
            : base(message, inner)
        {
            SourceName = source;
        }
    }
}
