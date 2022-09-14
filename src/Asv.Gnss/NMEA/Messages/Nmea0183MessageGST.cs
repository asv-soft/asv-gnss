﻿using System;

namespace Asv.Gnss
{
    /// <summary>
    /// GST - GPS Pseudorange Noise Statistics
    ///               1    2 3 4 5 6 7 8   9
    ///               |    | | | | | | |   |
    /// $ --GST,hhmmss.ss,x,x,x,x,x,x,x* hh<CR><LF>
    /// </summary>
    public class Nmea0183MessageGST : Nmea0183MessageBase
    {
        public const string NmeaMessageId = "GST";

        public override string MessageId => NmeaMessageId;

        protected override void InternalDeserializeFromStringArray(string[] items)
        {
            Time = Nmea0183Helper.ParseTime(items[1]);
            RmsSd = Nmea0183Helper.ParseDouble(items[2]);
            SdSemiMajorAxis = Nmea0183Helper.ParseDouble(items[3]);
            SdSemiMinorAxis = Nmea0183Helper.ParseDouble(items[4]);
            OrientationSemiMajorAxis = Nmea0183Helper.ParseDouble(items[5]);
            SdLatitude = Nmea0183Helper.ParseDouble(items[6]);
            SdLongitude = Nmea0183Helper.ParseDouble(items[7]);
            SdAltitude = Nmea0183Helper.ParseDouble(items[8]);
        }
        /// <summary>
        /// TC time of associated GGA fix
        /// </summary>
        public DateTime Time { get; set; }
        /// <summary>
        /// Total RMS standard deviation of ranges inputs to the navigation solution
        /// </summary>
        public double RmsSd { get; set; }
        /// <summary>
        /// Standard deviation (meters) of semi-major axis of error ellipse
        /// </summary>
        public double SdSemiMajorAxis { get; set; }
        /// <summary>
        /// Standard deviation (meters) of semi-minor axis of error ellipse
        /// </summary>
        public double SdSemiMinorAxis { get; set; }
        /// <summary>
        /// Orientation of semi-major axis of error ellipse (true north degrees)
        /// </summary>
        public double OrientationSemiMajorAxis { get; set; }
        /// <summary>
        /// Standard deviation (meters) of latitude error
        /// </summary>
        public double SdLatitude { get; set; }
        /// <summary>
        /// Standard deviation (meters) of longitude error
        /// </summary>
        public double SdLongitude { get; set; }
        /// <summary>
        /// Standard deviation (meters) of altitude error
        /// </summary>
        public double SdAltitude { get; set; }

    }
}