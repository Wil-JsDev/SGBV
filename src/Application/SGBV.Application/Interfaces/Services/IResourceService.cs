using SGBV.Application.DTOs;
using SGBV.Application.Utilities;

namespace SGBV.Application.Interfaces.Services;

public interface IResourceService
{
    Task<ResultT<ResourceDto>> GetResourceByIdAsync(Guid resourceId, CancellationToken cancellationToken);

    Task<ResultT<PagedResult<ResourceDto>>> SearchResourcesAsync(
        string? title = null,
        string? author = null,
        string? genre = null,
        short? publicationYear = null,
        int pageNumber = 1,
        int pageSize = 30);
    
    Task<ResultT<ResourceDto>> CreateResourceAsync(ResourceRequestDto resourceDto,
        CancellationToken cancellationToken);

    Task<ResultT<ResponseDto>> DeleteResourceAsync(Guid resourceId, CancellationToken cancellationToken);

    Task<ResultT<ResourceDto>> UpdateResourceAsync(Guid resourceId, ResourceRequestDto updatedDto,
        CancellationToken cancellationToken);

    Task<ResultT<PagedResult<GenreCountDto>>> GetGenresWithCountPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);
}