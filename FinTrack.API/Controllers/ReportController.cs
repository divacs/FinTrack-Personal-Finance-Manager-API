using FinTrack.API.DTOs.Report;
using FinTrack.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FinTrack.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReportController : ControllerBase
    {
        private readonly IReportRepository _repository;

        public ReportController(IReportRepository repository)
        {
            _repository = repository;
        }

        private string GetUserId() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";

        [HttpGet("monthly")]
        // Mnthly report for a given year and month
        public async Task<IActionResult> GetMonthly([FromQuery] int year, [FromQuery] int month)
        {
            var report = await _repository.GenerateMonthlyReportAsync(GetUserId(), year, month);

            var result = new MonthlyReportDto
            {
                Month = report.Month,
                TotalIncome = report.TotalIncome,
                TotalExpenses = report.TotalExpenses
            };

            return Ok(result);
        }

        [HttpGet("yearly")]
        // Yearly report for a given year
        public async Task<IActionResult> GetYearly([FromQuery] int year)
        {
            var report = await _repository.GenerateYearlyReportAsync(GetUserId(), year);

            var result = new YearlyReportDto
            {
                Year = year,
                TotalIncome = report.TotalIncome,
                TotalExpenses = report.TotalExpenses
            };

            return Ok(result);
        }
    }
}
