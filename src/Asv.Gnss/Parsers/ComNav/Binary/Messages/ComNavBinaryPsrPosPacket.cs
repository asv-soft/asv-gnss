using System;
using System.Text;
using Asv.IO;

namespace Asv.Gnss
{
    /// <summary>
    /// 4.2.7.5 PSRPOS Pseudorange Position
    /// This message includes position calculated using pseudorange and other information such as differential age, station id and so on.
    /// </summary>
    public class ComNavBinaryPsrPosPacket : ComNavBinaryMessageBase
    {
        public const ushort ComNavMessageId = 47;

        public override ushort MessageId => ComNavMessageId;
        public override string Name => "PSRPOS";

        protected override void InternalContentDeserialize(ref ReadOnlySpan<byte> buffer)
        {
            SolutionStatus = (ComNavSolutionStatus)BinSerialize.ReadUInt(ref buffer);
            PositionType = (ComNavPositionType)BinSerialize.ReadUInt(ref buffer);


            Latitude = BinSerialize.ReadDouble(ref buffer);
            Longitude = BinSerialize.ReadDouble(ref buffer);
            HeightMsl = BinSerialize.ReadDouble(ref buffer);
            Undulation = BinSerialize.ReadFloat(ref buffer);

            Datum = (ComNavDatum)BinSerialize.ReadUInt(ref buffer);
            
            LatitudeSd = BinSerialize.ReadFloat(ref buffer);
            LongitudeSd = BinSerialize.ReadFloat(ref buffer);
            HeightMslSd = BinSerialize.ReadFloat(ref buffer);
            var station = new byte[4];
            BinSerialize.ReadBlock(ref buffer,new Span<byte>(station));
            BaseStationId = Encoding.ASCII.GetString(station);
            
            DifferentialAgeSec = BinSerialize.ReadFloat(ref buffer);
            SolutionAgeSec = BinSerialize.ReadFloat(ref buffer);

            TracketSats = BinSerialize.ReadByte(ref buffer);
            
            SolutionSats = BinSerialize.ReadByte(ref buffer);

            PsrPosReserved1 = BinSerialize.ReadByte(ref buffer);
            PsrPosReserved2 = BinSerialize.ReadByte(ref buffer);
            PsrPosReserved3 = BinSerialize.ReadByte(ref buffer);

            ExtSolutionStatus = BinSerialize.ReadByte(ref buffer);
            PsrPosReserved4 = BinSerialize.ReadByte(ref buffer);

            SignalMask = BinSerialize.ReadByte(ref buffer);
        }

        public byte PsrPosReserved4 { get; set; }
        public byte PsrPosReserved3 { get; set; }
        public byte PsrPosReserved2 { get; set; }
        public byte PsrPosReserved1 { get; set; }

        protected override void InternalContentSerialize(ref Span<byte> buffer)
        {
            throw new NotImplementedException();
        }

        protected override int InternalGetContentByteSize()
        {
            return 72;
        }

        /// <summary>
        /// Signals used mask - if 0, signals used in solution are unknown. See Table 33.
        /// </summary>
        public byte SignalMask { get; set; }

        /// <summary>
        /// Extended solution status (default: 0)
        /// </summary>
        public byte ExtSolutionStatus { get; set; }

        /// <summary>
        /// Number of satellite vehicles used in solution
        /// </summary>
        public byte SolutionSats { get; set; }

        /// <summary>
        /// Number of satellite vehicles tracked
        /// </summary>
        public byte TracketSats { get; set; }


        /// <summary>
        /// Solution age in seconds
        /// </summary>
        public float SolutionAgeSec { get; set; }

        /// <summary>
        /// Differential age in seconds
        /// </summary>
        public float DifferentialAgeSec { get; set; }

        /// <summary>
        /// This is station ID of the station, who sending DGPS corrections (Not Current station id!)
        /// </summary>
        public string BaseStationId { get; set; }

        /// <summary>
        /// Height standard deviation
        /// </summary>
        public float HeightMslSd { get; set; }
        /// <summary>
        /// Longitude standard deviation
        /// </summary>
        public float LongitudeSd { get; set; }
        /// <summary>
        /// Latitude standard deviation
        /// </summary>
        public float LatitudeSd { get; set; }


