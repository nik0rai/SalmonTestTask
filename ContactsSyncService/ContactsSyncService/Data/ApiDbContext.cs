using ContactsSyncService.Models;
using Microsoft.EntityFrameworkCore;

namespace ContactsSyncService.Data
{
    public class ApiDbContext : DbContext
    {
        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
        {
            
        }

        public DbSet<Contacts> Contacts { get; set; }
        public DbSet<Address> Address { get; set; }
        public DbSet<Payments> Payments { get; set; }
    }
}
