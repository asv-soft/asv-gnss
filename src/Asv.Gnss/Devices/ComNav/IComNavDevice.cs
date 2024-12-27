using System.Threading;
using System.Threading.Tasks;

namespace Asv.Gnss
{
    public interface IComNavDevice
    {
        IGnssConnection Connection { get; }
        IGnssConnection RtcmV2Connection { get; }
        Task Push<T>(T pkt, CancellationToken cancel)
            where T : ComNavAsciiCommandBase;
        Task<TPacket> Pool<TPacket, TPoolPacket>(
            TPoolPacket pkt,
            CancellationToken cancel = default
        )
            where TPacket : ComNavAsciiMessageBase
            where TPoolPacket : ComNavAsciiCommandBase;
    }
}
