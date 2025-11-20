using SGBV.Domain.Common;

namespace SGBV.Domain.Models;

public sealed class Loan : BaseEntity
{
    public required Guid UserId { get; set; }

    public required Guid ResourceId { get; set; }

    public DateTime LoanDate { get; set; } = DateTime.UtcNow;

    public DateTime DueDate { get; set; }

    public DateTime? ReturnDate { get; set; }

    public required string Status { get; set; } = LoanStatus.Active;

    // Navigation Properties
    public User User { get; set; } = null!;

    public Resource Resource { get; set; } = null!;
}