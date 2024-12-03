using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace AnimalCrossingAPI.Models
{
    public class YelpReviewDbContext : DbContext
    {
        public DbSet<Review> Reviews { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { 
            optionsBuilder.UseNpgsql(connectionString: "Server=localhost;Port=5432;User Id=postgres;Password=password;Database=yelpreviews;Include Error Detail=true;"); 
            base.OnConfiguring(optionsBuilder); 
        }
    }
}
