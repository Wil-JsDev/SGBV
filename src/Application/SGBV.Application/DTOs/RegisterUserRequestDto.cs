namespace SGBV.Application.DTOs;

public record RegisterUserRequestDto(
    string Name,
    string Email,
    string Password
);