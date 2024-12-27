using System;
using System.Globalization;
using System.Text;
using Asv.IO;

namespace Asv.Gnss
{
    /// <summary>
    /// 1) Time (UTC)
    /// 2) Track made good, degrees true
    /// 4) Track made good, degrees magnetic
    /// 6) Speed over ground, knots
    /// 8) Speed over ground, kph
    /// 10) Positioning system mode.
    /// </summary>
    public class Nmea0183MessageVTG : Nmea0183MessageBase
    {
        private const byte TrueTrack = 0x54; // 'T'
        private const byte MagneticTrack = 0x4D; // 'M'
        private const byte KnotsUnit = 0x4E; // 'N'
        private const byte KphUnit = 0x4B; // 'K'

        /// <summary>
        /// Represents the GNSS message ID.
        /// </summary>
        public const string GnssMessageId = "VTG";

        /// <summary>
        /// Gets the message ID associated with this message.
        /// <returns>The message ID as a string.</returns>
        /// </summary>
        public override string MessageId => GnssMessageId;

        protected override void InternalDeserializeFromStringArray(string[] items)
        {
            TimeUtc = Nmea0183Helper.ParseTime(items[1]);
            TrackMadeGoodDegreesTrue = double.TryParse(
                items[2],
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out var trueTrack
            )
                ? trueTrack
                : double.NaN;
            TrackMadeGoodDegreesMagnetic = double.TryParse(
                items[4],
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out var magneticTrack
            )
                ? magneticTrack
                : double.NaN;
            SpeedOverGroundKnots = double.TryParse(
                items[6],
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out var speedKnots
            )
                ? speedKnots
                : double.NaN;
            SpeedOverGroundKph = double.TryParse(
                items[8],
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out var speedKph
            )
                ? speedKph
                : double.NaN;
            if (items.Length > 10)
            {
                PositioningSystemMode = items[10];
            }
        }

        protected override void InternalSerialize(ref Span<byte> buffer, Encoding encoding)
        {
            Nmea0183Helper.SerializeTime(TimeUtc).CopyTo(ref buffer, encoding);
            InsertSeparator(ref buffer);
            var trueTrack = double.IsNaN(TrackMadeGoodDegreesTrue)
                ? string.Empty
                : Math.Round(TrackMadeGoodDegreesTrue, 2)
                    .ToString("000.00", CultureInfo.InvariantCulture);
            trueTrack.CopyTo(ref buffer, encoding);
            InsertSeparator(ref buffer);
            InsertByte(ref buffer, TrueTrack);
            InsertSeparator(ref buffer);

            var magneticTrack = double.IsNaN(TrackMadeGoodDegreesMagnetic)
                ? string.Empty
                : Math.Round(TrackMadeGoodDegreesMagnetic, 2)
                    .ToString("000.00", CultureInfo.InvariantCulture);
            magneticTrack.CopyTo(ref buffer, encoding);
            InsertSeparator(ref buffer);
            InsertByte(ref buffer, MagneticTrack);
            InsertSeparator(ref buffer);

            var speedKnots = double.IsNaN(SpeedOverGroundKnots)
                ? string.Empty
                : Math.Round(SpeedOverGroundKnots, 2)
                    .ToString("000.00", CultureInfo.InvariantCulture);
            speedKnots.CopyTo(ref buffer, encoding);
            InsertSeparator(ref buffer);
            InsertByte(ref buffer, KnotsUnit);
            InsertSeparator(ref buffer);

            var speedKph = double.IsNaN(SpeedOverGroundKph)
                ? string.Empty
                : Math.Round(SpeedOverGroundKph, 2)
                    .ToString("000.00", CultureInfo.InvariantCulture);
            speedKph.CopyTo(ref buffer, encoding);
            InsertSeparator(ref buffer);
            InsertByte(ref buffer, KphUnit);
            InsertSeparator(ref buffer);

            if (string.IsNullOrWhiteSpace(PositioningSystemMode))
            {
                return;
            }

            InsertSeparator(ref buffer);
            PositioningSystemMode.CopyTo(ref buffer, encoding);
        }

        public DateTime? TimeUtc { get; set; }
        public double TrackMadeGoodDegreesTrue { get; set; }
        public double TrackMadeGoodDegreesMagnetic { get; set; }
        public double SpeedOverGroundKnots { get; set; }
        public double SpeedOverGroundKph { get; set; }
        public string PositioningSystemMode { get; set; } = "A";
    }
}
