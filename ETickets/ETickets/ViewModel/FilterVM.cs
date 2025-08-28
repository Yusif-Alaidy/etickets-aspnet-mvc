namespace ETickets.ViewModel
{
    public class FilterVM
    {
        public int? minPrice {  get; set; }
        public int? maxPrice { get; set; }
        public int? categoryId { get; set; }
        public int? cinemaId { get; set; }
        public string? search {  get; set; }
    }
}
