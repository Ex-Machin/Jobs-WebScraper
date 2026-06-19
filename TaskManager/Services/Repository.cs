using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Models;


namespace TaskManager.Services
{
    public class Repository : IRepository
    {
        private readonly MyAPIContext _context;

        public Repository(MyAPIContext context)
        {
            _context = context;
        }

        public async Task<List<MyTask>> GetAllTasks()
        {
            return await _context.MyTask.ToListAsync();
        }


    }
}
