using Microsoft.AspNetCore.Http;
using SGBV.Domain.Common;
using SGBV.Domain.Enum;

namespace SGBV.Application.DTOs;

public record ResourceRequestDto(
    string Title,
    string Author,
    string? Genre,
    IFormFile? CoverUrl,
    short? PublicationYear,
    string? Description,
    ResourceStatus? Status
    );