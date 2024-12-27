namespace Asv.Gnss
{
    public static class PositionFixStatusHelper
    {
        public static PositionFixStatus GetPositionFixStatus(this char src)
        {
            return src switch
            {
                'a' or 'A' => PositionFixStatus.Valid,
                'v' or 'V' => PositionFixStatus.Warning,
                _ => PositionFixStatus.Unknown,
            };
        }

        public static string Serialize(this PositionFixStatus src)
        {
            return src switch
            {
                PositionFixStatus.Valid => "A",
                PositionFixStatus.Warning => "V",
                _ => string.Empty,
            };
        }
    }
}
