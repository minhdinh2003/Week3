using Microsoft.EntityFrameworkCore;
using Week3.Models;
using Week3.MongoModels;
namespace Week3.Data
{

    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Reviewer> Reviewers { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<Review>();

            base.OnModelCreating(modelBuilder);
        }


    }


}
