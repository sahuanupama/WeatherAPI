using Microsoft.EntityFrameworkCore;

namespace WeatherApi.Models
    {
    public class WeatherApiDbContext : DbContext
        {

        public WeatherApiDbContext(DbContextOptions options) : base(options)
            {

            }
        public DbSet<Weather> Weathers { get; set; }
        public DbSet<Weather> ApiUsers { get; set; }
        
        }
    }

