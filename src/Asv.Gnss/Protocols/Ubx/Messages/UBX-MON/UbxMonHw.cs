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
/// <remarks>Payload fields follow u-blox document UBX-13003221 R23, section 32.16.4. Receiver and firmware support must be checked against the applicable interface description.</remarks>
public partial class UbxMonHw : UbxMessageBase
{
    private int _pinSel;
    private int _pinBank;
    private int _pinDir;
    private int _pinVal;
    private ushort _noise;
    private ushort _agcCnt;
    private AntennaSupervisorStateMachineStatus _aStatus;
    private AntennaPowerStatus _aPower;
    private bool _rtcCalib;
    private bool _safeBoot;
    private bool _xTalAbsent;
    private int _usedMask;
    private byte[]? _vp;
    private byte _jamInd;
    private int _pinIrq;
    private int _pullH;
    private int _pullL;
    private JammingStateEnum _jammingState;
    private double _cwJammingIndicator;
    private double _agcMonitor;

    public override byte Class => 0x0A;
    public override byte SubClass => 0x09;
    public override string Name => "UBX-MON-HW";

    /// <summary>
    /// Pin-selection bit mask: each bit distinguishes peripheral selection from GPIO selection.
    /// </summary>
    public int PinSel
    {
        get => _pinSel;
        set => _pinSel = value;
    }
    /// <summary>
    /// Pin-bank bit mask reported by the receiver hardware.
    /// </summary>
    public int PinBank
    {
        get => _pinBank;
        set => _pinBank = value;
    }
    /// <summary>
    /// GPIO direction bit mask; a set bit denotes an output.
    /// </summary>
    public int PinDir
    {
        get => _pinDir;
        set => _pinDir = value;
    }
    /// <summary>
    /// GPIO logic-level bit mask; a set bit denotes a high level.
    /// </summary>
    public int PinVal
    {
        get => _pinVal;
        set => _pinVal = value;
    }
    /// <summary>
    /// Receiver noise measurement per millisecond (noisePerMS); this is a hardware-dependent diagnostic count.
    /// </summary>
    public ushort Noise
    {
        get => _noise;
        set => _noise = value;
    }
    /// <summary>
    /// Automatic gain control count, with a documented range of 0 to 8191.
    /// </summary>
    public ushort AgcCnt
    {
        get => _agcCnt;
        set => _agcCnt = value;
    }
    /// <summary>
    /// Antenna supervisor state; interpret only when antenna supervision is supported and enabled.
    /// </summary>
    public AntennaSupervisorStateMachineStatus AStatus
    {
        get => _aStatus;
        set => _aStatus = value;
    }
    /// <summary>
    /// Reported antenna power state: off, on, or unknown.
    /// </summary>
    public AntennaPowerStatus APower
    {
        get => _aPower;
        set => _aPower = value;
    }
    /// <summary>
    /// Whether the real-time clock has been calibrated.
    /// </summary>
    public bool RtcCalib
    {
        get => _rtcCalib;
        set => _rtcCalib = value;
    }
    /// <summary>
    /// Whether the receiver is running in safe-boot mode.
    /// </summary>
    public bool SafeBoot
    {
        get => _safeBoot;
        set => _safeBoot = value;
    }
    /// <summary>
    /// Whether the RTC crystal is absent.
    /// </summary>
    public bool XTalAbsent
    {
        get => _xTalAbsent;
        set => _xTalAbsent = value;
    }
    /// <summary>
    /// Bit mask identifying pins assigned to virtual-pin functions.
    /// </summary>
    public int UsedMask
    {
        get => _usedMask;
        set => _usedMask = value;
    }
    /// <summary>
    /// Seventeen virtual-pin mapping bytes; entries are hardware-dependent pin assignments.
    /// </summary>
    public byte[] VP
    {
        get => _vp!;
        set => _vp = value;
    }
    /// <summary>
    /// Continuous-wave interference indicator, from 0 to 255; larger values indicate stronger interference.
    /// </summary>
    public byte JamInd
    {
        get => _jamInd;
        set => _jamInd = value;
    }
    /// <summary>
    /// Bit mask identifying pins with an enabled interrupt.
    /// </summary>
    public int PinIrq
    {
        get => _pinIrq;
        set => _pinIrq = value;
    }
    /// <summary>
    /// Bit mask identifying pins with a high pull resistor enabled.
    /// </summary>
    public int PullH
    {
        get => _pullH;
        set => _pullH = value;
    }
    /// <summary>
    /// Bit mask identifying pins with a low pull resistor enabled.
    /// </summary>
    public int PullL
    {
        get => _pullL;
        set => _pullL = value;
    }

    /// <summary>
    /// Interference monitor state: unknown or disabled, normal, warning, or critical.
    /// </summary>
    public JammingStateEnum JammingState
    {
        get => _jammingState;
        set => _jammingState = value;
    }
    /// <summary>
    /// Library-normalized continuous-wave interference indicator, equal to JamInd divided by 255.
    /// </summary>
    public double CwJammingIndicator
    {
        get => _cwJammingIndicator;
        set => _cwJammingIndicator = value;
    }
    /// <summary>
    /// Library-normalized automatic gain control count, equal to AgcCnt divided by 8191.
    /// </summary>
    public double AgcMonitor
    {
        get => _agcMonitor;
        set => _agcMonitor = value;
    }



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