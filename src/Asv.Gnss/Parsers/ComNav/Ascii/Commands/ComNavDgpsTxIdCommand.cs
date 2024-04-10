using System;

namespace Asv.Gnss
{
    /// <summary>
    /// Enumerates the possible values for DGPS transmission IDs.
    /// </summary>
    public enum DgpsTxIdEnum
    {
        /// <summary>
        /// Specifies the DgpsTxId values for the DgpsTxIdEnum.
        /// </summary>
        RTCM = 0,

        /// <summary>
        /// Represents the RTC A member of the DgpsTxIdEnum enumeration.
        /// </summary>
        RTCA = 1,

        /// <summary>
        /// Represents the member CMR of the DgpsTxIdEnum enumeration.
        /// </summary>
        CMR = 2,

        /// <summary>
        /// Represents the AUTO tx id for Differential GPS.
        /// </summary>
        AUTO = 10,

        /// <summary>
        /// Represents the RTCMV3 member of the DgpsTxIdEnum enumeration.
        /// </summary>
        RTCMV3 = 13,

        /// <summary>
        /// Represents the DgpsTxIdEnum value for NOVATELX.
        /// </summary>
        /// <remarks>
        /// This value is used as an identifier for the NOVATELX type of DgpsTxId.
        /// </remarks>
        NOVATELX = 14
    }

    /// <summary>
    /// Represents a command for setting the Differential GPS (DGPS) transmission identification (TxID) type and ID.
    /// </summary>
    public class ComNavDgpsTxIdCommand : ComNavAsciiCommandBase
    {
        /// <summary>
        /// Gets or sets the type of DgpsTxId.
        /// </summary>
        /// <value>
        /// The DgpsTxIdEnum type.
        /// </value>
        public DgpsTxIdEnum Type { get; set; }

        /// <summary>
        /// Serializes the object to an ASCII string representation.
        /// </summary>
        /// <returns>An ASCII string representation of the object.</returns>
        protected override string SerializeToAsciiString()
        {
            return Type switch
            {
                DgpsTxIdEnum.RTCM => $"DGPSTXID RTCM {Id:0000}",
                DgpsTxIdEnum.RTCA => $"DGPSTXID RTCA {Id:0000}",
                DgpsTxIdEnum.CMR => $"DGPSTXID CMR {Id:0000}",
                DgpsTxIdEnum.AUTO => $"DGPSTXID AUTO {Id:0000}",
                DgpsTxIdEnum.RTCMV3 => $"DGPSTXID RTCMV3 {Id:0000}",
                DgpsTxIdEnum.NOVATELX => $"DGPSTXID NOVATELX {Id:0000}",
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        /// <summary>
        /// Gets or sets the ID of the property.
        /// </summary>
        /// <value>
        /// The ID of the property.
        /// </value>
        /// <remarks>
        /// The ID property represents the unique identifier of a property.
        /// </remarks>
        public uint Id { get; set; }

        /// <summary>
        /// Gets the unique identifier for the message.
        /// </summary>
        /// <value>
        /// The unique identifier for the message.
        /// </value>
        public override string MessageId => "DGPSTXID";
    }
}