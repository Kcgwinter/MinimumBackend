using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Features.Todo.DTOs;
using Features.Todo.Interfaces;
using Features.Todo.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Features.Todo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TodoController : Controller
    {
        private readonly ITodoService _todoService;

        public TodoController(ITodoService todoService)
        {
            _todoService = todoService;
        }

        [HttpGet]
        public async Task<ActionResult<List<TodoResponseDto>>> GetAll()
        {
            var userId = GetUserId();
            var todos = await _todoService.GetUserTodosAsync(userId);
            return Ok(todos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TodoResponseDto>> GetById(int id)
        {
            var userId = GetUserId();
            var todo = await _todoService.GetTodoAsync(id, userId);

            if (todo == null) return NotFound();
            return Ok(todo);
        }

        [HttpPost]
        public async Task<ActionResult<TodoResponseDto>> Create(TodoCreateDto createDto)
        {
            var userId = GetUserId();
            var todo = await _todoService.CreateTodoAsync(createDto, userId);
            return CreatedAtAction(nameof(GetById), new { id = todo.Id }, todo);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TodoResponseDto>> Update(int id, TodoCreateDto updateDto)
        {
            var userId = GetUserId();
            var todo = await _todoService.UpdateTodoAsync(id, updateDto, userId);

            if (todo == null) return NotFound();
            return Ok(todo);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var userId = GetUserId();
            var deleted = await _todoService.DeleteTodoAsync(id, userId);

            if (!deleted) return NotFound();
            return NoContent();
        }

        private string GetUserId()
        {
            // Gets userId from JWT token (same auth as main API)
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? throw new UnauthorizedAccessException("User ID not found in token");
        }
    }
}