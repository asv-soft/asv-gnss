using System;
using System.Collections.Generic;
using System.Linq;
using Asv.IO;

namespace Asv.Gnss;

public class AsterixFieldI020Frn024Type030 : AsterixField
{
    public const byte StaticFrn = 24;
    public const string StaticName = "Warning/Error Conditions";
    public override string Name => StaticName;
    public override int Category => AsterixMessageI020.Category;
    public override byte FieldReferenceNumber => StaticFrn;
    
    private static StructType? type;
    public static StructType StructType => type ??= new StructType([
        WarningErrorConditionsField
    ]);
    
    private static readonly Field WarningErrorConditionsField = new Field.Builder()
        .Name(nameof(WarningErrorConditions))
        .Title("Warning/Error Conditions")
        .Description("List of warning/error condition values")
        .DataType(new ListType(UInt8Type.Default, 0, 255))
        .Build();
    
    public override void Deserialize(ref ReadOnlySpan<byte> buffer) => RawValue.Deserialize(ref buffer);

    public override void Serialize(ref Span<byte> buffer) => RawValue.Serialize(ref buffer);

    public override int GetByteSize() => RawValue.GetByteSize();
    
    public VariableLengthValue RawValue { get; } = new();

    public override void Accept(IVisitor visitor)
    {
        var warningErrorList = WarningErrorConditions.ToList();
        ListType.Accept(visitor, WarningErrorConditionsField, WarningErrorConditionsField.DataType, warningErrorList, (index, v, f, t) =>
        {
            var val = (byte)warningErrorList[index];
            UInt8Type.Accept(v, f, t, ref val);
            warningErrorList[index] = (WarningErrorCode)val;
        });
        
        // Update the internal collection
        _warningErrorConditions.Clear();
        _warningErrorConditions.AddRange(warningErrorList);
    }
    
    private readonly List<WarningErrorCode> _warningErrorConditions = new();
    
    /// <summary>
    /// Gets the list of warning/error conditions
    /// </summary>
    public IReadOnlyList<WarningErrorCode> WarningErrorConditions => _warningErrorConditions.AsReadOnly();
    
    /// <summary>
    /// Adds a warning/error condition
    /// </summary>
    /// <param name="condition">The warning/error condition to add</param>
    public void AddWarningErrorCondition(WarningErrorCode condition)
    {
        if (!_warningErrorConditions.Contains(condition))
        {
            _warningErrorConditions.Add(condition);
            UpdateRawValue();
        }
    }
    
    /// <summary>
    /// Removes a warning/error condition
    /// </summary>
    /// <param name="condition">The warning/error condition to remove</param>
    public void RemoveWarningErrorCondition(WarningErrorCode condition)
    {
        if (_warningErrorConditions.Remove(condition))
        {
            UpdateRawValue();
        }
    }
    
    /// <summary>
    /// Clears all warning/error conditions
    /// </summary>
    public void ClearWarningErrorConditions()
    {
        _warningErrorConditions.Clear();
        RawValue.Clear();
    }
    
    /// <summary>
    /// Checks if a specific warning/error condition exists
    /// </summary>
    /// <param name="condition">The condition to check for</param>
    /// <returns>True if the condition exists, false otherwise</returns>
    public bool HasWarningErrorCondition(WarningErrorCode condition)
    {
        return _warningErrorConditions.Contains(condition);
    }
    
    /// <summary>
    /// Gets the warning/error condition at the specified octet index
    /// </summary>
    /// <param name="octetIndex">The octet index (0-based)</param>
    /// <returns>The warning/error condition value, or null if not available</returns>
    public WarningErrorCode? GetWarningErrorConditionAt(int octetIndex)
    {
        if (octetIndex >= 0 && octetIndex < _warningErrorConditions.Count)
        {
            return _warningErrorConditions[octetIndex];
        }
        return null;
    }
    
    /// <summary>
    /// Updates the raw value based on current warning/error conditions
    /// </summary>
    private void UpdateRawValue()
    {
        RawValue.Clear();
        
        for (int i = 0; i < _warningErrorConditions.Count; i++)
        {
            var condition = _warningErrorConditions[i];
            var octetValue = (byte)condition;
            
            // Set FX bit (bit 1) if this is not the last octet
            if (i < _warningErrorConditions.Count - 1)
            {
                octetValue |= 0x01; // Set FX bit
            }
            
            // The condition value occupies bits 8-2, so shift left by 1
            var shiftedValue = (byte)(((byte)condition << 1) | (octetValue & 0x01));
            
            RawValue[i * 8] = (shiftedValue & 0x80) != 0; // bit 8
            RawValue[i * 8 + 1] = (shiftedValue & 0x40) != 0; // bit 7
            RawValue[i * 8 + 2] = (shiftedValue & 0x20) != 0; // bit 6
            RawValue[i * 8 + 3] = (shiftedValue & 0x10) != 0; // bit 5
            RawValue[i * 8 + 4] = (shiftedValue & 0x08) != 0; // bit 4
            RawValue[i * 8 + 5] = (shiftedValue & 0x04) != 0; // bit 3
            RawValue[i * 8 + 6] = (shiftedValue & 0x02) != 0; // bit 2
            RawValue[i * 8 + 7] = (shiftedValue & 0x01) != 0; // bit 1 (FX)
        }
    }
    
