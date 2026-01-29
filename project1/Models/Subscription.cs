using System.ComponentModel.DataAnnotations;
using System;

namespace project1.Models
{
    public class Subscription
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public float Price { get; set; } //به تومان
        public bool IsActive { get; set; }
    }
}

