using FakeItEasy;
using Microsoft.Extensions.Logging;
using Shouldly;
using TaskManagementSystem.Application.Abstractions;
using TaskManagementSystem.Application.Models;
using TaskManagementSystem.Application.Services;
using TaskManagementSystem.Domain.Entities;
using TaskManagementSystem.Domain.Exceptions;

namespace TaskManagementSystem.Application.UnitTests.Services;

public sealed class ManagementUserServiceTests
{
    private readonly IAccessTokenService _accessTokenService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IManagementUserRepository _repository;
    private readonly ILogger<ManagementUserService> _logger;
    private readonly ManagementUserService _service;

    public ManagementUserServiceTests()
    {
        _repository = A.Fake<IManagementUserRepository>();
        _passwordHasher = A.Fake<IPasswordHasher>();
        _accessTokenService = A.Fake<IAccessTokenService>();
        _logger = A.Fake<ILogger<ManagementUserService>>();
        _service = new ManagementUserService(_repository, _passwordHasher, _accessTokenService, _logger);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateUser_WhenUsernameAndEmailAreAvailable()
    {
        A.CallTo(() => _repository.GetByUsernameAsync("manager")).Returns(Task.FromResult<ManagementUser?>(null));
        A.CallTo(() => _repository.GetByEmailAsync("manager@example.com")).Returns(Task.FromResult<ManagementUser?>(null));
        A.CallTo(() => _passwordHasher.HashPassword("Password123!")).Returns("hashed-password");
        A.CallTo(() => _repository.CreateAsync(A<ManagementUser>._))
            .ReturnsLazily(call => Task.FromResult(call.GetArgument<ManagementUser>(0)!));

        var result = await _service.CreateAsync("manager", "manager@example.com", "Password123!");

        result.Username.ShouldBe("manager");
        result.Email.ShouldBe("manager@example.com");
        result.PasswordHash.ShouldBe("hashed-password");
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowConflict_WhenUsernameAlreadyExists()
    {
        A.CallTo(() => _repository.GetByUsernameAsync("manager"))
            .Returns(new ManagementUser(Guid.NewGuid(), "manager", "existing@example.com", "hash"));

        var exception = await Should.ThrowAsync<DuplicateManagementUserException>(() =>
            _service.CreateAsync("manager", "manager@example.com", "Password123!"));

        exception.Message.ShouldContain("username");
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowConflict_WhenEmailAlreadyExists()
    {
        A.CallTo(() => _repository.GetByUsernameAsync("manager")).Returns(Task.FromResult<ManagementUser?>(null));
        A.CallTo(() => _repository.GetByEmailAsync("manager@example.com"))
            .Returns(new ManagementUser(Guid.NewGuid(), "existing", "manager@example.com", "hash"));

        var exception = await Should.ThrowAsync<DuplicateManagementUserException>(() =>
            _service.CreateAsync("manager", "manager@example.com", "Password123!"));

        exception.Message.ShouldContain("email");
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnToken_WhenCredentialsAreValid()
    {
        var user = new ManagementUser(Guid.NewGuid(), "manager", "manager@example.com", "hashed-password");
        var authenticatedUser = new AuthenticatedManagementUser
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            AccessToken = "token",
            ExpiresAt = DateTime.UtcNow.AddHours(1)
        };
        A.CallTo(() => _repository.GetByEmailAsync(user.Email)).Returns(user);
        A.CallTo(() => _passwordHasher.VerifyPassword(user.PasswordHash, "Password123!")).Returns(true);
        A.CallTo(() => _accessTokenService.CreateToken(user)).Returns(authenticatedUser);

        var result = await _service.LoginAsync(user.Email, "Password123!");

        result.AccessToken.ShouldBe("token");
        result.Email.ShouldBe(user.Email);
    }

    [Fact]
    public async Task LoginAsync_ShouldThrowUnauthorized_WhenCredentialsAreInvalid()
    {
        var user = new ManagementUser(Guid.NewGuid(), "manager", "manager@example.com", "hashed-password");
        A.CallTo(() => _repository.GetByEmailAsync(user.Email)).Returns(user);
        A.CallTo(() => _passwordHasher.VerifyPassword(user.PasswordHash, "WrongPassword123!")).Returns(false);

        await Should.ThrowAsync<InvalidManagementUserCredentialsException>(() =>
            _service.LoginAsync(user.Email, "WrongPassword123!"));
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnRepositoryUser()
    {
        var user = new ManagementUser(Guid.NewGuid(), "manager", "manager@example.com", "hashed-password");
        A.CallTo(() => _repository.GetByIdAsync(user.Id)).Returns(user);

        var result = await _service.GetByIdAsync(user.Id);

        result.ShouldBe(user);
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnRepositoryResult()
    {
        var id = Guid.NewGuid();
        A.CallTo(() => _repository.ExistsAsync(id)).Returns(true);

        var result = await _service.ExistsAsync(id);

        result.ShouldBeTrue();
    }

    [Fact]
    public async Task CreateAsync_ShouldWriteInformationLog_WhenUserIsCreated()
    {
        A.CallTo(() => _repository.GetByUsernameAsync("manager")).Returns(Task.FromResult<ManagementUser?>(null));
        A.CallTo(() => _repository.GetByEmailAsync("manager@example.com")).Returns(Task.FromResult<ManagementUser?>(null));
        A.CallTo(() => _passwordHasher.HashPassword("Password123!")).Returns("hashed-password");
        A.CallTo(() => _repository.CreateAsync(A<ManagementUser>._))
            .ReturnsLazily(call => Task.FromResult(call.GetArgument<ManagementUser>(0)!));

        await _service.CreateAsync("manager", "manager@example.com", "Password123!");

        A.CallTo(_logger).Where(call =>
            call.Method.Name == nameof(ILogger.Log) &&
            call.GetArgument<LogLevel>(0) == LogLevel.Information &&
            call.GetArgument<object>(2).ToString()!.Contains("manager@example.com", StringComparison.Ordinal))
            .MustHaveHappened();
    }
}
