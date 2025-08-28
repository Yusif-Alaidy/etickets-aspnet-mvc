using System;
using System.Collections.Generic;

namespace ETickets.Models;

public partial class Actor
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? Bio { get; set; }

    public string? ProfilePicture { get; set; }

    public string? News { get; set; }
    public List<Movie> Movies { get; set; }
}
