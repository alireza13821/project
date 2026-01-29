
using System.ComponentModel.DataAnnotations;

namespace project1.Models
{
    public class Book
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public string? Description { get; set; }
        public string Author { get; set; } = null!;
        public int PublishedYear { get; set; }
        public int TotalQuantity { get; set; }
        public int AvailableQuantity { get; set; }
        public BookType Type { get; set; }
        public float Price { get; set; }
        public bool IsActive { get; set; }
        public ICollection<Reserve> Reserves { get; set; }

    }
    public enum BookType
    {
        Borrow,
        Sale
    }
}

