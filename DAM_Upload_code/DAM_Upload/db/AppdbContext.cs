using DAM_Upload.Models;
using Microsoft.EntityFrameworkCore;

namespace DAM_Upload.db
{
    public class AppdbContext : DbContext 
    {
        public AppdbContext(DbContextOptions<AppdbContext> options) : base(options)
        {
        }
        public DbSet<DAM_Upload.Models.File> Files { get; set; }
        public DbSet<Folder> Folders { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<BlacklistedToken> BlacklistedTokens { get; set; }
    }
}
