using System.Collections.Generic;

namespace Asv.Gnss
{
    /// <summary>
    /// GSV Satellites in view
    ///
    /// 1) total number of messages
    /// 2) message number
    /// 3) satellites in view
    /// 4) satellite number
    /// 5) elevation in degrees
    /// 6) azimuth in degrees to true
    /// 7) SNR in dB
    /// more satellite infos like 4)-7)
    /// n) Checksum
    /// </summary>
    public class Nmea0183MessageGSV : Nmea0183MessageBase
    {
        /// <summary>
        /// Represents the GNSS message ID.
        /// </summary>
        public const string GnssMessageId = "GSV";

        /// Gets the message ID associated with this message.
        /// @return The message ID as a string.
        /// /
        public override string MessageId => GnssMessageId;

        /// <summary>
        /// Internal method to deserialize an array of strings into the object properties.
        /// </summary>
        /// <param name="items">Array of strings representing the properties of the object</param>
        protected override void InternalDeserializeFromStringArray(string[] items)
        {
            if (!string.IsNullOrEmpty(items[1]))
                TotalNumberOfMsg = int.Parse(items[1]);
            if (!string.IsNullOrEmpty(items[2]))
                MessageNumber = int.Parse(items[2]);
            if (!string.IsNullOrEmpty(items[3]))
                SatellitesInView = int.Parse(items[3]);

            var length = (items.Length - 4) / 4;
            var satellites = new List<Satellite>();
            for (var i = 4; i < 4 + length * 4; i += 4)
            {
                int number;
                var elevationDeg = 0;
                var azimuthDeg = 0;
                var snrdB = 0;

                if (!string.IsNullOrEmpty(items[i]))
                    number = int.Parse(items[i]);
                else
                    continue;
                if (!string.IsNullOrEmpty(items[i + 1]))
                    elevationDeg = int.Parse(items[i + 1]);
                if (!string.IsNullOrEmpty(items[i + 2]))
                    azimuthDeg = int.Parse(items[i + 2]);
                if (!string.IsNullOrEmpty(items[i + 3]))
                    snrdB = int.Parse(items[i + 3]);
                var sat = new Satellite
                {
                    Number = number,
                    ElevationDeg = elevationDeg,
                    AzimuthDeg = azimuthDeg,
                    SnrdB = snrdB,
                };
                if (Nmea0183Helper.GetPrnFromNmeaSatId(SourceId, number, out var prn, out var nav))
                {
                    sat.ExtPRN = prn;
                    sat.ExtNavSys = nav;
                }
                satellites.Add(sat);
            }
            Satellites = satellites.ToArray();
        }

        /// Gets or sets the total number of messages.
        public int TotalNumberOfMsg { get; set; }

        /// <summary>
        /// Gets or sets the message number.
        /// </summary>
        /// <value>
        /// The message number.
        /// </value>
        public int MessageNumber { get; set; }

        /// <summary>
        /// Gets or sets the number of satellites in view.
        /// </summary>
        /// <remarks>
        /// The SatellitesInView property represents the number of satellites currently in view of the GPS receiver.
        /// This property is typically used in navigation systems to determine the availability and reliability of satellite signals.
        /// </remarks>
        public int SatellitesInView { get; set; }

        /// <summary>
        /// Represents a satellite object.
        /// </summary>
        public class Satellite
        {
            /// <summary>
            /// Gets or sets the value of the Number property.
            /// </summary>
            /// <value>
            /// An integer representing the value of the Number.
            /// </value>
            public int Number { get; set; }

            /// <summary>
            /// The elevation in degrees.
            /// </summary>
            /// <value>
            /// An integer representing the elevation in degrees.
            /// </value>
            public int ElevationDeg { get; set; }

            /// <summary>
            /// Gets or sets the azimuth degree value.
            /// </summary>
            /// <value>
            /// The azimuth degree value.
            /// </value>
            public int AzimuthDeg { get; set; }

            /// <summary>
            /// Gets or sets the SNR (Signal-to-Noise Ratio) in decibels.
            /// </summary>
            /// <value>
            /// The SNR value in decibels.
            /// </value>
            public int SnrdB { get; set; }

            // extended computed values
            /// <summary>
            /// Gets or sets the extended PRN (Pseudo-Random Number) value.
            /// </summary>
            /// <value>
            /// The extended PRN value.
            /// </value>
            public int? ExtPRN { get; set; }

            /// <summary>
            /// Gets or sets the external navigation system.
            /// </summary>
            /// <value>
            /// The external navigation system. It is represented as <see cref="NmeaNavigationSystemEnum"/>.
            /// </value>
            public NmeaNavigationSystemEnum? ExtNavSys { get; set; }
        }

        /// <summary>
        /// Gets or sets the array of Satellites.
        /// </summary>
        /// <value>
        /// An array of Satellite objects.
        /// </value>
        public Satellite[] Satellites { get; set; }
    }
}
