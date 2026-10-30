namespace CuahangNongduoc.BusinessObject
{
    public static class LyDoChiValidation
    {
        public static bool IsValid(string lyDo)
        {
            return !string.IsNullOrWhiteSpace(lyDo);
        }
    }
}
