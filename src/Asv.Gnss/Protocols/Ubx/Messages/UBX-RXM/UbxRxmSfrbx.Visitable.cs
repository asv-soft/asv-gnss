using Asv.IO;

namespace Asv.Gnss;

public partial class UbxRxmSfrbx
{
    private static readonly Field UbxGnssIdField = UbxVisitor.Enum(nameof(UbxGnssId), Int32Type.Default, "GNSS identifier",
            new EnumValue<int>(0, "GPS"),
            new EnumValue<int>(1, "SBAS"),
            new EnumValue<int>(2, "Galileo"),
            new EnumValue<int>(3, "BeiDou"),
            new EnumValue<int>(4, "IMES"),
            new EnumValue<int>(5, "QZSS"),
            new EnumValue<int>(6, "GLONASS"));
    private static readonly Field SvIdField = UbxVisitor.Field(nameof(SvId), UInt8Type.Default, "Satellite identifier");
    private static readonly Field Reserved1Field = UbxVisitor.Field(nameof(Reserved1), UInt8Type.Default, "Reserved in legacy layouts; signal identifier (sigId) in newer interfaces, including F9 HPG 1.50. The original property name is retained for API compatibility.");
    private static readonly Field FreqIdField = UbxVisitor.Field(nameof(FreqId), Int8Type.Default, "GLONASS frequency slot from -7 to 6; this library subtracts 7 from the encoded freqId value. Meaningful only for GLONASS.");
    private static readonly Field NumWordsField = UbxVisitor.Field(nameof(NumWords), UInt8Type.Default, "Number of 32-bit navigation words: version 1 permits 0 to 16; the R23 version-2 description lists up to 10 for supported signals.");
    private static readonly Field ChnField = UbxVisitor.Field(nameof(Chn), UInt8Type.Default, "Tracking channel number for payload version 2; reserved in version 1 and exposed as zero by this deserializer.");
    private static readonly Field VersionField = UbxVisitor.Field(nameof(Version), UInt8Type.Default, "Payload version: 1 for the protocol-17 layout, or 2 for the protocol-18-and-later layout described in R23.");
    private static readonly Field Reserved2Field = UbxVisitor.Field(nameof(Reserved2), UInt8Type.Default, "Reserved");
    private static readonly Field RawDataField = UbxVisitor.Field(nameof(RawData), new ListType(UInt32Type.Default, 0, 16), "Broadcast navigation words in constellation-specific layout; includes NumWords unsigned 32-bit values.");

    /// <inheritdoc />
    /// <remarks>
    /// Visits the UBX payload and raw navigation words. The optional GPS and GLONASS decoded
    /// views are derived from those words and belong to the separate navigation-data models.
    /// </remarks>
    public override void Accept(IVisitor visitor)
    {
        base.Accept(visitor);
        var ubxGnssId = (int)_ubxGnssId;
        Int32Type.Accept(visitor, UbxGnssIdField, UbxGnssIdField.DataType, ref ubxGnssId);
        _ubxGnssId = (UbxGnssTypeEnum)ubxGnssId;
        UInt8Type.Accept(visitor, SvIdField, SvIdField.DataType, ref _svId);
        UInt8Type.Accept(visitor, Reserved1Field, Reserved1Field.DataType, ref _reserved1);
        Int8Type.Accept(visitor, FreqIdField, FreqIdField.DataType, ref _freqId);
        UInt8Type.Accept(visitor, NumWordsField, NumWordsField.DataType, ref _numWords);
        UInt8Type.Accept(visitor, ChnField, ChnField.DataType, ref _chn);
        UInt8Type.Accept(visitor, VersionField, VersionField.DataType, ref _version);
        UInt8Type.Accept(visitor, Reserved2Field, Reserved2Field.DataType, ref _reserved2);
        UbxVisitor.VisitArray(visitor, RawDataField, ref _rawData, static () => default(uint),
            static (IVisitor v, Field f, IFieldType t, ref uint value) => UInt32Type.Accept(v, f, t, ref value));
    }
}
