using System.ComponentModel.DataAnnotations;

namespace project1.Models
{
    public class Reserve
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public int BookId { get; set; }
        public Book Book { get; set; } = null!;
        public DateTime ReservationDate { get; set; }
        public DateTime ExpireDate { get; set; }
        public bool IsActive { get; set; }
    }
}
