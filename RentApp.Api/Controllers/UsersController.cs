using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentApp.Api.Data;
using RentApp.Api.DTOs;

namespace RentApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly RentAppDbContext _db;

    public UsersController(RentAppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAll(CancellationToken ct)
    {
        var list = await _db.Users
            .AsNoTracking()
            .Select(u => new UserDto(u.Id, u.Name, u.Email, u.Phone, u.City, u.Country, u.CreatedAt))
            .ToListAsync(ct);
        return Ok(list);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserDto>> GetById(int id, CancellationToken ct)
    {
        var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, ct);
        if (user == null) return NotFound();
        return Ok(new UserDto(user.Id, user.Name, user.Email, user.Phone, user.City, user.Country, user.CreatedAt));
    }

    [HttpPost]
    public async Task<ActionResult<UserDto>> Create([FromBody] CreateUserRequest req, CancellationToken ct)
    {
        if (await _db.Users.AnyAsync(u => u.Email == req.Email, ct))
            return Conflict("A user with this email already exists.");
        var user = new RentApp.Api.Models.User
        {
            Name = req.Name,
            Email = req.Email,
            Phone = req.Phone,
            City = req.City,
            Country = req.Country
        };
        _db.Users.Add(user);
        await _db.SaveChangesAsync(ct);
        return CreatedAtAction(nameof(GetById), new { id = user.Id },
            new UserDto(user.Id, user.Name, user.Email, user.Phone, user.City, user.Country, user.CreatedAt));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<UserDto>> Update(int id, [FromBody] UpdateUserRequest req, CancellationToken ct)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id, ct);
        if (user == null) return NotFound();
        if (req.Name != null) user.Name = req.Name;
        if (req.Phone != null) user.Phone = req.Phone;
        if (req.City != null) user.City = req.City;
        if (req.Country != null) user.Country = req.Country;
        await _db.SaveChangesAsync(ct);
        return Ok(new UserDto(user.Id, user.Name, user.Email, user.Phone, user.City, user.Country, user.CreatedAt));
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id, CancellationToken ct)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id, ct);
        if (user == null) return NotFound();
        _db.Users.Remove(user);
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }
}
