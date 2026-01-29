using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace project1.Models
{
    public class LoginViewModel
    {
        [MaxLength(50)]
        [EmailAddress]
        [Required(ErrorMessage = "لطفا ایمیل را وارد کنید ")]
        [Remote("VerifyEmail", "Account")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "لطفا کلمه عبور را وارد کنید ")]
        [MaxLength(50)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;
    }
}
