using FIAP.TechChallenge.ContactsUpdateProducer.Api.IoC;
using FIAP.TechChallenge.ContactsUpdateProducer.Api.Logging;
using FIAP.TechChallenge.ContactsUpdateProducer.Api.Middleware;
using FIAP.TechChallenge.ContactsUpdateProducer.Domain.DTOs.Application;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Configuration.AddJsonFile($"appsettings.Dev.json", optional: true, reloadOnChange: true);
builder.Configuration.AddEnvironmentVariables();

IConfiguration configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                                                         .AddJsonFile("appsettings.json", false, true)
                                                         .AddJsonFile($"appsettings.Dev.json", true, true)
                                                         .AddEnvironmentVariables()
                                                         .Build();

builder.Services.AddDependencyResolver(builder.Configuration.GetConnectionString("DefaultConnection"));

builder.Services.AddSingleton<IConfiguration>(configuration);

var massTransitConfig = new MassTransitDTO()
{
    QueueName = configuration.GetSection("MassTransit")["Username"] ?? string.Empty,

    Server = configuration.GetSection("MassTransit")["Server"] ?? string.Empty,

    User = configuration.GetSection("MassTransit")["User"] ?? string.Empty,

    Password = configuration.GetSection("MassTransit")["Password"] ?? string.Empty
};

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(massTransitConfig.Server, "/", h =>
        {
            h.Username(massTransitConfig.User);
            h.Password(massTransitConfig.Password);
        });

        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks().ForwardToPrometheus();

builder.Logging.ClearProviders();
builder.Logging.AddProvider(
    new CustomLoggerProvider(
        new CustomLoggerProviderConfiguration
        {
            LogLevel = LogLevel.Information,
        }));

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TechChallenge Terceiro FIAP 2025", Version = "v1" });

    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var teste = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}";

    var xmlPath = System.IO.Path.Combine(System.AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"Cabeçalho de autorização JWT usando o esquema Bearer. 
                        Insira 'Bearer' [espaço] e, em seguida, seu token na entrada de texto abaixo.
                        Exemplo: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseReDoc(c =>
{
    c.DocumentTitle = "REDOC API Documentation";
    c.SpecUrl = "/swagger/v1/swagger.json";
});

app.UseListaUserMiddleware();

app.UseHealthChecks("/health");
app.UseHttpMetrics();
app.MapMetrics();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
