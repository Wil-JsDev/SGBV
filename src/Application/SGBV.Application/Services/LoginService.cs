using Microsoft.Extensions.Logging;
using SGBV.Application.DTOs;
using SGBV.Application.Interfaces.Repositories;
using SGBV.Application.Interfaces.Services;
using SGBV.Application.Utilities;

namespace SGBV.Application.Services;

public class LoginService(
    ILogger<LoginService> logger,
    IUserRepository userRepository,
    IAuthService authenticationService
) : ILoginService
{
    public async Task<ResultT<TokenResponseDto>> LoginAsync(string email, string password, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            logger.LogWarning("Login attempt with empty email or password.");
            return ResultT<TokenResponseDto>.Failure(
                Error.Failure("400", "Email or password cannot be empty.")
            );
        }

        var user = await userRepository.GetByEmailAsync(email, cancellationToken);
        if (user is null)
        {
            logger.LogWarning("User with email {Email} not found.", email);
            return ResultT<TokenResponseDto>.Failure(
                Error.Failure("404", "Account not found with the provided email.")
            );
        }

        if (user.IsDeleted)
        {
            logger.LogWarning("User {UserId} account is deactivated.", user.Id);
            return ResultT<TokenResponseDto>.Failure(
                Error.Failure("403", "Your account is deactivated. Please contact support.")
            );
        }

        if (string.IsNullOrEmpty(user.PasswordHash))
        {
            logger.LogWarning("User {UserId} tried to login with email/password but registered via external provider.", user.Id);
            return ResultT<TokenResponseDto>.Failure(
                Error.Failure("400", "You signed up using an external provider. Please login using that method.")
            );
        }

        var isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        if (!isPasswordValid)
        {
            logger.LogWarning("Invalid password for user {UserId}.", user.Id);
            return ResultT<TokenResponseDto>.Failure(
                Error.Failure("401", "Incorrect password. Please try again.")
            );
        }

        user.LoginAt = DateTime.UtcNow;
        await userRepository.UpdateAsync(user, cancellationToken);

        var accessToken = await authenticationService.GenerateAccessTokenAsync(user, cancellationToken);
        var refreshToken = await authenticationService.GenerateRefreshTokenAsync(user, cancellationToken);

        logger.LogInformation("User {UserId} logged in successfully. Access token and refresh token generated.", user.Id);

        return ResultT<TokenResponseDto>.Success(new TokenResponseDto(accessToken, refreshToken));
    }
}
