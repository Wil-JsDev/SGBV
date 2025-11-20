namespace SGBV.Application.DTOs;

public record RegisterUserDto(
    Guid Id,
    string Name,
    string Email,
    DateTime RegistrationDate,
    Guid RolId
);