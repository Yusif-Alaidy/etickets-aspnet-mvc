namespace ETickets.ViewModel
{
    public class ResendEmailConfirmationVM
    {
            public int Id { get; set; }
            [Required]
            public string EmailORUserName { get; set; } = string.Empty;
    }

}
