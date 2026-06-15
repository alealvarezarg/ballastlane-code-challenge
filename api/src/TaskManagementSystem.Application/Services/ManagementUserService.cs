using Microsoft.Extensions.Logging;
using TaskManagementSystem.Application.Abstractions;
using TaskManagementSystem.Application.Models;
using TaskManagementSystem.Domain.Entities;
using TaskManagementSystem.Domain.Exceptions;

namespace TaskManagementSystem.Application.Services;

public sealed class ManagementUserService : IManagementUserService
{
    private readonly IAccessTokenService _accessTokenService;
    private readonly ILogger<ManagementUserService> _logger;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IManagementUserRepository _repository;

    public ManagementUserService(
        IManagementUserRepository repository,
        IPasswordHasher passwordHasher,
        IAccessTokenService accessTokenService,
        ILogger<ManagementUserService> logger)
    {
        _repository = repository;
        _passwordHasher = passwordHasher;
        _accessTokenService = accessTokenService;
        _logger = logger;
    }

    public async Task<ManagementUser> CreateAsync(string username, string email, string password)
    {
        if (await _repository.GetByUsernameAsync(username) is not null)
        {
            throw new DuplicateManagementUserException("A management user with the supplied username already exists.");
        }

        if (await _repository.GetByEmailAsync(email) is not null)
        {
            throw new DuplicateManagementUserException("A management user with the supplied email already exists.");
        }

        var passwordHash = _passwordHasher.HashPassword(password);
        var user = new ManagementUser(Guid.NewGuid(), username, email, passwordHash);

        var createdUser = await _repository.CreateAsync(user);
        _logger.LogInformation("Created management user {UserId} with email {Email}.", createdUser.Id, createdUser.Email);
        return createdUser;
    }

    public async Task<AuthenticatedManagementUser> LoginAsync(string email, string password)
    {
        var user = await _repository.GetByEmailAsync(email);

        if (user is null || !_passwordHasher.VerifyPassword(user.PasswordHash, password))
        {
            throw new InvalidManagementUserCredentialsException("The supplied credentials are invalid.");
        }

        var authenticatedUser = _accessTokenService.CreateToken(user);
        _logger.LogInformation("Authenticated management user {UserId} with email {Email}.", user.Id, user.Email);
        return authenticatedUser;
    }

    public async Task<IReadOnlyCollection<ManagementUser>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _repository.ExistsAsync(id);
    }
}
