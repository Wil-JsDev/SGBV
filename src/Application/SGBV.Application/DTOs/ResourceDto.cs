namespace SGBV.Application.DTOs;

public record ResourceDto(
    Guid Id,
    string Title,
    string Author,
    string? Genre,
    short? PublicationYear,
    string? CoverUrl,
    string? Description,
    string ResourceStatus,
    string Status
);