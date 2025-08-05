using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Asv.IO;

namespace Asv.Gnss;

/// <summary>
/// Data Item I020/400, Contributing Devices
/// Definition: Overview of the devices, which have contributed to the Target
/// Detection. In case of active Multilateration systems this may
/// include transmitters.
/// Format: Repetitive Data Item starting with a one-octet Field Repetition
/// Indicator (REP) followed by at least one Contributing Units list
/// comprising one octet
/// Structure:
/// Octet no. 1
/// 16 15 14 13 12 11 10 9
/// REP
/// Octet no. 2+
/// 8 7 6 5 4 3 2 1
/// x x x x x x x x
/// Bits 16/9 (REP) Repetition Factor
/// Bit x (1≤ x ≤ 8) TUx/RUx Contribution
/// = 0 TUx/RUx has NOT contributed to the target detection
/// = 1 TUx/RUx has contributed to the target detection
/// Encoding Rule: This item is optional
/// </summary>
public class AsterixFieldI020Frn020Type400 : AsterixField, IEnumerable
{
    public const byte StaticFrn = 20;
    public const string StaticName = "Contributing Devices";
    
    public override string Name => StaticName;
    public override int Category => AsterixMessageI020.Category;
    public override byte FieldReferenceNumber => StaticFrn;
    
    private readonly List<byte> _contributingUnits = new();
    
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        _contributingUnits.Clear();
        
        // Read REP (Repetition Factor) - number of octets following
        var rep = buffer[0];
        buffer = buffer[1..];
        
        // Read each octet and extract contributing units
        for (var octetIndex = 0; octetIndex < rep; octetIndex++)
        {
            if (octetIndex >= buffer.Length) break;
            
            var octetValue = buffer[octetIndex];
            
            // Check each bit in the octet (bits 8-1, from MSB to LSB)
            for (var bitIndex = 0; bitIndex < 8; bitIndex++)
            {
                if ((octetValue & (1 << (7 - bitIndex))) != 0)
                {
                    // Calculate device number: octet * 8 + bit position + 1
                    var deviceNumber = (byte)(octetIndex * 8 + bitIndex + 1);
                    _contributingUnits.Add(deviceNumber);
                }
            }
        }
        
        buffer = buffer[rep..];
    }
    
    public override void Serialize(ref Span<byte> buffer)
    {
        if (_contributingUnits.Count == 0)
        {
            buffer[0] = 0; // REP = 0
            buffer = buffer[1..];
            return;
        }
        
        // Calculate the number of octets needed
        var maxDevice = _contributingUnits.Max();
        var rep = (byte)((maxDevice + 7) / 8); // Ceiling division
        
        buffer[0] = rep;
        buffer = buffer[1..];
        
        // Initialize octets to zero
        var octets = buffer[..rep];
        octets.Clear();
        
        // Set bits for contributing units
        foreach (var deviceNumber in _contributingUnits)
        {
            if (deviceNumber < 1) continue;
            
            var octetIndex = (deviceNumber - 1) / 8;
            var bitIndex = (deviceNumber - 1) % 8;
            
            if (octetIndex < rep)
            {
                octets[octetIndex] |= (byte)(1 << (7 - bitIndex));
            }
        }
        
        buffer = buffer[rep..];
    }
    
    public override int GetByteSize()
    {
        if (_contributingUnits.Count == 0)
            return 1; // Just REP byte
            
        var maxDevice = _contributingUnits.Max();
        var rep = (maxDevice + 7) / 8; // Ceiling division
        return 1 + rep; // REP byte + data octets
    }
    
    public override void Accept(IVisitor visitor)
    {
        ListType.Accept(visitor, ContributingUnitsField, ContributingUnitsField.DataType, _contributingUnits, (index, v, f, t) =>
        {
            var val = _contributingUnits[index];
            UInt8Type.Accept(v, f, t, ref val);
            _contributingUnits[index] = val;
        });
    }
    
    private static StructType? type;
    public static StructType StructType => type ??= new StructType([
        ContributingUnitsField
    ]);
    
    private static readonly Field ContributingUnitsField = new Field.Builder()
        .Name(nameof(ContributingUnits))
        .Title("Contributing Units")
        .Description("List of devices that have contributed to target detection")
        .DataType(new ListType(UInt8Type.Default, 0, 255))
        .Build();
    
    /// <summary>
    /// Gets the list of contributing unit numbers.
    /// Each number represents a device that has contributed to the target detection.
    /// </summary>
    public List<byte> ContributingUnits => _contributingUnits;
    
    /// <summary>
    /// Checks if a specific device number has contributed to the target detection.
    /// </summary>
    /// <param name="deviceNumber">Device number (1-based)</param>
    /// <returns>True if the device contributed, false otherwise</returns>
    public bool HasContributed(byte deviceNumber)
    {
        return _contributingUnits.Contains(deviceNumber);
    }
    
    /// <summary>
    /// Adds a contributing device.
    /// </summary>
    /// <param name="deviceNumber">Device number (1-based)</param>
    public void Add(byte deviceNumber)
    {
        if (deviceNumber < 1)
            throw new ArgumentException("Device number must be greater than 0", nameof(deviceNumber));
            
        if (!_contributingUnits.Contains(deviceNumber))
        {
            _contributingUnits.Add(deviceNumber);
            _contributingUnits.Sort(); // Keep sorted for consistent output
        }
    }
    
    /// <summary>
    /// Removes a contributing device.
    /// </summary>
    /// <param name="deviceNumber">Device number (1-based)</param>
    public void RemoveContributingDevice(byte deviceNumber)
    {
        _contributingUnits.Remove(deviceNumber);
    }
    
    /// <summary>
    /// Clears all contributing devices.
    /// </summary>
    public void ClearContributingDevices()
    {
        _contributingUnits.Clear();
    }

    public IEnumerator GetEnumerator()
    {
        return _contributingUnits.GetEnumerator();
    }
}