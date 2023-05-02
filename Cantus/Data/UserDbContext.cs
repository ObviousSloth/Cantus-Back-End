namespace Cantus.Data
{
    using Cantus.Models;
    using Microsoft.EntityFrameworkCore;
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
    }
}
