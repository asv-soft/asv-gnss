using System;

namespace Asv.Gnss;

// Concrete IDs allow the standard factory to return identified messages before deserialization.
public sealed class GlonassWord6 : GlonassWordEven
{
    public override byte WordId => 6;

    protected override void CheckWordId(byte wordId)
    {
        if (wordId != WordId)
            throw new FormatException($"Expected GLONASS string {WordId}, received {wordId}.");
    }
}

public sealed class GlonassWord7 : GlonassWordOdd
{
    public override byte WordId => 7;

    protected override void CheckWordId(byte wordId)
    {
        if (wordId != WordId)
            throw new FormatException($"Expected GLONASS string {WordId}, received {wordId}.");
    }
}

public sealed class GlonassWord8 : GlonassWordEven
{
    public override byte WordId => 8;

    protected override void CheckWordId(byte wordId)
    {
        if (wordId != WordId)
            throw new FormatException($"Expected GLONASS string {WordId}, received {wordId}.");
    }
}

public sealed class GlonassWord9 : GlonassWordOdd
{
    public override byte WordId => 9;

    protected override void CheckWordId(byte wordId)
    {
        if (wordId != WordId)
            throw new FormatException($"Expected GLONASS string {WordId}, received {wordId}.");
    }
}

public sealed class GlonassWord10 : GlonassWordEven
{
    public override byte WordId => 10;

    protected override void CheckWordId(byte wordId)
    {
        if (wordId != WordId)
            throw new FormatException($"Expected GLONASS string {WordId}, received {wordId}.");
    }
}

public sealed class GlonassWord11 : GlonassWordOdd
{
    public override byte WordId => 11;

    protected override void CheckWordId(byte wordId)
    {
        if (wordId != WordId)
            throw new FormatException($"Expected GLONASS string {WordId}, received {wordId}.");
    }
}

public sealed class GlonassWord12 : GlonassWordEven
{
    public override byte WordId => 12;

    protected override void CheckWordId(byte wordId)
    {
        if (wordId != WordId)
            throw new FormatException($"Expected GLONASS string {WordId}, received {wordId}.");
    }
}

public sealed class GlonassWord13 : GlonassWordOdd
{
    public override byte WordId => 13;

    protected override void CheckWordId(byte wordId)
    {
        if (wordId != WordId)
            throw new FormatException($"Expected GLONASS string {WordId}, received {wordId}.");
    }
}

public sealed partial class GlonassWord14 : GlonassWordEven
{
    public override byte WordId => 14;

    protected override void CheckWordId(byte wordId)
    {
        if (wordId != WordId)
            throw new FormatException($"Expected GLONASS string {WordId}, received {wordId}.");
    }
}

public sealed partial class GlonassWord15 : GlonassWordOdd
{
    public override byte WordId => 15;

    protected override void CheckWordId(byte wordId)
    {
        if (wordId != WordId)
            throw new FormatException($"Expected GLONASS string {WordId}, received {wordId}.");
    }
}
