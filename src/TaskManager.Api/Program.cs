using AutoMapper;
using TaskManager.Application.Mappings; // DomainToDtoProfile
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.OpenApi.Models;
using Serilog;
using TaskManager.Application;
using TaskManager.Application.Projects.CreateProject; // marker type
using TaskManager.Infrastructure;
using Microsoft.Extensions.Logging.Abstractions;

var builder = WebApplication.CreateBuilder(args);

// Serilog (optional)
Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).WriteTo.Console().CreateLogger();
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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGet("/ping", () => "pong");

app.Run();


public partial class Program { }
