using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TodoListApi.Data;
using TodoListApi.Dto.Todo;
using TodoListApi.Models;

namespace TodoListApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TodoController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TodoController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] TodoQueryParameters query)
    {
        var userId = GetCurrentUserId();
        var todoQuery = _context.TodoItems.Where(t => t.UserId == userId);

        if (query.IsComplete.HasValue)
        {
            todoQuery = todoQuery.Where(t => t.IsComplete == query.IsComplete.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var normalized = query.Search.Trim().ToLowerInvariant();
            todoQuery = todoQuery.Where(t => t.Title.ToLower().Contains(normalized) || (t.Description != null && t.Description.ToLower().Contains(normalized)));
        }

        todoQuery = query.SortBy?.ToLowerInvariant() switch
        {
            "title" => query.SortOrder.Equals("asc", StringComparison.OrdinalIgnoreCase)
                ? todoQuery.OrderBy(t => t.Title)
                : todoQuery.OrderByDescending(t => t.Title),
            "duedate" => query.SortOrder.Equals("asc", StringComparison.OrdinalIgnoreCase)
                ? todoQuery.OrderBy(t => t.DueDate)
                : todoQuery.OrderByDescending(t => t.DueDate),
            "priority" => query.SortOrder.Equals("asc", StringComparison.OrdinalIgnoreCase)
                ? todoQuery.OrderBy(t => t.Priority)
                : todoQuery.OrderByDescending(t => t.Priority),
            _ => query.SortOrder.Equals("asc", StringComparison.OrdinalIgnoreCase)
                ? todoQuery.OrderBy(t => t.CreatedAt)
                : todoQuery.OrderByDescending(t => t.CreatedAt)
        };

        var pageSize = Math.Clamp(query.PageSize, 1, 100);
        var page = Math.Max(query.Page, 1);
        var total = await todoQuery.CountAsync();
        var items = await todoQuery.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        Response.Headers["X-Total-Count"] = total.ToString();
        Response.Headers["X-Page"] = page.ToString();
        Response.Headers["X-Page-Size"] = pageSize.ToString();

        var result = items.Select(t => new TodoItemResponse
        {
            Id = t.Id,
            Title = t.Title,
            Description = t.Description,
            IsComplete = t.IsComplete,
            CreatedAt = t.CreatedAt,
            DueDate = t.DueDate,
            Priority = t.Priority
        });

        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var userId = GetCurrentUserId();
        var item = await _context.TodoItems.SingleOrDefaultAsync(t => t.Id == id && t.UserId == userId);
        if (item == null)
        {
            return NotFound();
        }

        return Ok(new TodoItemResponse
        {
            Id = item.Id,
            Title = item.Title,
            Description = item.Description,
            IsComplete = item.IsComplete,
            CreatedAt = item.CreatedAt,
            DueDate = item.DueDate,
            Priority = item.Priority
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TodoCreateRequest request)
    {
        var userId = GetCurrentUserId();
        var todo = new TodoItem
        {
            Title = request.Title,
            Description = request.Description,
            IsComplete = request.IsComplete,
            DueDate = request.DueDate,
            Priority = request.Priority,
            UserId = userId
        };

        _context.TodoItems.Add(todo);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = todo.Id }, new TodoItemResponse
        {
            Id = todo.Id,
            Title = todo.Title,
            Description = todo.Description,
            IsComplete = todo.IsComplete,
            CreatedAt = todo.CreatedAt,
            DueDate = todo.DueDate,
            Priority = todo.Priority
        });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] TodoUpdateRequest request)
    {
        var userId = GetCurrentUserId();
        var todo = await _context.TodoItems.SingleOrDefaultAsync(t => t.Id == id && t.UserId == userId);
        if (todo == null)
        {
            return NotFound();
        }

        if (!string.IsNullOrWhiteSpace(request.Title))
        {
            todo.Title = request.Title;
        }

        if (request.Description != null)
        {
            todo.Description = request.Description;
        }

        if (request.IsComplete.HasValue)
        {
            todo.IsComplete = request.IsComplete.Value;
        }

        if (request.DueDate.HasValue)
        {
            todo.DueDate = request.DueDate;
        }

        if (request.Priority.HasValue)
        {
            todo.Priority = request.Priority.Value;
        }

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = GetCurrentUserId();
        var todo = await _context.TodoItems.SingleOrDefaultAsync(t => t.Id == id && t.UserId == userId);
        if (todo == null)
        {
            return NotFound();
        }

        _context.TodoItems.Remove(todo);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        return int.TryParse(userIdClaim, out var userId) ? userId : 0;
    }
}
