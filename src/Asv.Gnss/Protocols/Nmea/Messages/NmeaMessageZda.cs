using System;

namespace Asv.Gnss;

    
public class NmeaMessageZda : NmeaMessageBase
{
    private int? _zoneHours;
    private int? _zoneMinutes;
    private DateOnly? _date;
    private TimeSpan? _time;
    public const string MessageName = "ZDA";
    public static readonly NmeaMessageId MessageId = new(MessageName);
    
    public override string Name => MessageName;
    public override NmeaMessageId Id => MessageId;
    protected override void InternalDeserialize(ref ReadOnlySpan<char> buffer)
    {
        ReadTime(ref buffer, out _time);
        ReadInt(ref buffer, out var day);
        ReadInt(ref buffer, out var month);
        ReadInt(ref buffer, out var year);
        ReadInt(ref buffer, out _zoneHours,false);
        ReadInt(ref buffer, out _zoneMinutes,false);

        if (year != null && month != null && day != null)
        {
            _date = new DateOnly(year.Value, month.Value, day.Value);    
        }
        
    }

    protected override void InternalSerialize(ref Span<byte> buffer)
    {
        WriteTime(ref buffer, in _time);
        WriteInt(ref buffer, _date?.Day, NmeaIntFormat.IntD2);
        WriteInt(ref buffer, _date?.Month, NmeaIntFormat.IntD2);
        WriteInt(ref buffer, _date?.Year, NmeaIntFormat.IntD4);
        WriteInt(ref buffer, _zoneHours, NmeaIntFormat.IntD2);
        WriteInt(ref buffer, _zoneMinutes, NmeaIntFormat.IntD2);
    }

    protected override int InternalGetByteSize()
    {
        return SizeOfTime(in _time)
            + SizeOfInt(_date?.Day, NmeaIntFormat.IntD2)
            + SizeOfInt(_date?.Month, NmeaIntFormat.IntD2)
            + SizeOfInt(_date?.Year, NmeaIntFormat.IntD4)
            + SizeOfInt(_zoneHours, NmeaIntFormat.IntD2)
            + SizeOfInt(_zoneMinutes, NmeaIntFormat.IntD2);
    }

    
}