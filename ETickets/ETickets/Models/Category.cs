using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ETickets.Models;

public partial class Category
{
    public int Id               { get; set; }

    [Required]
    [MinLength(3)]
    [MaxLength(10)]
    public string Name          { get; set; } = null!;

    public List<Movie>? Movies  { get; set; }
}
