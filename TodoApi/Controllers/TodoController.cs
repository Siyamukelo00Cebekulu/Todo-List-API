using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TodoApi;

[ApiController]
[Route("api/todos")]
[Authorize]
public class TodoController : ControllerBase
{
    private readonly AppDbContext _db;

    public TodoController(AppDbContext db)
    {
        _db = db;
    }

    private int GetUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetAll(
        int page = 1,
        int pageSize = 5,
        bool? completed = null)
    {
        var query = _db.Todos
            .Where(t => t.UserId == GetUserId());

        if (completed.HasValue)
            query = query.Where(t => t.IsCompleted == completed);

        var todos = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(todos);
    }

    [HttpPost]
    public async Task<IActionResult> Create(TodoDto dto)
    {
        var todo = new TodoItem
        {
            Title = dto.Title,
            UserId = GetUserId()
        };

        _db.Todos.Add(todo);
        await _db.SaveChangesAsync();
        return Ok(todo);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id)
    {
        var todo = await _db.Todos
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == GetUserId());

        if (todo == null) return NotFound();

        todo.IsCompleted = true;
        await _db.SaveChangesAsync();

        return Ok(todo);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var todo = await _db.Todos
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == GetUserId());

        if (todo == null) return NotFound();

        _db.Todos.Remove(todo);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
