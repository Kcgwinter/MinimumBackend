using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using Api.Middleware;
using Application.Interfaces;
using Application.Services;
using Application.Settings;
using Core.DTOs;
using Core.Validators;
// using Features.Todo;
// using Features.Todo.Data;
using FluentValidation;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Serilog;
using Serilog.AspNetCore;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs\\log-.txt", retainedFileCountLimit: 30)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Add Serilog to logger factory
builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new HeaderApiVersionReader("x-api-version"),
        new QueryStringApiVersionReader("api-version")
    );
});

builder.Services.AddEndpointsApiExplorer();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi("v1"); // Explicitly name it v1

// Configure Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Configure Dependency Injection
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddHttpContextAccessor();

//CQRS - Mediatr
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(IQuery<>).Assembly); // Registers handlers in the Application layer
});

// Configure AutoMapper
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddMaps(typeof(Program).Assembly);
});

// Configure Authentication
builder
    .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
            ),
        };
    });

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        }
    );
});

// Configure rate limiting
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.User.Identity?.Name
                ?? httpContext.Request.Headers.Host.ToString(),
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 10,
                QueueLimit = 0,
                Window = TimeSpan.FromMinutes(1),
            }
        )
    );
});

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.ReferenceHandler = System
        .Text
        .Json
        .Serialization
        .ReferenceHandler
        .IgnoreCycles;
});
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

// Validators
// builder.Services.AddValidatorsFromAssemblyContaining<UserLoginDtoValidator>();

// Uses AddDBContextCheck to add health checks for the database contexts
builder.Services.AddHealthChecks().AddDbContextCheck<AppDbContext>();

//Add SMTP Service
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));

var app = builder.Build();

app.UseStaticFiles();
app.UseCors("AllowAll");

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseSerilogRequestLogging();
app.UseAuthentication();
// app.UseMiddleware<CsrfTokenMiddleware>();
app.UseAuthorization();
app.UseMiddleware<LoggingMiddleware>(); // Moved to run after Authorization

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();   // Serves the generated JSON document
    app.UseSwaggerUI(); // Enables the interactive web interface
}

app.MapControllers();
app.UseRateLimiter();

app.MapHealthChecks("/health");

// Register Features
// var todoConnectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
// builder.Services.AddDbContext<TodoDbContext>(options => options.UseSqlite(todoConnectionString));
// builder.Services.AddTodoFeature(todoConnectionString);

// // Ensure database is created and migrations applied
// using (var scope = app.Services.CreateScope())
// {
//     var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//     var dbTodoContext = scope.ServiceProvider.GetRequiredService<TodoDbContext>();
//     dbContext.Database.EnsureCreated();
// 
//     await DBInitializer.SeedAsync(dbContext);
// }

app.Run();