using Microsoft.AspNetCore.Mvc;
using TaskManager.Models;

namespace TaskManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {

        static private List<MyTask> tasks = new List<MyTask>
        {
            new MyTask
            {
                Id =  1, // auto increment
                Title = "Read Book",
                Description = "Head First Design Patterns",
                Status = Status.Created
            },
            new MyTask
            {
                Id =  2, // auto increment
                Title = "Read Book",
                Description = "Cracking the coding Interview",
                Status = Status.Created
            }
        };


        [HttpGet]
        public ActionResult<List<MyTask>> Get()
        {
            return Ok(tasks);
        }

        [HttpGet("{id}")]
        public ActionResult<MyTask> GetByID(int id)
        {
            MyTask task = tasks.FirstOrDefault(task => task.Id == id);

            if (task == null)
            {
                return NotFound();
            }

            return Ok(task);
        }

        [HttpPost]
        public ActionResult<MyTask> Post(MyTask newTask)
        {
            if (newTask == null)
            {
                return BadRequest();
            }

            tasks.Add(newTask);

            return CreatedAtAction(nameof(Post), new {id=newTask.Id});
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, MyTask newTask)
        {
            MyTask task = tasks.FirstOrDefault(x => x.Id == id);

            if (task == null)
            {
                return NotFound();
            }

            task.Id = newTask.Id;
            task.Title = newTask.Title;
            task.Description = newTask.Description;
            task.Status = newTask.Status;

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            MyTask task = tasks.FirstOrDefault(x => x.Id == id);

            if (task == null)
            {
                return NotFound();
            }

            tasks.Remove(task);

            return NoContent();
        }


    }
}
