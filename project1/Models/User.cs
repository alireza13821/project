using System.ComponentModel.DataAnnotations;

namespace project1.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public string Email { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;
        public DateTime RegisterDate { get; set; }
        [Required]
        public string Role { get; set; } = null!;
        [Required]
        public string? Phone { get; set; }
        [Required]
        public bool IsPremium { get; set; } = false;
        [Required]
        public bool IsActive { get; set; } = true;
        public bool IsBlocked { get; set; } = false;
        public int TotalFineAmount { get; set; } = 0;
    }
}
