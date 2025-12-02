using SGBV.Domain.Models;
using SGBV.Application.Utilities;

namespace SGBV.Application.Interfaces.Repositories;

public interface ILoanRepository : IGenericRepository<Loan>
{
    /// <summary>
    /// Asynchronously retrieves a paged list of active (not yet returned) loans.
    /// </summary>
    Task<PagedResult<Loan>> GetActiveLoansPagedAsync(int pageNumber, int pageSize);

    /// <summary>
    /// Asynchronously retrieves a paged list of all loans associated with a specific user (borrower).
    /// </summary>
    Task<PagedResult<Loan>> GetLoansByUserIdPagedAsync(Guid userId, int pageNumber, int pageSize);

    /// <summary>
    /// Asynchronously retrieves a paged list of all loans that are currently overdue (return date is in the past).
    /// </summary>
    Task<PagedResult<Loan>> GetOverdueLoansPagedAsync(int pageNumber, int pageSize);
    
    /// <summary>
    /// Searches and retrieves a paged list of loans by borrower name, book title, or ID.
    /// This method is crucial for administrative search functionalities.
    /// </summary>
    /// <param name="searchQuery">The text to search for (e.g., partial name, title, or ID).</param>
    /// <param name="pageNumber">The index of the page to retrieve (1-based).</param>
    /// <param name="pageSize">The maximum number of items per page.</param>
    /// <returns>A <see cref="PagedResult{Loan}"/> containing matching loans.</returns>
    Task<PagedResult<Loan>> SearchLoansPagedAsync(string searchQuery, int pageNumber, int pageSize);

    /// <summary>
    /// Retrieves a paged list of loans that were completed (returned) within a specific date range.
    /// </summary>
    /// <param name="startDate">The start date for the return period.</param>
    /// <param name="endDate">The end date for the return period.</param>
    /// <param name="pageNumber">The index of the page to retrieve (1-based).</param>
    /// <param name="pageSize">The maximum number of items per page.</param>
    /// <returns>A <see cref="PagedResult{Loan}"/> containing the historical loans.</returns>
    Task<PagedResult<Loan>> GetHistoricalLoansPagedAsync(DateTime startDate, DateTime endDate, int pageNumber,
        int pageSize);
    
    /// <summary>
    /// Asynchronously marks an active loan as returned.
    /// </summary>
    /// <param name="loanId">The ID of the loan to be returned.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>True if the loan was successfully marked as returned; otherwise, false.</returns>
    Task<bool> ReturnBookAsync(Guid loanId, CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously extends the due date of an active loan.
    /// </summary>
    /// <param name="loanId">The ID of the loan to extend.</param>
    /// <param name="newDueDate">The new extended due date.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>True if the due date was successfully updated; otherwise, false.</returns>
    Task<bool> ExtendLoanAsync(Guid loanId, DateTime newDueDate, CancellationToken cancellationToken);
    
    /// <summary>
    /// Checks if a specific book is currently on loan (active).
    /// </summary>
    /// <param name="bookId">The ID of the book to check.</param>
    /// <returns>True if the book is currently loaned out; otherwise, false.</returns>
    Task<bool> IsBookLoanedOutAsync(Guid bookId);

    /// <summary>
    /// Checks if a user has any overdue loans.
    /// </summary>
    /// <param name="userId">The ID of the user to check.</param>
    /// <returns>True if the user has one or more overdue loans; otherwise, false.</returns>
    Task<bool> HasOverdueLoansAsync(Guid userId);
    
    Task<int> GetUserLoanCountAsync(Guid userId, CancellationToken cancellationToken);
    Task<int> GetUserBorrowedResourceCountAsync(Guid userId, CancellationToken cancellationToken);
    
    Task<int> GetActiveLoanCountAsync(CancellationToken cancellationToken);
    Task<int> GetOverdueLoanCountAsync(CancellationToken cancellationToken);

    Task<int> GetUserOverdueLoanCountAsync(Guid userId, CancellationToken cancellationToken);

}