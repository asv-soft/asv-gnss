using System;
using Asv.IO;

namespace Asv.Gnss;

public class UbxNavDopPool : UbxMessageBase
{
    public override string Name => "UBX-NAV-DOP";
    public override byte Class => 0x01;
    public override byte SubClass => 0x04;

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

/// <remarks>Payload fields follow u-blox document UBX-13003221 R23, section 32.17.6. Receiver and firmware support must be checked against the applicable interface description.</remarks>
public partial class UbxNavDop : UbxMessageBase
{
    private ulong _iTOW;
    private double _gDOP;
    private double _pDOP;
    private double _tDOP;
    private double _vDOP;
    private double _hDOP;
    private double _nDOP;
    private double _eDOP;

    public override string Name => "UBX-NAV-DOP";
    public override byte Class => 0x01;
    public override byte SubClass => 0x04;

    /// <summary>
    /// GPS time of week of the navigation epoch.
    /// </summary>
    public ulong iTOW
    {
        get => _iTOW;
        set => _iTOW = value;
    }

    /// <summary>
    /// Geometric DOP
    /// </summary>
    public double gDOP
    {
        get => _gDOP;
        set => _gDOP = value;
    }

    /// <summary>
    /// Position DOP
    /// </summary>
    public double pDOP
    {
        get => _pDOP;
        set => _pDOP = value;
    }

    /// <summary>
    /// Time DOP
    /// </summary>
    public double tDOP
    {
        get => _tDOP;
        set => _tDOP = value;
    }

    /// <summary>
    /// Vertical DOP
    /// </summary>
    public double vDOP
    {
        get => _vDOP;
        set => _vDOP = value;
    }

    /// <summary>
    /// Horizontal DOP
    /// </summary>
    public double hDOP
    {
        get => _hDOP;
        set => _hDOP = value;
    }

    /// <summary>
    /// Northing DOP
    /// </summary>
    public double nDOP
    {
        get => _nDOP;
        set => _nDOP = value;
    }

    /// <summary>
    /// Easting DOP
    /// </summary>
    public double eDOP
    {
        get => _eDOP;
        set => _eDOP = value;
    }

    protected override void SerializeContent(ref Span<byte> buffer)
    {

    }

    protected override void DeserializeContent(ref ReadOnlySpan<byte> buffer)
    {
        iTOW = BinSerialize.ReadUInt(ref buffer);
        gDOP = BinSerialize.ReadUShort(ref buffer) * 0.01;
        pDOP = BinSerialize.ReadUShort(ref buffer) * 0.01;
        tDOP = BinSerialize.ReadUShort(ref buffer) * 0.01;
        vDOP = BinSerialize.ReadUShort(ref buffer) * 0.01;
        hDOP = BinSerialize.ReadUShort(ref buffer) * 0.01;
        nDOP = BinSerialize.ReadUShort(ref buffer) * 0.01;
        eDOP = BinSerialize.ReadUShort(ref buffer) * 0.01;
    }

    protected override int GetContentByteSize() => 18;

    public override void Randomize(Random random)
    {

    }
}