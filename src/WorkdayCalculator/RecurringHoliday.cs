namespace WorkdayCalculator;

internal readonly record struct RecurringHoliday : IHoliday
{
    private int Month { get; }
    private int Day { get; }

    public RecurringHoliday(int month, int day)
    {
        if (month is < 1 or > 12)
            throw new ArgumentOutOfRangeException(nameof(month), "Month must be between 1 and 12");

        if (day is < 1 or > 31)
            throw new ArgumentOutOfRangeException(nameof(day), "Day must be between 1 and 31");

        if (!IsValidDayForMonth(day, month))
            throw new ArgumentException($"Day {day} is not valid for month {month}");

        Month = month;
        Day = day;
    }

    public bool IsHolidayOn(DateTime date) =>
        date.Month == Month && date.Day == Day;

    private static bool IsValidDayForMonth(int day, int month) =>
        day <= DateTime.DaysInMonth(DateTime.Now.Year, month);
}