    /// <summary>
    /// Updates the warning/error conditions list from the raw value
    /// </summary>
    private void UpdateFromRawValue()
    {
        _warningErrorConditions.Clear();
        
        var octetIndex = 0;
        bool hasExtension = true;
        
        while (hasExtension && octetIndex * 8 < RawValue.DataBitsCount)
        {
            // Extract the 7-bit value (bits 8-2)
            byte conditionValue = 0;
            
            for (int bitIndex = 0; bitIndex < 7; bitIndex++)
            {
                var bit = RawValue[octetIndex * 8 + bitIndex];
                if (bit.HasValue && bit.Value)
                {
                    conditionValue |= (byte)(1 << (6 - bitIndex));
                }
            }
            
            if (conditionValue != 0) // Only add non-zero conditions
            {
                _warningErrorConditions.Add((WarningErrorCode)conditionValue);
            }
            
            // Check FX bit (bit 1)
            var fxBit = RawValue[octetIndex * 8 + 7];
            hasExtension = fxBit.HasValue && fxBit.Value;
            
            octetIndex++;
        }
    }
}

/// <summary>
/// Warning/Error condition codes as defined in the specification
/// </summary>
public enum WarningErrorCode : byte
{
    /// <summary>
    /// Not defined; never used
    /// </summary>
    NotDefined = 0,
    
    /// <summary>
    /// Multipath Reply (Reflection)
    /// </summary>
    MultipathReply = 1,
    
    /// <summary>
    /// Split plot
    /// </summary>
    SplitPlot = 3,
    
    /// <summary>
    /// Phantom SSR plot
    /// </summary>
    PhantomSSRPlot = 10,
    
    /// <summary>
    /// Non-Matching Mode-3/A Code
    /// </summary>
    NonMatchingMode3ACode = 11,
    
    /// <summary>
    /// Mode C code / Mode S altitude code abnormal value compared to the track
    /// </summary>
    AbnormalAltitudeCode = 12,
    
    /// <summary>
    /// Transponder anomaly detected
    /// </summary>
    TransponderAnomaly = 15,
    
    /// <summary>
    /// Duplicated or Illegal Mode S Aircraft Address
    /// </summary>
    DuplicatedIllegalAddress = 16,
    
    /// <summary>
    /// Mode S error correction applied
    /// </summary>
    ModeSErrorCorrection = 17,
    
    /// <summary>
    /// Undecodable Mode C code / Mode S altitude code
    /// </summary>
    UndecodableAltitudeCode = 18
}

/// <summary>
/// Extension methods for WarningErrorCode enum
/// </summary>
public static class WarningErrorCodeExtensions
{
    /// <summary>
    /// Gets a human-readable description of the warning/error code
    /// </summary>
    /// <param name="code">The warning/error code</param>
    /// <returns>A description string</returns>
    public static string GetDescription(this WarningErrorCode code)
    {
        return code switch
        {
            WarningErrorCode.NotDefined => "Not defined; never used",
            WarningErrorCode.MultipathReply => "Multipath Reply (Reflection)",
            WarningErrorCode.SplitPlot => "Split plot",
            WarningErrorCode.PhantomSSRPlot => "Phantom SSR plot",
            WarningErrorCode.NonMatchingMode3ACode => "Non-Matching Mode-3/A Code",
            WarningErrorCode.AbnormalAltitudeCode => "Mode C code / Mode S altitude code abnormal value compared to the track",
            WarningErrorCode.TransponderAnomaly => "Transponder anomaly detected",
            WarningErrorCode.DuplicatedIllegalAddress => "Duplicated or Illegal Mode S Aircraft Address",
            WarningErrorCode.ModeSErrorCorrection => "Mode S error correction applied",
            WarningErrorCode.UndecodableAltitudeCode => "Undecodable Mode C code / Mode S altitude code",
            _ => $"Unknown warning/error code: {(byte)code}"
        };
    }
}