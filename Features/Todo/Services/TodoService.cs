using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Features.Todo.Data;
using Features.Todo.DTOs;
using Features.Todo.Interfaces;
using Features.Todo.Models;
using Microsoft.EntityFrameworkCore;

namespace Features.Todo.Services
{
    public class TodoService(TodoDbContext context) : ITodoService
    {
        private readonly TodoDbContext _context = context;

        public async Task<List<TodoResponseDto>> GetUserTodosAsync(string userId)
        {
            var todos = await _context.TodoItems
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            return todos.Select(t => MapToDto(t)).ToList();
        }

        public async Task<TodoResponseDto?> GetTodoAsync(int id, string userId)
        {
            var todo = await _context.TodoItems
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            return todo == null ? null : MapToDto(todo);
        }

        public async Task<TodoResponseDto> CreateTodoAsync(TodoCreateDto createDto, string userId)
        {
            var todo = new TodoItem
            {
                Title = createDto.Title,
                Description = createDto.Description,
                DueDate = createDto.DueDate,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            _context.TodoItems.Add(todo);
            await _context.SaveChangesAsync();

            return MapToDto(todo);
        }

        public async Task<TodoResponseDto?> UpdateTodoAsync(int id, TodoCreateDto updateDto, string userId)
        {
            var todo = await _context.TodoItems
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (todo == null) return null;

            todo.Title = updateDto.Title;
            todo.Description = updateDto.Description;
            todo.DueDate = updateDto.DueDate;

            await _context.SaveChangesAsync();
            return MapToDto(todo);
        }

        public async Task<bool> DeleteTodoAsync(int id, string userId)
        {
            var todo = await _context.TodoItems
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (todo == null) return false;

            _context.TodoItems.Remove(todo);
            await _context.SaveChangesAsync();
            return true;
        }

        private TodoResponseDto MapToDto(TodoItem todo)
        {
            return new TodoResponseDto
            {
                Id = todo.Id,
                Title = todo.Title,
                Description = todo.Description,
                IsCompleted = todo.IsCompleted,
                DueDate = todo.DueDate,
                CreatedAt = todo.CreatedAt,
                UserId = todo.UserId
            };
        }

        public async Task<List<TodoResponseDto>> GetAllNoUserAsync()
        {
            var todos = await _context.TodoItems.ToListAsync();

            return todos.Select(t => MapToDto(t)).ToList();
        }
    }
}