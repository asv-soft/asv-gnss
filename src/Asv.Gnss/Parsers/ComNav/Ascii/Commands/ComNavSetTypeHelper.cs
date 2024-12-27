namespace Asv.Gnss
{
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
}
