using System;
using Asv.IO;

namespace Asv.Gnss
{
    public class UbxNavSbasPool : UbxMessageBase
    {
        public override string Name => "UBX-NAV-SBAS-POOL";
        public override byte Class => 0x01;
        public override byte SubClass => 0x32;
        protected override void SerializeContent(ref Span<byte> buffer)
        {
             
        }

        protected override void DeserializeContent(ref ReadOnlySpan<byte> buffer)
        {
             
        }

        protected override int GetContentByteSize() => 0;

        public override void Randomize(Random random)
        {
             
        }
    }
    public class UbxNavSbas : UbxMessageBase
    {
        public override string Name => "UBX-NAV-SBAS";
        public override byte Class => 0x01;
        public override byte SubClass => 0x32;
        
        /// <summary>
        /// GPS time of week of the navigation epoch.
        /// </summary>
        public uint iTOW { get; set; }
        
        /// <summary>
        /// PRN Number of the GEO where correction and integrity data is used from
        /// </summary>
        public byte Geo { get; set; }

        /// <summary>
        /// SBAS Mode
        /// 0 Disabled
        /// 1 Enabled integrity
        /// 3 Enabled test mode
        /// </summary>
        public byte Mode { get; set; }

        /// <summary>
        /// SBAS System (WAAS/EGNOS/...)
        /// -1 Unknown
        /// 0 WAAS
        /// 1 EGNOS
        /// 2 MSAS
        /// 3 GAGAN
        /// 16 GPS
        /// </summary>
        public sbyte Sys { get; set; }

        /// <summary>
        /// SBAS Services available (see graphic below)
        /// </summary>
        public byte Service { get; set; }

        #region Service bits
        
        /// <summary>
        /// GEO may be used as ranging source
        /// </summary>
        public bool IsRanging { get; set; }
        
        /// <summary>
        /// GEO is providing correction data
        /// </summary>
        public bool IsCorrections { get; set; }

        /// <summary>
        /// GEO is providing integrity
        /// </summary>
        public bool IsIntegrity { get; set; }

        /// <summary>
        /// GEO is in test mode
        /// </summary>
        public bool IsTestmode { get; set; }

        /// <summary>
        /// Problem with signal or broadcast data indicated
        /// </summary>
        public bool IsBad { get; set; }
        
        #endregion

        /// <summary>
        /// Number of SV data following
        /// </summary>
        public byte Cnt { get; set; }

        /// <summary>
        /// SBAS status flags
        /// 0 = Unknown
        /// 1 = Integrity information is not available or SBAS integrity is not enabled
        /// 2 = Receiver uses only GPS satellites for which integrity information is available
        /// </summary>
        public byte StatusFlags { get; set; }

        /// <summary>
        /// Reserved 1
        /// </summary>
        public ushort Reserved1 { get; set; } 
        
        public SvData[] SvDatas { get; set; }

        protected override void SerializeContent(ref Span<byte> buffer)
        {
             
        }

        protected override void DeserializeContent(ref ReadOnlySpan<byte> buffer)
        {
            iTOW = BinSerialize.ReadUInt(ref buffer);
            Geo = BinSerialize.ReadByte(ref buffer);
            Mode = BinSerialize.ReadByte(ref buffer);
            Sys = BinSerialize.ReadSByte(ref buffer);
            Service = BinSerialize.ReadByte(ref buffer);

            IsBad = (Service & 0b0001_0000) != 0;
            IsTestmode = (Service & 0b0000_1000) != 0;
            IsIntegrity = (Service & 0b0000_0100) != 0;
            IsCorrections = (Service & 0b0000_0010) != 0;
            IsRanging = (Service & 0b0000_0001) != 0;
            
            Cnt = BinSerialize.ReadByte(ref buffer);
            StatusFlags = BinSerialize.ReadByte(ref buffer);
            Reserved1 = BinSerialize.ReadUShort(ref buffer);

            SvDatas = new SvData[Cnt];
            
            for (int i = 0; i < Cnt; i++)
            {
                SvDatas[i].SvId = BinSerialize.ReadByte(ref buffer);
                SvDatas[i].Flags = BinSerialize.ReadByte(ref buffer);
                SvDatas[i].Udre = BinSerialize.ReadByte(ref buffer);
                SvDatas[i].SvSys = BinSerialize.ReadByte(ref buffer);
                SvDatas[i].SvService = BinSerialize.ReadByte(ref buffer);
                
                SvDatas[i].IsBad = (SvDatas[i].SvService & 0b0001_0000) != 0;
                SvDatas[i].IsTestmode = (SvDatas[i].SvService & 0b0000_1000) != 0;
                SvDatas[i].IsIntegrity = (SvDatas[i].SvService & 0b0000_0100) != 0;
                SvDatas[i].IsCorrections = (SvDatas[i].SvService & 0b0000_0010) != 0;
                SvDatas[i].IsRanging = (SvDatas[i].SvService & 0b0000_0001) != 0;
                
                SvDatas[i].Reserved2 = BinSerialize.ReadByte(ref buffer);
                SvDatas[i].Prc = BinSerialize.ReadShort(ref buffer);
                SvDatas[i].SvId = BinSerialize.ReadByte(ref buffer);
                SvDatas[i].Reserved3 = BinSerialize.ReadUShort(ref buffer);
                SvDatas[i].Ic = BinSerialize.ReadShort(ref buffer);
            }
        }

        protected override int GetContentByteSize() => 12 + (12 * Cnt);

        public override void Randomize(Random random)
        {
             
        }
    }

    public class SvData
    {
        /// <summary>
        /// SV ID
        /// </summary>
        public byte SvId { get; set; }
        
        /// <summary>
        /// Flags for this SV
        /// </summary>
        public byte Flags { get; set; }
        
        /// <summary>
        /// Monitoring status
        /// </summary>
        public byte Udre { get; set; }
        
        /// <summary>
        /// System (WAAS/EGNOS/...)
        /// same as SYS
        /// </summary>
        public byte SvSys { get; set; }
        
        /// <summary>
        /// Services available
        /// same as SERVICE
        /// </summary>
        public byte SvService { get; set; }
        
        #region SvService bits
        
        /// <summary>
        /// GEO may be used as ranging source
        /// </summary>
        public bool IsRanging { get; set; }
        
        /// <summary>
        /// GEO is providing correction data
        /// </summary>
        public bool IsCorrections { get; set; }

        /// <summary>
        /// GEO is providing integrity
        /// </summary>
        public bool IsIntegrity { get; set; }

        /// <summary>
        /// GEO is in test mode
        /// </summary>
        public bool IsTestmode { get; set; }

        /// <summary>
        /// Problem with signal or broadcast data indicated
        /// </summary>
        public bool IsBad { get; set; }
        
        #endregion
        
        /// <summary>
        /// Reserved 2
        /// </summary>
        public byte Reserved2 { get; set; }
        
        /// <summary>
        /// Pseudo Range correction in [cm]
        /// </summary>
        public short Prc { get; set; }
        
        /// <summary>
        /// Reserved 3
        /// </summary>
        public ushort Reserved3 { get; set; }
        
        /// <summary>
        /// Ionosphere correction in [cm]
        /// </summary>
        public short Ic { get; set; }
    }
}