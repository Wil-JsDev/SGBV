namespace SGBV.Application.DTOs;

public record ResourceRequestDto(
    string Title,
    string Author,
    string? Genre,
    short? PublicationYear,
    string? Description
    );