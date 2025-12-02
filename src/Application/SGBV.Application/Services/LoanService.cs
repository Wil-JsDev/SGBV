using SGBV.Application.DTOs;
using SGBV.Application.Interfaces.Repositories;
using SGBV.Application.Interfaces.Services;
using SGBV.Application.Utilities;
using SGBV.Domain.Common;
using SGBV.Domain.Models;

namespace SGBV.Application.Services;

public class LoanService(
    ILoanRepository loanRepository,
    IUserRepository userRepository,
    IResourceRepository resourceRepository) : ILoanService
{
    public async Task<ResultT<LoanResponseDto>> CreateLoanAsync(LoanCreateDto loanDto,
        CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(loanDto.UserId, cancellationToken);
        if (user is null)
        {
            return ResultT<LoanResponseDto>.Failure(Error.NotFound("404", "User not found."));
        }

        var resource = await resourceRepository.GetByIdAsync(loanDto.ResourceId, cancellationToken);
        if (resource is null)
        {
            return ResultT<LoanResponseDto>.Failure(Error.NotFound("404", "Resource not found."));
        }

        Loan loan = new()
        {
            Id = Guid.NewGuid(),
            UserId = loanDto.UserId,
            ResourceId = loanDto.ResourceId,
            DueDate = loanDto.DueDate,
            Status = LoanStatus.Active
        };

        await loanRepository.CreateAsync(loan, cancellationToken);

        var response = new LoanResponseDto(
            Id: loan.Id,
            UserId: loan.UserId,
            ResourceId: loan.ResourceId,
            LoanDate: loan.LoanDate,
            DueDate: loan.DueDate,
            ReturnDate: loan.ReturnDate,
            Status: loan.Status
        );

        return ResultT<LoanResponseDto>.Success(response);
    }

    public async Task<ResultT<LoanReturnDto>> ReturnLoanAsync(LoanUpdateStatusDto loanUpdateStatus,
        CancellationToken cancellationToken = default)
    {
        var loan = await loanRepository.GetByIdAsync(loanUpdateStatus.LoanId, cancellationToken);
        if (loan is null)
        {
            return ResultT<LoanReturnDto>.Failure(
                Error.NotFound("404", "Loan not found.")
            );
        }

        if (loan.ReturnDate is not null)
        {
            return ResultT<LoanReturnDto>.Failure(
                Error.Failure("400", "This loan has already been returned.")
            );
        }

        loan.Status = loanUpdateStatus.Status.ToString();
        loan.ReturnDate = DateTime.UtcNow;

        await loanRepository.UpdateAsync(loan, cancellationToken);

        var response = new LoanReturnDto(
            LoanId: loan.Id,
            ReturnDate: loan.ReturnDate.Value
        );

        return ResultT<LoanReturnDto>.Success(response);
    }

    public async Task<ResultT<ResponseDto>> DeleteLoanAsync(Guid loanId, CancellationToken cancellationToken = default)
    {
        var loan = await loanRepository.GetByIdAsync(loanId, cancellationToken);
        if (loan is null)
            return ResultT<ResponseDto>.Failure(
                Error.NotFound("404", "Loan not found.")
            );

        await loanRepository.DeleteAsync(loan, cancellationToken);

        return ResultT<ResponseDto>.Success(new ResponseDto("Loan deleted successfully."));
    }

    public async Task<ResultT<PagedResult<LoanUserListDto>>> GetPagedLoanListAsync(Guid userId, int pageNumber,
        int pageSize, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            return ResultT<PagedResult<LoanUserListDto>>.Failure(Error.NotFound("404", "User not found."));
        }

        if (pageNumber <= 0 || pageSize <= 0)
        {
            return ResultT<PagedResult<LoanUserListDto>>.Failure(Error.Failure("400",
                "Page number and page size must be greater than zero."));
        }

        var items = await loanRepository.GetLoansByUserIdPagedAsync(userId, pageNumber, pageSize);

        var itemsDto = items.Items.Select(loan => new LoanUserListDto
        (
            Id: loan.Id,
            UserId: user.Id,
            UserName: user.Name,
            Resource: new ResourceDto(loan.ResourceId, loan.Resource.Title, loan.Resource.Author, loan.Resource.Genre,
                loan.Resource.PublicationYear, loan.Resource.CoverUrl, loan.Resource.Description,
                loan.Resource.ResourceStatus, loan.Resource.Status),
            LoanDate: loan.LoanDate,
            DueDate: loan.DueDate,
            ReturnDate: loan.ReturnDate,
            Status: loan.Status
        ));

        IEnumerable<LoanUserListDto> loanUserListDtos = itemsDto as LoanUserListDto[] ?? itemsDto.ToArray();

        if (!loanUserListDtos.Any())
        {
            return ResultT<PagedResult<LoanUserListDto>>.Success(
                new PagedResult<LoanUserListDto>([], items.TotalItems, pageNumber,
                    pageSize));
        }

        return ResultT<PagedResult<LoanUserListDto>>.Success(
            new PagedResult<LoanUserListDto>(loanUserListDtos, items.TotalItems, pageNumber, pageSize));
    }
    
    public async Task<ResultT<PagedResult<LoanUserListDto>>> GetPagedLoanListAsync(int pageNumber,
        int pageSize, CancellationToken cancellationToken = default)
    {
        if (pageNumber <= 0 || pageSize <= 0)
        {
            return ResultT<PagedResult<LoanUserListDto>>.Failure(Error.Failure("400",
                "Page number and page size must be greater than zero."));
        }

        var items = await loanRepository.GetLoansPagedAsync(pageNumber, pageSize);

        var itemsDto = items.Items.Select(loan => new LoanUserListDto
        (
            Id: loan.Id,
            UserId: loan.UserId,
            UserName: loan.User.Name,
            Resource: new ResourceDto(loan.ResourceId, loan.Resource.Title, loan.Resource.Author, loan.Resource.Genre,
                loan.Resource.PublicationYear, loan.Resource.CoverUrl, loan.Resource.Description,
                loan.Resource.ResourceStatus, loan.Resource.Status),
            LoanDate: loan.LoanDate,
            DueDate: loan.DueDate,
            ReturnDate: loan.ReturnDate,
            Status: loan.Status
        ));

        IEnumerable<LoanUserListDto> loanUserListDtos = itemsDto as LoanUserListDto[] ?? itemsDto.ToArray();

        if (!loanUserListDtos.Any())
        {
            return ResultT<PagedResult<LoanUserListDto>>.Success(
                new PagedResult<LoanUserListDto>([], items.TotalItems, pageNumber,
                    pageSize));
        }

        return ResultT<PagedResult<LoanUserListDto>>.Success(
            new PagedResult<LoanUserListDto>(loanUserListDtos, items.TotalItems, pageNumber, pageSize));
    }

    public async Task<ResultT<LoanResponseDto>> GetByIdAsync(Guid loanId, CancellationToken cancellationToken = default)
    {
        var loan = await loanRepository.GetByIdAsync(loanId, cancellationToken);
        if (loan is null)
        {
            return ResultT<LoanResponseDto>.Failure(Error.NotFound("404", "Loan not found."));
        }

        var response = new LoanResponseDto(
            Id: loan.Id,
            UserId: loan.UserId,
            ResourceId: loan.ResourceId,
            LoanDate: loan.LoanDate,
            DueDate: loan.DueDate,
            ReturnDate: loan.ReturnDate,
            Status: loan.Status
        );

        return ResultT<LoanResponseDto>.Success(response);
    }

    public async Task<ResultT<LoanResponseDto>> ExtendLoanAsync(
        LoanExtendDto dto,
        CancellationToken cancellationToken = default)
    {
        var loan = await loanRepository.GetByIdAsync(dto.LoanId, cancellationToken);
        if (loan is null)
        {
            return ResultT<LoanResponseDto>.Failure(
                Error.NotFound("404", "Loan not found.")
            );
        }

        if (loan.ReturnDate is not null)
        {
            return ResultT<LoanResponseDto>.Failure(
                Error.Failure("400", "This loan has already been returned and cannot be extended.")
            );
        }

        if (dto.NewDueDate <= loan.DueDate)
        {
            return ResultT<LoanResponseDto>.Failure(
                Error.Failure("400", "The new due date must be greater than the current due date.")
            );
        }

        loan.DueDate = dto.NewDueDate;
        loan.UpdatedOnUtc = DateTime.UtcNow;

        await loanRepository.UpdateAsync(loan, cancellationToken);

        var response = new LoanResponseDto(
            Id: loan.Id,
            UserId: loan.UserId,
            ResourceId: loan.ResourceId,
            LoanDate: loan.LoanDate,
            DueDate: loan.DueDate,
            ReturnDate: loan.ReturnDate,
            Status: loan.Status
        );

        return ResultT<LoanResponseDto>.Success(response);
    }
}