using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Asv.IO;

namespace Asv.Gnss;


public abstract class AsterixMessage : IProtocolMessage<byte>, IEnumerable<AsterixRecord>
{
    private ProtocolTags _tags = [];
    protected readonly List<AsterixRecord> Records = new(1);

    public abstract string Name { get; }
    public abstract byte Id { get; }

    public void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        var originalLength = buffer.Length;
        
        var cat = BinSerialize.ReadByte(ref buffer);
        if (cat != Id)
        {
            throw new InvalidOperationException($"Invalid category {cat} for {Name}");
        }
        var length = AsterixProtocol.ReadLength(ref buffer);
        if (length != originalLength)
        {
            throw new InvalidOperationException($"Invalid length {length} for {Name}, expected {originalLength}");
        }
        while (buffer.Length > 0)
        {
            var record = CreateRecord();
            record.Deserialize(ref buffer);
            Records.Add(record);
        }
    }

    protected abstract AsterixRecord CreateRecord();

    public void Serialize(ref Span<byte> buffer)
    {
        BinSerialize.WriteByte(ref buffer, Id);
        AsterixProtocol.WriteLength(ref buffer, GetByteSize());
        foreach (var record in Records)
        {
            if (record.Category != Id)
            {
                throw new InvalidOperationException($"Invalid record category {record.Category} for {Name}, expected {Id}");
            }
            record.Serialize(ref buffer);
        }
    }

    public int GetByteSize() => 1 + // Category
                                2 + // Length
                                Records.Sum(x => x.GetByteSize());

    public ref ProtocolTags Tags => ref _tags;

    public string GetIdAsString() => Id.ToString();

    public ProtocolInfo Protocol => AsterixProtocol.Info;

    public IEnumerator<AsterixRecord> GetEnumerator()
    {
        return Records.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public abstract class AsterixMessage<TRecord> : AsterixMessage 
    where TRecord : AsterixRecord, new()
{
    protected override AsterixRecord CreateRecord() => new TRecord();

    public void Add(TRecord record)
    {
        ArgumentNullException.ThrowIfNull(record);
        if (record.Category != Id)
        {
            throw new InvalidOperationException($"Invalid record category {record.Category} for {Name}, expected {Id}");
        }
        
        Records.Add(record);
    }
}


public abstract class AsterixRecord : ISizedSpanSerializable, IEnumerable<AsterixField>
{
    public virtual void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        var fspec = AsterixProtocol.ReadFspec(ref buffer);
        for (var i = 0; i < fspec.Count; i++)
        {
            if (fspec[i])
            {
                var field = InternalAddByFrn(i + 1);
                field.Deserialize(ref buffer);
            }
        }
    }

    protected abstract AsterixField InternalAddByFrn(int fieldReferenceNumber);
    
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

public abstract class AsterixField : ISizedSpanSerializable
{
    public abstract string Name { get; }
    public abstract int Category { get; }
    public abstract byte FieldReferenceNumber { get; }

    public abstract void Deserialize(ref ReadOnlySpan<byte> buffer);
    
    public abstract void Serialize(ref Span<byte> buffer);
    
    public abstract int GetByteSize();
}