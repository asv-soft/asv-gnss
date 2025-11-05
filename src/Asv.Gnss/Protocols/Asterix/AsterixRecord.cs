using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Asv.IO;

namespace Asv.Gnss;

public abstract class AsterixRecord : ISizedSpanSerializable, IEnumerable<AsterixField>
{
    public virtual void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        var fspec = new VariableLengthValue();
        fspec.Deserialize(ref buffer);
        for (var i = 0; i < fspec.DataBitsCount; i++)
        {
            if (fspec[i] == true)
            {
                AddFieldByFrn(i + 1).Deserialize(ref buffer);    
            }
        }
    }

    protected abstract AsterixField AddFieldByFrn(int fieldReferenceNumber);
    
    public virtual void Serialize(ref Span<byte> buffer)
    {
        var fspec = new VariableLengthValue();
        var lastFrn = 0;
        foreach (var field in this)
        {
            fspec[field.FieldReferenceNumber] = true;
            Debug.Assert(field.FieldReferenceNumber > lastFrn, "Fields must be sorted by field reference number");
            lastFrn = field.FieldReferenceNumber;
        }
        fspec.Serialize(ref buffer);
        
        foreach (var field in this)
        {
            field.Serialize(ref buffer);
        }
    }

    public virtual int GetByteSize()
    {
        var fspec = new VariableLengthValue();
        foreach (var field in this)
        {
            fspec[field.FieldReferenceNumber] = true;
        }
        return fspec.GetByteSize() + this.Sum(field => field.GetByteSize());
    }

    public abstract int Category { get; }
    public abstract IEnumerator<AsterixField> GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}