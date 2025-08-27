using System;
using Asv.IO;

namespace Asv.Gnss;

public abstract class RtcmV3Message1005and1006 : RtcmV3MessageBase
{
    protected override void InternalDeserialize(ReadOnlySpan<byte> buffer, ref int bitIndex)
    {
        var rr = new double[3];
        var re = new double[3];
        var pos = new double[3];

        ReferenceStationID = SpanBitHelper.GetBitU(buffer, ref bitIndex, 12);

        ITRF = (byte)SpanBitHelper.GetBitU(buffer, ref bitIndex, 6);
        bitIndex += 4;
        rr[0] = RtcmV3Protocol.GetBits38(buffer, ref bitIndex);
        SingleReceiverOscillatorIndicator = (byte)SpanBitHelper.GetBitU(buffer, ref bitIndex, 1);
        bitIndex += 1; // Reserved
        rr[1] = RtcmV3Protocol.GetBits38(buffer, ref bitIndex);
        QuarterCycleIndicator = (byte)SpanBitHelper.GetBitU(buffer, ref bitIndex, 2);
        rr[2] = RtcmV3Protocol.GetBits38(buffer, ref bitIndex);

        for (var i = 0; i < 3; i++)
            re[i] = rr[i] * 0.0001;

        RtcmV3Protocol.EcefToPos(re, pos);

        X = rr[0] * 0.0001;
        Y = rr[1] * 0.0001;
        Z = rr[2] * 0.0001;

        Latitude = pos[0] * RtcmV3Protocol.R2D;
        Longitude = pos[1] * RtcmV3Protocol.R2D;
        Altitude = pos[2];

        Height = 0.0;
    }

    protected override void InternalSerialize(Span<byte> buffer, ref int bitIndex)
    {
        SpanBitHelper.SetBitU(buffer, ref bitIndex, 12, ReferenceStationID);
        SpanBitHelper.SetBitU(buffer, ref bitIndex, 6, ITRF);
        var rr = new double[3];
        rr[0] = X * 10000;
        rr[1] = Y * 10000;
        rr[2] = Z * 10000;

        bitIndex += 4;
        RtcmV3Protocol.SetBits38(buffer, ref bitIndex, rr[0]);
        SpanBitHelper.SetBitU(buffer, ref bitIndex, 1, SingleReceiverOscillatorIndicator);
        bitIndex += 1; // Reserved
        RtcmV3Protocol.SetBits38(buffer, ref bitIndex, rr[1]);
        SpanBitHelper.SetBitU(buffer, ref bitIndex, 2, QuarterCycleIndicator);
        RtcmV3Protocol.SetBits38(buffer, ref bitIndex, rr[2]);
    }

    protected override int InternalGetBitSize()
    {
        return 12 + 6 + 4 + 38 + 1 + 1 + 38 + 2 + 38;
    }

    public byte QuarterCycleIndicator { get; set; }

    /// <summary>
    /// 0 - All raw data observations in messages 1001-1004 and 1009-1012
    /// may be measured at different instants.This indicator should be set
    /// to “0” unless all the conditions for “1” are clearly met.
    /// 1 - All raw data observations in messages 1001-1004 and 1009-1012
    /// are measured at the same instant, as described in Section 3.1.4.
    /// </summary>
    public byte SingleReceiverOscillatorIndicator { get; set; }


    /// <summary>
    /// Antenna Height
    /// </summary>
    public double Height { get; set; }

    /// <summary>
    /// Antenna Reference Point ECEF-X 
    /// </summary>
    public double X { get; set; }

    /// <summary>
    /// Antenna Reference Point ECEF-Y 
    /// </summary>
    public double Y { get; set; }

    /// <summary>
    /// Antenna Reference Point ECEF-Z 
    /// </summary>
    public double Z { get; set; }

    /// <summary>
    /// Antenna Reference Point WGS84-Latitude
    /// </summary>
    public double Latitude { get; set; }

    /// <summary>
    /// Antenna Reference Point WGS84-Longitude
    /// </summary>
    public double Longitude { get; set; }

    /// <summary>
    /// Antenna Reference Point WGS84-Altitude
    /// </summary>
    public double Altitude { get; set; }

    /// <summary>
    /// Since this field is reserved, all bits should be set to zero for now.
    /// However, since the value is subject to change in future versions,
    /// decoding should not rely on a zero value.
    /// The ITRF realization year identifies the datum definition used for
    /// coordinates in the message. 
    /// </summary>
    public byte ITRF { get; set; }


    /// <summary>
    /// The Reference Station ID is determined by the service provider. Its 
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
    public uint ReferenceStationID { get; set; }
}