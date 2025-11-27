namespace SGBV.Application.DTOs;

public sealed record LoanUserListDto(
    Guid Id,
    ResourceLoanDto Resource,
    DateTime LoanDate,
    DateTime DueDate,
    DateTime? ReturnDate,
    string Status
);
public sealed record ResourceLoanDto(
    Guid ResourceId,
    string ResourceName
);