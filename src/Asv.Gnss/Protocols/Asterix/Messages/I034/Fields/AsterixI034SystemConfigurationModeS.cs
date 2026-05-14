using System;

namespace Asv.Gnss;

/// <summary>
/// Mode-S system configuration and status subfield.
/// </summary>
public sealed class AsterixI034SystemConfigurationModeS
{
    /// <summary>
    /// Antenna selection.
    /// </summary>
    public bool Antenna { get; set; }

    /// <summary>
    /// Channel A/B status.
    /// </summary>
    public byte ChannelAB { get; set; }

    /// <summary>
    /// Surveillance overload status.
    /// </summary>
    public bool OverloadSurveillance { get; set; }

    /// <summary>
    /// Monitoring system connected bit.
    /// </summary>
    public bool MonitoringSystemConnected { get; set; }

    /// <summary>
    /// Surveillance coordination function status.
    /// </summary>
    public bool SurveillanceCoordinationFunction { get; set; }

    /// <summary>
    /// Data link function status.
    /// </summary>
    public bool DataLinkFunction { get; set; }

    /// <summary>
    /// Surveillance coordination function overload status.
    /// </summary>
    public bool OverloadSurveillanceCoordinationFunction { get; set; }

    /// <summary>
    /// Data link function overload status.
    /// </summary>
    public bool OverloadDataLinkFunction { get; set; }

    internal void Deserialize(ReadOnlySpan<byte> data)
    {
        var first = data[0];
        var second = data[1];
        Antenna = (first & 0x80) != 0;
        ChannelAB = (byte)((first >> 5) & 0x03);
        OverloadSurveillance = (first & 0x10) != 0;
        MonitoringSystemConnected = (first & 0x08) != 0;
        SurveillanceCoordinationFunction = (first & 0x04) != 0;
        DataLinkFunction = (first & 0x02) != 0;
        OverloadSurveillanceCoordinationFunction = (second & 0x80) != 0;
        OverloadDataLinkFunction = (second & 0x40) != 0;
    }

    internal void Serialize(Span<byte> data)
    {
        data[0] = (byte)((Antenna ? 0x80 : 0) |
                         ((ChannelAB & 0x03) << 5) |
                         (OverloadSurveillance ? 0x10 : 0) |
                         (MonitoringSystemConnected ? 0x08 : 0) |
                         (SurveillanceCoordinationFunction ? 0x04 : 0) |
                         (DataLinkFunction ? 0x02 : 0));
        data[1] = (byte)((OverloadSurveillanceCoordinationFunction ? 0x80 : 0) |
                         (OverloadDataLinkFunction ? 0x40 : 0));
    }
}