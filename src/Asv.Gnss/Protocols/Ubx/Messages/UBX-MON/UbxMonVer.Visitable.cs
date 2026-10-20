using Asv.IO;

namespace Asv.Gnss;

public partial class UbxMonVer
{
    private static readonly Field SoftwareField = UbxVisitor.Field(nameof(Software), new StringOptionalType(EncodingId.Ascii, 0, 30), "Nul-terminated software version string.");
    private static readonly Field HardwareField = UbxVisitor.Field(nameof(Hardware), new StringOptionalType(EncodingId.Ascii, 0, 10), "Nul-terminated hardware version string");
    private static readonly Field ExtensionsField = UbxVisitor.Field(nameof(Extensions), new ListType(new StringType(EncodingId.Ascii, 0, 30), 0, (ushort.MaxValue - 40) / 30), "Extended firmware, protocol, module, and GNSS information in repeated 30-character fields.");

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        base.Accept(visitor);
        StringOptionalType.Accept(visitor, SoftwareField, SoftwareField.DataType, ref _software);
        StringOptionalType.Accept(visitor, HardwareField, HardwareField.DataType, ref _hardware);
        UbxVisitor.VisitList(visitor, ExtensionsField, ref _extensions, static () => string.Empty,
            static (IVisitor v, Field f, IFieldType t, ref string value) => StringType.Accept(v, f, t, ref value));
    }
}
