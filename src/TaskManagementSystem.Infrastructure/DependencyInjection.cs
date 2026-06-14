using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TaskManagementSystem.Application.Abstractions;
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
        services.AddScoped(serviceProvider =>
            serviceProvider.GetRequiredService<IOptionsSnapshot<LiteDbSettings>>().Value);
        services.AddScoped<LiteDbContext>();
        services.AddScoped<IManagementTaskRepository, ManagementTaskRepository>();

        return services;
    }
}
