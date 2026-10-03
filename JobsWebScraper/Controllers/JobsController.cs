using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JobsWebScraper.Data;
using JobsWebScraper.Models;
using JobsWebScraper.Services;

namespace JobsWebScraper.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobsController : ControllerBase
    {
        private readonly MyAPIContext _context;
        public JobsController(MyAPIContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<List<Job>>> Get(int page = 1, int pageSize = 20)
        {
            int skipNumber = (page - 1) * pageSize;

            return await _context.Job.Skip(skipNumber).Take(pageSize).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Job>> GetById(int id)
        {
            var job = await _context.Job.FindAsync(id);

            if (job == null)
            {
                return NotFound();
            }

            return Ok(job);
        }

        [HttpPost]
        public async Task<ActionResult<List<Job>>> Post(List<Job> jobs)
        {
            if (jobs == null)
            {
                return BadRequest();
            }

            _context.Job.AddRange(jobs);
            await _context.SaveChangesAsync();

            List<Job> createdJobs = [.. jobs];

            return CreatedAtAction(nameof(Post), createdJobs);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Job newJob)
        {
            var job = await _context.Job.FindAsync(id);

            if (job == null)
            {
                return NotFound();
            }

            job.Company = newJob.Company;
            job.City = newJob.City;
            job.Department = newJob.Department;
            job.Title = newJob.Title;
            job.Link = newJob.Link;
            job.DatePublished = newJob.DatePublished;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var job = await _context.Job.FindAsync(id);

            if (job == null)
            {
                return NotFound();
            }

            _context.Remove(job);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("bulk/{companyName}")]
        public async Task<IActionResult> Delete(string companyName)
        {
            await _context.Job.Where(j => j.Company == companyName).ExecuteDeleteAsync();

            return NoContent();
        }


    }
}
