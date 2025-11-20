using Microsoft.AspNetCore.Mvc;
using SGBV.Application.DTOs;
using SGBV.Application.Interfaces.Services;
using SGBV.Application.Utilities;

namespace SGBV.Infrastructure.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
    ILoginService loginService
) : ControllerBase
{
    [HttpPost("login")]
    public async Task<ResultT<TokenResponseDto>> LoginAsync([FromBody] LoginRequestDto request,
        CancellationToken cancellationToken)
        => await loginService.LoginAsync(request.Email, request.Password, cancellationToken);
}