using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Asv.Gnss
{
    /// <summary>
    /// UbxFactory class provides functions to create default instances of UbxMessageBase derived classes and register them with a UbxBinaryParser.
    /// </summary>
    public static class UbxFactory
    {
        /// <summary>
        /// Returns a collection of functions that creates instances of various UBX message types.
        /// </summary>
        /// <returns>
        /// An IEnumerable of Func&lt;UbxMessageBase&gt;, where each Func&lt;UbxMessageBase&gt; represents a function that creates a specific UBX message instance.
        /// </returns>
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
                yield return () => new UbxMonVerPool();
                yield return () => new UbxNavSvinPool();
            }
        }

        /// <summary>
        /// Gets the collection of default messages.
        /// </summary>
        /// <returns>
        /// An enumerable collection of functions that create instances of UbxMessageBase objects.
        /// </returns>
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
                yield return () => new UbxMonVer();
                yield return () => new UbxNavSvin();
            }
        }

        /// <summary>
        /// Register the default messages in the UbxBinaryParser.
        /// </summary>
        /// <param name="src">The UbxBinaryParser object.</param>
        /// <returns>The UbxBinaryParser object with the default messages registered.</returns>
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
