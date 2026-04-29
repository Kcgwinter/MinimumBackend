using System;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities;

public class RefreshToken : BaseEntity
{
    public string Token { get; set; } = "";
    public int UserId { get; set; }
    public DateTime Expires { get; set; }
    public required User User { get; set; }
}
