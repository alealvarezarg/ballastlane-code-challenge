using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Shouldly;
using TaskManagementSystem.Api.Controllers;
using TaskManagementSystem.Api.Models;
using TaskManagementSystem.Application.Abstractions;
using TaskManagementSystem.Application.Models;
using TaskManagementSystem.Domain.Entities;

namespace TaskManagementSystem.Api.UnitTests.Controllers;

public sealed class ManagementUserControllerTests
{
    private readonly IManagementUserService _managementUserService;
    private readonly ManagementUserController _controller;

    public ManagementUserControllerTests()
    {
        _managementUserService = A.Fake<IManagementUserService>();
        _controller = new ManagementUserController(_managementUserService);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnCreatedAtRoute()
    {
        var request = new CreateManagementUserRequestDto
        {
            Username = "manager",
            Email = "manager@example.com",
            Password = "Password123!"
        };
        var user = new ManagementUser(Guid.NewGuid(), request.Username, request.Email, "hashed-password");
        A.CallTo(() => _managementUserService.CreateAsync(request.Username, request.Email, request.Password))
            .Returns(user);

        var result = await _controller.CreateAsync(request);

        var createdResult = result.Result.ShouldBeOfType<CreatedAtRouteResult>();
        createdResult.RouteName.ShouldBe("GetManagementUserById");
        var response = createdResult.Value.ShouldBeOfType<ManagementUserResponseDto>();
        response.Id.ShouldBe(user.Id);
        response.Email.ShouldBe(user.Email);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnOkWithToken()
    {
        var request = new LoginManagementUserRequestDto
        {
            Email = "manager@example.com",
            Password = "Password123!"
        };
        var authenticatedUser = new AuthenticatedManagementUser
        {
            Id = Guid.NewGuid(),
            Username = "manager",
            Email = request.Email,
            AccessToken = "access-token",
            ExpiresAt = DateTime.UtcNow.AddMinutes(30)
        };
        A.CallTo(() => _managementUserService.LoginAsync(request.Email, request.Password))
            .Returns(authenticatedUser);

        var result = await _controller.LoginAsync(request);

        var okResult = result.Result.ShouldBeOfType<OkObjectResult>();
        var response = okResult.Value.ShouldBeOfType<ManagementUserLoginResponseDto>();
        response.AccessToken.ShouldBe("access-token");
        response.User.Email.ShouldBe(request.Email);
        response.User.ShouldBeOfType<ManagementUserResponseDto>();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnOkWithResponseDto()
    {
        var user = new ManagementUser(Guid.NewGuid(), "manager", "manager@example.com", "hashed-password");
        A.CallTo(() => _managementUserService.GetByIdAsync(user.Id)).Returns(user);

        var result = await _controller.GetByIdAsync(user.Id);

        var okResult = result.Result.ShouldBeOfType<OkObjectResult>();
        var response = okResult.Value.ShouldBeOfType<ManagementUserResponseDto>();
        response.Id.ShouldBe(user.Id);
        response.Username.ShouldBe(user.Username);
    }

    [Fact]
    public void ManagementUserResponseDto_ShouldNotExposePasswordFields()
    {
        var propertyNames = typeof(ManagementUserResponseDto)
            .GetProperties()
            .Select(property => property.Name)
            .ToArray();

        propertyNames.ShouldNotContain("Password");
        propertyNames.ShouldNotContain("PasswordHash");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrowKeyNotFoundException_WhenUserDoesNotExist()
    {
        var id = Guid.NewGuid();
        A.CallTo(() => _managementUserService.GetByIdAsync(id)).Returns(Task.FromResult<ManagementUser?>(null));

        var exception = await Should.ThrowAsync<KeyNotFoundException>(() => _controller.GetByIdAsync(id));

        exception.Message.ShouldContain(id.ToString());
    }
}
