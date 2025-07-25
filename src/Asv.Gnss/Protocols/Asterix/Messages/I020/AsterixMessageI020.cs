using System;
using System.Collections.Generic;

namespace Asv.Gnss;

public class AsterixMessageI020 : AsterixMessage<AsterixRecordI020>
{
    public static byte Category => 20;
    public override string Name => "I020";
    public override byte Id => Category;
    
}

public class AsterixRecordI020 : AsterixRecord
{
    public AsterixFieldI020Frn001Type010? DataSourceIdentifier { get; set; }
    public AsterixFieldI020Frn002Type020? TargetReportDescriptor { get; set; }

    protected override AsterixField? AddFieldByFrn(int fieldReferenceNumber)
    {
        return fieldReferenceNumber switch
        {
            AsterixFieldI020Frn001Type010.StaticFrn => DataSourceIdentifier = new AsterixFieldI020Frn001Type010(),
            AsterixFieldI020Frn002Type020.StaticFrn => TargetReportDescriptor = new AsterixFieldI020Frn002Type020(),
            _ => null
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