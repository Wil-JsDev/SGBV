using SGBV.Domain.Common;

namespace SGBV.Domain.Models;

public class User : BaseEntity
{
    public required string Name { get; set; }

    public required string Email { get; set; }

    public required string PasswordHash { get; set; }
    public string? ProfileUrl { get; set; }

    public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;

    public DateTime LoginAt { get; set; }

    public Guid RolId { get; set; }

    // Navigation Properties
    public Role Rol { get; set; } = null!;

    public virtual ICollection<Loan> Loans { get; set; } = new List<Loan>();
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}