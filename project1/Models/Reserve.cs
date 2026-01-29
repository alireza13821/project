namespace project1.Models
{
    public class Reserve
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime CreateDate { get; set; }
        public bool IsFinaly { get; set; }
        public bool IsApproved { get; set; }
        public User User { get; set; } // Access to user information
        public string Status { get; set; } // Add status field
        public bool? IsRejected { get; set; }
        public string? RejectionReason { get; set; }
        public List<ReserveItem> ReserveItems { get; set; }
        public Reserve()
        {
            ReserveItems = new List<ReserveItem>();
        }
    }
}
