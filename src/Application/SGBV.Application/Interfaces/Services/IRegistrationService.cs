using SGBV.Application.DTOs;
using SGBV.Application.Utilities;
using SGBV.Domain.Enum;

namespace SGBV.Application.Interfaces.Services;

public interface IRegistrationService
{
    Task<ResultT<UserDto>> RegisterAsync(RegisterUserRequestDto request, CancellationToken cancellationToken);

    Task<ResultT<UserDto>> RegisterAdminAsync(RegisterUserRequestDto request,
        CancellationToken cancellationToken) => RegisterWithRoleAsync(request, nameof(Roles.Admin), cancellationToken);

    Task<ResultT<UserDto>> RegisterWithRoleAsync(
        RegisterUserRequestDto request,
        string roleName,
        CancellationToken cancellationToken);
}