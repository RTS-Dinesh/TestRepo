using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentApp.Api.Data;
using RentApp.Api.DTOs;

namespace RentApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly RentAppDbContext _db;

    public CategoriesController(RentAppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll(CancellationToken ct)
    {
        var list = await _db.Categories
            .AsNoTracking()
            .Select(c => new CategoryDto(c.Id, c.Name, c.Description, c.IconName, c.ParentCategoryId))
            .ToListAsync(ct);
        return Ok(list);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CategoryDto>> GetById(int id, CancellationToken ct)
    {
        var category = await _db.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id, ct);
        if (category == null) return NotFound();
        return Ok(new CategoryDto(category.Id, category.Name, category.Description, category.IconName, category.ParentCategoryId));
    }

    [HttpGet("{id:int}/listings")]
    public async Task<ActionResult<IEnumerable<object>>> GetListings(int id, CancellationToken ct)
    {
        var exists = await _db.Categories.AnyAsync(c => c.Id == id, ct);
        if (!exists) return NotFound();
        var list = await _db.Listings
            .AsNoTracking()
            .Where(l => l.CategoryId == id)
            .Select(l => new { l.Id, l.Title, l.PricePerDay, l.City, l.Country, l.IsAvailable })
            .ToListAsync(ct);
        return Ok(list);
    }

    [HttpPost]
    public async Task<ActionResult<CategoryDto>> Create([FromBody] CreateCategoryRequest req, CancellationToken ct)
    {
        var category = new RentApp.Api.Models.Category
        {
            Name = req.Name,
            Description = req.Description,
            IconName = req.IconName,
            ParentCategoryId = req.ParentCategoryId
        };
        _db.Categories.Add(category);
        await _db.SaveChangesAsync(ct);
        return CreatedAtAction(nameof(GetById), new { id = category.Id },
            new CategoryDto(category.Id, category.Name, category.Description, category.IconName, category.ParentCategoryId));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<CategoryDto>> Update(int id, [FromBody] UpdateCategoryRequest req, CancellationToken ct)
    {
        var category = await _db.Categories.FirstOrDefaultAsync(c => c.Id == id, ct);
        if (category == null) return NotFound();
        if (req.Name != null) category.Name = req.Name;
        if (req.Description != null) category.Description = req.Description;
        if (req.IconName != null) category.IconName = req.IconName;
        if (req.ParentCategoryId.HasValue) category.ParentCategoryId = req.ParentCategoryId;
        await _db.SaveChangesAsync(ct);
        return Ok(new CategoryDto(category.Id, category.Name, category.Description, category.IconName, category.ParentCategoryId));
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id, CancellationToken ct)
    {
        var category = await _db.Categories.FirstOrDefaultAsync(c => c.Id == id, ct);
        if (category == null) return NotFound();
        _db.Categories.Remove(category);
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }
}
