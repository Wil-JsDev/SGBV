using Microsoft.Extensions.Logging;
using SGBV.Application.DTOs;
using SGBV.Application.Interfaces.Repositories;
using SGBV.Application.Interfaces.Services;
using SGBV.Application.Utilities;

namespace SGBV.Application.Services;

public class UserService(ILogger<UserService> logger, IUserRepository userRepository, ILoanRepository loanRepository, IResourceRepository resourceRepository)
    : IUserService
{
    public async Task<ResultT<UserDto>> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetUserDetailsAsync(userId, cancellationToken);
        if (user is null)
        {
            logger.LogWarning("User not found with Id {UserId}", userId);
            return ResultT<UserDto>.Failure(
                Error.NotFound("404", "We couldn't find a user with this ID."));
        }

        var userDto = new UserDto(
            user.Id,
            user.Name,
            user.Email,
            user.RegistrationDate,
            user.LoginAt
        );

        logger.LogInformation("User details retrieved successfully for Id {UserId}", userId);
        return ResultT<UserDto>.Success(userDto);
    }

    public async Task<ResultT<ResponseDto>> DeleteUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            logger.LogWarning("Attempted to delete user, but user with Id {UserId} was not found.", userId);
            return ResultT<ResponseDto>.Failure(Error.NotFound("404",
                "We couldn't find the account you want to delete."));
        }

        await userRepository.DeleteAsync(user, cancellationToken);

        logger.LogInformation("User with Id {UserId} marked as deleted.", userId);
        return ResultT<ResponseDto>.Success(new ResponseDto(
            "The account has been removed successfully. You can restore it later if needed."
        ));
    }

    public async Task<ResultT<ResponseDto>> UpdatePasswordAsync(Guid userId, string currentPassword, string newPassword,
        CancellationToken cancellationToken)
    {
        if (userId == Guid.Empty || string.IsNullOrWhiteSpace(currentPassword) ||
            string.IsNullOrWhiteSpace(newPassword))
        {
            logger.LogWarning("Received incomplete request to update password for userId {UserId}.", userId);
            return ResultT<ResponseDto>.Failure(Error.Failure("400",
                "We couldn't update your password because some information is missing."));
        }

        var user = await userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            logger.LogWarning("User not found for password update with Id {UserId}.", userId);
            return ResultT<ResponseDto>.Failure(Error.NotFound("404", "We couldn't find your account."));
        }

        var passwordCheck = BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash);
        if (!passwordCheck)
        {
            logger.LogInformation("Current password is incorrect for userId {UserId}.", userId);
            return ResultT<ResponseDto>.Failure(Error.Failure("400",
                "The current password you entered is incorrect."));
        }

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);
        await userRepository.UpdatePasswordAsync(user, hashedPassword, cancellationToken);

        logger.LogInformation("Password updated successfully for userId {UserId}.", userId);
        return ResultT<ResponseDto>.Success(new ResponseDto("Your password has been updated successfully."));
    }

    public async Task<ResultT<UserDto>> UpdateUserNameAsync(Guid userId, string newName,
        CancellationToken cancellationToken)
    {
        if (userId == Guid.Empty || string.IsNullOrWhiteSpace(newName))
        {
            logger.LogWarning("Received incomplete request to update name for userId {UserId}.", userId);
            return ResultT<UserDto>.Failure(Error.Failure("400",
                "We couldn't update your name because some information is missing."));
        }

        var user = await userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            logger.LogWarning("User not found for name update with Id {UserId}.", userId);
            return ResultT<UserDto>.Failure(Error.NotFound("404", "We couldn't find your account."));
        }

        user.Name = newName;
        await userRepository.UpdateAsync(user, cancellationToken);

        var userDto = new UserDto(
            user.Id,
            user.Name,
            user.Email,
            user.RegistrationDate,
            user.LoginAt
        );

        logger.LogInformation("User name updated successfully for userId {UserId}. New name: {NewName}", userId,
            newName);
        return ResultT<UserDto>.Success(userDto);
    }

    public async Task<UserDashboardCountsDto> GetUserDashboardCountsAsync(Guid userId,
        CancellationToken cancellationToken)
    {
        var totalLoans = await loanRepository
            .GetUserLoanCountAsync(userId, cancellationToken);

        var activeLoans = await loanRepository
            .GetUserBorrowedResourceCountAsync(userId, cancellationToken);

        var overdueLoans = await loanRepository
            .GetUserOverdueLoanCountAsync(userId, cancellationToken);

        return new UserDashboardCountsDto(totalLoans, activeLoans, overdueLoans);
    }
    
    public async Task<AdminDashboardCountsDto> GetAdminDashboardCountsAsync(
        CancellationToken cancellationToken)
    {
        var activeLoans = await loanRepository
            .GetActiveLoanCountAsync(cancellationToken);

        var overdueLoans = await loanRepository
            .GetOverdueLoanCountAsync(cancellationToken);

        var availableResources = await resourceRepository
            .GetAvailableResourceCountAsync(cancellationToken);

        var totalUsers = 
            await userRepository.GetTotalUserCountAsync(cancellationToken);

        return new AdminDashboardCountsDto(activeLoans, overdueLoans, availableResources, totalUsers);
    }
}