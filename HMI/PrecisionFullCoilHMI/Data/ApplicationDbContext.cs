
using Microsoft.EntityFrameworkCore;
using PrecisionFullCoilHMI.Models;

namespace PrecisionFullCoilHMI.Data
{
 public class ApplicationDbContext : DbContext
    {
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<Job> Jobs { get; set; }
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }


}
}
