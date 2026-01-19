using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.Fresher.Core.Untils
{
    public static class ShiftTimeCalculator
    {
        private const int MinutesPerDay = 24 * 60;

        private static double CalculateMinutes(TimeSpan start, TimeSpan end)
        {
            var minutes = (end - start).TotalMinutes;

            // Ca qua ngày
            if (minutes < 0)
                minutes += MinutesPerDay;

            return minutes;
        }

        public static decimal CalculateBreakingTime(
            TimeSpan? begin,
            TimeSpan? end)
        {
            if (!begin.HasValue || !end.HasValue)
                return 0;

            var minutes = CalculateMinutes(begin.Value, end.Value);
            return Math.Round((decimal)minutes / 60, 2);
        }

        public static decimal CalculateWorkingTime(
            TimeSpan? beginShift,
            TimeSpan? endShift,
            TimeSpan? beginBreak,
            TimeSpan? endBreak)
        {
            if (!beginShift.HasValue || !endShift.HasValue)
                return 0;

            var shiftMinutes = CalculateMinutes(
             beginShift.Value,
             endShift.Value  );

            var breakMinutes = 0.0;
            if (beginBreak.HasValue && endBreak.HasValue)
            {
                breakMinutes = CalculateMinutes(
                    beginBreak.Value,
                    endBreak.Value);
            }

            var workingMinutes = shiftMinutes - breakMinutes;

            return Math.Round((decimal)workingMinutes / 60, 2);
        }
    }

}
