using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Asv.Gnss;

/// <summary>Native configuration value types defined by the X20 interface description, section 6.2.</summary>
public enum UbxX20ConfigurationType
{
    /// <summary>Single-bit logical value, stored in one byte.</summary>
    L,
    /// <summary>Unsigned 8-bit integer.</summary>
    U1,
    /// <summary>Unsigned 16-bit integer.</summary>
    U2,
    /// <summary>Unsigned 32-bit integer.</summary>
    U4,
    /// <summary>Unsigned 64-bit integer.</summary>
    U8,
    /// <summary>Signed 8-bit integer.</summary>
    I1,
    /// <summary>Signed 16-bit integer.</summary>
    I2,
    /// <summary>Signed 32-bit integer.</summary>
    I4,
    /// <summary>Signed 64-bit integer.</summary>
    I8,
    /// <summary>IEEE 754 single-precision floating-point value.</summary>
    R4,
    /// <summary>IEEE 754 double-precision floating-point value.</summary>
    R8,
    /// <summary>Unsigned 8-bit enumeration.</summary>
    E1,
    /// <summary>Unsigned 16-bit enumeration.</summary>
    E2,
    /// <summary>Unsigned 32-bit enumeration.</summary>
    E4,
    /// <summary>8-bit binary data or bit field.</summary>
    X1,
    /// <summary>16-bit binary data or bit field.</summary>
    X2,
    /// <summary>32-bit binary data or bit field.</summary>
    X4,
    /// <summary>64-bit binary data or bit field.</summary>
    X8,
}

/// <summary>A named value as written in the official JSON; an empty value remains unspecified.</summary>
/// <param name="Name">Official constant name.</param>
/// <param name="Value">Unmodified source value, including empty strings.</param>
/// <param name="Title">Official explanation.</param>
public sealed record UbxX20ConfigurationConstant(string Name, string Value, string Title);

/// <summary>Source metadata for one configuration key; values and scales are not converted to physical units.</summary>
/// <param name="KeyId">Complete 32-bit key ID.</param>
/// <param name="Name">Complete CFG-GROUP-ITEM name.</param>
/// <param name="Group">Official group name.</param>
/// <param name="Item">Official item name.</param>
/// <param name="Type">Native wire-value type.</param>
/// <param name="Scale">Unmodified scale expression, or null when absent.</param>
/// <param name="Unit">Unmodified unit, or null when absent.</param>
/// <param name="Title">Official short description.</param>
/// <param name="Description">Official detailed description.</param>
/// <param name="Constants">Named values and flags from the source.</param>
public sealed record UbxX20ConfigurationItem(
    uint KeyId, string Name, string Group, string Item, UbxX20ConfigurationType Type,
    string? Scale, string? Unit, string Title, string Description,
    IReadOnlyList<UbxX20ConfigurationConstant> Constants);

/// <summary>Source metadata for a configuration group and its wildcard key.</summary>
/// <param name="Id">Eight-bit group ID.</param>
/// <param name="Name">Official group name.</param>
/// <param name="WildcardKeyId">Key selecting every item in the group for VALGET or VALDEL.</param>
/// <param name="Title">Official short description.</param>
/// <param name="Description">Official detailed description.</param>
/// <param name="WildcardDescription">Official wildcard description.</param>
public sealed record UbxX20ConfigurationGroup(
    byte Id, string Name, uint WildcardKeyId, string Title, string Description,
    string WildcardDescription);

/// <summary>Configuration metadata from the official ZED-X20P X20 HPG 2.11 JSON, protocol version 50.11.</summary>
public static partial class UbxX20ConfigurationCatalog
{
    private static class Lookup
    {
        internal static readonly FrozenDictionary<uint, UbxX20ConfigurationItem> ItemsById =
            Items.ToFrozenDictionary(x => x.KeyId);
        internal static readonly FrozenDictionary<string, UbxX20ConfigurationItem> ItemsByName =
            Items.ToFrozenDictionary(x => x.Name, StringComparer.Ordinal);
    }

    /// <summary>Finds a known key by its complete 32-bit ID.</summary>
    public static bool TryGet(uint keyId, [NotNullWhen(true)] out UbxX20ConfigurationItem? item) =>
        Lookup.ItemsById.TryGetValue(keyId, out item);

    /// <summary>Finds a known key by its exact, case-sensitive CFG-GROUP-ITEM name.</summary>
    public static bool TryGet(string name, [NotNullWhen(true)] out UbxX20ConfigurationItem? item) =>
        Lookup.ItemsByName.TryGetValue(name, out item);
}
