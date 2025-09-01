using ETickets.Models;

namespace ETickets.ViewModel
{
    public class Home
    {
        public List<Movie>? Movies { get; set; }
        public List<Category>? Categories { get; set; }
        public List<Cinema>? Cinemas { get; set; }
        public int NumberOfOage { get; set; }
        
    }
}
