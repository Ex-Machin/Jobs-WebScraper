using Microsoft.AspNetCore.Mvc;
using TaskManager.Models;

namespace TaskManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobsController : ControllerBase
    {
        public static List<Job> jobs = new List<Job>
        {
            new Job
            {
                Id = 1,
                Company = "Epam",
                Status = JobStatus.Applied,
                InterviewRound = 0
            },
            new Job
            {
                Id = 1,
                Company = "Google",
                Status = JobStatus.Accepted,
                InterviewRound = 7
            }
        };


        [HttpGet]
        public ActionResult<List<Job>> Get()
        {
            return Ok(jobs);
        }

        [HttpGet("{id}")]
        public ActionResult<Job> GetById(int id)
        {
            var job = jobs.FirstOrDefault(x => x.Id == id);

            if (job == null)
            {
                return NotFound();
            }

            return Ok(job);
        }

        [HttpPost]
        public ActionResult<Job> Post(Job job)
        {
            if (job == null)
            {
                return BadRequest();
            }

            jobs.Add(job);

            return CreatedAtAction(nameof(Post), new {id=job.Id});
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, Job newJob)
        {
            var job = jobs.FirstOrDefault(x => x.Id == id);

            if (job == null)
            {
                return NotFound();
            }

            job.Id = newJob.Id;
            job.Company = newJob.Company;
            job.Status = newJob.Status;
            job.InterviewRound = newJob.InterviewRound;

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var job = jobs.FirstOrDefault(x => x.Id == id);

            if (job == null)
            {
                return NotFound();
            }

            jobs.Remove(job);

            return NoContent();
        }


    }
}
