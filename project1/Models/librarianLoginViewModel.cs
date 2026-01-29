using System.ComponentModel.DataAnnotations;

namespace project1.Models
{
    public class librarianLoginViewModel
    {
        [MaxLength(50)]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "ایمیل")]
        [Required(ErrorMessage = "ایمیل الزامی است.")]
        public string Email { get; set; }

        [MaxLength(50)]
        [Display(Name = "رمز عبور")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "رمز عبور الزامی است.")]
        public string Password { get; set; }

        [Display(Name = "مرا به خاطر بسپار")]
        public bool RememberMe { get; set; }
    }
}
