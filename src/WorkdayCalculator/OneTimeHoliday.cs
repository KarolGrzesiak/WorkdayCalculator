namespace WorkdayCalculator;

internal readonly record struct OneTimeHoliday : IHoliday
{
    private readonly DateTime _date;

    public OneTimeHoliday(DateTime date)
    {
        _date = date.Date;
    }

    public bool IsHolidayOn(DateTime date) =>
        _date.Day == date.Day && _date.Month == date.Month && _date.Year == date.Year;
}