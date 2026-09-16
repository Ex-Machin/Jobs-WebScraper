using Microsoft.EntityFrameworkCore;
using JobsWebScraper.Models;

namespace JobsWebScraper.Data
{
    public class MyAPIContext : DbContext
    {

        public MyAPIContext(DbContextOptions<MyAPIContext> options) : base(options) { }

        public DbSet<Job> Job { get; set; }

    }
}
