using System;

namespace A.TaskDispatching
{
    /// <summary>
    /// 可控制时间偏移，用于测试。
    /// </summary>
    public static class MinashiDateTime
    {
        public static TimeSpan Offset { get; set; }

        public static DateTime Now => DateTime.Now + Offset;

        public static DateTime Minashi(DateTime value) => value + Offset;
    }
}
