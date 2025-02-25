using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Entities;

[Table("users")]
public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ID   { get; set; }

    [Column("full_name")]
    public string FullName { get; set; }

    [Column("username")]
    public string Username { get; set; } = string.Empty;

    [Column("password")]
    public string Password { get; set; } = string.Empty;

    public List<Product> Products { get; set; } = new();
}
