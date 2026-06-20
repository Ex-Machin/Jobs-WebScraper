using TaskManager.Models;

namespace TaskManager.Services
{
    public interface ITaskRepository
    {
        Task<List<MyTask>> GetAllTasks();
        Task<MyTask> GetTaskById(int id);
        Task AddTask(MyTask newTask);
        Task PutTask(MyTask task, MyTask newTask);
        Task DeleteTask(MyTask task);
    }
}
