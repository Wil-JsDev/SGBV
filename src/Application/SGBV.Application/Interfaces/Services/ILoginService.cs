using SGBV.Application.DTOs;
using SGBV.Application.Utilities;

namespace SGBV.Application.Interfaces.Services;

public interface ILoginService
{
    Task<ResultT<TokenResponseDto>> LoginAsync(string email, string password, CancellationToken cancellationToken);
}