using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Features.TodoList.Models
{
    public class TodoItem
    {
        public class TodoItem
        {
            public int Id { get; set; }
            public string Title { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public bool IsCompleted { get; set; }
            public DateTime DueDate { get; set; }
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
            public DateTime? UpdatedAt { get; set; }

            // Store user ID as string from JWT claim
            public string UserId { get; set; } = string.Empty;
        }
    }
}