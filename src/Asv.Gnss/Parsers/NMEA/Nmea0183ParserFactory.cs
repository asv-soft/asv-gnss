using System;
using System.Collections.Generic;

namespace Asv.Gnss
{
    public static class Nmea0183ParserFactory
    {
        /// <summary>
        /// Gets represents a collection of default Nmea0183 message types.
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
                yield return () => new Nmea0183MessageGBS();
                yield return () => new Nmea0183MessageRMC();
                yield return () => new Nmea0183MessageVTG();
            }
        }

        public static IEnumerable<(
            Nmea0183GetMessageIdDelegate,
            Func<Nmea0183MessageBase>
        )> DefaultProprietaryMessages
        {
            get
            {
                yield return (
                    Nmea0183ProprietaryMessageGRMZ.MessageIdGetter,
                    () => new Nmea0183ProprietaryMessageGRMZ()
                );
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

            foreach (var func in DefaultProprietaryMessages)
            {
                src.RegisterProprietary(func.Item1, func.Item2);
            }

            return src;
        }
    }
}
