using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixFieldI021Frn019Type090
{
    private static readonly Field VisitorNucrOrNacvField = new Field.Builder().Name(nameof(NucrOrNacv)).Title(nameof(NucrOrNacv)).Description("NucrOrNacv").DataType(UInt8Type.Default).Build();
    private static readonly Field VisitorNucpOrNicField = new Field.Builder().Name(nameof(NucpOrNic)).Title(nameof(NucpOrNic)).Description("NucpOrNic").DataType(UInt8Type.Default).Build();
    private static readonly Field VisitorNicBaroField = new Field.Builder().Name(nameof(NicBaro)).Title(nameof(NicBaro)).Description("NicBaro").DataType(BoolType.Default).Build();
    private static readonly Field VisitorSilField = new Field.Builder().Name(nameof(Sil)).Title(nameof(Sil)).Description("Sil").DataType(UInt8Type.Default).Build();
    private static readonly Field VisitorNacpField = new Field.Builder().Name(nameof(Nacp)).Title(nameof(Nacp)).Description("Nacp").DataType(UInt8Type.Default).Build();
    private static readonly Field VisitorSilSupplementField = new Field.Builder().Name(nameof(SilSupplement)).Title(nameof(SilSupplement)).Description("SilSupplement").DataType(BoolType.Default).Build();
    private static readonly Field VisitorSdaField = new Field.Builder().Name(nameof(Sda)).Title(nameof(Sda)).Description("Sda").DataType(UInt8Type.Default).Build();
    private static readonly Field VisitorGvaField = new Field.Builder().Name(nameof(Gva)).Title(nameof(Gva)).Description("Gva").DataType(UInt8Type.Default).Build();
    private static readonly Field VisitorPicField = new Field.Builder().Name(nameof(Pic)).Title(nameof(Pic)).Description("Pic").DataType(UInt8Type.Default).Build();
    private static readonly Field VisitorHasFirstExtensionField = new Field.Builder().Name(nameof(HasFirstExtension)).Title(nameof(HasFirstExtension)).Description("HasFirstExtension").DataType(BoolType.Default).Build();
    private static readonly Field VisitorHasSecondExtensionField = new Field.Builder().Name(nameof(HasSecondExtension)).Title(nameof(HasSecondExtension)).Description("HasSecondExtension").DataType(BoolType.Default).Build();
    private static readonly Field VisitorHasThirdExtensionField = new Field.Builder().Name(nameof(HasThirdExtension)).Title(nameof(HasThirdExtension)).Description("HasThirdExtension").DataType(BoolType.Default).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorNucrOrNacvField, VisitorNucpOrNicField, VisitorNicBaroField, VisitorSilField, VisitorNacpField, VisitorSilSupplementField, VisitorSdaField, VisitorGvaField, VisitorPicField, VisitorHasFirstExtensionField, VisitorHasSecondExtensionField, VisitorHasThirdExtensionField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueNucrOrNacv = NucrOrNacv;
        UInt8Type.Accept(visitor, VisitorNucrOrNacvField, ref valueNucrOrNacv);
        if (!EqualityComparer<byte>.Default.Equals(valueNucrOrNacv, NucrOrNacv)) NucrOrNacv = valueNucrOrNacv;
        var valueNucpOrNic = NucpOrNic;
        UInt8Type.Accept(visitor, VisitorNucpOrNicField, ref valueNucpOrNic);
        if (!EqualityComparer<byte>.Default.Equals(valueNucpOrNic, NucpOrNic)) NucpOrNic = valueNucpOrNic;
        var valueNicBaro = NicBaro;
        BoolType.Accept(visitor, VisitorNicBaroField, ref valueNicBaro);
        if (!EqualityComparer<bool>.Default.Equals(valueNicBaro, NicBaro)) NicBaro = valueNicBaro;
        var valueSil = Sil;
        UInt8Type.Accept(visitor, VisitorSilField, ref valueSil);
        if (!EqualityComparer<byte>.Default.Equals(valueSil, Sil)) Sil = valueSil;
        var valueNacp = Nacp;
        UInt8Type.Accept(visitor, VisitorNacpField, ref valueNacp);
        if (!EqualityComparer<byte>.Default.Equals(valueNacp, Nacp)) Nacp = valueNacp;
        var valueSilSupplement = SilSupplement;
        BoolType.Accept(visitor, VisitorSilSupplementField, ref valueSilSupplement);
        if (!EqualityComparer<bool>.Default.Equals(valueSilSupplement, SilSupplement)) SilSupplement = valueSilSupplement;
        var valueSda = Sda;
        UInt8Type.Accept(visitor, VisitorSdaField, ref valueSda);
        if (!EqualityComparer<byte>.Default.Equals(valueSda, Sda)) Sda = valueSda;
        var valueGva = Gva;
        UInt8Type.Accept(visitor, VisitorGvaField, ref valueGva);
        if (!EqualityComparer<byte>.Default.Equals(valueGva, Gva)) Gva = valueGva;
        var valuePic = Pic;
        UInt8Type.Accept(visitor, VisitorPicField, ref valuePic);
        if (!EqualityComparer<byte>.Default.Equals(valuePic, Pic)) Pic = valuePic;
        var valueHasFirstExtension = HasFirstExtension;
        BoolType.Accept(visitor, VisitorHasFirstExtensionField, ref valueHasFirstExtension);
        if (!EqualityComparer<bool>.Default.Equals(valueHasFirstExtension, HasFirstExtension)) HasFirstExtension = valueHasFirstExtension;
        var valueHasSecondExtension = HasSecondExtension;
        BoolType.Accept(visitor, VisitorHasSecondExtensionField, ref valueHasSecondExtension);
        if (!EqualityComparer<bool>.Default.Equals(valueHasSecondExtension, HasSecondExtension)) HasSecondExtension = valueHasSecondExtension;
        var valueHasThirdExtension = HasThirdExtension;
        BoolType.Accept(visitor, VisitorHasThirdExtensionField, ref valueHasThirdExtension);
        if (!EqualityComparer<bool>.Default.Equals(valueHasThirdExtension, HasThirdExtension)) HasThirdExtension = valueHasThirdExtension;
    }
}
