using TaskManagementSystem.Application.Models;
using TaskManagementSystem.Domain.Entities;

namespace TaskManagementSystem.Application.Abstractions;

public interface IAccessTokenService
{
    AuthenticatedManagementUser CreateToken(ManagementUser user);
}
