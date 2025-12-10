using SGBV.Domain.Common;
using SGBV.Domain.Enum;

namespace SGBV.Application.DTOs;

public sealed record LoanUpdateStatusDto(
    Guid LoanId,
    LoanStatusEnum Status
);
