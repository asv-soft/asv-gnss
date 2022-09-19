using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Asv.Gnss
{


    public static class UbxFactory
    {
        public static IEnumerable<Func<UbxMessageBase>> DefaultPoolMessages
        {
            get
            {
                yield return () => new UbxCfgPrtPool();
                yield return () => new UbxCfgAntPool();
                yield return () => new UbxCfgRatePool();
                yield return () => new UbxCfgNav5Pool();
                yield return () => new UbxCfgMsgPool();
                yield return () => new UbxMonHwPool();
                yield return () => new UbxCfgTMode3Pool();
                yield return () => new UbxNavSatPool();
                yield return () => new UbxNavPvtPool();

            }
        }

        public static IEnumerable<Func<UbxMessageBase>> DefaultMessages
        {
            get
            {
                yield return () => new UbxAckAck();
                yield return () => new UbxAckNak();
                yield return () => new UbxCfgRst();
                yield return () => new UbxCfgAnt();
                yield return () => new UbxCfgPrt();
                yield return () => new UbxInfDebug();
                yield return () => new UbxInfError();
                yield return () => new UbxInfNotice();
                yield return () => new UbxInfTest();
                yield return () => new UbxCfgRate();
                yield return () => new UbxCfgNav5();
                yield return () => new UbxCfgMsg();
                yield return () => new UbxCfgCfg();
                yield return () => new UbxMonHw();
                yield return () => new UbxCfgTMode3();
                yield return () => new UbxNavSat();
                yield return () => new UbxNavPvt();

            }
        }

        public static UbxBinaryParser RegisterDefaultMessages(this UbxBinaryParser src)
        {
            foreach (var func in DefaultMessages)
            {
                src.Register(func);
            }

            return src;
        }
    }
}