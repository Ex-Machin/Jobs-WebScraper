using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Models;

namespace TaskManager.Controllers
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
        public async Task<ActionResult<List<Job>>> Get()
        {
            return Ok(await _context.Job.ToListAsync());
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
        public async Task<ActionResult<Job>> Post(Job job)
        {
            if (job == null)
            {
                return BadRequest();
            }

            _context.Job.Add(job);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Post), new { id = job.Id });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Job newJob)
        {
            var job = await _context.Job.FindAsync(id);

            if (job == null)
            {
                return NotFound();
            }

            job.Id = newJob.Id;
            job.Company = newJob.Company;
            job.Status = newJob.Status;
            job.InterviewRound = newJob.InterviewRound;

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

            _context.Job.Remove(job);
            await _context.SaveChangesAsync();

            return NoContent();
        }


    }
}
