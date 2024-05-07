using System;
using System.Linq;

namespace Simacek.DateTime
{
    public static partial class DateTimeExtensions
    {
        //This logic is a direct translation of [dbo].[businessDaysAdd] from Sam's SQL Function to C#
        public static System.DateTime AddBusinessDays(this System.DateTime source, int daysToAdd, params System.DateTime[] holidays)
        {
            var addsub = daysToAdd > 0 ? 1 : -1;
            while (daysToAdd != 0)
            {
                source = source.AddDays(1.0 * addsub);
                if (source.DayOfWeek != DayOfWeek.Saturday && source.DayOfWeek != DayOfWeek.Sunday && !holidays.Contains(source))
                {
                    daysToAdd -= (1 * addsub);
                }
            }

            return source;
        }
    }
}
