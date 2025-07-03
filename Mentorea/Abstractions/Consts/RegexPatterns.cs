namespace Mentorea.Abstractions.Consts
{
    public static class RegexPatterns
    {
        public const string Password = "(?=(.*[0-9]))(?=.*[\\!@#$%^&*()\\\\[\\]{}\\-_+=~`|:;\"'<>,./?])(?=.*[a-z])(?=(.*[A-Z]))(?=(.*)).{8,}";
        public const string Date = @"^([0][1-9]|[12][0-9]|3[01])/([0][1-9]|1[0-2])/([0-9]{4})$";
        public const string Time = @"^(?:[01]\d|2[0-3]):[0-5]\d$";
    }
}
