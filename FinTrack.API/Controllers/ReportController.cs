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
            var yearError = ValidateYear(year);
            if (yearError != null) return BadRequest(new { message = yearError });

            if (month < 1 || month > 12)
                return BadRequest(new { message = "Month must be between 1 and 12." });

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
            var yearError = ValidateYear(year);
            if (yearError != null) return BadRequest(new { message = yearError });

            var report = await _repository.GenerateYearlyReportAsync(GetUserId(), year);

            var result = new YearlyReportDto
            {
                Year = year,
                TotalIncome = report.TotalIncome,
                TotalExpenses = report.TotalExpenses
            };

            return Ok(result);
        }

        private static string? ValidateYear(int year)
        {
            var maxYear = DateTime.UtcNow.Year + 1;
            if (year < 1900 || year > maxYear)
                return $"Year must be between 1900 and {maxYear}.";

            return null;
        }
    }
}
