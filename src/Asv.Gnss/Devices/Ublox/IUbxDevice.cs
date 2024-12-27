using System.Threading;
using System.Threading.Tasks;

namespace Asv.Gnss
{
    /// <summary>
    /// Represents a UBX device that can send and receive UBX messages.
    /// </summary>
    public interface IUbxDevice
    {
        /// <summary>
        /// Gets the GNSS connection.
        /// </summary>
        /// <value>
        /// The GNSS connection.
        /// </value>
        IGnssConnection Connection { get; }

        /// <summary>
        /// Pushes a packet of type T to be processed asynchronously.
        /// </summary>
        /// <typeparam name="T">The type of the packet to be pushed.</typeparam>
        /// <param name="pkt">The packet to be pushed.</param>
        /// <param name="cancel">A token that can be used to cancel the push operation.</param>
        /// <returns>A Task representing the asynchronous push operation.</returns>
        /// <remarks>
        /// This method pushes a packet of type T to be processed asynchronously. The packet
        /// is processed in the background and this method returns a Task that can be used
        /// to track the progress or cancel the push operation if required.
        /// </remarks>
        Task Push<T>(T pkt, CancellationToken cancel)
            where T : UbxMessageBase;

        /// <summary>
        /// Pools a packet of type <typeparamref name="TPoolPacket"/> and converts it to a packet of type <typeparamref name="TPacket"/>.
        /// </summary>
        /// <typeparam name="TPacket">The type of the converted packet.</typeparam>
        /// <typeparam name="TPoolPacket">The type of the packet to be pooled.</typeparam>
        /// <param name="pkt">The packet to be pooled.</param>
        /// <param name="cancel">The cancellation token to cancel the pooling operation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous pooling operation. The result of the task is the pooled packet of type <typeparamref name="TPacket"/>.</returns>
        /// <remarks>
        /// This method pools a packet of type <typeparamref name="TPoolPacket"/> and converts it to a packet of type <typeparamref name="TPacket"/>.
        /// The <paramref name="cancel"/> parameter is optional and can be used to cancel the pooling operation.
        /// </remarks>
        Task<TPacket> Pool<TPacket, TPoolPacket>(
            TPoolPacket pkt,
            CancellationToken cancel = default
        )
            where TPacket : UbxMessageBase
            where TPoolPacket : UbxMessageBase;
    }
}
