using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Models;


namespace TaskManager.Services
{
    public class TaskRepository : ITaskRepository
    {
        private readonly MyAPIContext _context;

        public TaskRepository(MyAPIContext context)
        {
            _context = context;
        }

        public async Task<List<MyTask>> GetAllTasks()
        {
            return await _context.MyTask.ToListAsync();
        }

        public async Task<MyTask> GetTaskById(int id)
        {
            return await _context.MyTask.FindAsync(id);
        }

        public async Task AddTask(MyTask newTask)
        {
            _context.MyTask.Add(newTask);
            await _context.SaveChangesAsync();
        }

        public async Task PutTask(MyTask task, MyTask newTask)
        {
            task.Title = newTask.Title;
            task.Description = newTask.Description;
            task.State = newTask.State;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteTask(MyTask task)
        {
            _context.MyTask.Remove(task);
            await _context.SaveChangesAsync();
        }
    }
}
