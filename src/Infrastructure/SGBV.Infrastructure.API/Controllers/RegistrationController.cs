using Microsoft.AspNetCore.Mvc;
using SGBV.Application.DTOs;
using SGBV.Application.Interfaces.Services;
using SGBV.Application.Utilities;

namespace SGBV.Infrastructure.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RegistrationController(IRegistrationService registrationService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ResultT<RegisterUserDto>> RegisterUser(
        [FromForm] RegisterUserRequestDto request,
        CancellationToken cancellationToken) =>
        await registrationService.RegisterAsync(request, cancellationToken);

    [HttpPost("register-admin")]
    public async Task<ResultT<RegisterUserDto>> RegisterAdmin(
        [FromForm] RegisterUserRequestDto request,
        CancellationToken cancellationToken) =>
        await registrationService.RegisterAdminAsync(request, cancellationToken);
}