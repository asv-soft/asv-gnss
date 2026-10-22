using System;
using System.Collections.Immutable;
using System.Linq;

namespace Asv.Gnss;

/// <summary>[MP] MapProjection; GREIS 4.2.01 section 3.4.18.</summary>
public sealed partial class JavadMapProjectionMessage() : JavadTextMessage("MAPRJ", "N,000000.00,3")
{
    public override string Id => "MP";
    public override string Name => "MAPRJ";
    public char? UtcValidity
    {
        get => Character(0);
        set => Set(0, value);
    }
    public TimeSpan? UtcTime
    {
        get => Time(1);
        set => SetTime(1, value);
    }
    public int? PositionStatus
    {
        get => Integer(2);
        set => Set(2, value);
    }
    public char? GridValidity
    {
        get => Character(3);
        set => Set(3, value);
    }
    public string? GridSystem
    {
        get => Text(4);
        set => Set(4, value);
    }
    public int? Zone
    {
        get => Integer(5);
        set => Set(5, value);
    }
    public double? NorthingMeters
    {
        get => Number(6);
        set => Set(6, value);
    }
    public double? EastingMeters
    {
        get => Number(7);
        set => Set(7, value);
    }
    public double? AltitudeMeters
    {
        get => Number(8);
        set => Set(8, value);
    }

    protected override void ValidateFields()
    {
        Validate("cti");
        if (PositionStatus == 0)
            Validate("cticsifff");
    }
}
