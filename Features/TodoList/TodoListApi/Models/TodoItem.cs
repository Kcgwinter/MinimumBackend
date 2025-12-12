using System;
using Core.Entities;

namespace TodoListApi.Models;

public class TodoItem : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    //User Reference
    public int UserId { get; set; }
    public User? User { get; set; }

}
