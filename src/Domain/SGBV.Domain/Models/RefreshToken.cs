using SGBV.Domain.Common;

namespace SGBV.Domain.Models;

public sealed class RefreshToken: BaseEntity
{
    public Guid UserId { get; set; }
    public string Token { get; set; }
    public bool Used { get; set; } = false;
    public DateTime Expiration { get; set; }
    public bool Revoked { get; set; } = false;
    
    public User User { get; set; }
}