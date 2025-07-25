using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Asv.IO;

namespace Asv.Gnss;


public abstract class AsterixMessage : IProtocolMessage<byte>, IEnumerable<AsterixRecord>
{
    private ProtocolTags _tags = [];
    public abstract string Name { get; }
    public abstract byte Id { get; }
    public ref ProtocolTags Tags => ref _tags;
    public string GetIdAsString() => Id.ToString();
    public ProtocolInfo Protocol => AsterixProtocol.Info;
    public abstract IEnumerator<AsterixRecord> GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    public abstract void Deserialize(ref ReadOnlySpan<byte> buffer);
    public abstract void Serialize(ref Span<byte> buffer);
    public abstract int GetByteSize();
}

public abstract class AsterixMessage<TRecord> : AsterixMessage 
    where TRecord : AsterixRecord, new()
{
    private readonly List<AsterixRecord> _records = new(1);
    public void Add(TRecord record)
    {
        ArgumentNullException.ThrowIfNull(record);
        if (record.Category != Id)
        {
            throw new InvalidOperationException($"Invalid record category {record.Category} for {Name}, expected {Id}");
        }
        _records.Add(record);
    }
    
    public override IEnumerator<AsterixRecord> GetEnumerator()
    {
        return _records.GetEnumerator();
    }
    
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
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
            var record = new AsterixRecordI020();
            record.Deserialize(ref buffer);
            _records.Add(record);
        }
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        BinSerialize.WriteByte(ref buffer, Id);
        AsterixProtocol.WriteLength(ref buffer, GetByteSize());
        foreach (var record in _records)
        {
            if (record.Category != Id)
            {
                throw new InvalidOperationException($"Invalid record category {record.Category} for {Name}, expected {Id}");
            }
            record.Serialize(ref buffer);
        }
    }

    public override int GetByteSize() => 1 + // Category
                                2 + // Length
                                _records.Sum(x => x.GetByteSize());
}