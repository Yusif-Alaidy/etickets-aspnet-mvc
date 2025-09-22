using Microsoft.AspNetCore.Mvc.Rendering;

namespace ETickets.Areas.Admin.ModelView
{
    public class CreateMovieVM
    {


        public List<SelectListItem> Categories    { get; set; }
        public List<SelectListItem> Cinemas         { get; set; }
        public Movie Movie                  { get; set; }
    }
}