        public ComNavDatum Datum { get; set; }
        /// <summary>
        /// Undulation - the relationship between the geoids and the WGS84 ellipsoid (m)
        /// </summary>
        public float Undulation { get; set; }

        /// <summary>
        /// Height above mean sea level
        /// </summary>
        public double HeightMsl { get; set; }

        public double Longitude { get; set; }

        public double Latitude { get; set; }

        public ComNavPositionType PositionType { get; set; }

        public ComNavSolutionStatus SolutionStatus { get; set; }

        
    }


    public static class ComNavBinaryHelper
    {
        public static int GetPnrAndRinexCode(ComNavSatelliteSystemEnum sys, int svId, out string rCode)
        {
            switch (sys)
            {
                case ComNavSatelliteSystemEnum.GPS:
                    rCode = $"G{svId}";
                    return svId;
                case ComNavSatelliteSystemEnum.GLONASS:
                    rCode = $"R{svId - 37}";
                    return svId - 37;
                case ComNavSatelliteSystemEnum.GALILEO:
                    rCode = $"E{svId}";
                    return svId;
                case ComNavSatelliteSystemEnum.BD2:
                case ComNavSatelliteSystemEnum.BD3:
                    rCode = $"C{svId - 140}";
                    return svId - 140;
                default:
                    throw new ArgumentOutOfRangeException(nameof(sys), sys, null);
            }
        }
        public static ComNavSolutionStatus ParseSolutionStatus(byte[] buffer, int byteOffset)
        {
            var solStat = BitConverter.ToUInt32(buffer, byteOffset);
            return (ComNavSolutionStatus)solStat;
        }

        public static ComNavPositionType ParsePositionType(byte[] buffer, int byteOffset)
        {
            var id = BitConverter.ToUInt32(buffer, byteOffset);
            return (ComNavPositionType)id;
        }

        public static ComNavDatum ParseDatum(byte[] buffer, int offsetInBytes)
        {
            return (ComNavDatum)BitConverter.ToUInt32(buffer, offsetInBytes); ;
        }
    }

    public enum ComNavSolutionStatus
    {
        /// <summary>
        /// Solution computed
        /// </summary>
        SolComputed = 0,
        /// <summary>
        /// Insufficient observations
        /// </summary>
        InsufficientObs = 1,
        /// <summary>
        /// Not yet converged from cold start
        /// </summary>
        ColdStart = 6,
        /// <summary>
        /// The fixed position, entered using the FIX position command, is not valid
        /// </summary>
        InvalidFix = 19,
    }

    public enum ComNavPositionType
    {
        /// <summary>
        /// No solution
        /// </summary>
        None = 0,
        /// <summary>
        /// Position has been fixed by the FIX POSITION command 
        /// </summary>
        Fixedpos = 1,
        /// <summary>
        /// Note Velocity computed using instantaneous Doppler 
        /// </summary>
        DopplerVelocity = 8,
        /// <summary>
        /// Single point position 17 PSRDIFF Pseudorange differential solution 
        /// </summary>
        Single = 16,
        /// <summary>
        /// Solution calculated using corrections from an SBAS 
        /// </summary>
        Sbas = 18,
        /// <summary>
        /// Floating narrow-lane ambiguity solution 
        /// </summary>
        NarrowFloat = 34,
        /// <summary>
        /// Derivation solution 
        /// </summary>
        FixDerivation = 35,
        /// <summary>
        /// Integer wide-lane ambiguity solution 
        /// </summary>
        WideInt = 49,
        /// <summary>
        /// Integer narrow-lane ambiguity solution 
        /// </summary>
        NarrowInt = 50,
        /// <summary>
        /// WIDE-LANE Super wide-lane solution 
        /// </summary>
        Super = 51,
        /// <summary>
        /// Positioning solution 65 OMNISTAR_XP Positioning solution 
        /// </summary>
        OmnistarHp = 64,
        /// <summary>
        /// Converging TerraStar-C, TerraStar-C PRO or TerraStar-X solution 
        /// </summary>
        PppConverging = 68,
        /// <summary>
        /// Converged PPP solution 
        /// </summary>
        Ppp = 69,
        /// <summary>
        /// Solution accuracy is within UA Loperational limit 
        /// </summary>
        Operational = 70,
        /// <summary>
        /// Solution accuracy is outside UAL operational limit but within warning limit 
        /// </summary>
        Warning = 71,
        /// <summary>
        /// Solution accuracy is outside UAL limits
        /// </summary>
        OutOfBounds = 72,
    }

