using Microsoft.EntityFrameworkCore;
using project1.Models;

namespace project1.Data
{
    public class MyDbContext: DbContext 
    {
        public MyDbContext(DbContextOptions<MyDbContext> options):base(options)
        {
        }
        public DbSet<Book> Books { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Reserve> Reserves { get; set; }
        public DbSet <Borrow> Borrows { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<Message> Messages { get; set; } 
        public DbSet<Fine> Fines { get; set; }
        public DbSet<SmsLog> SmsLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ❌ جلوگیری از Multiple Cascade Path در Fine → User
            modelBuilder.Entity<Fine>()
                .HasOne(f => f.User)
                .WithMany()
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            // ❌ جلوگیری از Cascade دوم Fine → Borrow
            modelBuilder.Entity<Fine>()
                .HasOne(f => f.Borrow)
                .WithMany()
                .HasForeignKey(f => f.BorrowId)
                .OnDelete(DeleteBehavior.NoAction);

            //// (اگر ChatMessage هم داری، اینا هم باید باشن)
            //modelBuilder.Entity<Message>()
            //    .HasOne(c => c.Sender)
            //    .WithMany()
            //    .HasForeignKey(c => c.SenderId)
            //    .OnDelete(DeleteBehavior.NoAction);

            //modelBuilder.Entity<Message>()
            //    .HasOne(c => c.Receiver)
            //    .WithMany()
            //    .HasForeignKey(c => c.ReceiverId)
            //    .OnDelete(DeleteBehavior.NoAction);
        }
    }
}

