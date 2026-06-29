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
                     Id = 1,
                     Title = "Read Book",
                     Description = "Head First Design Patterns",
                     State = TaskState.Created
                 },
                new MyTask
                {
                    Id = 2,
                    Title = "Read Book",
                    Description = "Cracking the coding Interview",
                    State = TaskState.Created
                }
            );

            modelBuilder.Entity<Job>().HasData(
                new Job
                {
                    Id = 1,
                    Title = "Software Developer",
                    Company = "EPAM",
                    Status = 0,
                    InterviewRound = 1,
                    Departement = "asd",
                    Region = "sda",
                    City = "dfs"
                }
           );
        }
        public DbSet<MyTask> MyTask { get; set; }
        public DbSet<Job> Job { get; set; }

        public async Task<List<MyTask>> GetAllTasks()
        {
            return await MyTask.ToListAsync();
        }

    }
}
