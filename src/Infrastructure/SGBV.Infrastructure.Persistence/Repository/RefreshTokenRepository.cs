using Microsoft.EntityFrameworkCore;
using SGBV.Application.Interfaces.Repositories;
using SGBV.Domain.Models;
using SGBV.Infrastructure.Persistence.Context;
using SGBV.Infrastructure.Persistence.Repository;

namespace Rex.Infrastructure.Persistence.Repository;

public class RefreshTokenRepository(SgbvContext context)
    : GenericRepository<RefreshToken>(context), IRefreshTokenRepository
{
    public async Task CreateRefreshTokenAsync(RefreshToken token, CancellationToken cancellationToken)
    {
        await context.Set<RefreshToken>().AddAsync(token, cancellationToken);
        await SaveAsync(cancellationToken);
    }

    public async Task<RefreshToken> GetRefreshTokenByIdAsync(Guid tokenId, CancellationToken cancellationToken) =>
        await context.Set<RefreshToken>()
            .FirstOrDefaultAsync(t => t.Id == tokenId, cancellationToken);

    public async Task<bool> IsRefreshTokenValidAsync(Guid userId, string receivedToken,
        CancellationToken cancellationToken)
    {
        var activeTokens = await GetActiveTokensByUserIdAsync(userId, cancellationToken);

        return activeTokens.Any(t => t.Token == receivedToken);
    }

    public async Task MarkRefreshTokenAsUsedAsync(string token, CancellationToken cancellationToken)
    {
        var userToken = await context.Set<RefreshToken>()
            .FirstOrDefaultAsync(t => t.Token == token, cancellationToken);

        if (userToken != null)
        {
            userToken.Used = true;
            await SaveAsync(cancellationToken);
        }
    }

    public async Task RevokeOldRefreshTokensAsync(Guid userId, Guid tokenId,
        CancellationToken cancellationToken) =>
        await context.Set<RefreshToken>()
            .Where(c => c.Id != tokenId && !c.Used && c.Expiration > DateTime.UtcNow && c.UserId == userId)
            .ExecuteUpdateAsync(c => c.SetProperty(u => u.Revoked, true), cancellationToken);


    public async Task<List<RefreshToken>>
        GetActiveTokensByUserIdAsync(Guid userId, CancellationToken cancellationToken) =>
        await context.Set<RefreshToken>()
            .Where(t => t.UserId == userId && !t.Used && !t.Revoked && t.Expiration > DateTime.UtcNow)
            .ToListAsync(cancellationToken);
}