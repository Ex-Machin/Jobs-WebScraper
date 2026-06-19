using TaskManager.Models;

namespace TaskManager.Services
{
    public interface IRepository
    {
        Task<List<MyTask>> GetAllTasks();
    }
}
