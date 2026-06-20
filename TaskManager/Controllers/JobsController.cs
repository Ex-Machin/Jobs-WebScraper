using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Models;
using TaskManager.Services;

namespace TaskManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobsController : ControllerBase
    {
        private readonly IJobsRepository _repository;
        public JobsController(IJobsRepository repository)
        {
            _repository = repository;
        }
        [HttpGet]
        public async Task<ActionResult<List<Job>>> Get()
        {
            return Ok(await _repository.GetAllJobs());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Job>> GetById(int id)
        {
            var job = await _repository.GetJobById(id);

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

            await _repository.AddJob(job);

            return CreatedAtAction(nameof(Post), new { id = job.Id });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Job newJob)
        {
            var job = await _repository.GetJobById(id);

            if (job == null)
            {
                return NotFound();
            }

            await _repository.PutJob(job, newJob);


            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var job = await _repository.GetJobById(id);

            if (job == null)
            {
                return NotFound();
            }

            await _repository.DeleteJob(job);

            return NoContent();
        }


    }
}
