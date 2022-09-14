using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Asv.Gnss
{


    public static class UbxFactory
    {
        public static IEnumerable<Func<UbxMessageBase>> DefaultMessages
        {
            get
            {
                yield return () => new UbxAckAck();
                yield return () => new UbxAckNak();
                yield return () => new UbxCfgAnt();

                yield return () => new UbxInfDebug();
                yield return () => new UbxInfError();
                yield return () => new UbxInfNotice();
                yield return () => new UbxInfTest();
            }
        }

        public static Task<bool> PoolUbxCfgAnt(this IGnssConnection src, CancellationToken cancel)
        {
            return src.Send(new UbxCfgAntPool(), cancel);
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