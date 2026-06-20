using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Models;

namespace TaskManager.Services
{
    public class JobsRepository : IJobsRepository
    {
        private readonly MyAPIContext _context;

        public JobsRepository(MyAPIContext context)
        {
            _context = context; 
        }

        public async Task<List<Job>> GetAllJobs()
        {
            return await _context.Job.ToListAsync();
        }

        public async Task<Job> GetJobById(int id)
        {
            return await _context.Job.FindAsync(id);
        }

        public async Task AddJob(Job newJob)
        {
            _context.Job.Add(newJob);
            await _context.SaveChangesAsync();
        }

        public async Task PutJob(Job job, Job newJob)
        {
            job.Company = newJob.Company;
            job.Status = newJob.Status;
            job.InterviewRound = newJob.InterviewRound;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteJob(Job job)
        {
            _context.Remove(job);
            await _context.SaveChangesAsync();
        }
    }
}
