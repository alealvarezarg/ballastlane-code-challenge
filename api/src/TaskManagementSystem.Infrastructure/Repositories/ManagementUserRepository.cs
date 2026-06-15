using TaskManagementSystem.Application.Abstractions;
using TaskManagementSystem.Domain.Entities;
using TaskManagementSystem.Infrastructure.Database;

namespace TaskManagementSystem.Infrastructure.Repositories;

public sealed class ManagementUserRepository : IManagementUserRepository
{
    private readonly LiteDbContext _context;

    public ManagementUserRepository(LiteDbContext context)
    {
        _context = context;
    }

    public Task<ManagementUser> CreateAsync(ManagementUser user)
    {
        var document = ManagementUserDocument.FromDomain(user);
        _context.Users.Insert(document);
        return Task.FromResult(document.ToDomain());
    }

    public Task<IReadOnlyCollection<ManagementUser>> GetAllAsync()
    {
        IReadOnlyCollection<ManagementUser> users = _context.Users
            .FindAll()
            .Select(document => document.ToDomain())
            .ToArray();

        return Task.FromResult(users);
    }

    public Task<ManagementUser?> GetByIdAsync(Guid id)
    {
        var document = _context.Users.FindById(id);
        return Task.FromResult(document?.ToDomain());
    }

    public Task<ManagementUser?> GetByEmailAsync(string email)
    {
        var document = _context.Users
            .FindAll()
            .FirstOrDefault(user => string.Equals(user.Email, email.Trim(), StringComparison.OrdinalIgnoreCase));

        return Task.FromResult(document?.ToDomain());
    }

    public Task<ManagementUser?> GetByUsernameAsync(string username)
    {
        var document = _context.Users
            .FindAll()
            .FirstOrDefault(user => string.Equals(user.Username, username.Trim(), StringComparison.OrdinalIgnoreCase));

        return Task.FromResult(document?.ToDomain());
    }

    public Task<bool> ExistsAsync(Guid id)
    {
        return Task.FromResult(_context.Users.Exists(user => user.Id == id));
    }
}
