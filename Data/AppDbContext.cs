using Microsoft.EntityFrameworkCore;
using PostsBlogApi.Models;

namespace PostsBlogApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Article> Articles { get; set; }
    }
}