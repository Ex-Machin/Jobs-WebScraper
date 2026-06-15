using Microsoft.EntityFrameworkCore;
using TaskManager.Models;

namespace TaskManager.Data
{
    public class MyAPIContext : DbContext
    {

        public MyAPIContext(DbContextOptions<MyAPIContext> options) : base(options) { }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MyTask>().HasData(
                 new MyTask
                 {
                     Id = 1, // auto increment
                     Title = "Read Book",
                     Description = "Head First Design Patterns",
                     State = TaskState.Created
                 },
            new MyTask
            {
                Id = 2, // auto increment
                Title = "Read Book",
                Description = "Cracking the coding Interview",
                State = TaskState.Created
            }
                );
        }
        public DbSet<MyTask> MyTask { get; set; }

    }
}
