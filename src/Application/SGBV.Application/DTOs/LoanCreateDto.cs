using Microsoft.AspNetCore.Http;

namespace SGBV.Application.DTOs;

public sealed record LoanCreateDto(
    Guid UserId,
    Guid ResourceId,
    DateTime DueDate
);
