using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Api.Data;
using Api.Models;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly AppDbContext _db;

    public CategoriesController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<List<Category>>> GetAll(CancellationToken ct)
    {
        return await _db.Categories.OrderBy(c => c.Name).ToListAsync(ct);
    }

    [HttpPost]
    public async Task<ActionResult<Category>> Create([FromBody] Category category, CancellationToken ct)
    {
        category.Id = 0;
        if (string.IsNullOrWhiteSpace(category.Name))
            return BadRequest("Le nom de la catégorie est requis.");
        _db.Categories.Add(category);
        await _db.SaveChangesAsync(ct);
        return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Category>> GetById(int id, CancellationToken ct)
    {
        var c = await _db.Categories.FindAsync([id], ct);
        if (c is null) return NotFound();
        return c;
    }
}
