﻿namespace Asv.Gnss
{
    /// <summary>
    /// ASCII output with informational contents
    /// Supported on:
    /// • u-blox 8 / u-blox M8 protocol versions 15, 15.01, 16, 17, 18, 19, 19.1, 19.2, 20, 20.01,
    /// 20.1, 20.2, 20.3, 22, 22.01, 23 and 23.01
    /// 
    /// </summary>
    public class UbxInfNotice : UbxInfBase
    {
        public override string Name => "UBX-INF-NOTICE";
        public override byte Class => 0x04;
        public override byte SubClass => 0x02;
    }
}