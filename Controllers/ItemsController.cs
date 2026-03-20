using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Api.Data;
using Api.Models;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItemsController : ControllerBase
{
    private readonly AppDbContext _db;

    public ItemsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<List<Item>>> GetAll(CancellationToken ct)
    {
        return await _db.Items
            .Include(i => i.Category)
            .OrderBy(x => x.Id)
            .ToListAsync(ct);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Item>> GetById(int id, CancellationToken ct)
    {
        var item = await _db.Items.Include(i => i.Category).FirstOrDefaultAsync(i => i.Id == id, ct);
        if (item is null) return NotFound();
        return item;
    }

    [HttpPost]
    public async Task<ActionResult<Item>> Create([FromBody] Item item, CancellationToken ct)
    {
        item.Id = 0;
        item.CreatedAt = DateTime.UtcNow;
        _db.Items.Add(item);
        await _db.SaveChangesAsync(ct);
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<Item>> Update(int id, [FromBody] Item item, CancellationToken ct)
    {
        if (id != item.Id) return BadRequest();
        var existing = await _db.Items.FindAsync([id], ct);
        if (existing is null) return NotFound();
        existing.Name = item.Name;
        existing.Description = item.Description;
        existing.IsDone = item.IsDone;
        existing.CategoryId = item.CategoryId;
        existing.DueDate = item.DueDate;
        await _db.SaveChangesAsync(ct);
        return existing;
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var item = await _db.Items.FindAsync([id], ct);
        if (item is null) return NotFound();
        _db.Items.Remove(item);
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }
}
