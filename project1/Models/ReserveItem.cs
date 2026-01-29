using Microsoft.EntityFrameworkCore;

namespace project1.Models
{
    [PrimaryKey(nameof(BookId), nameof(ReserveId))]
    public class ReserveItem
    {
        public int BookId { get; set; }
        public int ReserveId { get; set; }
        public Book book { get; set; }
        public Reserve reserve { get; set; }

    }
}
