using Microsoft.EntityFrameworkCore;
using SGBV.Application.Interfaces.Repositories;
using SGBV.Application.Utilities;
using SGBV.Domain.Common;
using SGBV.Domain.Models;
using SGBV.Infrastructure.Persistence.Context;

namespace SGBV.Infrastructure.Persistence.Repository;

public class LoanRepository(SgbvContext context)
    : GenericRepository<Loan>(context), ILoanRepository
{
    public async Task<PagedResult<Loan>> GetActiveLoansPagedAsync(
        int pageNumber, int pageSize)
    {
        var query = context.Set<Loan>()
            .AsNoTracking()
            .Where(l => l.ReturnDate == null && l.Status == LoanStatus.Active);

        var total = await query.CountAsync();

        var items = await query
            .OrderBy(l => l.DueDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Loan>(items, total, pageNumber, pageSize);
    }

    public async Task<PagedResult<Loan>> GetLoansByUserIdPagedAsync(
        Guid userId, int pageNumber, int pageSize)
    {
        var query = context.Set<Loan>()
            .AsNoTracking()
            .Include(l => l.Resource)
            .Where(l => l.UserId == userId);

        var total = await query.CountAsync();

        var items = await query
            .OrderByDescending(l => l.LoanDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Loan>(items, total, pageNumber, pageSize);
    }

    public async Task<PagedResult<Loan>> GetOverdueLoansPagedAsync(
        int pageNumber, int pageSize)
    {
        var now = DateTime.UtcNow;

        var query = context.Set<Loan>()
            .AsNoTracking()
            .Where(l => l.ReturnDate == null && l.DueDate < now);

        var total = await query.CountAsync();

        var items = await query
            .OrderBy(l => l.DueDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Loan>(items, total, pageNumber, pageSize);
    }

    public async Task<PagedResult<Loan>> SearchLoansPagedAsync(
        string searchQuery, int pageNumber, int pageSize)
    {
        searchQuery = searchQuery?.Trim() ?? string.Empty;

        var query = context.Set<Loan>()
            .AsNoTracking()
            .Include(l => l.User)
            .Include(l => l.Resource)
            .Where(l =>
                EF.Functions.Like(l.User.Name, $"%{searchQuery}%") ||
                EF.Functions.Like(l.Resource.Title, $"%{searchQuery}%") ||
                EF.Functions.Like(l.Id.ToString(), $"%{searchQuery}%")
            );

        var total = await query.CountAsync();

        var items = await query
            .OrderByDescending(l => l.LoanDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Loan>(items, total, pageNumber, pageSize);
    }

    public async Task<PagedResult<Loan>> GetHistoricalLoansPagedAsync(
        DateTime startDate, DateTime endDate,
        int pageNumber, int pageSize)
    {
        var query = context.Set<Loan>()
            .AsNoTracking()
            .Where(l => l.ReturnDate != null &&
                        l.ReturnDate >= startDate &&
                        l.ReturnDate <= endDate);

        var total = await query.CountAsync();

        var items = await query
            .OrderByDescending(l => l.ReturnDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Loan>(items, total, pageNumber, pageSize);
    }

    public async Task<bool> ReturnBookAsync(
        Guid loanId, CancellationToken cancellationToken)
    {
        var loan = await context.Set<Loan>()
            .FirstOrDefaultAsync(l => l.Id == loanId, cancellationToken);

        if (loan == null)
            return false;

        loan.ReturnDate = DateTime.UtcNow;
        loan.Status = LoanStatus.Return;
        loan.UpdatedOnUtc = DateTime.UtcNow;

        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> ExtendLoanAsync(
        Guid loanId, DateTime newDueDate,
        CancellationToken cancellationToken)
    {
        var loan = await context.Set<Loan>()
            .FirstOrDefaultAsync(l => l.Id == loanId, cancellationToken);

        loan.DueDate = newDueDate;
        loan.UpdatedOnUtc = DateTime.UtcNow;

        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> IsBookLoanedOutAsync(Guid resourceId)
    {
        return await context.Set<Loan>()
            .AnyAsync(l => l.ResourceId == resourceId && l.ReturnDate == null);
    }

    public async Task<bool> HasOverdueLoansAsync(Guid userId)
    {
        var now = DateTime.UtcNow;

        return await context.Set<Loan>()
            .AnyAsync(l =>
                l.UserId == userId &&
                l.ReturnDate == null &&
                l.DueDate < now);
    }
}