using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shouldly;
using System.Text.Json.Serialization;
using TaskManagementSystem.Api;
using TaskManagementSystem.Api.ExceptionHandling;
using TaskManagementSystem.Api.Models;
using TaskManagementSystem.Application;
using TaskManagementSystem.Application.Abstractions;
using TaskManagementSystem.Infrastructure.Authentication;
using TaskManagementSystem.Infrastructure;
using TaskManagementSystem.Infrastructure.Database;

namespace TaskManagementSystem.Api.UnitTests.Controllers;

public sealed class ProgramServiceRegistrationTests
{
    [Fact]
    public void AddTaskManagementApi_ShouldRegisterExpectedServices()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                [$"{LiteDbSettings.SectionName}:ConnectionString"] = "Filename=test-api.db;Connection=shared",
                [$"{LiteDbSettings.SectionName}:TasksCollectionName"] = "management_tasks",
                [$"{LiteDbSettings.SectionName}:UsersCollectionName"] = "management_users",
                [$"{JwtAuthenticationSettings.SectionName}:Issuer"] = "TaskManagementSystem",
                [$"{JwtAuthenticationSettings.SectionName}:Audience"] = "TaskManagementSystem.Api",
                [$"{JwtAuthenticationSettings.SectionName}:SigningKey"] = "TaskManagementSystemDevelopmentSigningKey123!",
                [$"{JwtAuthenticationSettings.SectionName}:AccessTokenLifetimeMinutes"] = "60"
            })
            .Build();

        services.AddApplicationDependencies();
        services.AddInfrastructureDependencies(configuration);
        services.AddTaskManagementApi(configuration);

        services.ShouldContain(descriptor =>
            descriptor.ServiceType == typeof(IManagementTaskService) &&
            descriptor.Lifetime == ServiceLifetime.Scoped);

        services.ShouldContain(descriptor =>
            descriptor.ServiceType == typeof(IManagementUserService) &&
            descriptor.Lifetime == ServiceLifetime.Scoped);

        services.ShouldContain(descriptor =>
            descriptor.ServiceType == typeof(LiteDbContext) &&
            descriptor.Lifetime == ServiceLifetime.Scoped);

        services.ShouldContain(descriptor =>
            descriptor.ServiceType == typeof(IValidator<CreateManagementTaskRequestDto>));

        services.ShouldContain(descriptor =>
            descriptor.ServiceType == typeof(IValidator<UpdateManagementTaskRequestDto>));

        services.ShouldContain(descriptor =>
            descriptor.ServiceType == typeof(IValidator<ManagementTaskQueryRequestDto>));

        services.ShouldContain(descriptor =>
            descriptor.ServiceType == typeof(IValidator<PatchManagementTaskRequestDto>));

        services.ShouldContain(descriptor =>
            descriptor.ServiceType == typeof(IValidator<UpdateManagementTaskStatusRequestDto>));

        services.ShouldContain(descriptor =>
            descriptor.ServiceType == typeof(IValidator<CreateManagementUserRequestDto>));

        services.ShouldContain(descriptor =>
            descriptor.ServiceType == typeof(IValidator<LoginManagementUserRequestDto>));

        services.ShouldContain(descriptor =>
            descriptor.ServiceType == typeof(IExceptionHandler) &&
            descriptor.ImplementationType == typeof(GlobalExceptionHandler));

        services.ShouldContain(descriptor =>
            descriptor.ServiceType == typeof(IAuthenticationSchemeProvider));

        var provider = services.BuildServiceProvider();
        var jsonOptions = provider.GetRequiredService<IOptions<JsonOptions>>().Value;
        jsonOptions.JsonSerializerOptions.AllowTrailingCommas.ShouldBeTrue();
        jsonOptions.JsonSerializerOptions.Converters.ShouldContain(converter =>
            converter is JsonStringEnumConverter);
    }

    [Fact]
    public void ProgramConfiguration_ShouldUseSerilogConsoleLogging()
    {
        var apiRoot = TestPathHelper.GetApiRoot();
        var programFile = Path.Combine(apiRoot, "src", "TaskManagementSystem.Api", "Program.cs");

        File.Exists(programFile).ShouldBeTrue();

        var content = File.ReadAllText(programFile);

        content.ShouldContain("UseSerilog");
        content.ShouldContain("WriteTo.Console");
    }
}
