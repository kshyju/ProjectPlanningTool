using System;

namespace TeamBins.Infrastrucutre.Extensions
{
    public static class DateTimeExtensions
    {
        public static bool IsPastDue(this DateTime? value)
        {
           
            return value!=null && DateTime.Now > value;
        }
        public static string ToShortDateString(this DateTime dateTime) => dateTime.ToString("d");
        public static string ToShortTimeString(this DateTime dateTime) => dateTime.ToString("t");
        public static string ToLongDateString(this DateTime dateTime) => dateTime.ToString("D");
        public static string ToLongTimeString(this DateTime dateTime) => dateTime.ToString("T");

    }
}