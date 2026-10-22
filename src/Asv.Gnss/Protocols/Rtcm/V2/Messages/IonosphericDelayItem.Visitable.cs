using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class IonosphericDelayItem : IVisitable
{
    private static readonly Field PayloadNavigationSystemField = ProtocolPayloadVisitor.Field(nameof(NavigationSystem), Int32Type.Default, enumType: typeof(NavigationSystemEnum));
    private static readonly Field PayloadPrnField = ProtocolPayloadVisitor.Field(nameof(Prn), UInt8Type.Default);
    private static readonly Field PayloadIonosphericDelayField = ProtocolPayloadVisitor.Field(nameof(IonosphericDelay), DoubleType.Default);
    private static readonly Field PayloadIonoRateOfChangeField = ProtocolPayloadVisitor.Field(nameof(IonoRateOfChange), DoubleType.Default);

    internal static readonly StructType PayloadVisitorType = new([PayloadNavigationSystemField, PayloadPrnField, PayloadIonosphericDelayField, PayloadIonoRateOfChangeField]);

    /// <inheritdoc />
    public virtual void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);

        NavigationSystemEnum valueNavigationSystem = NavigationSystem;
        var numericNavigationSystem = (int)valueNavigationSystem;
        Int32Type.Accept(visitor, PayloadNavigationSystemField, PayloadNavigationSystemField.DataType, ref numericNavigationSystem);
        valueNavigationSystem = (NavigationSystemEnum)numericNavigationSystem;
        if (!EqualityComparer<NavigationSystemEnum>.Default.Equals(valueNavigationSystem, NavigationSystem))
        {
            NavigationSystem = valueNavigationSystem;
        }

        byte valuePrn = Prn;
        UInt8Type.Accept(visitor, PayloadPrnField, PayloadPrnField.DataType, ref valuePrn);
        if (!EqualityComparer<byte>.Default.Equals(valuePrn, Prn))
        {
            Prn = valuePrn;
        }

        double valueIonosphericDelay = IonosphericDelay;
        DoubleType.Accept(visitor, PayloadIonosphericDelayField, PayloadIonosphericDelayField.DataType, ref valueIonosphericDelay);
        if (!EqualityComparer<double>.Default.Equals(valueIonosphericDelay, IonosphericDelay))
        {
            IonosphericDelay = valueIonosphericDelay;
        }

        double valueIonoRateOfChange = IonoRateOfChange;
        DoubleType.Accept(visitor, PayloadIonoRateOfChangeField, PayloadIonoRateOfChangeField.DataType, ref valueIonoRateOfChange);
        if (!EqualityComparer<double>.Default.Equals(valueIonoRateOfChange, IonoRateOfChange))
        {
            IonoRateOfChange = valueIonoRateOfChange;
        }
    }
}
