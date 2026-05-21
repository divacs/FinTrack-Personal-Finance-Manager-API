using FinTrack.API.Service;
using FinTrack.API.Utility.Seeders;
using FinTrack.Application.Interfaces;
using FinTrack.Application.Jobs;
using FinTrack.Application.Services;
using FinTrack.Domain.Entities;
using FinTrack.Infrastructure.Data;
using FinTrack.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using TaskFlow.Utility.Service;
using Hangfire;
using Hangfire.Dashboard;

var builder = WebApplication.CreateBuilder(args);

ConfigurationValidator.Validate(builder.Configuration);

// ============================================
// Add services to the container
// ============================================
builder.Services.AddControllers();

// ============================================
// Configure Database (EF Core + SQL Server)
// ============================================
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ============================================
// Configure Identity
// ============================================
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// ============================================
// Clear default claim mapping
// (this MUST come BEFORE AddJwtBearer)
// ============================================
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

// ============================================
// JWT Authentication configuration
// ============================================
var jwtSettings = builder.Configuration.GetSection("JWT");
var jwtIssuer = jwtSettings["Issuer"]!;
var jwtAudience = jwtSettings["Audience"]!;
var jwtSigningKey = jwtSettings["SigningKey"]!;

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSigningKey))
    };
});

// ============================================
// Register custom application services
// ============================================
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddScoped<IBankAccountRepository, BankAccountRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IBudgetRepository, BudgetRepository>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IReportJobLogRepository, ReportJobLogRepository>();

builder.Services.AddScoped<ReportJob>();

// ============================================
// Configure Hangfire
// ============================================
builder.Services.AddHangfire(config =>
    config.UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHangfireServer();

// ============================================
// Swagger + JWT Authentication support
// ============================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "FinTrack API",
        Version = "v1",
        Description = "API documentation for the FinTrack Personal Finance Management system."
    });

    // JWT Authorization in Swagger
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter JWT Bearer token **_only_**",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };

    c.AddSecurityDefinition("Bearer", securityScheme);

    var securityRequirement = new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    };

    c.AddSecurityRequirement(securityRequirement);
});

var app = builder.Build();

// ============================================
// Seed Roles & Users
// ============================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    await RoleSeeder.SeedRoles(roleManager);

    if (app.Environment.IsDevelopment())
    {
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        await UserSeeder.SeedUsers(userManager);
    }
}

// ============================================
// Configure middleware pipeline
// ============================================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();  // Must come before Authorization
app.UseAuthorization();

// Hangfire Dashboard
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new HangfireAdminAuthorizationFilter() }
});

// Schedule recurring jobs
RecurringJob.AddOrUpdate<ReportJob>(
    "monthly-report-job",
    job => job.SendMonthlyReportsAsync(),
    Cron.Monthly);

RecurringJob.AddOrUpdate<ReportJob>(
    "yearly-report-job",
    job => job.SendYearlyReportsAsync(),
    Cron.Yearly);

app.MapControllers();

app.Run();

public class HangfireAdminAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();
        return httpContext.User.Identity?.IsAuthenticated == true &&
               httpContext.User.IsInRole("Admin");
    }
}

public static class ConfigurationValidator
{
    private const int MinJwtSigningKeyLength = 32;
    private static readonly HashSet<string> PlaceholderValues = new(StringComparer.OrdinalIgnoreCase)
    {
        "key",
        "secret",
        "signingkey",
        "issuer",
        "audience",
        "password",
        "changeme",
        "change-me",
        "email@gmail.com",
        "bla bla bla bla"
    };

    public static void Validate(IConfiguration configuration)
    {
        var jwtSigningKey = GetRequiredValue(configuration, "JWT:SigningKey");
        var jwtIssuer = GetRequiredValue(configuration, "JWT:Issuer");
        var jwtAudience = GetRequiredValue(configuration, "JWT:Audience");

        if (jwtSigningKey.Length < MinJwtSigningKeyLength)
            throw new InvalidOperationException($"Configuration value 'JWT:SigningKey' must be at least {MinJwtSigningKeyLength} characters long.");

        RejectPlaceholder("JWT:SigningKey", jwtSigningKey);
        RejectPlaceholder("JWT:Issuer", jwtIssuer);
        RejectPlaceholder("JWT:Audience", jwtAudience);

        var smtpServer = GetRequiredValue(configuration, "EmailSettings:SmtpServer");
        var smtpUsername = GetRequiredValue(configuration, "EmailSettings:Username");
        var smtpPassword = GetRequiredValue(configuration, "EmailSettings:Password");

        RejectPlaceholder("EmailSettings:SmtpServer", smtpServer);
        RejectPlaceholder("EmailSettings:Username", smtpUsername);
        RejectPlaceholder("EmailSettings:Password", smtpPassword);

        var smtpPortValue = GetRequiredValue(configuration, "EmailSettings:Port");
        if (!int.TryParse(smtpPortValue, out var smtpPort) || smtpPort < 1 || smtpPort > 65535)
            throw new InvalidOperationException("Configuration value 'EmailSettings:Port' must be a valid TCP port between 1 and 65535.");
    }

    private static string GetRequiredValue(IConfiguration configuration, string key)
    {
        var value = configuration[key];
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidOperationException($"Missing required configuration value '{key}'.");

        return value;
    }

    private static void RejectPlaceholder(string key, string value)
    {
        if (PlaceholderValues.Contains(value.Trim()))
            throw new InvalidOperationException($"Configuration value '{key}' must not use a placeholder/default value.");
    }
}
