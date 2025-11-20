using SGBV.Application.DTOs;
using SGBV.Application.Utilities;
using SGBV.Domain.Models;

namespace SGBV.Application.Interfaces.Services;

public interface IAuthService
{
    Task<string> GenerateAccessTokenAsync(User user, CancellationToken cancellationToken);

    Task<string> GenerateRefreshTokenAsync(User user, CancellationToken cancellationToken);
    Task<ResultT<TokenResponseDto>> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);
    
}