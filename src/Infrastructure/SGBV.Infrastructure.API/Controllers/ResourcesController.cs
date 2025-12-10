using Microsoft.AspNetCore.Mvc;
using SGBV.Application.DTOs;
using SGBV.Application.Interfaces.Services;
using SGBV.Application.Utilities;
using SGBV.Domain.Enum;

namespace SGBV.Infrastructure.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ResourceController(IResourceService resourceService) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<ResultT<ResourceDto>> GetById([FromRoute] Guid id, CancellationToken cancellationToken) =>
        await resourceService.GetResourceByIdAsync(id, cancellationToken);

    [HttpPost]
    public async Task<ResultT<ResourceDto>> Create([FromForm] ResourceRequestDto resourceDto,
        CancellationToken cancellationToken) =>
        await resourceService.CreateResourceAsync(resourceDto, cancellationToken);

    [HttpPut("{id}")]
    public async Task<ResultT<ResourceDto>> Update([FromRoute] Guid id, [FromForm] ResourceRequestDto resourceDto,
        CancellationToken cancellationToken) =>
        await resourceService.UpdateResourceAsync(id, resourceDto, cancellationToken);

    [HttpDelete("{id}")]
    public async Task<ResultT<ResponseDto>> Delete([FromRoute] Guid id, CancellationToken cancellationToken) =>
        await resourceService.DeleteResourceAsync(id, cancellationToken);

    [HttpGet]
    public async Task<ResultT<PagedResult<ResourceDto>>> Search(
        [FromQuery] string? title,
        [FromQuery] string? author,
        [FromQuery] string? genre,
        [FromQuery] short? publicationYear,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 30
    ) =>
        await resourceService.SearchResourcesAsync(title, author, genre, publicationYear, pageNumber, pageSize);
    
    [HttpGet("genres/count")]
    public async Task<ResultT<PagedResult<GenreCountDto>>> GetGenresWithCountPaged(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 30,
        CancellationToken cancellationToken = default)
    {
        return await resourceService.GetGenresWithCountPagedAsync(
            pageNumber,
            pageSize,
            cancellationToken
        );
    }

}