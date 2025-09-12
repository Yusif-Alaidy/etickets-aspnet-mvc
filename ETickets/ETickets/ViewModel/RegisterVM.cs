using System.ComponentModel.DataAnnotations;

namespace ETickets.ViewModel
{
    public class RegisterVM
    {
        public int Id { get; set; }
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required, DataType(DataType.EmailAddress)]
        public string Email { get; set; } = string.Empty;

        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required, DataType(DataType.Password), Compare(nameof(Password))]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
