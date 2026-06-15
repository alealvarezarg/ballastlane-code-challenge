using Microsoft.Extensions.Options;
using Shouldly;
using TaskManagementSystem.Domain.Entities;
using TaskManagementSystem.Infrastructure.Database;
using TaskManagementSystem.Infrastructure.Repositories;

namespace TaskManagementSystem.Infrastructure.UnitTests.Repositories;

public sealed class ManagementUserRepositoryTests : IDisposable
{
    private readonly LiteDbContext _context;
    private readonly string _databasePath;
    private readonly ManagementUserRepository _repository;

    public ManagementUserRepositoryTests()
    {
        _databasePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.db");
        var settings = new LiteDbSettings
        {
            ConnectionString = $"Filename={_databasePath};Connection=shared"
        };

        _context = new LiteDbContext(Options.Create(settings));
        _repository = new ManagementUserRepository(_context);
    }

    [Fact]
    public async Task CreateAsync_ShouldPersistUser()
    {
        var user = CreateUser();

        var result = await _repository.CreateAsync(user);

        result.Id.ShouldBe(user.Id);
        var stored = await _repository.GetByIdAsync(user.Id);
        stored.ShouldNotBeNull();
        stored.Email.ShouldBe(user.Email);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllUsers()
    {
        var firstUser = CreateUser();
        var secondUser = new ManagementUser(Guid.NewGuid(), "manager-2", "manager2@example.com", "hashed-password-2");
        await _repository.CreateAsync(firstUser);
        await _repository.CreateAsync(secondUser);

        var result = await _repository.GetAllAsync();

        result.Count.ShouldBe(2);
        result.ShouldContain(user => user.Email == firstUser.Email);
        result.ShouldContain(user => user.Email == secondUser.Email);
    }

    [Fact]
    public async Task GetByEmailAsync_ShouldReturnMatchingUser()
    {
        var user = CreateUser();
        await _repository.CreateAsync(user);

        var result = await _repository.GetByEmailAsync(user.Email);

        result.ShouldNotBeNull();
        result.Id.ShouldBe(user.Id);
    }

    [Fact]
    public async Task GetByUsernameAsync_ShouldReturnMatchingUser()
    {
        var user = CreateUser();
        await _repository.CreateAsync(user);

        var result = await _repository.GetByUsernameAsync(user.Username);

        result.ShouldNotBeNull();
        result.Id.ShouldBe(user.Id);
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnFalse_WhenUserDoesNotExist()
    {
        var result = await _repository.ExistsAsync(Guid.NewGuid());

        result.ShouldBeFalse();
    }

    public void Dispose()
    {
        _context.Dispose();

        if (File.Exists(_databasePath))
        {
            File.Delete(_databasePath);
        }
    }

    private static ManagementUser CreateUser()
    {
        return new ManagementUser(Guid.NewGuid(), "manager", "manager@example.com", "hashed-password");
    }
}
