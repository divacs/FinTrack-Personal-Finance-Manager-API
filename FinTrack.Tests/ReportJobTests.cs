using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Xunit;
using FinTrack.Application.Jobs;
using FinTrack.Application.Interfaces;
using FinTrack.Domain.Entities;

public class ReportJobTests
{
    private readonly Mock<IReportRepository> _reportRepo = new();
    private readonly Mock<IUserRepository> _userRepo = new();
    private readonly Mock<IEmailService> _emailService = new();
    private readonly Mock<IReportJobLogRepository> _logRepo = new();

    private ReportJob CreateJob() =>
        new ReportJob(_reportRepo.Object, _userRepo.Object, _emailService.Object, _logRepo.Object);

    [Fact]
    public async Task SendMonthlyReportsAsync_SendsEmailAndLogsSuccess()
    {
        // Arrange
        var user = new ApplicationUser { Id = "u1", Email = "user@x.com", FirstName = "Ana" };
        _userRepo.Setup(r => r.GetAllUsersAsync()).ReturnsAsync(new[] { user });

        var report = new Report
        {
            UserId = user.Id,
            Month = "2025-11",
            TotalIncome = 1000m,
            TotalExpenses = 400m,
            GeneratedAt = DateTime.UtcNow
        };
        _reportRepo.Setup(r => r.GenerateMonthlyReportAsync(user.Id, It.IsAny<int>(), It.IsAny<int>()))
                   .ReturnsAsync(report);

        // Act
        var job = CreateJob();
        await job.SendMonthlyReportsAsync();

        // Assert
        _emailService.Verify(e => e.SendEmailAsync(
            user.Email,
            It.Is<string>(s => s.Contains("Monthly")),
            It.IsAny<string>()), Times.Once);

        _logRepo.Verify(l => l.AddAsync(It.Is<ReportJobLogs>(log =>
            log.UserId == user.Id &&
            log.ReportType == "Monthly" &&
            log.IsSuccess)), Times.Once);
    }

    [Fact]
    public async Task SendMonthlyReportsAsync_WhenEmailThrows_LogsFailure()
    {
        // Arrange
        var user = new ApplicationUser { Id = "u2", Email = "bad@x.com", FirstName = "Marko" };
        _userRepo.Setup(r => r.GetAllUsersAsync()).ReturnsAsync(new[] { user });

        var report = new Report
        {
            UserId = user.Id,
            Month = "2025-11",
            TotalIncome = 500m,
            TotalExpenses = 200m,
            GeneratedAt = DateTime.UtcNow
        };
        _reportRepo.Setup(r => r.GenerateMonthlyReportAsync(user.Id, It.IsAny<int>(), It.IsAny<int>()))
                   .ReturnsAsync(report);

        _emailService.Setup(e => e.SendEmailAsync(user.Email, It.IsAny<string>(), It.IsAny<string>()))
                     .ThrowsAsync(new Exception("SMTP error"));

        // Act
        var job = CreateJob();
        await job.SendMonthlyReportsAsync();

        // Assert
        _logRepo.Verify(l => l.AddAsync(It.Is<ReportJobLogs>(log =>
            log.UserId == user.Id &&
            log.ReportType == "Monthly" &&
            !log.IsSuccess &&
            log.ErrorMessage.Contains("SMTP error"))), Times.Once);
    }
}
