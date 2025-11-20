using SGBV.Domain.Common;

namespace SGBV.Domain.Models;

public sealed class Resource : BaseEntity
{
    public string Title { get; set; } = string.Empty;

    public string Author { get; set; } = string.Empty;

    public string? Genre { get; set; }

    public short? PublicationYear { get; set; }
    public string? CoverUrl { get; set; }

    public string? Description { get; set; }

    public string ResourceStatus { get; set; } = ResourceType.Book;
    public string Status { get; set; } = ResourcesStatus.Available;

    public ICollection<Loan> Loans { get; set; } = new List<Loan>();
}