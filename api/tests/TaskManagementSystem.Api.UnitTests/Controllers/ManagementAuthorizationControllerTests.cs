using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Shouldly;
using TaskManagementSystem.Api.Controllers;

namespace TaskManagementSystem.Api.UnitTests.Controllers;

public sealed class ManagementAuthorizationControllerTests
{
    [Fact]
    public void ManagementTaskController_ShouldRequireAuthorization()
    {
        typeof(ManagementTaskController).GetCustomAttribute<AuthorizeAttribute>().ShouldNotBeNull();
    }

    [Theory]
    [InlineData(nameof(ManagementUserController.CreateAsync))]
    [InlineData(nameof(ManagementUserController.LoginAsync))]
    [InlineData(nameof(ManagementUserController.GetByIdAsync))]
    public void ManagementUserEndpoints_ShouldAllowAnonymous(string methodName)
    {
        var method = typeof(ManagementUserController).GetMethod(methodName);

        method.ShouldNotBeNull();
        method!.GetCustomAttribute<AllowAnonymousAttribute>().ShouldNotBeNull();
    }
}
