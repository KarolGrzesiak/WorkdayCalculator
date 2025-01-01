namespace WorkdayCalculator;

public class WorkdayCalendar : IWorkdayCalendar
{
    private readonly HashSet<IHoliday> _holidays = [];
    private TimeSpan _workdayStart = TimeSpan.FromHours(8);
    private TimeSpan _workdayEnd = TimeSpan.FromHours(16);

    private TimeSpan WorkdayDuration => _workdayEnd - _workdayStart;

    public void SetHoliday(DateTime date)
    {
        _holidays.Add(new OneTimeHoliday(date));
    }


    public void SetRecurringHoliday(int month, int day)
    {
        _holidays.Add(new RecurringHoliday(month, day));
    }

    public void SetWorkdayStartAndStop(int startHours, int startMinutes, int stopHours, int stopMinutes)
    {
        if (startHours is < 0 or >= 24)
            throw new ArgumentOutOfRangeException(nameof(startHours), "Hours must be between 0 and 23");

        if (stopHours is < 0 or >= 24)
            throw new ArgumentOutOfRangeException(nameof(stopHours), "Hours must be between 0 and 23");

        if (startMinutes is < 0 or >= 60)
            throw new ArgumentOutOfRangeException(nameof(startMinutes), "Minutes must be between 0 and 59");

        if (stopMinutes is < 0 or >= 60)
            throw new ArgumentOutOfRangeException(nameof(stopMinutes), "Minutes must be between 0 and 59");

        var start = new TimeSpan(startHours, startMinutes, 0);
        var end = new TimeSpan(stopHours, stopMinutes, 0);

        if (end <= start)
            throw new ArgumentException("Work day end time must be after start time");

        _workdayStart = start;
        _workdayEnd = end;
    }

    public DateTime GetWorkdayIncrement(DateTime startDate, decimal incrementInWorkdays)
    {
        var normalizedDate = NormalizeDateTime(startDate);

        if (incrementInWorkdays == 0)
            return normalizedDate;

        var totalMinutesToAdd =  (long)(incrementInWorkdays * (decimal)WorkdayDuration.TotalMinutes);
        return AddWorkMinutes(normalizedDate, totalMinutesToAdd);
    }


    private DateTime NormalizeDateTime(DateTime date)
    {
        if (!IsWorkday(date))
        {
            return GetStartOfNextWorkday(date);
        }

        var timeOfDay = date.TimeOfDay;

        if (timeOfDay > _workdayEnd)
        {
            return GetStartOfNextWorkday(date);
        }


        if (timeOfDay < _workdayStart)
        {
            return date.Date + _workdayStart;
        }

        return date;
    }


    private DateTime AddWorkMinutes(DateTime date, long minutes)
    {
        var direction = Math.Sign(minutes);
        var remainingMinutes = Math.Abs(minutes);
        var currentDate = date;

        while (remainingMinutes > 0)
        {
            var minutesInCurrentDay = GetRemainingMinutesInWorkday(currentDate, direction);

            if (remainingMinutes <= minutesInCurrentDay)
                return currentDate.Add(TimeSpan.FromMinutes(direction * remainingMinutes));

            remainingMinutes -= minutesInCurrentDay;
            currentDate = direction > 0
                ? GetStartOfNextWorkday(currentDate)
                : GetEndOfPreviousWorkday(currentDate);
        }

        return currentDate;
    }


    private long GetRemainingMinutesInWorkday(DateTime date, int direction)
    {
        var timeOfDay = date.TimeOfDay;
        return direction > 0
            ? (long)(_workdayEnd - timeOfDay).TotalMinutes
            : (long)(timeOfDay - _workdayStart).TotalMinutes;
    }

    private DateTime GetStartOfNextWorkday(DateTime date)
    {
        var next = GetWorkday(date.Date.AddDays(1), 1);
        return next + _workdayStart;
    }

    private DateTime GetEndOfPreviousWorkday(DateTime date)
    {
        var prev = GetWorkday(date.Date.AddDays(-1), -1);
        return prev + _workdayEnd;
    }


    private DateTime GetWorkday(DateTime date, int incrementInDays)
    {
        while (!IsWorkday(date))
        {
            date = date.AddDays(incrementInDays);
        }

        return date;
    }


    private bool IsWorkday(DateTime date) =>
        date.DayOfWeek != DayOfWeek.Saturday &&
        date.DayOfWeek != DayOfWeek.Sunday &&
        !_holidays.Any(x => x.IsHolidayOn(date));
}