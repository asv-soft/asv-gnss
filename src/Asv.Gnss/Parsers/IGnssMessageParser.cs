using System;

namespace Asv.Gnss
{
    /// <summary>
    /// Represents a GNSS message parser interface.
    /// </summary>
    public interface IGnssMessageParser : IDisposable
    {
        /// <summary>
        /// Gets the number of input bytes for the statistic.
        /// </summary>
        int StatisticInputBytes { get; }

        /// <summary>
        /// Gets the identifier of the protocol.
        /// </summary>
        /// <remarks>
        /// The ProtocolId property represents the unique identifier of the protocol.
        /// This identifier is used to distinguish between different protocols.
        /// </remarks>
        /// <value>
        /// A string representing the protocol identifier.
        /// </value>
        string ProtocolId { get; }

        /// <summary>
        /// Reads the given byte and returns a boolean value.
        /// </summary>
        /// <param name="data">The byte to be read.</param>
        /// <returns>A boolean value indicating whether the read operation was successful.</returns>
        bool Read(byte data);

        /// <summary>
        /// Resets the state of the method.
        /// </summary>
        /// <remarks>
        /// This method resets any internal variables or properties of the method to their initial state.
        /// </remarks>
        void Reset();

        /// <summary>
        /// Gets an observable sequence of <see cref="GnssParserException"/> that represents the errors occurred during parsing.
        /// </summary>
        /// <value>
        /// An observable sequence of <see cref="GnssParserException"/> that represents the errors occurred during parsing.
        /// </value>
        IObservable<GnssParserException> OnError { get; }

        /// <summary>
        /// Gets an observable sequence of GNSS messages received.
        /// </summary>
        /// <value>
        /// An observable sequence that emits <see cref="IGnssMessageBase"/> objects whenever a GNSS message is received.
        /// </value>
        IObservable<IGnssMessageBase> OnMessage { get; }
    }
}
