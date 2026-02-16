using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentApp.Api.Data;
using RentApp.Api.DTOs;
using RentApp.Api.Models;

namespace RentApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RentalsController : ControllerBase
{
    private readonly RentAppDbContext _db;

    public RentalsController(RentAppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RentalDto>>> GetAll(
        [FromQuery] int? listingId,
        [FromQuery] int? renterId,
        CancellationToken ct)
    {
        var query = _db.Rentals
            .AsNoTracking()
            .Include(r => r.Listing)
            .Include(r => r.Renter)
            .AsQueryable();
        if (listingId.HasValue) query = query.Where(r => r.ListingId == listingId);
        if (renterId.HasValue) query = query.Where(r => r.RenterId == renterId);
        var list = await query
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new RentalDto(r.Id, r.ListingId, r.Listing.Title, r.RenterId, r.Renter.Name,
                r.StartDate, r.EndDate, r.TotalAmount, r.Status, r.Notes, r.CreatedAt))
            .ToListAsync(ct);
        return Ok(list);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<RentalDto>> GetById(int id, CancellationToken ct)
    {
        var rental = await _db.Rentals
            .AsNoTracking()
            .Include(r => r.Listing)
            .Include(r => r.Renter)
            .FirstOrDefaultAsync(r => r.Id == id, ct);
        if (rental == null) return NotFound();
        return Ok(new RentalDto(rental.Id, rental.ListingId, rental.Listing.Title, rental.RenterId, rental.Renter.Name,
            rental.StartDate, rental.EndDate, rental.TotalAmount, rental.Status, rental.Notes, rental.CreatedAt));
    }

    [HttpPost]
    public async Task<ActionResult<RentalDto>> Create([FromBody] CreateRentalRequest req, CancellationToken ct)
    {
        if (req.EndDate <= req.StartDate)
            return BadRequest("EndDate must be after StartDate.");
        var listing = await _db.Listings
            .Include(l => l.Owner)
            .FirstOrDefaultAsync(l => l.Id == req.ListingId, ct);
        if (listing == null) return BadRequest("Listing not found.");
        if (!listing.IsAvailable) return BadRequest("Listing is not available for rent.");
        var renter = await _db.Users.FindAsync([req.RenterId], ct);
        if (renter == null) return BadRequest("Renter not found.");
        var days = (req.EndDate - req.StartDate).Days;
        if (days <= 0) days = 1;
        var totalAmount = listing.PricePerDay * days;
        var rental = new Rental
        {
            ListingId = req.ListingId,
            RenterId = req.RenterId,
            StartDate = req.StartDate,
            EndDate = req.EndDate,
            TotalAmount = totalAmount,
            Notes = req.Notes
        };
        _db.Rentals.Add(rental);
        await _db.SaveChangesAsync(ct);
        rental = await _db.Rentals.Include(r => r.Listing).Include(r => r.Renter).FirstAsync(r => r.Id == rental.Id, ct);
        return CreatedAtAction(nameof(GetById), new { id = rental.Id },
            new RentalDto(rental.Id, rental.ListingId, rental.Listing.Title, rental.RenterId, rental.Renter.Name,
                rental.StartDate, rental.EndDate, rental.TotalAmount, rental.Status, rental.Notes, rental.CreatedAt));
    }

    [HttpPatch("{id:int}/status")]
    public async Task<ActionResult<RentalDto>> UpdateStatus(int id, [FromBody] UpdateRentalStatusRequest req, CancellationToken ct)
    {
        var rental = await _db.Rentals.Include(r => r.Listing).Include(r => r.Renter).FirstOrDefaultAsync(r => r.Id == id, ct);
        if (rental == null) return NotFound();
        rental.Status = req.Status;
        await _db.SaveChangesAsync(ct);
        return Ok(new RentalDto(rental.Id, rental.ListingId, rental.Listing.Title, rental.RenterId, rental.Renter.Name,
            rental.StartDate, rental.EndDate, rental.TotalAmount, rental.Status, rental.Notes, rental.CreatedAt));
    }
}
