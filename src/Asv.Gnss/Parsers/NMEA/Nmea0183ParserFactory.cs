using System;
using System.Collections.Generic;

namespace Asv.Gnss
{
    public static class Nmea0183ParserFactory
    {
        public static IEnumerable<Func<Nmea0183MessageBase>> DefaultMessages
        {
            get
            {
                yield return () => new Nmea0183MessageGGA();
                yield return () => new Nmea0183MessageGLL();
                yield return () => new Nmea0183MessageGSA();
                yield return () => new Nmea0183MessageGST();
                yield return () => new Nmea0183MessageGSV();
            }
        }

        public static Nmea0183Parser RegisterDefaultMessages(this Nmea0183Parser src)
        {
            foreach (var func in DefaultMessages)
            {
                src.Register(func);
            }
            return src;
        }
    }
}