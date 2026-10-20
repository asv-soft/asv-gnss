using System;
using Asv.IO;

namespace Asv.Gnss;

public partial class UbxNavDop
{
    private static readonly Field iTOWField = UbxVisitor.Field(nameof(iTOW), UInt64Type.Default, "GPS time of week of the navigation epoch.", "ms");
    private static readonly Field gDOPField = UbxVisitor.Field(nameof(gDOP), DoubleType.Default, "Geometric DOP");
    private static readonly Field pDOPField = UbxVisitor.Field(nameof(pDOP), DoubleType.Default, "Position DOP");
    private static readonly Field tDOPField = UbxVisitor.Field(nameof(tDOP), DoubleType.Default, "Time DOP");
    private static readonly Field vDOPField = UbxVisitor.Field(nameof(vDOP), DoubleType.Default, "Vertical DOP");
    private static readonly Field hDOPField = UbxVisitor.Field(nameof(hDOP), DoubleType.Default, "Horizontal DOP");
    private static readonly Field nDOPField = UbxVisitor.Field(nameof(nDOP), DoubleType.Default, "Northing DOP");
    private static readonly Field eDOPField = UbxVisitor.Field(nameof(eDOP), DoubleType.Default, "Easting DOP");

    /// <summary>Describes the public navigation payload fields exposed to visitors.</summary>
    public static StructType StructType { get; } = new([
        iTOWField,
        gDOPField,
        pDOPField,
        tDOPField,
        vDOPField,
        hDOPField,
        nDOPField,
        eDOPField
    ]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        base.Accept(visitor);
        UInt64Type.Accept(visitor, iTOWField, iTOWField.DataType, ref _iTOW);
        DoubleType.Accept(visitor, gDOPField, gDOPField.DataType, ref _gDOP);
        DoubleType.Accept(visitor, pDOPField, pDOPField.DataType, ref _pDOP);
        DoubleType.Accept(visitor, tDOPField, tDOPField.DataType, ref _tDOP);
        DoubleType.Accept(visitor, vDOPField, vDOPField.DataType, ref _vDOP);
        DoubleType.Accept(visitor, hDOPField, hDOPField.DataType, ref _hDOP);
        DoubleType.Accept(visitor, nDOPField, nDOPField.DataType, ref _nDOP);
        DoubleType.Accept(visitor, eDOPField, eDOPField.DataType, ref _eDOP);
    }
}
