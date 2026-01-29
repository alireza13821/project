
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace project1.Models
{
    public class AddBookViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "نام کتاب الزامی است")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "توضیحات الزامی است")]
        public string? Description { get; set; }
        [Required(ErrorMessage = "نویسنده الزامی است")]
        public string Author { get; set; } = null!;
        [Required(ErrorMessage = "سال انتشار کتاب را وارد کنید")]
        public int PublishedYear { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "تعداد باید حداقل ۱ باشد")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "نوع کتاب را مشخص کنید")]
        public BookType Type { get; set; }
        public float Price { get; set; }

        [Display(Name = "عکس کتاب")]
        public IFormFile? Picture { get; set; } 
    }
}
