using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Core.Interfaces;
using Api.Controllers;
using TodoListApi.DTOs;
using TodoListApi.Models;

namespace TodoListApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodosController : ApiControllerBase
    {
        public TodosController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<ActionResult<List<TodoResponseDto>>> GetAll()
        {
            var todos = await 
            var todoDtos = todos.Select(todo => new TodoResponseDto
            {
                Id = todo.Id,
                Title = todo.Title,
                Description = todo.Description,
                IsCompleted = todo.IsCompleted,
                DueDate = todo.DueDate,
                CreatedAt = todo.CreatedAt,
                UserId = todo.UserId
            }).ToList();

            return Ok(todoDtos);
        }
    }
}
