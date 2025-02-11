namespace TaskDispatching
{
    public static class MinashiDateTime
    {
        public static TimeSpan Offset { get; set; }

        public static DateTime Now => DateTime.Now + Offset;

        public static DateTime Minashi(DateTime value) => value + Offset;
    }
}
