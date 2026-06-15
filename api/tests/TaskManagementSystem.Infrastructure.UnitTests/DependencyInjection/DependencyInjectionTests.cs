using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shouldly;
using TaskManagementSystem.Application.Abstractions;
using TaskManagementSystem.Infrastructure.Authentication;
using TaskManagementSystem.Infrastructure;
using TaskManagementSystem.Infrastructure.Database;
using TaskManagementSystem.Infrastructure.Repositories;

namespace TaskManagementSystem.Infrastructure.UnitTests.DependencyInjection;

public sealed class DependencyInjectionTests
{
    [Fact]
    public void AddInfrastructureDependencies_ShouldRegisterDatabaseAndRepositoryAsScoped()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                [$"{LiteDbSettings.SectionName}:ConnectionString"] = "Filename=test.db;Connection=shared",
                [$"{LiteDbSettings.SectionName}:TasksCollectionName"] = "management_tasks",
                [$"{LiteDbSettings.SectionName}:UsersCollectionName"] = "management_users",
                [$"{JwtAuthenticationSettings.SectionName}:Issuer"] = "TaskManagementSystem",
                [$"{JwtAuthenticationSettings.SectionName}:Audience"] = "TaskManagementSystem.Api",
                [$"{JwtAuthenticationSettings.SectionName}:SigningKey"] = "TaskManagementSystemDevelopmentSigningKey123!",
                [$"{JwtAuthenticationSettings.SectionName}:AccessTokenLifetimeMinutes"] = "60"
            })
            .Build();

        services.AddInfrastructureDependencies(configuration);

        services.ShouldContain(service =>
            service.ServiceType == typeof(LiteDbSettings) &&
            service.Lifetime == ServiceLifetime.Scoped);

        services.ShouldContain(service =>
            service.ServiceType == typeof(LiteDbContext) &&
            service.Lifetime == ServiceLifetime.Scoped);

        services.ShouldContain(service =>
            service.ServiceType == typeof(IManagementTaskRepository) &&
            service.ImplementationType == typeof(ManagementTaskRepository) &&
            service.Lifetime == ServiceLifetime.Scoped);

        services.ShouldContain(service =>
            service.ServiceType == typeof(IManagementUserRepository) &&
            service.ImplementationType == typeof(ManagementUserRepository) &&
            service.Lifetime == ServiceLifetime.Scoped);

        services.ShouldContain(service =>
            service.ServiceType == typeof(IPasswordHasher) &&
            service.ImplementationType == typeof(AspNetPasswordHasher) &&
            service.Lifetime == ServiceLifetime.Scoped);

        services.ShouldContain(service =>
            service.ServiceType == typeof(IAccessTokenService) &&
            service.ImplementationType == typeof(JwtAccessTokenService) &&
            service.Lifetime == ServiceLifetime.Scoped);

        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();
        var settings = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<LiteDbSettings>>().Value;

        settings.ConnectionString.ShouldBe("Filename=test.db;Connection=shared");
        settings.TasksCollectionName.ShouldBe("management_tasks");
        settings.UsersCollectionName.ShouldBe("management_users");
    }
}
