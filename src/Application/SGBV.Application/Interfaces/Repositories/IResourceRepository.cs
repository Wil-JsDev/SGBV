using SGBV.Application.DTOs;
using SGBV.Application.Utilities;
using SGBV.Domain.Models;

namespace SGBV.Application.Interfaces.Repositories;

/// <summary>
/// Defines the contract for managing operations specific to the Resource entity (e.g., books, articles).
/// </summary>
public interface IResourceRepository : IGenericRepository<Resource>
{
    /// <summary>
    /// Asynchronously updates the status of a resource (e.g., from 'Available' to 'Maintenance').
    /// </summary>
    /// <param name="resourceId">The unique identifier of the resource.</param>
    /// <param name="newStatus">The new status to set (e.g., 'Available', 'Loaned', 'Maintenance').</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    Task UpdateResourceStatusAsync(Guid resourceId, string newStatus, CancellationToken cancellationToken);

    Task<PagedResult<Resource>> SearchResourcesAdvancedPagedAsync(
        string? title,
        string? author,
        string? genre,
        short? publicationYear,
        int pageNumber,
        int pageSize);

    Task<int> GetAvailableResourceCountAsync(CancellationToken cancellationToken);

    Task<PagedResult<GenreCountDto>> GetGenresWithCountPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);
}