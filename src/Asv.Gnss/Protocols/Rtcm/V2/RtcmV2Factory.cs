using System;
using System.Collections.Generic;

namespace Asv.Gnss;

public static class RtcmV2Factory
{
    public static IEnumerable<Func<RtcmV2MessageBase>> DefaultMessages
    {
        get
        {
            yield return () => new RtcmV2Message1();
            yield return () => new RtcmV2Message9();
            yield return () => new RtcmV2Message14();
            yield return () => new RtcmV2Message15();
            yield return () => new RtcmV2Message17();
            yield return () => new RtcmV2Message21();
            yield return () => new RtcmV2Message31();
        }
    }
}
