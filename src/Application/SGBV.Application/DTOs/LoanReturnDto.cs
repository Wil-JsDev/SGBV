namespace SGBV.Application.DTOs;

public sealed record LoanReturnDto(
    Guid LoanId,
    DateTime ReturnDate
);