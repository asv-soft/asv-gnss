using System;
using Asv.IO;

namespace Asv.Gnss
{
    public class SbfPacketGpsNav : SbfMessageBase
    {
        public override ushort MessageRevision => 1;
        public override ushort MessageType => 5891;
        public override string Name => "GPSNav";

        protected override void DeserializeContent(ref ReadOnlySpan<byte> buffer)
        {
            Prn = BinSerialize.ReadByte(ref buffer);
            var reserved = BinSerialize.ReadByte(ref buffer);
            Week = BinSerialize.ReadUShort(ref buffer);
            var CAorPonL2 = BinSerialize.ReadByte(ref buffer);
            Accuracy = BinSerialize.ReadByte(ref buffer);
            Health = BinSerialize.ReadByte(ref buffer);
            var L2DataFlag = BinSerialize.ReadByte(ref buffer);
            iodc = BinSerialize.ReadUShort(ref buffer);
            iode = BinSerialize.ReadByte(ref buffer);
            var iode3 = BinSerialize.ReadByte(ref buffer);
            var fitIntFlg = BinSerialize.ReadByte(ref buffer);
            var reserved2 = BinSerialize.ReadByte(ref buffer);
            Tgd = BinSerialize.ReadFloat(ref buffer);
            var tocSec = BinSerialize.ReadUInt(ref buffer);
            Af2 = BinSerialize.ReadFloat(ref buffer);
            Af1 = BinSerialize.ReadFloat(ref buffer);
            Af0 = BinSerialize.ReadFloat(ref buffer);
            Crs = BinSerialize.ReadFloat(ref buffer);
            DeltaN = BinSerialize.ReadFloat(ref buffer);
            M0 = BinSerialize.ReadDouble(ref buffer);
            Cuc = BinSerialize.ReadFloat(ref buffer);
            E = BinSerialize.ReadDouble(ref buffer);
            Cus = BinSerialize.ReadFloat(ref buffer);
            A = Math.Pow(BinSerialize.ReadDouble(ref buffer), 2);
            ToeSec = BinSerialize.ReadUInt(ref buffer);
            Cic = BinSerialize.ReadFloat(ref buffer);
            OMEGA0 = BinSerialize.ReadDouble(ref buffer);
            Cis = BinSerialize.ReadFloat(ref buffer);
            i0 = BinSerialize.ReadDouble(ref buffer);
            Crc = BinSerialize.ReadFloat(ref buffer);
            omega = BinSerialize.ReadDouble(ref buffer);
            OmegaDot = BinSerialize.ReadFloat(ref buffer);
            idot = BinSerialize.ReadFloat(ref buffer);
            var weekToc = BinSerialize.ReadUShort(ref buffer);
            var weekToe = BinSerialize.ReadUShort(ref buffer);

            Toe = GpsRawHelper.Gps2Time(weekToe, ToeSec);
            Toc = GpsRawHelper.Gps2Time(weekToc, tocSec);
            TimeTrans = GpsRawHelper.Utc2Gps(UtcTime);
        }

        public byte Prn { get; set; }

        public ushort Week { get; set; }

        public uint ToeSec { get; set; }

        public byte iode { get; set; }

        public ushort iodc { get; set; }

        public byte Health { get; set; }

        public byte Accuracy { get; set; }

        public float Af0 { get; set; }

        public float Af1 { get; set; }

        public float Af2 { get; set; }

        public DateTime Toe { get; set; }

        public DateTime Toc { get; set; }

        public DateTime TimeTrans { get; set; }

        #region SV orbit parameters
        public double A { get; set; }
        public double E { get; set; }
        public double omega { get; set; }
        public double OMEGA0 { get; set; }
        public double i0 { get; set; }
        public double M0 { get; set; }
        public float DeltaN { get; set; }
        public float OmegaDot { get; set; }
        public float idot { get; set; }
        public float Crc { get; set; }
        public float Crs { get; set; }
        public float Cuc { get; set; }
        public float Cus { get; set; }
        public float Cic { get; set; }
        public float Cis { get; set; }
        #endregion

        public float Tgd { get; set; }
    }
}
