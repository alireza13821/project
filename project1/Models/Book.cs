namespace project1.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public List<ReserveItem> ReserveItems { get; set; }

        public Book()
        {
            ReserveItems = new List<ReserveItem>();
        }
    }
}
