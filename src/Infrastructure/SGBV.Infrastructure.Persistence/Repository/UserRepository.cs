using Microsoft.EntityFrameworkCore;
using SGBV.Application.Interfaces.Repositories;
using SGBV.Domain.Models;
using SGBV.Infrastructure.Persistence.Context;

namespace SGBV.Infrastructure.Persistence.Repository;

public class UserRepository(SgbvContext context) : GenericRepository<User>(context), IUserRepository
{
    public async Task<User> GetByEmailAsync(string email, CancellationToken cancellationToken) =>
        await context.Set<User>()
            .AsNoTracking()
            .Where(u => u.Email == email)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<bool> EmailInUseAsync(string email, Guid userId, CancellationToken cancellationToken) =>
        await ValidateAsync(u => u.Email == email && u.Id != userId, cancellationToken);

    public async Task<bool> EmailInUseByYouAsync(Guid userId, string email, CancellationToken cancellationToken) =>
        await ValidateAsync(u => u.Email == email && u.Id == userId, cancellationToken);

    public async Task UpdatePasswordAsync(User user, string newPassword, CancellationToken cancellationToken)
    {
        user.PasswordHash = newPassword;
        context.Set<User>().Update(user);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> EmailExistAsync(string email, CancellationToken cancellationToken) =>
        await ValidateAsync(u => u.Email == email, cancellationToken);


    public async Task<User?> GetUserDetailsAsync(Guid id, CancellationToken cancellationToken) =>
        await context.Set<User>()
            .Where(u => u.Id == id)
            .Select(u => new User
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                PasswordHash = u.PasswordHash,
                ProfileUrl = u.ProfileUrl,
                RegistrationDate = u.RegistrationDate,
                LoginAt = u.LoginAt
            })
            .FirstOrDefaultAsync(cancellationToken);
    
    public async Task<int> GetTotalUserCountAsync(CancellationToken cancellationToken)
    {
        return await context.Users.CountAsync(cancellationToken);
    }

}