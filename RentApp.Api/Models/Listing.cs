namespace RentApp.Api.Models;

public class Listing
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public int OwnerId { get; set; }
    public decimal PricePerDay { get; set; }
    public decimal? PricePerWeek { get; set; }
    public decimal? PricePerMonth { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsAvailable { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public Category Category { get; set; } = null!;
    public User Owner { get; set; } = null!;
    public ICollection<Rental> Rentals { get; set; } = new List<Rental>();
}
