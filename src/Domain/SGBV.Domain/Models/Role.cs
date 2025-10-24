using SGBV.Domain.Common;

namespace SGBV.Domain.Models;

public class Role : BaseEntity
{
    public Guid RolId { get; set; }

    public required string NameRol { get; set; }

    public ICollection<User> Users { get; set; } = new List<User>();
}