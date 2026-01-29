using System.ComponentModel.DataAnnotations;

namespace project1.Models
{
    public class AddBookViewModel
    {

        [Required(ErrorMessage = "نام کتاب الزامی است.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "تعداد کتاب الزامی است.")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "توضیحات کتاب الزامی است.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "مسیر تصویر کتاب الزامی است.")]
        public IFormFile picture { get; set; }
    }
}
