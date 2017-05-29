using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2NG_Final.Pointers
{
    public static class DateTimeExtensions
    {
        public static long MSecToNow(this DateTime date)
        {
            return (DateTime.Now.Ticks - date.Ticks) / TimeSpan.TicksPerMillisecond;
        }

        public static bool Passed(this DateTime date)
        {
            return date <= DateTime.Now;
        }
    }
}
