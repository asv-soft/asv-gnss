using System;
using Asv.IO;

namespace Asv.Gnss;

public abstract class AsterixField : ISizedSpanSerializable, IVisitable
{
    public abstract string Name { get; }
    public abstract int Category { get; }
    public abstract byte FieldReferenceNumber { get; }

    public abstract void Deserialize(ref ReadOnlySpan<byte> buffer);
    
    public abstract void Serialize(ref Span<byte> buffer);
    
    public abstract int GetByteSize();
    public abstract void Accept(IVisitor visitor);
}