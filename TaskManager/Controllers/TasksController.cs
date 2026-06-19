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

        private readonly MyAPIContext _context;
        private readonly IRepository _repository;
        public TasksController(
            MyAPIContext context,
            IRepository repository
        )
        {
            _context = context;
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
            var task = await _context.MyTask.FindAsync(id);

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

            _context.MyTask.Add(newTask);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Post), new { id = newTask.Id });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, MyTask newTask)
        {
            var task = await _context.MyTask.FindAsync(id);

            if (task == null)
            {
                return NotFound();
            }

            task.Id = newTask.Id;
            task.Title = newTask.Title;
            task.Description = newTask.Description;
            task.State = newTask.State;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var task = await _context.MyTask.FindAsync(id);

            if (task == null)
            {
                return NotFound();
            }

            _context.MyTask.Remove(task);
            await _context.SaveChangesAsync();

            return NoContent();
        }


    }
}
