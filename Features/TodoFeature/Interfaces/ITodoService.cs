using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Features.Todo.Interfaces
{
    public interface ITodoService
    {
        Task<List<TodoResponseDto>> GetUserTodosAsync(string userId);
        Task<TodoResponseDto?> GetTodoAsync(int id, string userId);
        Task<TodoResponseDto> CreateTodoAsync(TodoCreateDto createDto, string userId);
        Task<TodoResponseDto?> UpdateTodoAsync(int id, TodoCreateDto updateDto, string userId);
        Task<bool> DeleteTodoAsync(int id, string userId);
    }
}