using TaskManagementSystem.Application.Models;
using TaskManagementSystem.Domain.Entities;

namespace TaskManagementSystem.Application.Abstractions;

public interface IManagementUserService
{
    Task<ManagementUser> CreateAsync(string username, string email, string password);

    Task<AuthenticatedManagementUser> LoginAsync(string email, string password);

    Task<IReadOnlyCollection<ManagementUser>> GetAllAsync();

    Task<bool> ExistsAsync(Guid id);
}
