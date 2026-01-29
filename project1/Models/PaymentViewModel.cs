using System.ComponentModel.DataAnnotations;

namespace project1.Models
{
    public class PaymentViewModel
    {
        public string Title { get; set; } = null!;   // عنوان صفحه
        public string Receiver { get; set; } = "کتابخانه آبی";
        public int Amount { get; set; }   // مبلغ (تومان)
        // اطلاعات کارت (نمایشی)
        [Required(ErrorMessage ="شماره کارت خود را وارد کنید")]
        [StringLength(16, MinimumLength = 16)]
        public string CardNumber { get; set; } = null!;
        [Required(ErrorMessage = "CVV2")]
        [StringLength(4, MinimumLength = 3)]
        public string CVV2 { get; set; } = null!;
        [Required(ErrorMessage = "ماه")]
        [MinLength(2)]
        [MaxLength(2)]
        public string ExpireMonth { get; set; } = null!;
        [Required(ErrorMessage = "سال")]
        [MinLength(2)]
        [MaxLength(2)]
        public string ExpireYear { get; set; } = null!;
        [Required(ErrorMessage = "کد امنیتی را وارد کنید")]
        public string SecurityCode { get; set; } = null!;
        // برای تشخیص نوع پرداخت
        public string PaymentType { get; set; } = null!;
        public int? RefId { get; set; }   // OrderId یا UserId
    }
}
