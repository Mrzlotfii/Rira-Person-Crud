using Microsoft.AspNetCore.Server.Kestrel.Core;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Profile.Features.Persons;
using Profile.Infrastructure.Database.Store;
using Profile.Services;
using SharedKernel.Domain;
using SharedKernel.Infrastructure.Helper;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel to support both HTTP/1.1 and HTTP/2
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(5115, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
    });
});

builder.Services.AddGrpc().AddJsonTranscoding();
builder.Services.AddGrpcReflection();

// Register validators
builder.Services.AddValidatorsFromAssemblyContaining<CreatePerson.CreatePersonRequestValidator>();

// Register handlers
builder.Services.Scan(scan => scan
    .FromAssemblies(typeof(CreatePerson.CreatePersonHandler).Assembly)
    .AddClasses(classes => classes.AssignableTo<IHandler>(), publicOnly: false)
    .AsSelf()
    .WithScopedLifetime());

builder.Services.AddSingleton<ISnowflakeGenerator, SnowflakeGenerator>();

builder.Services.AddSingleton<PersonStore>();

var app = builder.Build();

app.MapGrpcService<PersonCrudService>();
app.MapGrpcReflectionService();
app.MapGet("/", () => "gRPC PersonCrud service is running on http://localhost:5115");

app.Run();
