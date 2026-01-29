namespace project1.Models
{
    public class Borrow
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int BookId { get; set; }
        public Book Book { get; set; } = null!;

        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }

        public int RenewedCount { get; set; }

        public bool IsReturned { get; set; }
    }
}
