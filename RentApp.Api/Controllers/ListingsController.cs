using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentApp.Api.Data;
using RentApp.Api.DTOs;

namespace RentApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ListingsController : ControllerBase
{
    private readonly RentAppDbContext _db;

    public ListingsController(RentAppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ListingDto>>> Search([FromQuery] ListingSearchRequest? search, CancellationToken ct)
    {
        var query = _db.Listings
            .AsNoTracking()
            .Include(l => l.Category)
            .Include(l => l.Owner)
            .AsQueryable();

        if (search != null)
        {
            if (search.CategoryId.HasValue) query = query.Where(l => l.CategoryId == search.CategoryId);
            if (!string.IsNullOrWhiteSpace(search.City)) query = query.Where(l => l.City != null && l.City.Contains(search.City));
            if (!string.IsNullOrWhiteSpace(search.Country)) query = query.Where(l => l.Country != null && l.Country.Contains(search.Country));
            if (search.MaxPricePerDay.HasValue) query = query.Where(l => l.PricePerDay <= search.MaxPricePerDay.Value);
            if (search.IsAvailableOnly == true) query = query.Where(l => l.IsAvailable);
        }

        var list = await query
            .OrderByDescending(l => l.CreatedAt)
            .Select(l => new ListingDto(
                l.Id, l.Title, l.Description, l.CategoryId, l.Category.Name, l.OwnerId, l.Owner.Name,
                l.PricePerDay, l.PricePerWeek, l.PricePerMonth, l.City, l.Country, l.ImageUrl,
                l.IsAvailable, l.CreatedAt))
            .ToListAsync(ct);
        return Ok(list);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ListingDto>> GetById(int id, CancellationToken ct)
    {
        var listing = await _db.Listings
            .AsNoTracking()
            .Include(l => l.Category)
            .Include(l => l.Owner)
            .FirstOrDefaultAsync(l => l.Id == id, ct);
        if (listing == null) return NotFound();
        return Ok(new ListingDto(
            listing.Id, listing.Title, listing.Description, listing.CategoryId, listing.Category.Name,
            listing.OwnerId, listing.Owner.Name, listing.PricePerDay, listing.PricePerWeek, listing.PricePerMonth,
            listing.City, listing.Country, listing.ImageUrl, listing.IsAvailable, listing.CreatedAt));
    }

    [HttpPost]
    public async Task<ActionResult<ListingDto>> Create([FromBody] CreateListingRequest req, CancellationToken ct)
    {
        var category = await _db.Categories.FindAsync([req.CategoryId], ct);
        var owner = await _db.Users.FindAsync([req.OwnerId], ct);
        if (category == null || owner == null) return BadRequest("Invalid CategoryId or OwnerId.");
        var listing = new RentApp.Api.Models.Listing
        {
            Title = req.Title,
            Description = req.Description,
            CategoryId = req.CategoryId,
            OwnerId = req.OwnerId,
            PricePerDay = req.PricePerDay,
            PricePerWeek = req.PricePerWeek,
            PricePerMonth = req.PricePerMonth,
            City = req.City,
            Country = req.Country,
            ImageUrl = req.ImageUrl
        };
        _db.Listings.Add(listing);
        await _db.SaveChangesAsync(ct);
        listing = await _db.Listings.Include(l => l.Category).Include(l => l.Owner).FirstAsync(l => l.Id == listing.Id, ct);
        return CreatedAtAction(nameof(GetById), new { id = listing.Id },
            new ListingDto(listing.Id, listing.Title, listing.Description, listing.CategoryId, listing.Category.Name,
                listing.OwnerId, listing.Owner.Name, listing.PricePerDay, listing.PricePerWeek, listing.PricePerMonth,
                listing.City, listing.Country, listing.ImageUrl, listing.IsAvailable, listing.CreatedAt));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ListingDto>> Update(int id, [FromBody] UpdateListingRequest req, CancellationToken ct)
    {
        var listing = await _db.Listings.Include(l => l.Category).Include(l => l.Owner).FirstOrDefaultAsync(l => l.Id == id, ct);
        if (listing == null) return NotFound();
        if (req.Title != null) listing.Title = req.Title;
        if (req.Description != null) listing.Description = req.Description;
        if (req.CategoryId.HasValue) listing.CategoryId = req.CategoryId.Value;
        if (req.PricePerDay.HasValue) listing.PricePerDay = req.PricePerDay.Value;
        if (req.PricePerWeek.HasValue) listing.PricePerWeek = req.PricePerWeek;
        if (req.PricePerMonth.HasValue) listing.PricePerMonth = req.PricePerMonth;
        if (req.City != null) listing.City = req.City;
        if (req.Country != null) listing.Country = req.Country;
        if (req.ImageUrl != null) listing.ImageUrl = req.ImageUrl;
        if (req.IsAvailable.HasValue) listing.IsAvailable = req.IsAvailable.Value;
        listing.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return Ok(new ListingDto(listing.Id, listing.Title, listing.Description, listing.CategoryId, listing.Category.Name,
            listing.OwnerId, listing.Owner.Name, listing.PricePerDay, listing.PricePerWeek, listing.PricePerMonth,
            listing.City, listing.Country, listing.ImageUrl, listing.IsAvailable, listing.CreatedAt));
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id, CancellationToken ct)
    {
        var listing = await _db.Listings.FirstOrDefaultAsync(l => l.Id == id, ct);
        if (listing == null) return NotFound();
        _db.Listings.Remove(listing);
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }
}
