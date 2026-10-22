using System;
using System.Buffers.Binary;
using System.Collections.Generic;

namespace Asv.Gnss;

/// <summary>Writable configuration layers used by VALSET and VALDEL.</summary>
[Flags]
public enum UbxX20ConfigurationLayers : byte
{
    /// <summary>Current volatile configuration; not supported by VALDEL.</summary>
    Ram = 1,
    /// <summary>Battery-backed configuration.</summary>
    Bbr = 2,
    /// <summary>External flash configuration, where available.</summary>
    Flash = 4,
}

/// <summary>One source layer for VALGET; these values are not a bit mask.</summary>
public enum UbxX20ConfigurationLayer : byte
{
    /// <summary>Current volatile configuration.</summary>
    Ram = 0,
    /// <summary>Battery-backed configuration.</summary>
    Bbr = 1,
    /// <summary>External flash configuration.</summary>
    Flash = 2,
    /// <summary>Read-only factory defaults.</summary>
    Default = 7,
}

/// <summary>Version 1 VALSET or VALDEL transaction action.</summary>
public enum UbxX20ConfigurationTransaction : byte
{
    /// <summary>Cancel any pending transaction and apply this message immediately.</summary>
    None = 0,
    /// <summary>Start or restart a transaction.</summary>
    Begin = 1,
    /// <summary>Continue an already started transaction.</summary>
    Continue = 2,
    /// <summary>Apply and end an already started transaction.</summary>
    Apply = 3,
}

/// <summary>A key and its raw native value, without applying the catalog scale.</summary>
/// <param name="KeyId">Complete configuration key ID.</param>
/// <param name="Value">Exact CLR value type: bool, byte, ushort, uint, ulong, sbyte, short, int, long, float, or double. Unknown keys use byte[].</param>
public readonly record struct UbxX20ConfigurationValue(uint KeyId, object Value);

/// <summary>Encodes X20 CFG-VAL configuration data and complete UBX payloads without the UBX frame header or checksum.</summary>
/// <remarks>
/// Known values use the catalog native type and little-endian encoding. Unknown valid key IDs use
/// raw byte arrays because their size does not determine signedness or floating-point interpretation.
/// Scales, accepted ranges and cross-key dependencies remain receiver responsibilities.
/// Transaction sequencing is stateful receiver behavior; these helpers validate each message only.
/// </remarks>
public static class UbxX20ConfigurationCodec
{
    /// <summary>Maximum number of keys or key-value pairs in one CFG-VAL message.</summary>
    public const int MaximumEntries = 64;

    /// <summary>Wildcard selecting all groups and items for VALGET or VALDEL.</summary>
    public const uint AllKeys = 0x0FFFFFFF;

    /// <summary>Gets the value's wire size and rejects reserved or wildcard item IDs.</summary>
    public static int GetValueSize(uint keyId)
    {
        var group = (keyId >> 16) & 0xFF;
        var item = keyId & 0xFFF;
        if ((keyId & 0x8F00F000) != 0 || group is 0 or 0xFF || item is 0 or 0xFFF)
        {
            throw new ArgumentOutOfRangeException(nameof(keyId), keyId, "Reserved configuration key bits, group or item ID.");
        }

        return ((keyId >> 28) & 7) switch
        {
            1 or 2 => 1,
            3 => 2,
            4 => 4,
            5 => 8,
            _ => throw new ArgumentOutOfRangeException(nameof(keyId), keyId, "Configuration storage size must be 1 through 5."),
        };
    }

    /// <summary>Encodes at most 64 key-value pairs for the CfgData property of VALSET or a VALGET response.</summary>
    public static byte[] EncodeValues(IEnumerable<UbxX20ConfigurationValue> values)
    {
        ArgumentNullException.ThrowIfNull(values);
        var buffer = new byte[MaximumEntries * 12];
        var offset = 0;
        var count = 0;
        foreach (var entry in values)
        {
            CheckCount(++count);
            var size = GetValueSize(entry.KeyId);
            BinaryPrimitives.WriteUInt32LittleEndian(buffer.AsSpan(offset), entry.KeyId);
            WriteValue(entry.KeyId, entry.Value, buffer.AsSpan(offset + 4, size));
            offset += 4 + size;
        }

        return buffer.AsSpan(0, offset).ToArray();
    }

