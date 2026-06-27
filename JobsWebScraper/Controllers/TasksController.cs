using Microsoft.AspNetCore.Mvc;
using TaskManager.Data;
using TaskManager.Models;
using TaskManager.Services;
using Microsoft.EntityFrameworkCore;

namespace TaskManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {

        private readonly ITaskRepository _repository;
        public TasksController(ITaskRepository repository)
        {
            _repository = repository;
        }


        [HttpGet]
        public async Task<ActionResult<List<MyTask>>> Get()
        {
            return Ok(await _repository.GetAllTasks());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MyTask>> GetByID(int id)
        {
            var task = await _repository.GetTaskById(id);

            if (task == null)
            {
                return NotFound();
            }

            return Ok(task);
        }

        [HttpPost]
        public async Task<ActionResult<MyTask>> Post(MyTask newTask)
        {
            if (newTask == null)
            {
                return BadRequest();
            }

            await _repository.AddTask(newTask);

            return CreatedAtAction(nameof(Post), new { id = newTask.Id });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, MyTask newTask)
        {
            var task = await _repository.GetTaskById(id);

            if (task == null)
            {
                return NotFound();
            }
            await _repository.PutTask(task, newTask);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var task = await _repository.GetTaskById(id);

            if (task == null)
            {
                return NotFound();
            }

            await _repository.DeleteTask(task);

            return NoContent();
        }


    }
}
