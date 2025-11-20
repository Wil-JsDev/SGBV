using Microsoft.EntityFrameworkCore;
using SGBV.Application.Interfaces.Repositories;
using SGBV.Application.Utilities;
using SGBV.Domain.Common;
using SGBV.Domain.Models;
using SGBV.Infrastructure.Persistence.Context;

namespace SGBV.Infrastructure.Persistence.Repository;

public class ResourceRepository(SgbvContext context)
    : GenericRepository<Resource>(context), IResourceRepository
{

    public async Task UpdateResourceStatusAsync(
        Guid resourceId, string newStatus, CancellationToken cancellationToken)
    {
        var resource = await context.Set<Resource>()
            .FirstOrDefaultAsync(r => r.Id == resourceId, cancellationToken);

        resource.Status = newStatus;
        resource.UpdatedOnUtc = DateTime.UtcNow;

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<PagedResult<Resource>> SearchResourcesAdvancedPagedAsync(
        string? title,
        string? author,
        string? genre,
        short? publicationYear,
        int pageNumber,
        int pageSize)
    {
        title = title?.Trim() ?? string.Empty;
        author = author?.Trim() ?? string.Empty;
        genre = genre?.Trim() ?? string.Empty;

        var query = context.Set<Resource>()
            .AsNoTracking()
            .Where(r =>
                (EF.Functions.Like(r.Title, $"%{title}%") || string.IsNullOrEmpty(title)) &&
                (EF.Functions.Like(r.Author, $"%{author}%") || string.IsNullOrEmpty(author)) &&
                (EF.Functions.Like(r.Genre!, $"%{genre}%") || string.IsNullOrEmpty(genre)) &&
                (!publicationYear.HasValue || r.PublicationYear == publicationYear.Value) &&
                r.Status == ResourcesStatus.Available
            );

        var total = await query.CountAsync();

        var items = await query
            .OrderBy(r => r.Title)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Resource>(items, total, pageNumber, pageSize);
    }
}