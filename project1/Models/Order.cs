using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace project1.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public float TotalPrice { get; set; }
        public bool IsPaid { get; set; }
        public User User { get; set; } = null!;       
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}

