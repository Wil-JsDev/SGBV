using Microsoft.AspNetCore.Http;

namespace SGBV.Application.DTOs;

public record ResourceRequestDto(
    string Title,
    string Author,
    string? Genre,
    IFormFile? CoverUrl,
    short? PublicationYear,
    string? Description
    );