using Microsoft.AspNetCore.Mvc;
using SGBV.Application.DTOs;
using SGBV.Application.Interfaces.Services;
using SGBV.Application.Utilities;

namespace SGBV.Infrastructure.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(IUserService userService, IRegistrationService registrationService) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<ResultT<UserDto>> GetById([FromRoute] Guid id, CancellationToken cancellationToken) =>
        await userService.GetUserByIdAsync(id, cancellationToken);

    [HttpDelete("{id}")]
    public async Task<ResultT<ResponseDto>> Delete([FromRoute] Guid id, CancellationToken cancellationToken) =>
        await userService.DeleteUserAsync(id, cancellationToken);

    [HttpPatch("{id}/password")]
    public async Task<ResultT<ResponseDto>> UpdatePassword(
        [FromRoute] Guid id,
        [FromBody] UpdatePasswordDto updatePasswordDto,
        CancellationToken cancellationToken) =>
        await userService.UpdatePasswordAsync(id, updatePasswordDto.CurrentPassword, updatePasswordDto.NewPassword,
            cancellationToken);

    [HttpPatch("{id}/name")]
    public async Task<ResultT<UserDto>> UpdateName(
        [FromRoute] Guid id,
        [FromBody] UpdateNameDto updateName,
        CancellationToken cancellationToken) =>
        await userService.UpdateUserNameAsync(id, updateName.NewName, cancellationToken);

    [HttpGet("{id}/dashboard")]
    public async Task<ResultT<UserDashboardCountsDto>> GetUserDashboard(
        [FromRoute] Guid id,
        CancellationToken cancellationToken) =>
        await userService.GetUserDashboardCountsAsync(id, cancellationToken);


    [HttpGet("admin/dashboard")]
    public async Task<ResultT<AdminDashboardCountsDto>> GetAdminDashboard(
        CancellationToken cancellationToken) =>
        await userService.GetAdminDashboardCountsAsync(cancellationToken);
    
    [HttpGet("users")]
    public async Task<ResultT<PagedResult<UserDto>>> GetAllUsers([FromQuery] int pageNumber, [FromQuery] int pageSize, CancellationToken cancellationToken) =>
        await userService.GetAllAsync(pageNumber, pageSize, cancellationToken);
}