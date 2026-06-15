using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TaskManagementSystem.Application.Abstractions;
using TaskManagementSystem.Infrastructure.Authentication;
using TaskManagementSystem.Infrastructure.Database;
using TaskManagementSystem.Infrastructure.Repositories;

namespace TaskManagementSystem.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureDependencies(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<LiteDbSettings>(configuration.GetSection(LiteDbSettings.SectionName));
        services.Configure<JwtAuthenticationSettings>(configuration.GetSection(JwtAuthenticationSettings.SectionName));

        services.AddScoped(serviceProvider =>
            serviceProvider.GetRequiredService<IOptionsSnapshot<LiteDbSettings>>().Value);

        services.AddScoped<LiteDbContext>();
        services.AddScoped<ManagementSeedDataInitializer>();

        services.AddScoped<IManagementTaskRepository, ManagementTaskRepository>();
        services.AddScoped<IManagementUserRepository, ManagementUserRepository>();
        services.AddScoped<IPasswordHasher, AspNetPasswordHasher>();
        services.AddScoped<IAccessTokenService, JwtAccessTokenService>();

        return services;
    }

    public static async Task InitializeInfrastructureAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var settings = scope.ServiceProvider.GetRequiredService<LiteDbSettings>();

        var databaseFile = settings.ConnectionString
            .Split(';')
            .First(x => x.StartsWith("Filename=", StringComparison.OrdinalIgnoreCase))
            .Replace("Filename=", "");

        if (!File.Exists(databaseFile))
        {
            var initializer = scope.ServiceProvider
                .GetRequiredService<ManagementSeedDataInitializer>();

            await initializer.InitializeAsync();
        }
    }
}
