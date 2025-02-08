using donate.Models;
using Microsoft.EntityFrameworkCore;
using myapp.Models;

namespace myapp.Data
{
    public class myappContext : DbContext
    {
        public myappContext(DbContextOptions<myappContext> opt) :base(opt)
        {

        }
       
        public DbSet<Customer> Customers { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<OrderItem> OrderItems { get; set; } = null!;
        public DbSet<Category> Categorys { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Donation> Donations { get; set; } = null!;
        public DbSet<Request> Requests { get; set; } = null!;




    }
}
