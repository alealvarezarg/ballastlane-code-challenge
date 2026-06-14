using TaskManagementSystem.Application.Abstractions;
using TaskManagementSystem.Application.Models;
using TaskManagementSystem.Domain.Entities;
using TaskManagementSystem.Domain.Exceptions;

namespace TaskManagementSystem.Application.Services;

public sealed class ManagementUserService : IManagementUserService
{
    private readonly IAccessTokenService _accessTokenService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IManagementUserRepository _repository;

    public ManagementUserService(
        IManagementUserRepository repository,
        IPasswordHasher passwordHasher,
        IAccessTokenService accessTokenService)
    {
        _repository = repository;
        _passwordHasher = passwordHasher;
        _accessTokenService = accessTokenService;
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

        return await _repository.CreateAsync(user);
    }

    public async Task<AuthenticatedManagementUser> LoginAsync(string email, string password)
    {
        var user = await _repository.GetByEmailAsync(email);

        if (user is null || !_passwordHasher.VerifyPassword(user.PasswordHash, password))
        {
            throw new InvalidManagementUserCredentialsException("The supplied credentials are invalid.");
        }

        return _accessTokenService.CreateToken(user);
    }

    public async Task<ManagementUser?> GetByIdAsync(Guid id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _repository.ExistsAsync(id);
    }
}
