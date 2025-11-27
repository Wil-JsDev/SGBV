namespace SGBV.Application.DTOs;

public sealed record LoanResponseDto(
    Guid Id,
    Guid UserId,
    Guid ResourceId,
    DateTime LoanDate,
    DateTime DueDate,
    DateTime? ReturnDate,
    string Status
);
