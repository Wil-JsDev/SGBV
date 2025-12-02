using Microsoft.AspNetCore.Mvc;
using SGBV.Application.DTOs;
using SGBV.Application.Interfaces.Services;
using SGBV.Application.Utilities;

namespace SGBV.Infrastructure.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoanController(ILoanService service) : ControllerBase
{
    [HttpPost]
    public async Task<ResultT<LoanResponseDto>> CreateLoan([FromBody] LoanCreateDto loanDto) =>
        await service.CreateLoanAsync(loanDto);

    [HttpPut]
    public async Task<ResultT<LoanReturnDto>> ReturnLoan([FromBody] LoanUpdateStatusDto loanDto) =>
        await service.ReturnLoanAsync(loanDto);

    [HttpDelete("{id:guid}")]
    public async Task<ResultT<ResponseDto>> DeleteLoan([FromRoute] Guid id) =>
        await service.DeleteLoanAsync(id);

    [HttpGet("paged/{userId:guid}")]
    public async Task<ResultT<PagedResult<LoanUserListDto>>> GetPagedLoanListByUserId([FromRoute] Guid userId,
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize) =>
        await service.GetPagedLoanListAsync(userId, pageNumber, pageSize);
    
    [HttpGet("paged")]
    public async Task<ResultT<PagedResult<LoanUserListDto>>> GetPagedLoanList([FromQuery] int pageNumber,
        [FromQuery] int pageSize) =>
        await service.GetPagedLoanListAsync(pageNumber, pageSize);

    [HttpGet("{loanId:guid}")]
    public async Task<ResultT<LoanResponseDto>> GetById([FromRoute] Guid loanId) =>
        await service.GetByIdAsync(loanId);
    
    [HttpPut("extend")]
    public async Task<ResultT<LoanResponseDto>> ExtendLoan([FromBody] LoanExtendDto dto) =>
        await service.ExtendLoanAsync(dto);

}