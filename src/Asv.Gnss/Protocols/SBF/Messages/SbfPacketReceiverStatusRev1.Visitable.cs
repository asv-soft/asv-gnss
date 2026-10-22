using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class SbfPacketReceiverStatusRev1
{
    private static readonly Field PayloadAGCStateField = SbfComNavVisitor.Field(nameof(AGCState), new ListType(SbfReceiverStatusAGCState.PayloadVisitorType, 0, ushort.MaxValue), "AGCState");
    private static readonly Field PayloadTemperatureField = SbfComNavVisitor.Field(nameof(Temperature), Int32Type.Default, "Receiver temperature with an offset of 100. Remove 100 to get the temperature in degree Celsius.");
    private static readonly Field PayloadCmdCountField = SbfComNavVisitor.Field(nameof(CmdCount), UInt8Type.Default, "Command cyclic counter, incremented each time a command is entered that changes the receiver configuration.After the counter has reached 255, it resets to 1.");
    private static readonly Field PayloadRxErrorField = SbfComNavVisitor.Field(nameof(RxError), UInt32Type.Default, "Bit field indicating whether an error occurred previously. If this field is not equal to zero, at least one error has been detected.", enumType: typeof(SbfRxErrorEnum));
    private static readonly Field PayloadRxStateField = SbfComNavVisitor.Field(nameof(RxState), UInt32Type.Default, "Bit field indicating the status of key components of the receiver", enumType: typeof(SbfRxStateEnum));
    private static readonly Field PayloadUpTimeField = SbfComNavVisitor.Field(nameof(UpTime), TimeSpanType.Default, "Number of seconds elapsed since the start-up of the receiver, or since the last reset.");
    private static readonly Field PayloadExtErrorField = SbfComNavVisitor.Field(nameof(ExtError), UInt8Type.Default, "Bit field reporting external errors, i.e. errors detected in external data. Upon detection of an error, the corresponding bit is set for a duration of one second, and then resets.", enumType: typeof(SbfExtErrorEnum));
    private static readonly Field PayloadCPULoadField = SbfComNavVisitor.Field(nameof(CPULoad), UInt8Type.Default, "Load on the receiver\u2019s CPU. The load should stay below 80% in normal operation.Higher loads might result in data loss.");

    internal new static readonly StructType PayloadVisitorType = new([..SbfMessageBase.PayloadVisitorType.Fields, PayloadAGCStateField, PayloadTemperatureField, PayloadCmdCountField, PayloadRxErrorField, PayloadRxStateField, PayloadUpTimeField, PayloadExtErrorField, PayloadCPULoadField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        base.Accept(visitor);

        var valueAGCState = AGCState;
        SbfComNavVisitor.VisitArray(visitor, PayloadAGCStateField, PayloadAGCStateField.DataType, ref valueAGCState, static () => new SbfReceiverStatusAGCState(),
            static (IVisitor v, Field f, IFieldType t, ref SbfReceiverStatusAGCState item) =>
            {
                if (item is not null)
                {
                    StructType.Accept(v, f, t, item);
                }
                else
                {
                    v.VisitUnknown(f, t);
                }
            });
        if (!ReferenceEquals(valueAGCState, AGCState))
        {
            AGCState = valueAGCState!;
        }

        var valueTemperature = Temperature;
        Int32Type.Accept(visitor, PayloadTemperatureField, PayloadTemperatureField.DataType, ref valueTemperature);
        if (!EqualityComparer<int>.Default.Equals(valueTemperature, Temperature))
        {
            Temperature = valueTemperature;
        }

        var valueCmdCount = CmdCount;
        UInt8Type.Accept(visitor, PayloadCmdCountField, PayloadCmdCountField.DataType, ref valueCmdCount);
        if (!EqualityComparer<byte>.Default.Equals(valueCmdCount, CmdCount))
        {
            CmdCount = valueCmdCount;
        }

        var valueRxError = RxError;
        var numericvalueRxError = (uint)valueRxError;
        UInt32Type.Accept(visitor, PayloadRxErrorField, PayloadRxErrorField.DataType, ref numericvalueRxError);
        valueRxError = (SbfRxErrorEnum)numericvalueRxError;
        if (!EqualityComparer<SbfRxErrorEnum>.Default.Equals(valueRxError, RxError))
        {
            RxError = valueRxError;
        }

        var valueRxState = RxState;
        var numericvalueRxState = (uint)valueRxState;
        UInt32Type.Accept(visitor, PayloadRxStateField, PayloadRxStateField.DataType, ref numericvalueRxState);
        valueRxState = (SbfRxStateEnum)numericvalueRxState;
        if (!EqualityComparer<SbfRxStateEnum>.Default.Equals(valueRxState, RxState))
        {
            RxState = valueRxState;
        }

        var valueUpTime = UpTime;
        TimeSpanType.Accept(visitor, PayloadUpTimeField, PayloadUpTimeField.DataType, ref valueUpTime);
        if (!EqualityComparer<TimeSpan>.Default.Equals(valueUpTime, UpTime))
        {
            UpTime = valueUpTime;
        }

        var valueExtError = ExtError;
        var numericvalueExtError = (byte)valueExtError;
        UInt8Type.Accept(visitor, PayloadExtErrorField, PayloadExtErrorField.DataType, ref numericvalueExtError);
        valueExtError = (SbfExtErrorEnum)numericvalueExtError;
        if (!EqualityComparer<SbfExtErrorEnum>.Default.Equals(valueExtError, ExtError))
        {
            ExtError = valueExtError;
        }

        var valueCPULoad = CPULoad;
        UInt8Type.Accept(visitor, PayloadCPULoadField, PayloadCPULoadField.DataType, ref valueCPULoad);
        if (!EqualityComparer<byte>.Default.Equals(valueCPULoad, CPULoad))
        {
            CPULoad = valueCPULoad;
        }
    }
}
