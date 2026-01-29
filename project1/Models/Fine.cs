using System.ComponentModel.DataAnnotations;

namespace project1.Models
{
    public class Fine
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int BorrowId { get; set; }
        public Borrow Borrow { get; set; } = null!;

        public int DaysLate { get; set; }
        public int Amount { get; set; }   // به تومان
        public bool IsPaid { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
