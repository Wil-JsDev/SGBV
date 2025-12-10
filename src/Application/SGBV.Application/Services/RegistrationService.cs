using Microsoft.Extensions.Logging;
using SGBV.Application.DTOs;
using SGBV.Application.Interfaces.Repositories;
using SGBV.Application.Interfaces.Services;
using SGBV.Application.Utilities;
using SGBV.Domain.Enum;
using SGBV.Domain.Models;

namespace SGBV.Application.Services;

public class RegistrationService(
    ILogger<RegistrationService> logger,
    IUserRepository userRepository,
    IUserRoleService roleService,
    ICloudinaryService cloudinaryService
) : IRegistrationService
{
    public Task<ResultT<UserDto>> RegisterAsync(RegisterUserRequestDto request,
        CancellationToken cancellationToken) =>
        RegisterWithRoleAsync(request, nameof(Roles.User), cancellationToken);

    public Task<ResultT<UserDto>> RegisterAdminAsync(RegisterUserRequestDto request,
        CancellationToken cancellationToken) =>
        RegisterWithRoleAsync(request, nameof(Roles.Admin), cancellationToken);

    public async Task<ResultT<UserDto>> RegisterWithRoleAsync(
        RegisterUserRequestDto request,
        string roleName,
        CancellationToken cancellationToken)
    {
        if (await userRepository.EmailExistAsync(request.Email, cancellationToken))
        {
            logger.LogInformation("Email already exists: {Email}", request.Email);
            return ResultT<UserDto>.Failure(Error.Conflict("409",
                "This email is already in use. Try logging in or use another email."));
        }

        var role = await roleService.GetRoleByNameAsync(roleName, cancellationToken);
        if (role is null)
        {
            logger.LogWarning("Role '{RoleName}' not found when registering user: {Email}", roleName, request.Email);
            return ResultT<UserDto>.Failure(Error.Failure("400",
                $"Something went wrong: the role does not exist."));
        }

        string profilePhoto = "";
        if (request.ProfilePhoto is not null && request.ProfilePhoto.Length > 0)
        {
            await using var stream = request.ProfilePhoto.OpenReadStream();
            profilePhoto =
                await cloudinaryService.UploadImageCloudinaryAsync(stream, request.ProfilePhoto.FileName,
                    cancellationToken);
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            ProfileUrl = profilePhoto,
            RegistrationDate = DateTime.UtcNow,
            RolId = role.Id,
            Rol = role
        };

        await userRepository.CreateAsync(user, cancellationToken);
        logger.LogInformation("User {UserId} created successfully with email {Email} and role {Role}.", user.Id,
            user.Email, roleName);

        var userInfo = await userRepository.GetByIdAsync(user.Id, cancellationToken);
        if (userInfo is null)
        {
            logger.LogWarning("User {UserId} was created but could not be retrieved from the database.", user.Id);
            return ResultT<UserDto>.Failure(Error.Failure("400",
                "The user was created but could not be retrieved. Please try again."));
        }

        var userDto = new UserDto(
            Id: userInfo.Id,
            Name: userInfo.Name,
            Email: userInfo.Email,
            RegistrationDate: userInfo.RegistrationDate,
            ProfileUrl: userInfo.ProfileUrl ?? String.Empty
        );

        logger.LogInformation("User registration completed successfully for {UserId}.", userInfo.Id);
        return ResultT<UserDto>.Success(userDto);
    }
}