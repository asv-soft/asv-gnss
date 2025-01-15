using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asv.Gnss
{
    public enum ComNavSetTypeEnum
    {
        /// <summary>
        /// Param1: SYNCH or ASYNCH Set RTK in synchronous mode or asynchronous mode
        /// </summary>
        DIFFMATCHMODE,

        /// <summary>
        /// Param1: ON = Enable atom clock; OFF = Disable atom clock
        /// </summary>
        ATOM,

        /// <summary>
        /// Param1 is known antenna height of a receiver
        /// </summary>
        ANTHIGH,

        /// <summary>
        /// Param1:
        /// ON = start a static file collection;
        /// OFF = end a static file collection.
        /// Start or end static data collection
        /// </summary>
        STATIC,

        /// <summary>
        /// Param1 … Param6:
        /// A: the long axle of the earth; 1/F:
        /// F is the Earth flat rate;
        /// B0: reference latitude(in degree);
        /// L0: reference longitude(in degree);
        /// N0: reference north coordinate;
        /// E0: reference east coordinate
        /// </summary>
        PJKPARA,

        /// <summary>
        /// Param1 … Param3 (X (WGS84), Y (WGS84), Z (WGS84))
        /// In timing mode, this command is used to set reference station coordinates as x, y and z in WGS84 coordination frame.
        /// </summary>
        TIMINGREFXYZ,

        /// <summary>
        /// Param1: PVT or TIMING; (SET WORKMODE TIMING + SET TIMINGREFXYZ X Y Z or SET WORKMODE PVT)
        /// </summary>
        WORKMODE,

        /// <summary>
        /// Param1 Param1: B1I, B2I or B3I, AUTO
        /// </summary>
        BD2PVTOBS,

        /// <summary>
        /// Param1 is valid CPU frequency in Hz: 208, 416(default), 624, 806.
        /// </summary>
        CPUFREQ,

        /// <summary>
        /// Param1 is valid PVT frequency in Hz: 1, 2, 5(default), 10, 20.
        /// </summary>
        PVTFREQ,

        /// <summary>
        /// Param1 is valid RTK frequency in Hz: 1, 2, 5(default), 10.
        /// Notice: please keep RTK frequency is not higher than PVT frequency.
        /// </summary>
        RTKFREQ,

        /// <summary>
        /// Param1 is a fixed baseline length of a rover (>0)
        /// </summary>
        BASELINELENGTH,

        /// <summary>
        /// Param1:
        /// ON = to carry out the modulation;
        /// OFF = no modulation (default)
        /// </summary>
        MODIFYCPTOPR,

        /// <summary>
        /// Param1 [Param2] [Param3]
        /// Param1: smooth enable switch, ON/OFF;
        /// Param2: smoothing time contant (10 ~ 200 seconds. Its default value is 50s.);
        /// Param3: Tracking time threshhold (0 ~ 60 seconds. Its default value is 15s)
        /// </summary>
        CPSMOOTHPR,

        /// <summary>
        /// Param1 is RTK Obs mode: AUTO, MANUAL [Default]
        /// </summary>
        RTKOBSMODE,

        /// <summary>
        /// Param1 is a vector length of a rover (>0)
        /// </summary>
        VECTORLENGTH,

        /// <summary>
        /// Param1 (codetype) is: pcode: P code; ccode: C code; auto: Track the L2C automatically (AUTO, MANUAL[Default]).
        /// The setting status can be checked by the command: logcodetype
        /// </summary>
        GPSL2CODETYPE,

        /// <summary>
        /// Param1 (codetype) is: pcode: P code; ccode: C code; Auto: N/A (Default mode: pcode)
        /// </summary>
        GLONASSCODETYPE,

        /// <summary>
        /// ON
        /// </summary>
        EXTERNALCOORD,

        /// <summary>
        /// switcher fileperiod sampleint eraseint:
        /// switcher: ENABLE/DISABLE,
        /// fileperiod: file saving period (hour);
        /// sampleint: file saving sampling interval (sec);
        /// eraseint: file erasing time interval (sec)
        /// </summary>
        CYCLESAVE,

        /// <summary>
        /// mode portA portB Mode: “master” - base station, “slave” - rover;
        /// PortA: port for receiving the differential data from the base station (com1, com2, com3);
        /// PortB: port for sending differential messages from base station;
        /// Interval interval for sending the differential messages. The parameter is float pointing
        /// </summary>
        STATIONMODE,

        /// <summary>
        /// ON/OFF ON: active the EMMC chip; OFF: close the EMMC chip
        /// </summary>
        EMMC,

        /// <summary>
        /// XX XX: is the AODC value
        /// </summary>
        BD2PVTMAXAODC,

        /// <summary>
        /// XX XX: is the AODE value
        /// </summary>
        BD2PVTMAXAODE,

        /// <summary>
        /// Param1 can be set with gauss and utm.
        /// Gauss: means setting the projection type as Gauss-Boaga projectiontype.
        /// utm: universal transverse Mercator projection.
        /// </summary>
        PROJECTIONTYPE,

        /// <summary>
        /// Keyword: COMNAV, STANDARD, NORMAL, LONG. COMNAV: default setting.
        /// </summary>
        nmeamsgformat,

        /// <summary>
        /// gx p1 p2 …… p14 gx: GLONASS frequency index. gx = 1 means G1, gx = 2 means G2; pn from -700 ~ 600
        /// Or keyword DEFAULT. Set both corrections of G1 and G2 in all channels to be 0.
        /// </summary>
        GLOPRBIAS,

        /// <summary>
        /// gx chan p. gx = 1 means G1, gx = 2 means G2; chan from -7 ~ 6; pn from -700 ~ 600
        /// </summary>
        GLOCHANPRBIAS,

        /// <summary>
        /// switch port. Forwarding RTCMV3 differential data.
        /// switch: on/off;
        /// port: com1/com2/com3/com4
        /// </summary>
        relayrtcmv3,

        /// <summary>
        /// param1: lband/rtcm3/bqrtcm3 (Bqrtcm3 is a new mode: the SSR correction number of RTCM3 of markweapon group)
        /// </summary>
        pppsource,

        /// <summary>
        /// enable/disable
        /// </summary>
        ledlowon,

        /// <summary>
        /// enable
        /// </summary>
        headingledshow,

        /// <summary>
        /// param1:
        /// 1 - Mobile base station, variable base line length;
        /// 2 - Mobile base station mode with fixed baseline length.
        /// </summary>
        vectorlenmode,
    }

    public static class ComNavSetTypeHelper
    {
        public static string GetSetTypeName(this ComNavSetTypeEnum src)
        {
            return src switch
            {
                ComNavSetTypeEnum.DIFFMATCHMODE => "DIFFMATCHMODE",
                ComNavSetTypeEnum.ATOM => "ATOM",
                ComNavSetTypeEnum.ANTHIGH => "ANTHIGH",
                ComNavSetTypeEnum.STATIC => "STATIC",
                ComNavSetTypeEnum.PJKPARA => "PJKPARA",
                ComNavSetTypeEnum.TIMINGREFXYZ => "TIMINGREFXYZ",
                ComNavSetTypeEnum.WORKMODE => "WORKMODE",
                ComNavSetTypeEnum.BD2PVTOBS => "BD2PVTOBS",
                ComNavSetTypeEnum.CPUFREQ => "CPUFREQ",
                ComNavSetTypeEnum.PVTFREQ => "PVTFREQ",
                ComNavSetTypeEnum.RTKFREQ => "RTKFREQ",
                ComNavSetTypeEnum.BASELINELENGTH => "BASELINELENGTH",
                ComNavSetTypeEnum.MODIFYCPTOPR => "MODIFYCPTOPR",
                ComNavSetTypeEnum.CPSMOOTHPR => "CPSMOOTHPR",
                ComNavSetTypeEnum.RTKOBSMODE => "RTKOBSMODE",
                ComNavSetTypeEnum.VECTORLENGTH => "VECTORLENGTH",
                ComNavSetTypeEnum.GPSL2CODETYPE => "GPSL2CODETYPE",
                ComNavSetTypeEnum.GLONASSCODETYPE => "GLONASSCODETYPE",
                ComNavSetTypeEnum.EXTERNALCOORD => "EXTERNALCOORD",
                ComNavSetTypeEnum.CYCLESAVE => "CYCLESAVE",
                ComNavSetTypeEnum.STATIONMODE => "STATIONMODE",
                ComNavSetTypeEnum.EMMC => "EMMC",
                ComNavSetTypeEnum.BD2PVTMAXAODC => "BD2PVTMAXAODC",
                ComNavSetTypeEnum.BD2PVTMAXAODE => "BD2PVTMAXAODE",
                ComNavSetTypeEnum.PROJECTIONTYPE => "PROJECTIONTYPE",
                ComNavSetTypeEnum.nmeamsgformat => "nmeamsgformat",
                ComNavSetTypeEnum.GLOPRBIAS => "GLOPRBIAS",
                ComNavSetTypeEnum.GLOCHANPRBIAS => "GLOCHANPRBIAS",
                ComNavSetTypeEnum.relayrtcmv3 => "relayrtcmv3",
                ComNavSetTypeEnum.pppsource => "pppsource",
                ComNavSetTypeEnum.ledlowon => "ledlowon",
                ComNavSetTypeEnum.headingledshow => "headingledshow",
                ComNavSetTypeEnum.vectorlenmode => "vectorlenmode",
                _ => string.Empty,
            };
        }
    }

    public class ComNavAsciiSetCommand : ComNavAsciiCommandBase
    {
        public override string MessageId => "SET";
        public ComNavSetTypeEnum Type { get; set; }

        public IList<string> Params => _params;
        private readonly List<string> _params = new();

        protected override string SerializeToAsciiString()
        {
            var sb = new StringBuilder();
            sb.Append("SET");
            sb.Append($" {Type.GetSetTypeName()}");
            foreach (var param in _params.Where(param => !string.IsNullOrWhiteSpace(param)))
            {
                sb.Append($" {param}");
            }

            return sb.ToString();
        }
    }
}
