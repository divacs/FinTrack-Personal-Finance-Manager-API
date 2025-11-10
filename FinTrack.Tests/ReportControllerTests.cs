using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using FluentAssertions;
using FinTrack.API.Controllers;
using FinTrack.Application.Interfaces;
using FinTrack.Domain.Entities;

public class ReportControllerTests
{
    [Fact]
    public async Task GetMonthly_ReturnsOk_WithDto()
    {
        // Arrange
        var repoMock = new Mock<IReportRepository>();
        var controller = new ReportController(repoMock.Object);

        // mock user claims
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, "user-1")
        }, "mock"));

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        var report = new Report { Month = "2025-11", TotalIncome = 100, TotalExpenses = 50 };
        repoMock.Setup(r => r.GenerateMonthlyReportAsync("user-1", 2025, 11)).ReturnsAsync(report);

        // Act
        var result = await controller.GetMonthly(2025, 11);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var ok = result as OkObjectResult;
        ok!.Value.Should().BeOfType<FinTrack.API.DTOs.Report.MonthlyReportDto>();
        var dto = ok.Value as FinTrack.API.DTOs.Report.MonthlyReportDto;
        dto!.Month.Should().Be("2025-11");
        dto.TotalIncome.Should().Be(100);
    }
}
