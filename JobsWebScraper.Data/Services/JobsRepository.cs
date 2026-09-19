using Microsoft.EntityFrameworkCore;
using JobsWebScraper.Data;
using JobsWebScraper.Models;

namespace JobsWebScraper.Services
{
    public class JobsRepository : IJobsRepository
    {
        private readonly MyAPIContext _context;

        public JobsRepository(MyAPIContext context)
        {
            _context = context; 
        }

        public async Task<List<Job>> GetAllJobs(int page = 1, int pageSize = 20)
        {
            int skipNumber = (page - 1) * pageSize;

            return await _context.Job.Skip(skipNumber).Take(pageSize).ToListAsync();
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

        public async Task AddJobs(List<Job> jobs)
        {
            _context.Job.AddRange(jobs);
            await _context.SaveChangesAsync();
        }

        public async Task PutJob(Job job, Job newJob)
        {
            job.Company = newJob.Company;
            job.City = newJob.City;
            job.Department = newJob.Department;
            job.Title = newJob.Title;
            job.Link = newJob.Link;
            job.DatePublished = newJob.DatePublished;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteJob(Job job)
        {
            _context.Remove(job);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteJobByCompany(string companyName)
        {
            await _context.Job.Where(j => j.Company == companyName).ExecuteDeleteAsync();
        }
    }
}
