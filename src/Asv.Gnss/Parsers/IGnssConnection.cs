using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Asv.Common;
using Asv.IO;

namespace Asv.Gnss
{
    /// <summary>
    /// Represents a connection to a GNSS device.
    /// </summary>
    public interface IGnssConnection:IDisposable
    {
        /// <summary>
        /// Gets the data stream associated with the object.
        /// </summary>
        /// <returns>The data stream.</returns>
        /// <remarks>
        /// The data stream provides the functionality to read and write data from and to the object.
        /// </remarks>
        IDataStream Stream { get; }

        /// <summary>
        /// Gets the statistic for the received bytes.
        /// </summary>
        /// <value>
        /// The statistic for the received bytes.
        /// </value>
        IRxValue<int> StatisticRxBytes { get; }

        /// <summary>
        /// Gets the statistic value for transmitted bytes.
        /// </summary>
        /// <remarks>
        /// This property provides the total number of transmitted bytes as an integer value.
        /// </remarks>
        /// <returns>
        /// The statistic value for transmitted bytes.
        /// </returns>
        IRxValue<int> StatisticTxBytes { get; }

        /// <summary>
        /// Gets the collection of GNSS message parsers.
        /// </summary>
        /// <value>
        /// The collection of GNSS message parsers.
        /// </value>
        /// <remarks>
        /// GNSS message parsers are responsible for parsing GNSS messages into usable objects.
        /// </remarks>
        IEnumerable<IGnssMessageParser> Parsers { get; }

        /// <summary>
        /// Gets an observable sequence of <see cref="GnssParserException"/> that represents the errors occurred during parsing.
        /// </summary>
        /// <remarks>
        /// This property returns an <see cref="IObservable{T}"/> that emits instances of <see cref="GnssParserException"/> class whenever an error occurs during parsing.
        /// Subscribers can handle and log these errors to provide appropriate error handling and notification.
        /// </remarks>
        /// <value>
        /// An <see cref="IObservable{T}"/> of <see cref="GnssParserException"/> that represents the errors occurred during parsing.
        /// </value>
        IObservable<GnssParserException> OnError { get; }

        /// <summary>
        /// Gets the observable sequence that emits GNSS messages.
        /// </summary>
        /// <remarks>
        /// GNSS messages are emitted as instances of <see cref="IGnssMessageBase"/>.
        /// </remarks>
        /// <value>
        /// The observable sequence that emits GNSS messages.
        /// </value>
        IObservable<IGnssMessageBase> OnMessage { get; }

        /// <summary>
        /// Gets an IObservable interface for subscribing to the OnTxMessage event, which notifies when a GNSS message is transmitted.
        /// </summary>
        /// <value>
        /// An IObservable interface that can be used to subscribe to the OnTxMessage event.
        /// </value>
        IObservable<IGnssMessageBase> OnTxMessage { get; }

        /// <summary>
        /// Sends the GNSS message.
        /// </summary>
        /// <param name="msg">The GNSS message to send.</param>
        /// <param name="cancel">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation, returning true if the message was sent successfully; otherwise, false.</returns>
        Task<bool> Send(IGnssMessageBase msg, CancellationToken cancel);
        
    }


    /// <summary>
    /// Factory class for creating GNSS connections and filtering messages.
    /// </summary>
    public static class GnssFactory
    {
        /// <summary>
        /// Creates a default instance of <see cref="IGnssConnection"/> using the specified connection string. </summary> <param name="connectionString">The connection string to be used for the GNSS connection.</param> <returns>An instance of <see cref="IGnssConnection"/> configured with default parsers and registered default messages.</returns>
        /// /
        public static IGnssConnection CreateDefault(string connectionString)
        {
            return new GnssConnection(connectionString,
                new AsvMessageParser().RegisterDefaultMessages(),
                new ComNavBinaryParser().RegisterDefaultMessages(),
                new Nmea0183Parser().RegisterDefaultMessages(),
                new RtcmV2Parser().RegisterDefaultMessages(),
                new RtcmV3Parser().RegisterDefaultMessages(),
                new UbxBinaryParser().RegisterDefaultMessages(),
                new SbfBinaryParser().RegisterDefaultMessages());
        }

