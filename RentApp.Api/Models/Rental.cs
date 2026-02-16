namespace RentApp.Api.Models;

public enum RentalStatus
{
    Pending,
    Confirmed,
    InProgress,
    Completed,
    Cancelled
}

public class Rental
{
    public int Id { get; set; }
    public int ListingId { get; set; }
    public int RenterId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalAmount { get; set; }
    public RentalStatus Status { get; set; } = RentalStatus.Pending;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Listing Listing { get; set; } = null!;
    public User Renter { get; set; } = null!;
}