    /// <summary>Decodes at most 64 key-value pairs, preserving unknown valid-sized values as raw byte arrays.</summary>
    /// <exception cref="FormatException">An entry has an invalid key ID or incomplete value, or there are more than 64 entries.</exception>
    public static IReadOnlyList<UbxX20ConfigurationValue> DecodeValues(ReadOnlySpan<byte> data)
    {
        var result = new List<UbxX20ConfigurationValue>();
        while (!data.IsEmpty)
        {
            if (result.Count == MaximumEntries || data.Length < 4)
            {
                throw new FormatException("Configuration data contains too many entries or a truncated key ID.");
            }

            var keyId = BinaryPrimitives.ReadUInt32LittleEndian(data);
            var size = GetDecodedSize(keyId);
            if (data.Length < 4 + size)
            {
                throw new FormatException($"Configuration value for key 0x{keyId:X8} is truncated.");
            }

            result.Add(new UbxX20ConfigurationValue(keyId, ReadValue(keyId, data.Slice(4, size))));
            data = data[(4 + size)..];
        }

        return result.AsReadOnly();
    }

    /// <summary>Encodes at most 64 key IDs for VALGET requests or VALDEL, including documented group/all-item wildcards.</summary>
    public static byte[] EncodeKeys(IEnumerable<uint> keys)
    {
        ArgumentNullException.ThrowIfNull(keys);
        var buffer = new byte[MaximumEntries * 4];
        var count = 0;
        foreach (var keyId in keys)
        {
            CheckCount(++count);
            ValidateListKey(keyId);
            BinaryPrimitives.WriteUInt32LittleEndian(buffer.AsSpan((count - 1) * 4), keyId);
        }

        return buffer.AsSpan(0, count * 4).ToArray();
    }

    /// <summary>Decodes a VALGET-request or VALDEL key list, accepting documented wildcards.</summary>
    public static IReadOnlyList<uint> DecodeKeys(ReadOnlySpan<byte> data)
    {
        if (data.Length % 4 != 0 || data.Length / 4 > MaximumEntries)
        {
            throw new FormatException("Configuration key list is truncated or contains more than 64 entries.");
        }

        var result = new uint[data.Length / 4];
        for (var index = 0; index < result.Length; index++)
        {
            var keyId = BinaryPrimitives.ReadUInt32LittleEndian(data[(index * 4)..]);
            try
            {
                ValidateListKey(keyId);
            }
            catch (ArgumentOutOfRangeException exception)
            {
                throw new FormatException($"Invalid configuration key ID 0x{keyId:X8}.", exception);
            }

            result[index] = keyId;
        }

        return Array.AsReadOnly(result);
    }

    /// <summary>Creates a complete VALSET payload. A null transaction selects version 0; any action selects version 1.</summary>
    public static byte[] CreateValSet(IEnumerable<UbxX20ConfigurationValue> values,
        UbxX20ConfigurationLayers layers = UbxX20ConfigurationLayers.Ram,
        UbxX20ConfigurationTransaction? transaction = null)
    {
        ValidateWriteHeader(layers, transaction, false);
        return CreateWritePayload(EncodeValues(values), layers, transaction);
    }

    /// <summary>Creates a complete VALDEL payload. Only BBR and Flash layers can be deleted.</summary>
    public static byte[] CreateValDelete(IEnumerable<uint> keys,
        UbxX20ConfigurationLayers layers = UbxX20ConfigurationLayers.Bbr | UbxX20ConfigurationLayers.Flash,
        UbxX20ConfigurationTransaction? transaction = null)
    {
        ValidateWriteHeader(layers, transaction, true);
        return CreateWritePayload(EncodeKeys(keys), layers, transaction);
    }

