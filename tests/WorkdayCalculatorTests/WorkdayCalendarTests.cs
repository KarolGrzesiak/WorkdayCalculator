using System.Collections;
using FluentAssertions;
using WorkdayCalculator;

namespace WorkdayCalculatorTests;

public class WorkdayCalendarTests
{
    public static IEnumerable TestCases
    {
        get
        {
            yield return new TestCaseData(
                new DateTime(2004, 5, 24, 18, 5, 0),
                -5.5m,
                (8, 0, 16, 0),
                new DateTime(2004, 5, 27, 0, 0, 0),
                (5, 17),
                new DateTime(2004, 5, 14, 12, 0, 0));

            yield return new TestCaseData(
                new DateTime(2004, 5, 24, 15, 7, 0),
                0.25m,
                (8, 0, 16, 0),
                new DateTime(2004, 5, 17, 0, 0, 0),
                (5, 17),
                new DateTime(2004, 5, 25, 9, 7, 0));

            yield return new TestCaseData(
                new DateTime(2004, 5, 24, 19, 3, 0),
                44.723656m,
                (8, 0, 16, 0),
                new DateTime(2004, 5, 27, 0, 0, 0),
                (5, 17),
                new DateTime(2004, 7, 27, 13, 47, 0));

            yield return new TestCaseData(
                new DateTime(2004, 5, 24, 18, 3, 0),
                -6.7470217m,
                (8, 0, 16, 0),
                new DateTime(2004, 5, 27, 0, 0, 0),
                (5, 17),
                new DateTime(2004, 5, 13, 10, 2, 0));

            yield return new TestCaseData(
                new DateTime(2004, 5, 24, 8, 3, 0),
                12.782709m,
                (8, 0, 16, 0),
                new DateTime(2004, 5, 27, 0, 0, 0),
                (5, 17),
                new DateTime(2004, 6, 10, 14, 18, 0));

            yield return new TestCaseData(
                new DateTime(2004, 5, 24, 7, 3, 0),
                8.276628m,
                (8, 0, 16, 0),
                new DateTime(2004, 5, 27, 0, 0, 0),
                (5, 17),
                new DateTime(2004, 6, 4, 10, 12, 0));
            yield return new TestCaseData(
                new DateTime(2024, 1, 15, 22, 30, 0),
                1.5m,
                (6, 0, 14, 30),
                new DateTime(2024, 1, 16, 0, 0, 0),
                (1, 1),
                new DateTime(2024, 1, 18, 10, 15, 0));

            yield return new TestCaseData(
                new DateTime(2024, 2, 14, 3, 30, 0),
                0.0625m,
                (8, 30, 16, 45),
                new DateTime(2024, 2, 15, 0, 0, 0),
                (2, 14),
                new DateTime(2024, 2, 16, 9, 0, 0));

            yield return new TestCaseData(
                new DateTime(2024, 3, 1, 7, 15, 0),
                0.125m,
                (8, 15, 17, 45),
                new DateTime(2024, 3, 4, 0, 0, 0),
                (3, 17),
                new DateTime(2024, 3, 1, 9, 26, 0));

     
            yield return new TestCaseData(
                new DateTime(2024, 4, 30, 16, 45, 0),
                -2.75m,
                (7, 30, 15, 45),
                new DateTime(2024, 5,   1, 0, 0, 0),
                (5, 1),
                new DateTime(2024, 4, 26, 9, 34, 0));

            yield return new TestCaseData(
                new DateTime(2024, 5, 31, 14, 20, 0),
                -10.875m,
                (7, 0, 15, 30),
                new DateTime(2024, 5, 27, 0, 0, 0),
                (5, 30),
                new DateTime(2024, 5, 14, 15, 24, 0)); //

            yield return new TestCaseData(
                new DateTime(2024, 7, 3, 12, 0, 0),
                5.5m,
                (10, 0, 19, 0),
                new DateTime(2024, 7, 4, 0, 0, 0),
                (7, 4),
                new DateTime(2024, 7, 11, 16, 30, 0));

            yield return new TestCaseData(
                new DateTime(2024, 12, 20, 15, 45, 0),
                3.25m,
                (9, 30, 18, 0),
                new DateTime(2024, 12, 24, 0, 0, 0),
                (12, 25),
                new DateTime(2024, 12, 27, 17, 52, 0));
        }
    }

    [Test]
    [TestCaseSource(nameof(TestCases))]
    public void GetWorkdayIncrement_WhenValidDatesAndIncrementsArePassed_ReturnsExpectedResult(
        DateTime startDate, decimal increment,
        (int StartHours, int StartMinutes, int StopHours, int StopMinutes) workingTime,
        DateTime oneTimeHoliday, (int Month, int Day) recurringHoliday,
        DateTime expectedDate)
    {
        var calendar = new WorkdayCalendar();
        calendar.SetWorkdayStartAndStop(workingTime.StartHours, workingTime.StartMinutes, workingTime.StopHours,
            workingTime.StopMinutes);
        calendar.SetHoliday(oneTimeHoliday);
        calendar.SetRecurringHoliday(recurringHoliday.Month, recurringHoliday.Day);

        var result = calendar.GetWorkdayIncrement(startDate, increment);

            result.Should().Be(expectedDate);
    }

    [Test]
    public void SetRecurringHoliday_WhenDayIsInvalid_ThrowsArgumentOutOfRangeException()
    {
        var calendar = new WorkdayCalendar();

        calendar.Invoking(x => x.SetRecurringHoliday(1, 32)).Should().Throw<ArgumentOutOfRangeException>();
    }

    [Test]
    public void SetRecurringHoliday_WhenDayIsInvalidForAMonth_ThrowsArgumentException()
    {
        var calendar = new WorkdayCalendar();

        calendar.Invoking(x => x.SetRecurringHoliday(2, 30)).Should().Throw<ArgumentException>();
    }

    [Test]
    [TestCase(-1, 0, 16, 0)]
    [TestCase(24, 0, 16, 0)]
    [TestCase(8, -1, 16, 0)]
    [TestCase(8, 60, 16, 0)]
    [TestCase(8, 0, -1, 0)]
    [TestCase(8, 0, 24, 0)]
    [TestCase(8, 0, 16, -1)]
    [TestCase(8, 0, 16, 60)]
    public void SetWorkdayStartAndStop_WhenTimesAreInvalid_ThrowsArgumentOutOfRangeException(
        int startHours, int startMinutes, int stopHours, int stopMinutes)
    {
        var calendar = new WorkdayCalendar();

        calendar.Invoking(x => x.SetWorkdayStartAndStop(startHours, startMinutes, stopHours, stopMinutes)).Should()
            .Throw<ArgumentException>();
    }
}