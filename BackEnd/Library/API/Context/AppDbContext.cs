using API.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Context
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base (options)
        {

        }

        public DbSet<User> AppUsers { get; set; }
        public DbSet<Loan> Loans { get; set; }
        public DbSet<Librarian> Librarians { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Book> Books { get; set; }
    }
}
