using SGBV.Domain.Models;

namespace SGBV.Application.Interfaces.Services;

public interface IUserRoleService
{
    /// <summary>
    /// Gets the list of role names assigned to a specific user.
    /// </summary>
    Task<List<string>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken);
    
    Task<Role> GetRoleByNameAsync(string roleName, CancellationToken cancellationToken);
}