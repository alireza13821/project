using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace project1.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "لطفا نام خود را وارد کنید ")]
        [MaxLength(26)]
        [MinLength(2)]
        public string Name { get; set; } = null!;

        [MaxLength(50)]
        [EmailAddress]
        [Required(ErrorMessage = "لطفا ایمیل را وارد کنید ")]
        [Remote("IdentifyDuplicateEmail", "Account")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "لطفا رمز عبور را وارد کنید ")]
        [MaxLength(20)]
        [DataType(DataType.Password)]
        [MinLength(4)]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "لطفا رمز عبور را مجددا وارد کنید ")]
        [MaxLength(20)]
        [DataType(DataType.Password)]
        [MinLength(4)]
        [Compare("Password", ErrorMessage = "عدم مطابقت رمز عبور")]
        public string Repassword { get; set; } = null!;

        [Required(ErrorMessage = "لطفا شماره تلفن همراه خود را وارد کنید ")]
        [MaxLength(11)]
        [Remote("VerifyPhone", "Account")]
        public string? Phone { get; set; }
    }
}