        /// <summary>
        /// Filters the source observable stream to only include messages of type TMsg and with matching MessageId.
        /// </summary>
        /// <typeparam name="TMsg">The type of messages to filter.</typeparam>
        /// <typeparam name="TMsgId">The type of the MessageId.</typeparam>
        /// <param name="src">The source observable stream of GnssMessageBase<TMsgId>.</param>
        /// <returns>An observable stream of type TMsg.</returns>
        public static IObservable<TMsg> Filter<TMsg,TMsgId>(this IObservable<GnssMessageBase<TMsgId>> src) 
            where TMsg :GnssMessageBase<TMsgId>, new()
        {
            var msg = new TMsg();
            var id = msg.MessageId;
            return src.Where(_ => _.MessageId.Equals(id) && _ is TMsg).Cast<TMsg>();
        }

        /// <summary>
        /// Filters the messages of the given type from the GNSS connection messages.
        /// </summary>
        /// <typeparam name="TMsg">The type of message to filter.</typeparam>
        /// <param name="src">The GNSS connection to filter the messages from.</param>
        /// <returns>An Observable that contains only the messages of type TMsg.</returns>
        public static IObservable<TMsg> Filter<TMsg>(this IGnssConnection src)
        {
            return src.OnMessage.Where(_ => _ is TMsg).Cast<TMsg>();
        }

        /// <summary>
        /// Retrieves real-time corrected measurement messages from a GNSS connection in RTCMv3 format.
        /// </summary>
        /// <param name="src">The GNSS connection.</param>
        /// <returns>An observable sequence of RtcmV3RawMessage objects representing the raw RTCMv3 messages.</returns>
        public static IObservable<RtcmV3RawMessage> GetRtcmV3RawMessages(this IGnssConnection src)
        {
            return ((RtcmV3Parser)src.Parsers.First(_ => _ is RtcmV3Parser)).OnRawMessage;
        }

        /// <summary>
        /// Filters the messages received from the GNSS connection based on the specified tag.
        /// </summary>
        /// <typeparam name="TMsg">The type of the messages.</typeparam>
        /// <param name="src">The GNSS connection.</param>
        /// <param name="setTagCallback">The callback method to set the tag for the messages.</param>
        /// <returns>An observable sequence of messages of type TMsg.</returns>
        public static IObservable<TMsg> FilterWithTag<TMsg>(this IGnssConnection src, Action<TMsg> setTagCallback)
        {
            return src.OnMessage.Where(_ => _ is TMsg).Cast<TMsg>().Do(setTagCallback);
        }

        /// <summary>
        /// Filters the messages from the GNSS connection based on the specified tag.
        /// </summary>
        /// <typeparam name="TMsg">The type of the GNSS message.</typeparam>
        /// <param name="src">The GNSS connection to filter.</param>
        /// <param name="tag">The tag to filter the messages.</param>
        /// <returns>An observable sequence of messages that match the specified tag.</returns>
        public static IObservable<TMsg> FilterWithTag<TMsg>(this IGnssConnection src, object tag)
            where TMsg : IGnssMessageBase
        {
            return src.OnMessage.Where(_ => _ is TMsg).Cast<TMsg>().Do(_ => _.Tag = tag);
        }

        /// <summary>
        /// Filters the messages received from the GNSS connection based on the specified filter function.
        /// </summary>
        /// <typeparam name="TMsg">The type of the messages.</typeparam>
        /// <param name="src">The GNSS connection.</param>
        /// <param name="filter">The filter function to apply on the messages.</param>
        /// <returns>An observable sequence of messages that satisfy the filter function.</returns>
        public static IObservable<TMsg> Filter<TMsg>(this IGnssConnection src, Func<TMsg, bool> filter)
        {
            return src.OnMessage.Where(_ => _ is TMsg).Cast<TMsg>().Where(filter);
        }
    }
}