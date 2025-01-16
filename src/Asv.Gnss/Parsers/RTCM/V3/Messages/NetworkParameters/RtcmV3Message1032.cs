using System;
using Asv.IO;

namespace Asv.Gnss
{
    public class RtcmV3Message1032 : RtcmV3MessageBase
    {
        public const int RtcmMessageRecAntId = 1032;
        public override ushort MessageId => RtcmMessageRecAntId;
        public override string Name => "Physical Reference Station Position";

        protected override void DeserializeContent(
            ReadOnlySpan<byte> buffer,
            ref int bitIndex,
            int messageLength
        )
        {
            var rr = new double[3];
            var re = new double[3];
            var pos = new double[3];

            NonPhysicalReferenceStationID = SpanBitHelper.GetBitU(buffer, ref bitIndex, 12);
            PhysicalReferenceStationID = SpanBitHelper.GetBitU(buffer, ref bitIndex, 12);

            ITRFEpochYear = (byte)SpanBitHelper.GetBitU(buffer, ref bitIndex, 6);
            rr[0] = RtcmV3Helper.GetBits38(buffer, ref bitIndex);
            rr[1] = RtcmV3Helper.GetBits38(buffer, ref bitIndex);
            rr[2] = RtcmV3Helper.GetBits38(buffer, ref bitIndex);

            for (var i = 0; i < 3; i++)
            {
                re[i] = rr[i] * 0.0001;
            }

            RtcmV3Helper.EcefToPos(re, pos);

            X = rr[0] * 0.0001;
            Y = rr[1] * 0.0001;
            Z = rr[2] * 0.0001;

            Latitude = pos[0] * RtcmV3Helper.R2D;
            Longitude = pos[1] * RtcmV3Helper.R2D;
            Altitude = pos[2];
        }

        /// <summary>
        /// Gets or sets antenna Reference Point ECEF-X.
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Gets or sets antenna Reference Point ECEF-Y.
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// Gets or sets antenna Reference Point ECEF-Z.
        /// </summary>
        public double Z { get; set; }

        /// <summary>
        /// Gets or sets antenna Reference Point WGS84-Latitude.
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Gets or sets antenna Reference Point WGS84-Longitude.
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// Gets or sets antenna Reference Point WGS84-Altitude.
        /// </summary>
        public double Altitude { get; set; }

        /// <summary>
        /// Gets or sets since this field is reserved, all bits should be set to zero for now.
        /// However, since the value is subject to change in future versions,
        /// decoding should not rely on a zero value.
        /// The ITRF realization year identifies the datum definition used for
        /// coordinates in the message.
        /// </summary>
        public byte ITRFEpochYear { get; set; }

        /// <summary>
        /// Gets or sets the Reference Station ID is determined by the service provider. Its
        /// primary purpose is to link all message data to their unique sourceName. It is
        /// useful in distinguishing between desired and undesired data in cases
        /// where more than one service may be using the same data link
        /// frequency. It is also useful in accommodating multiple reference
        /// stations within a single data link transmission.
        /// In reference network applications the Reference Station ID plays an
        /// important role, because it is the link between the observation messages
        /// of a specific reference station and its auxiliary information contained in
        /// other messages for proper operation. Thus the Service Provider should
        /// ensure that the Reference Station ID is unique within the whole
        /// network, and that ID’s should be reassigned only when absolutely
        /// necessary.
        /// Service Providers may need to coordinate their Reference Station ID
        /// assignments with other Service Providers in their region in order to
        /// avoid conflicts. This may be especially critical for equipment
        /// accessing multiple services, depending on their services and means of
        /// information distribution.
        /// </summary>
        public uint NonPhysicalReferenceStationID { get; set; }

        /// <summary>
        /// Gets or sets the Physical Reference Station ID specifies the station ID of a real
        /// reference station, when the data stream itself is based on a nonphysical reference station.
        /// Consequently, for the Physical Reference Station ID the same notes
        /// apply as for DF003.
        /// </summary>
        public uint PhysicalReferenceStationID { get; set; }
    }
}
