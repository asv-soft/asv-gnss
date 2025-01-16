using System.Linq;

namespace Asv.Gnss
{
    /// <summary>
    /// GSA GPS DOP and active satellites.
    /// 1) Selection mode
    /// 2) Mode
    /// 3) ID of 1st satellite used for fix
    /// 4) ID of 2nd satellite used for fix
    /// ...
    /// 14) ID of 12th satellite used for fix
    /// 15) PDOP in meters
    /// 16) HDOP in meters
    /// 17) VDOP in meters
    /// 18) Checksum.
    /// </summary>
    public class Nmea0183MessageGSA : Nmea0183MessageBase
    {
        /// <summary>
        /// Gets the message identifier for a specific message.
        /// </summary>
        /// <value>
        /// The message identifier.
        /// </value>
        public override string MessageId => "GSA";

        /// <summary>
        /// Method for deserializing an array of string items and setting the corresponding properties.
        /// </summary>
        /// <param name="items">The array of string items to deserialize.</param>
        protected override void InternalDeserializeFromStringArray(string[] items)
        {
            SelectionMode = items[1];
            Mode = items[2];

            SatelliteId = items
                .Skip(3)
                .Take(12)
                .Where(_ => !string.IsNullOrEmpty(_))
                .Select(int.Parse)
                .ToArray();

            PDop = Nmea0183Helper.ParseDouble(items[15]);
            HDop = Nmea0183Helper.ParseDouble(items[16]);
            VDop = Nmea0183Helper.ParseDouble(items[17]);
        }

        /// <summary>
        /// Gets or sets the selection mode for the property.
        /// </summary>
        /// <value>
        /// The selection mode for the property.
        /// </value>
        /// <remarks>
        /// The selection mode determines how the property is selected.
        /// Valid values are:
        /// <list type="bullet">
        /// <item><description>"Single": Only one value can be selected at a time.</description></item>
        /// <item><description>"Multiple": Multiple values can be selected at a time.</description></item>
        /// <item><description>"Extended": Multiple values can be selected using keyboard modifiers (Ctrl, Shift) in addition to the mouse.</description></item>
        /// </list>
        /// </remarks>
        public string SelectionMode { get; set; }

        /// <summary>
        /// Gets or sets the mode property.
        /// </summary>
        /// <value>
        /// The mode.
        /// </value>
        public string Mode { get; set; }

        /// <summary>
        /// Gets or sets the ID of the satellite.
        /// </summary>
        /// <value>
        /// An array of integers representing the ID of the satellite.
        /// </value>
        public int[] SatelliteId { get; set; }

        /// <summary>
        /// Gets or sets the PDop property.
        /// </summary>
        /// <remarks>
        /// PDop stands for Position Dilution of Precision.
        /// </remarks>
        /// <value>
        /// The PDop value.
        /// </value>
        public double PDop { get; set; }

        /// <summary>
        /// Gets or sets the HDop value.
        /// HDop stands for Horizontal Dilution of Precision and is a measure of the accuracy of the GPS signal in terms of horizontal positioning.
        /// </summary>
        /// <value>
        /// The HDop value. The lower the value, the more accurate the positioning.
        /// </value>
        public double HDop { get; set; }

        /// <summary>
        /// Gets or sets the VDop property.
        /// </summary>
        /// <value>
        /// The VDop property represents the vertical dilution of precision (VDOP) value.
        /// </value>
        public double VDop { get; set; }
    }
}
