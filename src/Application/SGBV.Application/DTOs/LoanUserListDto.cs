namespace SGBV.Application.DTOs;

public sealed record LoanUserListDto(
    Guid Id,
    string UserName,
    ResourceDto Resource,
    DateTime LoanDate,
    DateTime DueDate,
    DateTime? ReturnDate,
    string Status
);
public sealed record ResourceLoanDto(
    Guid ResourceId,
    string ResourceName,
    string CoverUrl
);