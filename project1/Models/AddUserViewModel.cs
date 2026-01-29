using System.ComponentModel.DataAnnotations;

namespace project1.Models
{
    public class AddUserViewModel
    {
        [MaxLength(50)]
        [Required(ErrorMessage = "نام الزامی است.")]
        public string Name { get; set; }

        [MaxLength(50)]
        [Required(ErrorMessage = "نام خانوادگی الزامی است.")]
        public string LastName { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "ایمیل الزامی است.")]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "رمز عبور الزامی است.")]
        public string Password { get; set; }
    }
}
