namespace SGBV.Application.DTOs;

public record LoanExtendDto(
    Guid LoanId,
    DateTime NewDueDate
);