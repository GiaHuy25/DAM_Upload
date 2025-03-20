using DAM_Upload.Models;
using Microsoft.EntityFrameworkCore;

namespace DAM_Upload.db
{
    public class AppdbContext : DbContext 
    {
        public AppdbContext(DbContextOptions<AppdbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
    }
}
