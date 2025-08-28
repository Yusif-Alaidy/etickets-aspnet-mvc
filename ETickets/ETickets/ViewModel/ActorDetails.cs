using ETickets.Models;

namespace ETickets.ViewModel
{
    public class ActorDetails
    {
        public Actor Actor {  get; set; }
        public List<Movie> Movies { get; set; }
    }
}
