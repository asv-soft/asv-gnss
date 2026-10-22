using System;
using System.Buffers.Binary;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Xunit;

namespace Asv.Gnss;

public class UbxX20ConfigurationTest
{
    [Fact]
    public void Catalog_CompletePinnedSource_HasAllGroupsAndKeyNamesIdsAndTypes()
    {
        var items = UbxX20ConfigurationCatalog.Items;

        Assert.Equal(38, UbxX20ConfigurationCatalog.Groups.Count);
        Assert.Equal(826, items.Count);
        Assert.Equal(items.Count, items.Select(x => x.KeyId).Distinct().Count());
        Assert.Equal(items.Count, items.Select(x => x.Name).Distinct().Count());
        var sourceList = string.Concat(items.Select(x => $"0x{x.KeyId:x8} {x.Name} {x.Type}\n"));
        var hash = Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(sourceList)));
        Assert.Equal("0db99ec83b241b7b352beab2a825aa6f148f1afb4ace684fdfb4e688908246f8", hash);
        Assert.All(items, item =>
        {
            Assert.True(UbxX20ConfigurationCatalog.TryGet(item.KeyId, out var byId));
            Assert.True(UbxX20ConfigurationCatalog.TryGet(item.Name, out var byName));
            Assert.Same(item, byId);
            Assert.Same(item, byName);
        });
        Assert.False(UbxX20ConfigurationCatalog.TryGet("cfg-rate-meas", out _));
    }

    [Fact]
    public void Catalog_ScalesUnitsAndUnspecifiedConstants_PreserveOfficialMetadata()
    {
        Assert.True(UbxX20ConfigurationCatalog.TryGet(UbxX20ConfigurationKeys.Rate.Meas, out var rate));
        Assert.Equal(UbxX20ConfigurationType.U2, rate.Type);
        Assert.Equal("0.001", rate.Scale);
        Assert.Equal("s", rate.Unit);
        Assert.Contains("100 ms", rate.Description);
        Assert.True(UbxX20ConfigurationCatalog.TryGet(UbxX20ConfigurationKeys.Bds.D1d2Navdata, out var mode));
        var unspecified = Assert.Single(mode.Constants, x => x.Name == "ALL");
        Assert.Equal(string.Empty, unspecified.Value);
        Assert.Contains("default", unspecified.Title);
        Assert.Equal("1", Assert.Single(mode.Constants, x => x.Name == "B1I").Value);
        Assert.Equal(0x1034FFFFu, Assert.Single(UbxX20ConfigurationCatalog.Groups, x => x.Name == "BDS").WildcardKeyId);
    }

    [Fact]
    public void EncodeDecodeValues_MixedNativeTypes_MatchIndependentLittleEndianBytes()
    {
        UbxX20ConfigurationValue[] values =
        [
            new(UbxX20ConfigurationKeys.Bds.UseGeoPrn, true),
            new(UbxX20ConfigurationKeys.Navspg.InfilMinelev, (sbyte)-5),
            new(UbxX20ConfigurationKeys.Tp.AntCabledelay, (short)-300),
            new(UbxX20ConfigurationKeys.Geofence.Fence1Lat, -1234567),
            new(UbxX20ConfigurationKeys.Navspg.UsrdatDx, -1.5f),
            new(UbxX20ConfigurationKeys.Navspg.UsrdatFlat, 256.0),
            new(UbxX20ConfigurationKeys.Pmp.UniqueWord, 0x0102030405060708UL),
        ];
        byte[] expected =
        [
            0x14, 0x00, 0x34, 0x10, 0x01,
            0xA4, 0x00, 0x11, 0x20, 0xFB,
            0x01, 0x00, 0x05, 0x30, 0xD4, 0xFE,
            0x21, 0x00, 0x24, 0x40, 0x79, 0x29, 0xED, 0xFF,
            0x64, 0x00, 0x11, 0x40, 0x00, 0x00, 0xC0, 0xBF,
            0x63, 0x00, 0x11, 0x50, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x70, 0x40,
            0x1A, 0x00, 0xB1, 0x50, 0x08, 0x07, 0x06, 0x05, 0x04, 0x03, 0x02, 0x01,
        ];

        Assert.Equal(expected, UbxX20ConfigurationCodec.EncodeValues(values));
        var decoded = UbxX20ConfigurationCodec.DecodeValues(expected);
        Assert.Equal(values.Length, decoded.Count);
        for (var index = 0; index < values.Length; index++)
        {
            Assert.Equal(values[index].KeyId, decoded[index].KeyId);
            Assert.Equal(values[index].Value.GetType(), decoded[index].Value.GetType());
            Assert.Equal(values[index].Value, decoded[index].Value);
        }
    }

    [Fact]
    public void EncodeDecodeValues_EveryCatalogType_PreservesNativeClrTypes()
    {
        foreach (var item in UbxX20ConfigurationCatalog.Items)
        {
            var value = NativeZero(item.Type);
            var wire = UbxX20ConfigurationCodec.EncodeValues([new(item.KeyId, value)]);
            var decoded = Assert.Single(UbxX20ConfigurationCodec.DecodeValues(wire));
            Assert.Equal(item.KeyId, decoded.KeyId);
            Assert.Equal(value.GetType(), decoded.Value.GetType());
            Assert.Equal(value, decoded.Value);
        }
    }

    [Fact]
    public void EncodeDecodeValues_UnknownSizedKey_PreservesRawBytesWithoutGuessingItsType()
    {
        byte[] expected = [0x23, 0x01, 0xFE, 0x40, 0xDE, 0xAD, 0xBE, 0xEF];

        var decoded = Assert.Single(UbxX20ConfigurationCodec.DecodeValues(expected));

        Assert.Equal(0x40FE0123u, decoded.KeyId);
        Assert.Equal(new byte[] { 0xDE, 0xAD, 0xBE, 0xEF }, Assert.IsType<byte[]>(decoded.Value));
        Assert.Equal(expected, UbxX20ConfigurationCodec.EncodeValues([decoded]));
        Assert.Throws<ArgumentException>(() => UbxX20ConfigurationCodec.EncodeValues([new(decoded.KeyId, 0xEFBEADDEu)]));
        Assert.Throws<ArgumentException>(() => UbxX20ConfigurationCodec.EncodeValues([new(decoded.KeyId, new byte[3])]));
    }

    [Theory]
    [InlineData(0x10FE0123u, 1)]
    [InlineData(0x20FE0123u, 1)]
    [InlineData(0x30FE0123u, 2)]
    [InlineData(0x40FE0123u, 4)]
    [InlineData(0x50FE0123u, 8)]
    public void EncodeDecodeValues_UnknownKeyEveryStorageSize_PreservesExactlyTheEncodedWidth(uint keyId, int size)
    {
        var raw = Enumerable.Range(1, size).Select(x => (byte)x).ToArray();
        var encoded = UbxX20ConfigurationCodec.EncodeValues([new(keyId, raw)]);

        Assert.Equal(size + 4, encoded.Length);
        Assert.Equal(keyId, BinaryPrimitives.ReadUInt32LittleEndian(encoded));
        Assert.Equal(raw, encoded[4..]);
        Assert.Equal(raw, Assert.IsType<byte[]>(Assert.Single(UbxX20ConfigurationCodec.DecodeValues(encoded)).Value));
    }

    [Theory]
    [InlineData(0x00010001u)]
    [InlineData(0x60010001u)]
    [InlineData(0x70010001u)]
    [InlineData(0x90010001u)]
    [InlineData(0x11010001u)]
    [InlineData(0x10001001u)]
    [InlineData(0x10000001u)]
    [InlineData(0x10FF0001u)]
    [InlineData(0x10010000u)]
    [InlineData(0x10010FFFu)]
    [InlineData(0x1034FFFFu)]
    public void Values_ReservedOrWildcardKey_RejectsMalformedIds(uint keyId)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => UbxX20ConfigurationCodec.GetValueSize(keyId));
        var bytes = new byte[12];
        BinaryPrimitives.WriteUInt32LittleEndian(bytes, keyId);
        Assert.Throws<FormatException>(() => UbxX20ConfigurationCodec.DecodeValues(bytes));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(8)]
    [InlineData(9)]
    [InlineData(10)]
    public void DecodeValues_TruncatedKeyOrEightByteValue_RejectsIncompleteEntry(int length)
    {
        byte[] keyAndValue = [0x1A, 0x00, 0xB1, 0x50, 0x08, 0x07, 0x06, 0x05, 0x04, 0x03, 0x02, 0x01];
        var truncated = keyAndValue[..(length + 1)];
        Assert.Throws<FormatException>(() => UbxX20ConfigurationCodec.DecodeValues(truncated));
    }

    [Fact]
    public void EncodeValues_WrongNativeSignednessOrFloatingType_RejectsImplicitConversion()
    {
        Assert.Throws<ArgumentException>(() => UbxX20ConfigurationCodec.EncodeValues([new(UbxX20ConfigurationKeys.Navspg.InfilMinelev, (byte)5)]));
        Assert.Throws<ArgumentException>(() => UbxX20ConfigurationCodec.EncodeValues([new(UbxX20ConfigurationKeys.Navspg.UsrdatDx, 1.5)]));
        Assert.Throws<ArgumentException>(() => UbxX20ConfigurationCodec.EncodeValues([new(UbxX20ConfigurationKeys.Rate.Meas, 1000)]));
        Assert.Throws<ArgumentException>(() => UbxX20ConfigurationCodec.EncodeValues([new(UbxX20ConfigurationKeys.Bds.UseGeoPrn, (byte)1)]));
    }

    [Fact]
    public void EncodeDecodeValues_LogicalReservedBits_UsesOnlyDocumentedLeastSignificantBit()
    {
        Assert.True(Assert.IsType<bool>(Assert.Single(UbxX20ConfigurationCodec.DecodeValues([0x14, 0x00, 0x34, 0x10, 0xFF])).Value));
        Assert.False(Assert.IsType<bool>(Assert.Single(UbxX20ConfigurationCodec.DecodeValues([0x14, 0x00, 0x34, 0x10, 0xFE])).Value));
    }

    [Fact]
    public void ValuesAndKeyLists_Maximum64Entries_AcceptsBoundaryAndRejects65()
    {
        var entry = new UbxX20ConfigurationValue(UbxX20ConfigurationKeys.Bds.UseGeoPrn, true);
        var values = Enumerable.Repeat(entry, 64).ToArray();
        var valueBytes = UbxX20ConfigurationCodec.EncodeValues(values);
        Assert.Equal(64, UbxX20ConfigurationCodec.DecodeValues(valueBytes).Count);
        Assert.Throws<ArgumentException>(() => UbxX20ConfigurationCodec.EncodeValues(values.Append(entry)));
        Assert.Throws<FormatException>(() => UbxX20ConfigurationCodec.DecodeValues(valueBytes.Concat(valueBytes[..5]).ToArray()));

        var keys = Enumerable.Repeat(entry.KeyId, 64).ToArray();
        var keyBytes = UbxX20ConfigurationCodec.EncodeKeys(keys);
        Assert.Equal(keys, UbxX20ConfigurationCodec.DecodeKeys(keyBytes));
        Assert.Throws<ArgumentException>(() => UbxX20ConfigurationCodec.EncodeKeys(keys.Append(entry.KeyId)));
        Assert.Throws<FormatException>(() => UbxX20ConfigurationCodec.DecodeKeys(keyBytes.Concat(keyBytes[..4]).ToArray()));
        Assert.Throws<FormatException>(() => UbxX20ConfigurationCodec.DecodeKeys(keyBytes[..^1]));
    }

    [Fact]
    public void CreateValSet_VersionLayersAndTransaction_MatchIndependentPayloadBytes()
    {
        UbxX20ConfigurationValue[] values = [new(UbxX20ConfigurationKeys.Rate.Meas, (ushort)1000)];
        Assert.Equal(new byte[] { 0x00, 0x01, 0x00, 0x00, 0x01, 0x00, 0x21, 0x30, 0xE8, 0x03 },
            UbxX20ConfigurationCodec.CreateValSet(values));
        Assert.Equal(new byte[] { 0x01, 0x07, 0x01, 0x00, 0x01, 0x00, 0x21, 0x30, 0xE8, 0x03 },
            UbxX20ConfigurationCodec.CreateValSet(values,
                UbxX20ConfigurationLayers.Ram | UbxX20ConfigurationLayers.Bbr | UbxX20ConfigurationLayers.Flash,
                UbxX20ConfigurationTransaction.Begin));
        Assert.Equal(new byte[] { 0x01, 0x01, 0x03, 0x00 }, UbxX20ConfigurationCodec.CreateValSet([], transaction: UbxX20ConfigurationTransaction.Apply));
        Assert.Equal(new byte[] { 0x01, 0x01, 0x00, 0x00 }, UbxX20ConfigurationCodec.CreateValSet([], transaction: UbxX20ConfigurationTransaction.None));
    }

    [Fact]
    public void CreateValGetAndValDelete_WildcardsPositionAndLayers_MatchIndependentPayloadBytes()
    {
        uint[] keys = [UbxX20ConfigurationKeys.Bds.All, UbxX20ConfigurationCodec.AllKeys];
        byte[] keyBytes = [0xFF, 0xFF, 0x34, 0x10, 0xFF, 0xFF, 0xFF, 0x0F];

        Assert.Equal(keyBytes, UbxX20ConfigurationCodec.EncodeKeys(keys));
        Assert.Equal(keys, UbxX20ConfigurationCodec.DecodeKeys(keyBytes));
        Assert.Equal(new byte[] { 0x00, 0x07, 0x34, 0x12 }.Concat(keyBytes),
            UbxX20ConfigurationCodec.CreateValGetRequest(keys, UbxX20ConfigurationLayer.Default, 0x1234));
        Assert.Equal(new byte[] { 0x01, 0x06, 0x02, 0x00 }.Concat(keyBytes),
            UbxX20ConfigurationCodec.CreateValDelete(keys, transaction: UbxX20ConfigurationTransaction.Continue));
        Assert.Equal(new byte[] { 0x00, 0x06, 0x00, 0x00 }.Concat(keyBytes),
            UbxX20ConfigurationCodec.CreateValDelete(keys));
        Assert.Equal(new byte[] { 0x01, 0x02, 0x40, 0x00, 0x14, 0x00, 0x34, 0x10, 0x01 },
            UbxX20ConfigurationCodec.CreateValGetResponse([new(UbxX20ConfigurationKeys.Bds.UseGeoPrn, true)], UbxX20ConfigurationLayer.Flash, 64));
    }

    [Fact]
    public void KeyLists_WildcardsUseGroupAndItemFields_IgnoreValueStorageBits()
    {
        uint[] keys = [0x0034FFFF, 0x7034FFFF, 0x1FFFFFFF, 0x0FFF0000];
        byte[] expected =
        [
            0xFF, 0xFF, 0x34, 0x00,
            0xFF, 0xFF, 0x34, 0x70,
            0xFF, 0xFF, 0xFF, 0x1F,
            0x00, 0x00, 0xFF, 0x0F,
        ];
        Assert.Equal(expected, UbxX20ConfigurationCodec.EncodeKeys(keys));
        Assert.Equal(keys, UbxX20ConfigurationCodec.DecodeKeys(expected));
        Assert.Throws<ArgumentOutOfRangeException>(() => UbxX20ConfigurationCodec.EncodeKeys([0x10FFFFFF]));
        Assert.Throws<ArgumentOutOfRangeException>(() => UbxX20ConfigurationCodec.EncodeKeys([0x1000FFFF]));
    }

    [Theory]
    [InlineData(0u)]
    [InlineData(1u)]
    [InlineData(2u)]
    [InlineData(3u)]
    [InlineData(4u)]
    [InlineData(5u)]
    [InlineData(6u)]
    [InlineData(7u)]
    public void KeyLists_WildcardStorageCodesZeroThroughSeven_AreNotInterpretedAsValueSizes(uint storage)
    {
        uint[] keys = [(storage << 28) | 0x0034FFFF, (storage << 28) | 0x0FFFFFFF];
        Assert.Equal(keys, UbxX20ConfigurationCodec.DecodeKeys(UbxX20ConfigurationCodec.EncodeKeys(keys)));
    }

    [Theory]
    [InlineData(0x8034FFFFu)]
    [InlineData(0x8FFFFFFFu)]
    [InlineData(0xF034FFFFu)]
    public void KeyLists_WildcardsWithReservedBit31_RejectsReservedKey(uint keyId)
    {
        var bytes = new byte[4];
        BinaryPrimitives.WriteUInt32LittleEndian(bytes, keyId);
        Assert.Throws<ArgumentOutOfRangeException>(() => UbxX20ConfigurationCodec.EncodeKeys([keyId]));
        Assert.Throws<FormatException>(() => UbxX20ConfigurationCodec.DecodeKeys(bytes));
    }

    [Fact]
    public void PayloadBuilders_InvalidLayersOrTransaction_RejectsUnsupportedHeaderValues()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => UbxX20ConfigurationCodec.CreateValSet([], (UbxX20ConfigurationLayers)0));
        Assert.Throws<ArgumentOutOfRangeException>(() => UbxX20ConfigurationCodec.CreateValSet([], (UbxX20ConfigurationLayers)8));
        Assert.Throws<ArgumentOutOfRangeException>(() => UbxX20ConfigurationCodec.CreateValSet([], transaction: (UbxX20ConfigurationTransaction)4));
        Assert.Throws<ArgumentOutOfRangeException>(() => UbxX20ConfigurationCodec.CreateValDelete([], UbxX20ConfigurationLayers.Ram));
        Assert.Throws<ArgumentOutOfRangeException>(() => UbxX20ConfigurationCodec.CreateValDelete([], (UbxX20ConfigurationLayers)7));
        Assert.Throws<ArgumentOutOfRangeException>(() => UbxX20ConfigurationCodec.CreateValGetRequest([], (UbxX20ConfigurationLayer)3));
        Assert.Throws<ArgumentOutOfRangeException>(() => UbxX20ConfigurationCodec.CreateValGetResponse([], (UbxX20ConfigurationLayer)4));
    }

    [Fact]
    public void GeneratedCfgModels_TypedDataAndPayloadHelpers_ProduceIdenticalFramesForEveryVariant()
    {
        UbxX20ConfigurationValue[] values = [new(UbxX20ConfigurationKeys.Rate.Meas, (ushort)1000)];
        uint[] keys = [UbxX20ConfigurationKeys.Rate.All];
        var cfgData = UbxX20ConfigurationCodec.EncodeValues(values);

        Assert.Equal(UbxTestHelper.CreateFrame(0x06, 0x8A, UbxX20ConfigurationCodec.CreateValSet(values)),
            UbxTestHelper.Serialize(new UbxX20CfgValsetData0 { Layers = 1, CfgData = cfgData }));
        Assert.Equal(UbxTestHelper.CreateFrame(0x06, 0x8A,
                UbxX20ConfigurationCodec.CreateValSet(values, UbxX20ConfigurationLayers.Ram | UbxX20ConfigurationLayers.Bbr,
                    UbxX20ConfigurationTransaction.Begin)),
            UbxTestHelper.Serialize(new UbxX20CfgValsetData1 { Layers = 3, Transaction = 1, CfgData = cfgData }));
        Assert.Equal(UbxTestHelper.CreateFrame(0x06, 0x8B,
                UbxX20ConfigurationCodec.CreateValGetRequest(keys, UbxX20ConfigurationLayer.Default, 64)),
            UbxTestHelper.Serialize(new UbxX20CfgValgetPollid { Layer = 7, Position = 64, Keys = keys }));
        Assert.Equal(UbxTestHelper.CreateFrame(0x06, 0x8B,
                UbxX20ConfigurationCodec.CreateValGetResponse(values, UbxX20ConfigurationLayer.Default, 64)),
            UbxTestHelper.Serialize(new UbxX20CfgValgetData0 { Layer = 7, Position = 64, CfgData = cfgData }));
        Assert.Equal(UbxTestHelper.CreateFrame(0x06, 0x8C, UbxX20ConfigurationCodec.CreateValDelete(keys)),
            UbxTestHelper.Serialize(new UbxX20CfgValdelData0 { Layers = 6, Keys = keys }));
        Assert.Equal(UbxTestHelper.CreateFrame(0x06, 0x8C,
                UbxX20ConfigurationCodec.CreateValDelete(keys, transaction: UbxX20ConfigurationTransaction.Apply)),
            UbxTestHelper.Serialize(new UbxX20CfgValdelData1 { Layers = 6, Transaction = 3, Keys = keys }));
    }

    private static object NativeZero(UbxX20ConfigurationType type) => type switch
    {
        UbxX20ConfigurationType.L => false,
        UbxX20ConfigurationType.U1 or UbxX20ConfigurationType.E1 or UbxX20ConfigurationType.X1 => (byte)0,
        UbxX20ConfigurationType.U2 or UbxX20ConfigurationType.E2 or UbxX20ConfigurationType.X2 => (ushort)0,
        UbxX20ConfigurationType.U4 or UbxX20ConfigurationType.E4 or UbxX20ConfigurationType.X4 => 0u,
        UbxX20ConfigurationType.U8 or UbxX20ConfigurationType.X8 => 0UL,
        UbxX20ConfigurationType.I1 => (sbyte)0,
        UbxX20ConfigurationType.I2 => (short)0,
        UbxX20ConfigurationType.I4 => 0,
        UbxX20ConfigurationType.I8 => 0L,
        UbxX20ConfigurationType.R4 => 0f,
        UbxX20ConfigurationType.R8 => 0.0,
        _ => throw new ArgumentOutOfRangeException(nameof(type)),
    };
}
