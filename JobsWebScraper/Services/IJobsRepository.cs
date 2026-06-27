using TaskManager.Models;

namespace TaskManager.Services
{
    public interface IJobsRepository
    {
        Task<List<Job>> GetAllJobs();
        Task<Job> GetJobById(int id);
        Task AddJob(Job newJob);
        Task PutJob(Job job, Job newJob);
        Task DeleteJob(Job job);
    }
}
