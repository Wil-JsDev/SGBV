using Microsoft.AspNetCore.Http;

namespace SGBV.Application.DTOs;

public record RegisterUserRequestDto(
    string Name,
    string Email,
    string Password,
    IFormFile ProfilePhoto
);