using System.ComponentModel.DataAnnotations;

namespace project1.Models
{
    public class SmsLog
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public SmsType Type { get; set; }
        public string Message { get; set; } = null!;
        public DateTime SentAt { get; set; }
        public bool IsSuccessful { get; set; }
    }
    public enum SmsType
    {
        Purchase,
        Borrow,
        DueDateReminder,
        ExpireWarning,
        FineWarning,
        BlockNotification
    }
}
