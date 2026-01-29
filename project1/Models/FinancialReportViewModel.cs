namespace project1.Models
{
    public class FinancialReportViewModel
    {
        public List<Order> BookOrders { get; set; } = new();
        public List<Subscription> Subscriptions { get; set; } = new();
        public List<Fine> PaidFines { get; set; } = new();
        public List<Fine> UnpaidFines { get; set; } = new();

        public float TotalBookSales { get; set; }
        public float TotalSubscriptionSales { get; set; }
        public int TotalPaidFines { get; set; }
        public int TotalUnpaidFines { get; set; }
    }
}
