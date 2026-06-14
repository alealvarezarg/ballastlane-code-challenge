using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using TaskManagementSystem.Application.Abstractions;
using TaskManagementSystem.Application.Services;

namespace TaskManagementSystem.Application.UnitTests.DependencyInjection;

public sealed class DependencyInjectionTests
{
    [Fact]
    public void AddApplicationDependencies_ShouldRegisterApplicationServices()
    {
        var services = new ServiceCollection();

        services.AddApplicationDependencies();

        services.Any(service => service.ServiceType == typeof(IManagementTaskService) &&
                                service.ImplementationType == typeof(ManagementTaskService))
            .ShouldBeTrue();
        services.Any(service => service.ServiceType == typeof(IMemoryCache))
            .ShouldBeTrue();
    }
}
