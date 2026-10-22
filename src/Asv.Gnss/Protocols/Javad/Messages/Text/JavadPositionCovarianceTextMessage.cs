using System;
using System.Collections.Immutable;
using System.Linq;

namespace Asv.Gnss;

/// <summary>[AP] PositionCovarianceText; GREIS 4.2.01 section 3.4.18.</summary>
public sealed partial class JavadPositionCovarianceTextMessage()
    : JavadTextMessage("POSCOV", "N,000000.00,N,A,0,0,0,0,0,0,0,0,0,0")
{
    public override string Id => "AP";
    public override string Name => "POSCOV";
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
    public char? CovarianceValidity
    {
        get => Character(2);
        set => Set(2, value);
    }
    public char? ComputationMode
    {
        get => Character(3);
        set => Set(3, value);
    }
    public double? Covariance11
    {
        get => Number(4);
        set => Set(4, value);
    }
    public double? Covariance22
    {
        get => Number(5);
        set => Set(5, value);
    }
    public double? Covariance33
    {
        get => Number(6);
        set => Set(6, value);
    }
    public double? Covariance44
    {
        get => Number(7);
        set => Set(7, value);
    }
    public double? Covariance12
    {
        get => Number(8);
        set => Set(8, value);
    }
    public double? Covariance13
    {
        get => Number(9);
        set => Set(9, value);
    }
    public double? Covariance14
    {
        get => Number(10);
        set => Set(10, value);
    }
    public double? Covariance23
    {
        get => Number(11);
        set => Set(11, value);
    }
    public double? Covariance24
    {
        get => Number(12);
        set => Set(12, value);
    }
    public double? Covariance34
    {
        get => Number(13);
        set => Set(13, value);
    }

    protected override void ValidateFields()
    {
        Validate("ctccffffffffff");
    }
}
