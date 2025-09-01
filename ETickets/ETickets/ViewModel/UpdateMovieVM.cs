using ETickets.Models;

namespace ETickets.ViewModel
{
    public class UpdateMovieVM
    {
        public List<Cinema> Cinema { get; set; }
        public List<Category> Category { get; set; }
        public Movie Movie { get; set; }
    }
}
