using SGBV.Application.DTOs;
using SGBV.Application.Utilities;

namespace SGBV.Application.Interfaces.Services;

public interface IUserService
{
    Task<ResultT<UserDto>> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<ResultT<ResponseDto>> DeleteUserAsync(Guid userId, CancellationToken cancellationToken);

    Task<ResultT<ResponseDto>> UpdatePasswordAsync(Guid userId, string currentPassword, string newPassword,
        CancellationToken cancellationToken);

    Task<ResultT<UserDto>> UpdateUserNameAsync(Guid userId, string newName,
        CancellationToken cancellationToken);

    Task<UserDashboardCountsDto> GetUserDashboardCountsAsync(Guid userId,
        CancellationToken cancellationToken);

    Task<AdminDashboardCountsDto> GetAdminDashboardCountsAsync(
        CancellationToken cancellationToken);

    Task<ResultT<PagedResult<RegisterUserDto>>> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
}