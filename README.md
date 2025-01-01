# Chooose - Workday Calculator

Few things worth noting:

1. Tests are not perfect here, there is a low level of isolation when it comes to testing `GetWorkdayIncrement` method - in order to test different scenarios, other setup methods have to be called (setting up holidays, workday bounds), which also probably should be tested in isolation (but that would require rethinking the design). With that being said, for the sake of checking if the solutions works I think it's fine.
2. I'm assuming that whenever we need to normalize the input in the `GetWorkdayIncrement` method - for example if the time is outside of working hours, or the date is a holiday - I can just move it to the start of the next workday.
3. I'm not bothering here with timezones, even though in the proper implementation everything probably should be stored in the UTC format for the consistency sake. With that being said, I think in this case it shouldn't really affect the calculator anyway, as even if the holiday is set with a different timezone, and `GetWorkdayIncrement` input gets a date also in a different timezone, it shouldn't really matter because the holidays are set on a day/month/year, not hour.
4. I'm assuming that rounding calculations to minutes is a good enough precision for this implementation, you can see it here: `var totalMinutesToAdd =  (long)(incrementInWorkdays * (decimal)WorkdayDuration.TotalMinutes);` (also not guarding here against overflow).
5. When it comes to instances of `OneTimeHoliday` to determine if the date is a holiday I'm checking for a match by comparing day+month+year, not day+month.

If any of my assumptions are wrong, please do correct me/clarify it, this should all be easily changed if needed.
