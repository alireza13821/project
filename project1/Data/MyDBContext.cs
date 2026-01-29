using Microsoft.EntityFrameworkCore;
using project1.Models;

namespace project1.Data
{
    public class MyDBContext: DbContext 
    {
        public MyDBContext(DbContextOptions<MyDBContext> options):base(options)
        {

        }
        public DbSet<Book> Books { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Reserve> Reserves { get; set; }
        public DbSet <ReserveItem> ReserveItems { get; set; }


    }
}
