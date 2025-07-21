using System;
using System.Collections.Generic;

namespace Asv.Gnss;

public class AsterixMessageI020 : AsterixMessage<AsterixRecordI020>
{
    public static byte Category => 20;
    public override string Name => "I020";
    public override byte Id => Category;
    protected override AsterixRecord CreateRecord() => new AsterixRecordI020();

    public void Add(AsterixRecordI020 asterixRecordI020)
    {
        throw new NotImplementedException();
    }
}

public class AsterixRecordI020 : AsterixRecord
{
    public AsterixFieldI020_010? DataSourceIdentifier { get; set; }

    protected override AsterixField InternalAddByFrn(int fieldReferenceNumber)
    {
        return fieldReferenceNumber switch
        {
            1 => DataSourceIdentifier = new AsterixFieldI020_010(),
            _ => throw new ArgumentOutOfRangeException(nameof(fieldReferenceNumber),
                $"Field reference number {fieldReferenceNumber} is not supported in {nameof(AsterixRecordI020)}.")
        };
    }

    public override int Category => AsterixMessageI020.Category;
    public override IEnumerator<AsterixField> GetEnumerator()
    {
        if (DataSourceIdentifier != null)
        {
            yield return DataSourceIdentifier;
        }
    }
}

public class AsterixFieldI020_010 : AsterixField
{
    public override string Name => "Data Source Identifier";
    public override int Category => AsterixMessageI020.Category;
    public override byte FieldReferenceNumber => 1;
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        throw new NotImplementedException();
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        throw new NotImplementedException();
    }

    public override int GetByteSize()
    {
        throw new NotImplementedException();
    }
}