    /// <summary>Creates a version 0 VALGET poll payload, including pagination position and optional wildcard keys.</summary>
    public static byte[] CreateValGetRequest(IEnumerable<uint> keys,
        UbxX20ConfigurationLayer layer = UbxX20ConfigurationLayer.Ram, ushort position = 0) =>
        CreateReadPayload(EncodeKeys(keys), 0, layer, position);

    /// <summary>Creates a version 1 VALGET response payload, primarily for receiver emulation and fixtures.</summary>
    public static byte[] CreateValGetResponse(IEnumerable<UbxX20ConfigurationValue> values,
        UbxX20ConfigurationLayer layer = UbxX20ConfigurationLayer.Ram, ushort position = 0) =>
        CreateReadPayload(EncodeValues(values), 1, layer, position);

    private static byte[] CreateWritePayload(byte[] data, UbxX20ConfigurationLayers layers,
        UbxX20ConfigurationTransaction? transaction)
    {
        var result = new byte[4 + data.Length];
        result[0] = transaction.HasValue ? (byte)1 : (byte)0;
        result[1] = (byte)layers;
        result[2] = (byte)(transaction ?? UbxX20ConfigurationTransaction.None);
        data.CopyTo(result, 4);
        return result;
    }

    private static byte[] CreateReadPayload(byte[] data, byte version, UbxX20ConfigurationLayer layer,
        ushort position)
    {
        if (layer is not (UbxX20ConfigurationLayer.Ram or UbxX20ConfigurationLayer.Bbr or
            UbxX20ConfigurationLayer.Flash or UbxX20ConfigurationLayer.Default))
        {
            throw new ArgumentOutOfRangeException(nameof(layer));
        }

        var result = new byte[4 + data.Length];
        result[0] = version;
        result[1] = (byte)layer;
        BinaryPrimitives.WriteUInt16LittleEndian(result.AsSpan(2), position);
        data.CopyTo(result, 4);
        return result;
    }

    private static void ValidateWriteHeader(UbxX20ConfigurationLayers layers,
        UbxX20ConfigurationTransaction? transaction, bool delete)
    {
        var mask = delete ? 6 : 7;
        if ((byte)layers == 0 || ((byte)layers & ~mask) != 0)
        {
            throw new ArgumentOutOfRangeException(nameof(layers), "Specify at least one permitted configuration layer.");
        }

        if (transaction.HasValue && (byte)transaction.Value > 3)
        {
            throw new ArgumentOutOfRangeException(nameof(transaction));
        }
    }

    private static void CheckCount(int count)
    {
        if (count > MaximumEntries)
        {
            throw new ArgumentException("A CFG-VAL message can contain at most 64 entries.");
        }
    }

    private static void ValidateListKey(uint keyId)
    {
        var group = (keyId >> 16) & 0xFFF;
        // VALGET/VALDEL define wildcards by group/item fields, independently of value storage size.
        if ((keyId & 0x80000000) == 0 && (group == 0xFFF ||
            (group is > 0 and < 0xFF && (keyId & 0xFFFF) == 0xFFFF)))
        {
            return;
        }

        _ = GetValueSize(keyId);
    }

    private static int GetDecodedSize(uint keyId)
    {
        try
        {
            return GetValueSize(keyId);
        }
        catch (ArgumentOutOfRangeException exception)
        {
            throw new FormatException($"Invalid configuration key ID 0x{keyId:X8}.", exception);
        }
    }

    private static object ReadValue(uint keyId, ReadOnlySpan<byte> bytes)
    {
        if (!UbxX20ConfigurationCatalog.TryGet(keyId, out var item))
        {
            return bytes.ToArray();
        }

