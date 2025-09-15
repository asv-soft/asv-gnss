using System;
using Asv.IO;

namespace Asv.Gnss;

public class UbxMonHwPool : UbxMessageBase
{
    public override byte Class => 0x0A;
    public override byte SubClass => 0x09;
    public override string Name => "UBX-MON-HW-POOL";

    protected override void SerializeContent(ref Span<byte> buffer)
    {

    }

    protected override void DeserializeContent(ref ReadOnlySpan<byte> buffer)
    {

    }

    protected override int GetContentByteSize() => 0;


    public override void Randomize(Random random)
    {

    }


}

// [SerializationNotSupported]
public class UbxMonHw : UbxMessageBase
{
    public override byte Class => 0x0A;
    public override byte SubClass => 0x09;
    public override string Name => "UBX-MON-HW";

    public int PinSel { get; set; }
    public int PinBank { get; set; }
    public int PinDir { get; set; }
    public int PinVal { get; set; }
    public ushort Noise { get; set; }
    public ushort AgcCnt { get; set; }
    public AntennaSupervisorStateMachineStatus AStatus { get; set; }
    public AntennaPowerStatus APower { get; set; }
    public bool RtcCalib { get; set; }
    public bool SafeBoot { get; set; }
    public bool XTalAbsent { get; set; }
    public int UsedMask { get; set; }
    public byte[] VP { get; set; }
    public byte JamInd { get; set; }
    public int PinIrq { get; set; }
    public int PullH { get; set; }
    public int PullL { get; set; }

    public JammingStateEnum JammingState { get; set; }
    public double CwJammingIndicator { get; set; }
    public double AgcMonitor { get; set; }



    protected override void SerializeContent(ref Span<byte> buffer)
    {
        throw new NotImplementedException();
    }

    protected override int GetContentByteSize() => 28;


    protected override void DeserializeContent(ref ReadOnlySpan<byte> buffer)
    {
        PinSel = BinSerialize.ReadInt(ref buffer);
        PinBank = BinSerialize.ReadInt(ref buffer);
        PinDir = BinSerialize.ReadInt(ref buffer);
        PinVal = BinSerialize.ReadInt(ref buffer);
        Noise = BinSerialize.ReadUShort(ref buffer);
        AgcCnt = BinSerialize.ReadUShort(ref buffer);
        AStatus = (AntennaSupervisorStateMachineStatus)BinSerialize.ReadByte(ref buffer);
        APower = (AntennaPowerStatus)BinSerialize.ReadByte(ref buffer);
        var tmp = BinSerialize.ReadByte(ref buffer);
        RtcCalib = (tmp & 0x1) != 0;
        SafeBoot = (tmp & 0x2) != 0;
        JammingState = (JammingStateEnum)((tmp & 0x0C) >> 2);
        XTalAbsent = (tmp & 0x10) != 0;
        BinSerialize.ReadByte(ref buffer); // reserved

        UsedMask = BinSerialize.ReadInt(ref buffer);
        VP = new byte[17];
        for (var i = 0; i < 17; i++)
        {
            VP[i] = BinSerialize.ReadByte(ref buffer);
        }

        JamInd = BinSerialize.ReadByte(ref buffer);
        BinSerialize.ReadUShort(ref buffer); // reserved 2
        PinIrq = BinSerialize.ReadInt(ref buffer);
        PullH = BinSerialize.ReadInt(ref buffer);
        PullL = BinSerialize.ReadInt(ref buffer);

        AgcMonitor = AgcCnt / 8191.0;
        CwJammingIndicator = JamInd / 255.0;
    }

    public override void Randomize(Random random)
    {
        PinSel = random.Next();
        PinBank = random.Next();
        PinDir = random.Next();
        PinVal = random.Next();
        Noise = (ushort)random.Next(0, ushort.MaxValue);
        AgcCnt = (ushort)random.Next(0, ushort.MaxValue);
    }

    public enum AntennaSupervisorStateMachineStatus
    {
        Init = 0,
        DontKnow = 1,
        Ok = 2,
        Short = 3,
        Open = 4
    }

    public enum AntennaPowerStatus
    {
        Off = 0,
        On = 1,
        DontKnow = 2
    }

    public enum JammingStateEnum
    {
        Unknown = 0,
        Ok = 1,
        Warning = 2,
        Critical = 3
    }
}