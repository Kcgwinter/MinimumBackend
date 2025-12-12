using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Features.Todo.Controllers
{
    [Route("[controller]")]
    public class TodoControllerI(TodoService todoService) : Controller
    {
        private readonly ITodoService _todoService = todoService;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = GetUserId();
            var todos = await _todoService.GetUserTodosAsync(userId);
            return Ok(todos);
        }

        private string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? throw new UnauthorizedAccessException();
        }
    }
}