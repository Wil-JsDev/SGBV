using SGBV.Application.DTOs;
using SGBV.Application.Utilities;
using SGBV.Domain.Enum;

namespace SGBV.Application.Interfaces.Services;

public interface IRegistrationService
{
    Task<ResultT<RegisterUserDto>> RegisterAsync(RegisterUserRequestDto request, CancellationToken cancellationToken);

    Task<ResultT<RegisterUserDto>> RegisterAdminAsync(RegisterUserRequestDto request,
        CancellationToken cancellationToken) => RegisterWithRoleAsync(request, nameof(Roles.Admin), cancellationToken);

    Task<ResultT<RegisterUserDto>> RegisterWithRoleAsync(
        RegisterUserRequestDto request,
        string roleName,
        CancellationToken cancellationToken);
}