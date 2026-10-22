using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class RtcmV3Message1008
{
    private static readonly Field RtcmSerialNumberCounterMField = RtcmV3Visitor.Field(nameof(SerialNumberCounterM), UInt32Type.Default, "The Serial Number Counter defines the number of characters (bytes) to follow in Antenna Serial Number");
    private static readonly Field RtcmAntennaSerialNumberField = RtcmV3Visitor.Field(nameof(AntennaSerialNumber), StringOptionalType.Utf8, "Alphanumeric characters. The Antenna Serial Number is the individual antenna serial number as issued by the manufacturer of the antenna.A possible duplication of the Antenna Serial Number is not possible, because together with the Antenna Descriptor only one antenna with the particular number will be available.In order to avoid confusion the Antenna Serial Number should be omitted when the record is used together with reverse reduction to model type calibration values, because it cannot be allocated to a real physical antenna.");

    internal new static readonly StructType RtcmVisitorType = new([..RtcmV3Message1007and1008.RtcmVisitorType.Fields, RtcmSerialNumberCounterMField, RtcmAntennaSerialNumberField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        base.Accept(visitor);

        var valueSerialNumberCounterM = SerialNumberCounterM;
        UInt32Type.Accept(visitor, RtcmSerialNumberCounterMField, RtcmSerialNumberCounterMField.DataType, ref valueSerialNumberCounterM);
        if (!EqualityComparer<uint>.Default.Equals(valueSerialNumberCounterM, SerialNumberCounterM))
        {
            SerialNumberCounterM = valueSerialNumberCounterM;
        }

        string? valueAntennaSerialNumber = AntennaSerialNumber;
        StringOptionalType.Accept(visitor, RtcmAntennaSerialNumberField, RtcmAntennaSerialNumberField.DataType, ref valueAntennaSerialNumber);
        if (!EqualityComparer<string>.Default.Equals(valueAntennaSerialNumber, AntennaSerialNumber))
        {
            AntennaSerialNumber = valueAntennaSerialNumber!;
        }
    }
}
