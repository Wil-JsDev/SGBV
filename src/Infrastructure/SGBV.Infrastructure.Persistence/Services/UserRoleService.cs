using Microsoft.EntityFrameworkCore;
using SGBV.Application.Interfaces.Services;
using SGBV.Domain.Models;
using SGBV.Infrastructure.Persistence.Context;

namespace SGBV.Infrastructure.Persistence.Services;

public class UserRoleService(SgbvContext context) : IUserRoleService
{
    public Task<List<string>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken) =>
        context.Set<User>()
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .Select(u => u.Rol.NameRol)
            .ToListAsync(cancellationToken);

    public async Task<Role> GetRoleByNameAsync(string roleName, CancellationToken cancellationToken) =>
        await context.Set<Role>()
            .FirstOrDefaultAsync(c => c.NameRol == roleName, cancellationToken);
    
}