using Microsoft.AspNetCore.Mvc;
using PunchInAndOut.Service;
using System;

namespace PunchInAndOut.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class ZealogicsPunchController : ControllerBase
{
    private readonly ILogger<ZealogicsPunchController> _logger;
    private readonly ZealogicsPunchService _zealogicsService;

    public ZealogicsPunchController(ILogger<ZealogicsPunchController> logger, ZealogicsPunchService workDoPunchService)
    {
        _logger = logger;
        _zealogicsService = workDoPunchService;
    }

    [HttpGet]
    public async Task PunchAsync(int year, int month, int dateStart, int dateEnd)
    {
        await _zealogicsService.Login();

        var businessDaysInRange = GetBusinessDays(
                     new DateTime(year, month, dateStart),
                     new DateTime(year, month, dateEnd));
        foreach (var date in businessDaysInRange)
        {
            await _zealogicsService.PunchIn(date);
            await Task.Delay(50); // 避免過快連續請求
        }
    }
    
    /// <summary>
     /// 取得指定區間內的上班日（排除週末與假日）
     /// </summary>
     /// <param name="start">起始日期（包含）</param>
     /// <param name="end">結束日期（包含）</param>
     /// <param name="publicHolidays">可選假日清單</param>
     /// <returns>介於起迄日期間的所有上班日</returns>
    public static IEnumerable<DateTime> GetBusinessDays(
        DateTime start,
        DateTime end,
        IEnumerable<DateTime> publicHolidays = null)
    {
        var holidays = new HashSet<DateTime>(
            (publicHolidays ?? Enumerable.Empty<DateTime>())
            .Select(d => d.Date)
        );

        for (var day = start.Date; day <= end.Date; day = day.AddDays(1))
        {
            // 排除週末
            if (day.DayOfWeek == DayOfWeek.Saturday ||
                day.DayOfWeek == DayOfWeek.Sunday)
            {
                continue;
            }

            // 排除假日
            if (holidays.Contains(day))
            {
                continue;
            }

            yield return day;
        }
    }
}
