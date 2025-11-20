using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SGBV.Application.DTOs;
using SGBV.Application.Interfaces.Repositories;
using SGBV.Application.Interfaces.Services;
using SGBV.Application.Utilities;
using SGBV.Domain.Models;
using SGBV.Domain.Settings;

namespace SGBV.Infrastructure.Shared.Services;

public class AuthService(
    ILogger<AuthService> logger,
    IOptions<JWTSettings> jwtConfiguration,
    IUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IUserRoleService userRoleService
) : IAuthService
{
    private readonly JWTSettings _jwtSettings = jwtConfiguration.Value;

    public async Task<string> GenerateAccessTokenAsync(User user, CancellationToken cancellationToken)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("type", "access")
        };

        var roles = await userRoleService.GetUserRolesAsync(user.Id, cancellationToken);
        claims.AddRange(roles.Select(r => new Claim("role", r)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
            signingCredentials: creds
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        logger.LogInformation("Access token generated for user {UserId}", user.Id);

        return tokenString;
    }

    public async Task<string> GenerateRefreshTokenAsync(User user, CancellationToken cancellationToken)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("type", "refresh"),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiration = DateTime.UtcNow.AddDays(7);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: expiration,
            signingCredentials: creds
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = tokenString,
            Expiration = expiration,
            CreatedOnUtc = DateTime.UtcNow
        };

        await refreshTokenRepository.CreateRefreshTokenAsync(refreshToken, cancellationToken);
        await refreshTokenRepository.RevokeOldRefreshTokensAsync(user.Id, refreshToken.Id, cancellationToken);

        logger.LogInformation("Refresh token generated for user {UserId}", user.Id);

        return tokenString;
    }

    public async Task<ResultT<TokenResponseDto>> RefreshTokenAsync(string refreshToken,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(refreshToken))
            return ResultT<TokenResponseDto>.Failure(
                Error.Unauthorized("401", "Refresh token is missing. Please log in again.")
            );

        var handler = new JwtSecurityTokenHandler();
        handler.ValidateToken(refreshToken, new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = _jwtSettings.Issuer,
            ValidAudience = _jwtSettings.Audience,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key!)),
            ClockSkew = TimeSpan.Zero
        }, out SecurityToken validatedToken);

        var jwt = (JwtSecurityToken)validatedToken;
        if (jwt.Claims.FirstOrDefault(c => c.Type == "type")?.Value != "refresh")
            return ResultT<TokenResponseDto>.Failure(Error.Unauthorized("401",
                "The provided token is not a refresh token."));

        var userId = Guid.Parse(jwt.Subject);

        var isValid = await refreshTokenRepository.IsRefreshTokenValidAsync(userId, refreshToken, cancellationToken);
        if (!isValid)
            return ResultT<TokenResponseDto>.Failure(Error.Unauthorized("401",
                "Refresh token is invalid, used, revoked, or expired."));

        var user = await userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null)
            return ResultT<TokenResponseDto>.Failure(Error.NotFound("404", "User not found."));

        var newAccessToken = await GenerateAccessTokenAsync(user, cancellationToken);
        var newRefreshToken = await GenerateRefreshTokenAsync(user, cancellationToken);
        await refreshTokenRepository.MarkRefreshTokenAsUsedAsync(refreshToken, cancellationToken);

        logger.LogInformation("Refresh token used and new access token issued for user {UserId}", user.Id);

        return ResultT<TokenResponseDto>.Success(new TokenResponseDto(newAccessToken, newRefreshToken));
    }
}