using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ETickets.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string Name      { get; set; } = string.Empty;
        public string? State    { get; set; }
        public string? City     { get; set; }
        public string? Street   { get; set; }
        public string? ZipCode  { get; set; }
    }
}
