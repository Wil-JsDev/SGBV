using Microsoft.Extensions.Logging;
using SGBV.Application.DTOs;
using SGBV.Application.Interfaces.Repositories;
using SGBV.Application.Interfaces.Services;
using SGBV.Application.Utilities;
using SGBV.Domain.Common;
using SGBV.Domain.Enum;
using SGBV.Domain.Models;

namespace SGBV.Application.Services;

public class ResourceService(
    ILogger<ResourceService> logger,
    IResourceRepository resourceRepository,
    IGenericRepository<Resource> genericRepository,
    ICloudinaryService cloudinaryService
) : IResourceService
{
    public async Task<ResultT<ResourceDto>> GetResourceByIdAsync(Guid resourceId, CancellationToken cancellationToken)
    {
        var resource = await resourceRepository.GetByIdAsync(resourceId, cancellationToken);
        if (resource is null)
        {
            logger.LogWarning("Resource not found with Id {ResourceId}", resourceId);
            return ResultT<ResourceDto>.Failure(Error.NotFound("404", "We couldn't find this resource."));
        }

        var dto = new ResourceDto(
            resource.Id,
            resource.Title,
            resource.Author,
            resource.Genre,
            resource.PublicationYear,
            resource.CoverUrl,
            resource.Description,
            resource.ResourceStatus,
            resource.Status
        );

        logger.LogInformation("Resource details retrieved successfully for Id {ResourceId}", resourceId);
        return ResultT<ResourceDto>.Success(dto);
    }

    public async Task<ResultT<ResourceDto>> CreateResourceAsync(ResourceRequestDto resourceDto,
        CancellationToken cancellationToken)
    {
        string cover = "";
        if (resourceDto.CoverUrl is not null)
        {
            await using var stream = resourceDto.CoverUrl.OpenReadStream();
            cover = await cloudinaryService.UploadImageCloudinaryAsync(stream, resourceDto.CoverUrl.FileName,
                cancellationToken);
        }

        var resource = new Resource
        {
            Id = Guid.NewGuid(),
            Title = resourceDto.Title,
            Author = resourceDto.Author,
            Genre = resourceDto.Genre,
            PublicationYear = resourceDto.PublicationYear,
            CoverUrl = cover,
            Description = resourceDto.Description,
            Status = ResourcesStatus.Available
        };

        await genericRepository.CreateAsync(resource, cancellationToken);

        logger.LogInformation("Resource created successfully with Id {ResourceId}", resource.Id);

        var dto = new ResourceDto(
            resource.Id,
            resource.Title,
            resource.Author,
            resource.Genre,
            resource.PublicationYear,
            resource.CoverUrl,
            resource.Description,
            resource.ResourceStatus,
            resource.Status
        );

        return ResultT<ResourceDto>.Success(dto);
    }

    public async Task<ResultT<ResponseDto>> DeleteResourceAsync(Guid resourceId, CancellationToken cancellationToken)
    {
        var resource = await resourceRepository.GetByIdAsync(resourceId, cancellationToken);
        if (resource is null)
        {
            logger.LogWarning("Attempted to delete resource with Id {ResourceId}, but it was not found.", resourceId);
            return ResultT<ResponseDto>.Failure(Error.NotFound("404", "We couldn't find this resource."));
        }

        await genericRepository.DeleteAsync(resource, cancellationToken);

        logger.LogInformation("Resource deleted successfully with Id {ResourceId}", resourceId);

        return ResultT<ResponseDto>.Success(new ResponseDto("The resource has been deleted successfully."));
    }


    public async Task<ResultT<ResourceDto>> UpdateResourceAsync(Guid resourceId, ResourceRequestDto updatedDto,
        CancellationToken cancellationToken)
    {
        var resource = await resourceRepository.GetByIdAsync(resourceId, cancellationToken);
        if (resource is null)
        {
            logger.LogWarning("Attempted to update resource with Id {ResourceId}, but it was not found.", resourceId);
            return ResultT<ResourceDto>.Failure(Error.NotFound("404", "We couldn't find this resource."));
        }
        
        string cover = resource?.CoverUrl;
        if (updatedDto.CoverUrl is not null && updatedDto.CoverUrl.Length > 0)
        {
            await using var stream = updatedDto.CoverUrl.OpenReadStream();
            cover = await cloudinaryService.UploadImageCloudinaryAsync(stream, updatedDto.CoverUrl.FileName,
                cancellationToken);
        }

        resource.Title = updatedDto.Title;
        resource.Author = updatedDto.Author;
        resource.Genre = updatedDto.Genre;
        resource.PublicationYear = updatedDto.PublicationYear;
        resource.Description = updatedDto.Description;
        resource.CoverUrl = cover;

        await genericRepository.UpdateAsync(resource, cancellationToken);

        logger.LogInformation("Resource updated successfully with Id {ResourceId}", resource.Id);

        var dto = new ResourceDto(
            resource.Id,
            resource.Title,
            resource.Author,
            resource.Genre,
            resource.PublicationYear,
            resource.CoverUrl,
            resource.Description,
            resource.ResourceStatus,
            resource.Status
        );

        return ResultT<ResourceDto>.Success(dto);
    }

    public async Task<ResultT<PagedResult<ResourceDto>>> SearchResourcesAsync(
        string? title = null,
        string? author = null,
        string? genre = null,
        short? publicationYear = null,
        int pageNumber = 1,
        int pageSize = 30)
    {
        var resourcesPaged = await resourceRepository.SearchResourcesAdvancedPagedAsync(
            title, author, genre, publicationYear, pageNumber, pageSize);

        if (!resourcesPaged.Items.Any())
        {
            logger.LogInformation("No resources found matching the filters.");
            return ResultT<PagedResult<ResourceDto>>.Success(
                new PagedResult<ResourceDto>(Enumerable.Empty<ResourceDto>(), resourcesPaged.TotalItems, pageNumber,
                    resourcesPaged.TotalPages));
        }

        var resourceDtos = resourcesPaged.Items.Select(r => new ResourceDto(
            r.Id,
            r.Title,
            r.Author,
            r.Genre,
            r.PublicationYear,
            r.CoverUrl,
            r.Description,
            r.ResourceStatus,
            r.Status
        )).ToList();

        logger.LogInformation("Successfully retrieved {Count} resources.", resourceDtos.Count);
        return ResultT<PagedResult<ResourceDto>>.Success(
            new PagedResult<ResourceDto>(resourceDtos, resourcesPaged.TotalItems, pageNumber,
                resourcesPaged.TotalPages));
    }

    public async Task<ResultT<ResponseDto>> UpdateResourceStatusAsync(Guid resourceId,
        UpdateResourceStatusDto newStatus,
        CancellationToken cancellationToken)
    {
        var resource = await resourceRepository.GetByIdAsync(resourceId, cancellationToken);
        if (resource is null)
        {
            logger.LogWarning("Attempted to update status, but resource with Id {ResourceId} was not found.",
                resourceId);
            return ResultT<ResponseDto>.Failure(Error.NotFound("404", "We couldn't find this resource."));
        }

        await resourceRepository.UpdateResourceStatusAsync(resourceId, newStatus.ToString(), cancellationToken);

        logger.LogInformation("Resource status updated successfully for Id {ResourceId} to '{Status}'.", resourceId,
            newStatus);
        return ResultT<ResponseDto>.Success(
            new ResponseDto($"The status has been updated successfully."));
    }

    public async Task<ResultT<PagedResult<GenreCountDto>>> GetGenresWithCountPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var result = await resourceRepository.GetGenresWithCountPagedAsync(
            pageNumber,
            pageSize,
            cancellationToken);

        if (!result.Items.Any())
        {
            logger.LogInformation("No genres found.");
            return ResultT<PagedResult<GenreCountDto>>.Success(
                new PagedResult<GenreCountDto>(
                    Enumerable.Empty<GenreCountDto>(), 
                    result.TotalItems,
                    pageNumber,
                    result.TotalPages
                )
            );
        }

        logger.LogInformation("Successfully retrieved genres with counts: {Count}", result.Items.Count());

        return ResultT<PagedResult<GenreCountDto>>.Success(result);
    }

}