    /// <summary>
    ///  Datum Transformation Parameters
    ///  Format in comments: DXb DYb DZb DATUM DESCRIPTION ELLIPSOID
    /// </summary>
    public enum ComNavDatum
    {
        /// <summary>
        /// -162 -12 206 This datum has been updated, see ID# 65c Clarke 1880
        /// </summary>
        Adind = 1,
        /// <summary>
        /// -143 -90 -294 ARC 1950 (SW & SE Africa) Clarke 1880
        /// </summary>
        Arc50 = 2,
        /// <summary>
        /// -160 -8 -300 This datum has been updated, see ID# 66c Clarke 1880
        /// </summary>
        Arc60 = 3,
        /// <summary>
        /// -133 -48 148 Australian Geodetic Datum 1966 Australian National
        /// </summary>
        Agd66 = 4,
        /// <summary>
        /// -134 -48 149 Australian Geodetic Datum 1984 Australian National
        /// </summary>
        Agd84 = 5,
        /// <summary>
        /// -384 664 -48 Bukit Rimpah (Indonesia) Bessel 1841
        /// </summary>
        Bukit = 6,
        /// <summary>
        /// -104 -129 239 Camp Area Astro (Antarctica) International 1924
        /// </summary>
        Astro = 7,
        /// <summary>
        /// 175 -38 113 Chatham 1971 (New Zealand) International 1924
        /// </summary>
        Chatm = 8,
        /// <summary>
        /// -263 6 431 Carthage (Tunisia) Clarke 1880
        /// </summary>
        Carth = 9,
        /// <summary>
        /// -136 -108 -292 CAPE (South Africa) Clarke 1880
        /// </summary>
        Cape = 10,
        /// <summary>
        /// -377 681 -50 Djakarta (Indonesia) Bessel 1841
        /// </summary>
        Djaka = 11,
        /// <summary>
        /// -130 110 -13 Old Egyptian Helmert 1906
        /// </summary>
        Egypt = 12,
        /// <summary>
        /// -87 -98 -121 European 1950 International 1924
        /// </summary>
        Ed50 = 13,
        /// <summary>
        /// -86 -98 -119 European 1979 International 1924
        /// </summary>
        Ed79 = 14,
        /// <summary>
        /// -403 684 41 G. Segara (Kalimantan - Indonesia) Bessel 1841
        /// </summary>
        Gunsg = 15,
        /// <summary>
        /// 84 -22 209 Geodetic Datum 1949 (New Zealand) International 1924
        /// </summary>
        Geo49 = 16,
        /// <summary>
        /// 375 -111 431 Do not use. Use ID# 76 insteadd Airy 1830
        /// </summary>
        Grb36 = 17,
        /// <summary>
        /// -100 -248 259 Guam 1963 (Guam Island) Clarke 1866
        /// </summary>
        Guam = 18,
        /// <summary>
        /// 89 -279 -183 Do not use. Use ID# 77 or ID# 81 insteadd Clarke 1866
        /// </summary>
        Hawaii = 19,
        /// <summary>
        /// 45 -290 -172 Do not use. Use ID# 78 or ID# 82 insteadd Clarke 1866
        /// </summary>
        Kauai = 20,
        /// <summary>
        /// 65 -290 -190 Do not use. Use ID# 79 or ID# 83 insteadd Clarke 1866
        /// </summary>
        Maui = 21,
        /// <summary>
        /// 56 -284 -181 Do not use. Use ID# 80 or ID# 84 insteadd Clarke 1866
        /// </summary>
        Oahu = 22,
        /// <summary>
        /// -333 -222 114 Herat North (Afghanistan) International 1924
        /// </summary>
        Herat = 23,
        /// <summary>
        /// -73 46 -86 Hjorsey 1955 (Iceland) International 1924
        /// </summary>
        Hjors = 24,
        /// <summary>
        /// -156 -271 -189 Hong Kong 1963 International 1924
        /// </summary>
        Hongk = 25,
        /// <summary>
        /// -634 -549 -201 This datum has been updated, see ID# 68c International 1924
        /// </summary>
        Hutzu = 26,
        /// <summary>
        /// 289 734 257 Do not use. Use ID# 69 or ID# 70 insteadd Everest (EA)
        /// </summary>
        India = 27,
        /// <summary>
        /// 506 -122 611 Do not use. Use ID# 71 insteadd Modified Airy
        /// </summary>
        Ire65 = 28,
        /// <summary>
        /// -11 851 5 Kertau 1948 (West Malaysia and Singapore) Everest (EE)
        /// </summary>
        Kerta = 29,
        /// <summary>
        /// -97 787 86 Kandawala (Sri Lanka) Everest (EA)
        /// </summary>
        Kanda = 30,
        /// <summary>
        /// -90 40 88 Liberia 1964 Clarke 1880
        /// </summary>
        Liber = 31,
        /// <summary>
        /// -133 -77 -51 Do not use. Use ID# 72 insteadd Clarke 1866
        /// </summary>
        Luzon = 32,
        /// <summary>
        /// -133 -70 -72 This datum has been updated, see ID# 73c Clarke 1866
        /// </summary>
        Minda = 33,
        /// <summary>
        /// 31 146 47 Merchich (Morocco) Clarke 1880
        /// </summary>
        Merch = 34,
        /// <summary>
        /// -231 -196 482 This datum has been updated, see ID# 74c Clarke 1880
        /// </summary>
        Nahr = 35,
        /// <summary>
        /// 0 0 0 N. American 1983 (Includes Areas 37-42) GRS-80
        /// </summary>
        Nad83 = 36,
        /// <summary>
        /// -10 158 187 N. American Canada 1927 Clarke 1866
        /// </summary>
        Canada = 37,
        /// <summary>
        /// -5 135 172 N. American Alaska 1927 Clarke 1866
        /// </summary>
        Alaska = 38,
        /// <summary>
        /// -8 160 176 N. American Conus 1927 Clarke 1866
        /// </summary>
        Nad27 = 39,
        /// <summary>
        /// -7 152 178 This datum has been updated, see ID# 75c Clarke 1866
        /// </summary>
        Caribb = 40,
        /// <summary>
        /// -12 130 190 N. American Mexico Clarke 1866
        /// </summary>
        Mexico = 41,
        /// <summary>
        /// 0 125 194 N. American Central America Clarke 1866
        /// </summary>
        Camer = 42,
        /// <summary>
        /// -92 -93 122 Nigeria (Minna) Clarke 1880
        /// </summary>
        Minna = 43,
        /// <summary>
        /// -346 -1 224 Oman Clarke 1880
        /// </summary>
        Oman = 44,
        /// <summary>
        /// 11 72 -101 Puerto Rica and Virgin Islands Clarke 1866
        /// </summary>
        Puerto = 45,
        /// <summary>
        /// 164 138 -189 Qornoq (South Greenland) International 1924
        /// </summary>
        Qorno = 46,
        /// <summary>
        /// -255 -65 9 Rome 1940 Sardinia Island International 1924
        /// </summary>
        Rome = 47,
        /// <summary>
        /// -134 229 -29 South American Chua Astro (Paraguay) International 1924
        /// </summary>
        Chua = 48,
        /// <summary>
        /// -288 175 -376 South American (Provisional 1956) International 1924
        /// </summary>
        Sam56 = 49,
        /// <summary>
        /// -57 1 -41 South American 1969 S. American 1969
        /// </summary>
        Sam69 = 50,
        /// <summary>
        /// -148 136 90 S. American Campo Inchauspe (Argentina) International 1924
        /// </summary>
        Campo = 51,
        /// <summary>
        /// -206 172 -6 South American Corrego Alegre (Brazil) International 1924
        /// </summary>
        Sacor = 52,
        /// <summary>
        /// -155 171 37 South American Yacare (Uruguay) International 1924
        /// </summary>
        Yacar = 53,
        /// <summary>
        /// -189 -242 -91 Tananarive Observatory 1925 (Madagascar) International 1924
        /// </summary>
        Tanan = 54,
        /// <summary>
        /// -689 691 -46 This datum has been updated, see ID# 85c Everest (EB)
        /// </summary>
        Timba = 55,
        /// <summary>
        /// -128 481 664 This datum has been updated, see ID# 86c Bessel 1841
        /// </summary>
        Tokyo = 56,
        /// <summary>
        /// -632 438 -609 Tristan Astro 1968 (Tristan du Cunha) International 1924
        /// </summary>
        Trist = 57,
        /// <summary>
        /// 51 391 -36 Viti Levu 1916 (Fiji Islands) Clarke 1880
        /// </summary>
        Viti = 58,
        /// <summary>
        /// 101 52 -39 This datum has been updated, see ID# 67c Hough 1960
        /// </summary>
        Wak60 = 59,
        /// <summary>
        /// 0 0 4.5 World Geodetic System - 72 WGS72
        /// </summary>
        Wgs72 = 60,
        /// <summary>
        /// 0 0 0 World Geodetic System - 84 WGS84
        /// </summary>
        Wgs84 = 61,
        /// <summary>
        /// -265 120 -358 Zanderidj (Surinam) International 1924
        /// </summary>
        Zande = 62,
        /// <summary>
        /// 0 0 0 User Defined Datum Defaults User a
        /// </summary>
        User = 63,
        /// <summary>
        /// Time-variable 7 parameter transformation
        /// </summary>
        Csrs = 64,
        /// <summary>
        /// -166 -15 204 Adindan (Ethiopia, Mali, Senegal & Sudan)c Clarke 1880
        /// </summary>
        Adim = 65,
        /// <summary>
        /// -160 -6 -302 ARC 1960 (Kenya, Tanzania)c Clarke 1880
        /// </summary>
        Arsm = 66,
        /// <summary>
        /// 102 52 -38 Wake-Eniwetok (Marshall Islands)c Hough 1960
        /// </summary>
        Enw = 67,
        /// <summary>
        /// -637 -549 -203 Hu-Tzu-Shan (Taiwan)c International 1924
        /// </summary>
        Htn = 68,
        /// <summary>
        /// 282 726 254 Indian (Bangladesh)d Everest (EA)
        /// </summary>
        Indb = 69,
        /// <summary>
        /// 295 736 257 Indian (India, Nepal)d Everest (EA)
        /// </summary>
        Indi = 70,
        /// <summary>
        /// 506 -122 611 Ireland 1965 d Modified Airy
        /// </summary>
        Irl = 71,
        /// <summary>
        /// -133 -77 -51 Luzon (Philippines excluding Mindanoa Is.)de Clarke 1866
        /// </summary>
        Luza = 72,
        /// <summary>
        /// -133 -79 -72 Mindanoa Islandc Clarke 1866
        /// </summary>
        Luzb = 73,
        /// <summary>
        /// -243 -192 477 Nahrwan (Saudi Arabia)c Clarke 1880
        /// </summary>
        Nahc = 74,
        /// <summary>
        /// -3 142 183 N. American Caribbeanc Clarke 1866
        /// </summary>
        Nasp = 75,
        /// <summary>
        /// 375 -111 431 Great Britain 1936 (Ordinance Survey)d Airy 1830
        /// </summary>
        Ogbm = 76,
        /// <summary>
        /// 89 -279 -183 Hawaiian Hawaii d Clarke 1866
        /// </summary>
        Ohaa = 77,
        /// <summary>
        /// 45 -290 -172 Hawaiian Kauaiid Clarke 1866
        /// </summary>
        Ohab = 78,
        /// <summary>
        /// 65 -290 -190 Hawaiian Mauid Clarke 1866
        /// </summary>
        Ohac = 79,
        /// <summary>
        /// 58 -283 -182 Hawaiian Oahud Clarke 1866
        /// </summary>
        Ohad = 80,
        /// <summary>
        /// 229 -222 -348 Hawaiian Hawaiid International 1924
        /// </summary>
        Ohia = 81,
        /// <summary>
        /// 185 -233 -337 Hawaiian Kauaid International 1924
        /// </summary>
        Ohib = 82,
        /// <summary>
        /// 205 -233 -355 Hawaiian Mauid International 1924
        /// </summary>
        Ohic = 83,
        /// <summary>
        /// 198 -226 -347 Hawaiian Oahud International 1924
        /// </summary>
        Ohid = 84,
        /// <summary>
        /// -679 669 -48 Timbalai (Brunei and East Malaysia) 1948c Everest (EB)
        /// </summary>
        Til = 85,
        /// <summary>
        /// -148 507 685 Tokyo (Japan, Korea and Okinawa)c Bessel 1841
        /// </summary>
        Toym = 86,
    }
}