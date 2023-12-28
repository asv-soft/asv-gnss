using System;
using System.Collections.Generic;

namespace Asv.Gnss
{
    public static class Nmea0183ParserFactory
    {
        /// <summary>
        /// Represents a collection of default Nmea0183 message types.
        /// </summary>
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

        /// <summary>
        /// Registers the default messages to the Nmea0183Parser instance.
        /// </summary>
        /// <param name="src">The Nmea0183Parser instance.</param>
        /// <returns>The Nmea0183Parser instance with the default messages registered.</returns>
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