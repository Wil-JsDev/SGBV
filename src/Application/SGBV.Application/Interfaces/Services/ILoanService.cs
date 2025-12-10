using SGBV.Application.DTOs;
using SGBV.Application.Utilities;

namespace SGBV.Application.Interfaces.Services;

public interface ILoanService
{
    Task<ResultT<LoanResponseDto>>
        CreateLoanAsync(LoanCreateDto loanDto, CancellationToken cancellationToken = default);

    Task<ResultT<LoanReturnDto>> ReturnLoanAsync(LoanUpdateStatusDto loanUpdateStatus,
        CancellationToken cancellationToken = default);

    Task<ResultT<ResponseDto>> DeleteLoanAsync(Guid loanId, CancellationToken cancellationToken = default);

    Task<ResultT<PagedResult<LoanUserListDto>>> GetPagedLoanListAsync(Guid userId, int pageNumber, int pageSize,
        CancellationToken cancellationToken = default);

    Task<ResultT<LoanResponseDto>> GetByIdAsync(Guid loanId, CancellationToken cancellationToken = default);

    Task<ResultT<LoanResponseDto>> ExtendLoanAsync(LoanExtendDto dto, CancellationToken cancellationToken = default);

    Task<ResultT<PagedResult<LoanUserListDto>>> GetPagedLoanListAsync(int pageNumber,
        int pageSize, CancellationToken cancellationToken = default);
}