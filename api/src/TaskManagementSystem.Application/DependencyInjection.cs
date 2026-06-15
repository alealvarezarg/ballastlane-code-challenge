using Microsoft.Extensions.DependencyInjection;
using TaskManagementSystem.Application.Abstractions;
using TaskManagementSystem.Application.Services;

namespace TaskManagementSystem.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationDependencies(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddScoped<IManagementTaskService, ManagementTaskService>();
        services.AddScoped<IManagementUserService, ManagementUserService>();

        return services;
    }
}
