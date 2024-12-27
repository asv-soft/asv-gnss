using Asv.Common;

namespace Asv.Gnss.Shell
{
    public interface ITrackLogger
    {
        IRxValue<PvtInfo> OnPvtInfo { get; }
        IRxValue<Nmea0183MessageBase> OnNmea { get; }
        public void Init();
    }
}
