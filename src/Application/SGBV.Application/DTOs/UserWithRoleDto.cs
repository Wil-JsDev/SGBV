namespace SGBV.Application.DTOs;

public record UserWithRoleDto(
    Guid Id,
    string Name,
    string Email,
    string ProfileUrl,
    string? Rol,
    DateTime RegistrationDate,
    DateTime? LoginAt = null
    );