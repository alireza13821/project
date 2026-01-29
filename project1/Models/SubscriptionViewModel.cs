namespace project1.Models
{
    public class SubscriptionViewModel
    {
        public bool HasActiveSubscription { get; set; }
        public DateTime? ExpireDate { get; set; }
        public int RemainingDays { get; set; }
    }
}
