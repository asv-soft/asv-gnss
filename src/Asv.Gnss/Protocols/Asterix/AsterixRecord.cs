using System;
using System.Collections;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public abstract class AsterixRecord : ISizedSpanSerializable, IEnumerable<AsterixField>
{
    public virtual void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        var fSpec = AsterixProtocol.ReadVariableLengthFieldAsBitArray(ref buffer);
        for (var i = 0; i < fSpec.Count; i++)
        {
            if (fSpec[i])
            {
                AddFieldByFrn(i + 1)?.Deserialize(ref buffer); 
            }
        }
    }

    protected abstract AsterixField? AddFieldByFrn(int fieldReferenceNumber);
    
    public virtual void Serialize(ref Span<byte> buffer)
    {
        
    }

    public virtual int GetByteSize()
    {
        return 0;
    }

    public abstract int Category { get; }
    public abstract IEnumerator<AsterixField> GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}