        return item!.Type switch
        {
            UbxX20ConfigurationType.L => (bytes[0] & 1) != 0,
            UbxX20ConfigurationType.U1 or UbxX20ConfigurationType.E1 or UbxX20ConfigurationType.X1 => bytes[0],
            UbxX20ConfigurationType.U2 or UbxX20ConfigurationType.E2 or UbxX20ConfigurationType.X2 => BinaryPrimitives.ReadUInt16LittleEndian(bytes),
            UbxX20ConfigurationType.U4 or UbxX20ConfigurationType.E4 or UbxX20ConfigurationType.X4 => BinaryPrimitives.ReadUInt32LittleEndian(bytes),
            UbxX20ConfigurationType.U8 or UbxX20ConfigurationType.X8 => BinaryPrimitives.ReadUInt64LittleEndian(bytes),
            UbxX20ConfigurationType.I1 => unchecked((sbyte)bytes[0]),
            UbxX20ConfigurationType.I2 => BinaryPrimitives.ReadInt16LittleEndian(bytes),
            UbxX20ConfigurationType.I4 => BinaryPrimitives.ReadInt32LittleEndian(bytes),
            UbxX20ConfigurationType.I8 => BinaryPrimitives.ReadInt64LittleEndian(bytes),
            UbxX20ConfigurationType.R4 => BinaryPrimitives.ReadSingleLittleEndian(bytes),
            UbxX20ConfigurationType.R8 => BinaryPrimitives.ReadDoubleLittleEndian(bytes),
            _ => throw new InvalidOperationException("Unsupported catalog configuration type."),
        };
    }

    private static void WriteValue(uint keyId, object value, Span<byte> bytes)
    {
        if (!UbxX20ConfigurationCatalog.TryGet(keyId, out var item))
        {
            if (value is not byte[] raw || raw.Length != bytes.Length)
            {
                throw new ArgumentException($"Unknown key 0x{keyId:X8} requires exactly {bytes.Length} raw bytes.", nameof(value));
            }

            raw.CopyTo(bytes);
            return;
        }

        switch (item!.Type, value)
        {
            case (UbxX20ConfigurationType.L, bool logical):
                bytes[0] = logical ? (byte)1 : (byte)0;
                break;
            case (UbxX20ConfigurationType.U1 or UbxX20ConfigurationType.E1 or UbxX20ConfigurationType.X1, byte number):
                bytes[0] = number;
                break;
            case (UbxX20ConfigurationType.U2 or UbxX20ConfigurationType.E2 or UbxX20ConfigurationType.X2, ushort number):
                BinaryPrimitives.WriteUInt16LittleEndian(bytes, number);
                break;
            case (UbxX20ConfigurationType.U4 or UbxX20ConfigurationType.E4 or UbxX20ConfigurationType.X4, uint number):
                BinaryPrimitives.WriteUInt32LittleEndian(bytes, number);
                break;
            case (UbxX20ConfigurationType.U8 or UbxX20ConfigurationType.X8, ulong number):
                BinaryPrimitives.WriteUInt64LittleEndian(bytes, number);
                break;
            case (UbxX20ConfigurationType.I1, sbyte number):
                bytes[0] = unchecked((byte)number);
                break;
            case (UbxX20ConfigurationType.I2, short number):
                BinaryPrimitives.WriteInt16LittleEndian(bytes, number);
                break;
            case (UbxX20ConfigurationType.I4, int number):
                BinaryPrimitives.WriteInt32LittleEndian(bytes, number);
                break;
            case (UbxX20ConfigurationType.I8, long number):
                BinaryPrimitives.WriteInt64LittleEndian(bytes, number);
                break;
            case (UbxX20ConfigurationType.R4, float number):
                BinaryPrimitives.WriteSingleLittleEndian(bytes, number);
                break;
            case (UbxX20ConfigurationType.R8, double number):
                BinaryPrimitives.WriteDoubleLittleEndian(bytes, number);
                break;
            default:
                throw new ArgumentException($"{item.Name} requires its exact native CLR type ({item.Type}).", nameof(value));
        }
    }
}
