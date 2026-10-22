using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixCategoryVersion : IVisitable
{
    private static readonly Field VisitorCategoryField = new Field.Builder().Name(nameof(Category)).Title(nameof(Category)).Description("Category").DataType(UInt8Type.Default).Build();
    private static readonly Field VisitorMajorField = new Field.Builder().Name(nameof(Major)).Title(nameof(Major)).Description("Major").DataType(UInt8Type.Default).Build();
    private static readonly Field VisitorMinorField = new Field.Builder().Name(nameof(Minor)).Title(nameof(Minor)).Description("Minor").DataType(UInt8Type.Default).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorCategoryField, VisitorMajorField, VisitorMinorField]);

    /// <inheritdoc />
    public void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueCategory = Category;
        UInt8Type.Accept(visitor, VisitorCategoryField, ref valueCategory);
        if (!EqualityComparer<byte>.Default.Equals(valueCategory, Category)) Category = valueCategory;
        var valueMajor = Major;
        UInt8Type.Accept(visitor, VisitorMajorField, ref valueMajor);
        if (!EqualityComparer<byte>.Default.Equals(valueMajor, Major)) Major = valueMajor;
        var valueMinor = Minor;
        UInt8Type.Accept(visitor, VisitorMinorField, ref valueMinor);
        if (!EqualityComparer<byte>.Default.Equals(valueMinor, Minor)) Minor = valueMinor;
    }
}
