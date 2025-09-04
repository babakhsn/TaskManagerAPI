using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;
using TaskManager.Api.Middleware;
using TaskManager.Application;
using TaskManager.Application.Mappings; // DomainToDtoProfile
using TaskManager.Application.Projects.CreateProject; // marker type
using TaskManager.Infrastructure;
using Microsoft.EntityFrameworkCore;
using TaskManager.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Serilog (optional)
Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "TaskManager.Api")
    .Enrich.WithCorrelationId()
    .WriteTo.Console().CreateLogger();
builder.Host.UseSerilog();

// MVC + FluentValidation
builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation()
                .AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<CreateProjectCommandValidator>();
builder.Services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();

// Swagger + JWT security scheme
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TaskManager API", Version = "v1" });
    c.EnableAnnotations();
    var xml = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xml);
    if (File.Exists(xmlPath))
        c.IncludeXmlComments(xmlPath);

    var scheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer {token}'"
    };
    c.AddSecurityDefinition("Bearer", scheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { scheme, Array.Empty<string>() }
    });
});

// app layers
//builder.Services.AddAutoMapper(typeof(DomainToDtoProfile).Assembly);


var mapperConfig = new AutoMapper.MapperConfiguration(
    cfg => { cfg.AddProfile<DomainToDtoProfile>(); },
    NullLoggerFactory.Instance // v15 requires a logger factory
);
IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseSerilogRequestLogging();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.Use((ctx, next) =>
{
    // Put TraceIdentifier into log context
    using (Serilog.Context.LogContext.PushProperty("TraceId", ctx.TraceIdentifier))
        return next();
});


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGet("/ping", () => "pong");

if (Environment.GetEnvironmentVariable("APPLY_MIGRATIONS") == "true")
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

if (Environment.GetEnvironmentVariable("SEED_DEMO") == "true")
{
    using var scope = app.Services.CreateScope();
    await DbSeeder.SeedAsync(scope.ServiceProvider);
}

app.Run();


public partial class Program { }
