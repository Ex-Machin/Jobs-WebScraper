using Microsoft.EntityFrameworkCore;
using TaskManager.Models;

namespace TaskManager.Data
{
    public class MyAPIContext : DbContext
    {

        public MyAPIContext(DbContextOptions<MyAPIContext> options) : base(options) { }

        public DbSet<Job> Job { get; set; }

    }
}
