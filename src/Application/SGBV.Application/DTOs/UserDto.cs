namespace SGBV.Application.DTOs;

public record UserDto(
    Guid Id,
    string Name,
    string Email,
    string ProfileUrl,
    DateTime RegistrationDate,
    DateTime? LoginAt = null
);