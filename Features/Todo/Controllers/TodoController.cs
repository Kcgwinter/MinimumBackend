using System.Security.Claims;
using Features.Todo.DTOs;
using Features.Todo.Interfaces;
using Features.Todo.Validators; 
using FluentValidation; 
using Microsoft.AspNetCore.Mvc;

namespace Features.Todo.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class TodoController : ControllerBase
    {
        private readonly ITodoService _todoService;
        private readonly IValidator<TodoCreateDto> _createValidator; // <-- Inject validator
        private readonly IValidator<TodoUpdateDto> _updateValidator; // <-- Inject validator

        public TodoController(
            ITodoService todoService,
            IValidator<TodoCreateDto> createValidator,
            IValidator<TodoUpdateDto> updateValidator
        ) // <-- Update constructor
        {
            _todoService = todoService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
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

            if (todo == null)
                return NotFound();
            return Ok(todo);
        }

        [HttpPost]
        public async Task<ActionResult<TodoResponseDto>> Create(TodoCreateDto createDto)
        {
            // --- VALIDATION CHECK START ---
            var validationResult = await _createValidator.ValidateAsync(createDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors); // Assumes you have an extension method to format errors
            }
            // --- VALIDATION CHECK END ---

            var userId = GetUserId();
            var todo = await _todoService.CreateTodoAsync(createDto, userId);
            return CreatedAtAction(nameof(GetById), new { id = todo.Id }, todo);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TodoResponseDto>> Update(int id, TodoUpdateDto updateDto)
        {
            // --- VALIDATION CHECK START ---
            var validationResult = await _updateValidator.ValidateAsync(updateDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors); // Assumes you have an extension method to format errors
            }
            // --- VALIDATION CHECK END ---

            var userId = GetUserId();
            var todo = await _todoService.UpdateTodoAsync(id, updateDto, userId);

            if (todo == null)
                return NotFound();
            return Ok(todo);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var userId = GetUserId();
            var deleted = await _todoService.DeleteTodoAsync(id, userId);

            if (!deleted)
                return NotFound();
            return NoContent();
        }

        private string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? throw new UnauthorizedAccessException("User ID not found in token");
        }
    }
}
