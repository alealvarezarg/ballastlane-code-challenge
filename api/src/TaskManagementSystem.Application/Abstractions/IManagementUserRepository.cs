using TaskManagementSystem.Domain.Entities;

namespace TaskManagementSystem.Application.Abstractions;

public interface IManagementUserRepository
{
    Task<ManagementUser> CreateAsync(ManagementUser user);

    Task<ManagementUser?> GetByIdAsync(Guid id);

    Task<ManagementUser?> GetByEmailAsync(string email);

    Task<ManagementUser?> GetByUsernameAsync(string username);

    Task<bool> ExistsAsync(Guid id);
}
