using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using To_do_list.Data;
using To_do_list.Models;
using To_do_list.Repositories;

namespace To_do_list.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly IToDoRepository _toDoRepository;

        public TaskController(DataContext context, IToDoRepository toDoRepository)
        {
            _dataContext = context;
            _toDoRepository = toDoRepository;
        }

        [Authorize]
        [HttpPost("CreateNewTask")]
        public IActionResult CreateTask(string task, string startDate, string deadline, string status)
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value;
                if (userId == null) return Unauthorized();

                _toDoRepository.CreateTask(task, startDate, deadline, status, int.Parse(userId));

                return Ok("Task created successfully.");
            }
            catch (DbUpdateException)
            {
                return BadRequest("Failed to create task. Please try again later.");
            }
        }

        [Authorize]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var allTasks = await _toDoRepository.GetAllTasks();
                return Ok(allTasks);
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest("Error retrieving tasks.");
            }
        }


        [Authorize]
        [HttpGet("GetByID/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var task = await _dataContext.ToDoModel.FindAsync(id);
                return task != null ? Ok(task) : NotFound("Task not found.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [Authorize]
        [HttpGet("GetByTitle")]
        public async Task<IActionResult> GetByTitle(string title)
        {
            try
            {
                var task = await _toDoRepository.GetByTitle(title);
                return task != null ? Ok(task) : NotFound("Task not found.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [Authorize]
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            try
            {
                var task = await _dataContext.ToDoModel.FindAsync(id);
                if (task != null)
                {
                    _toDoRepository.DeleteTask(id);
                    return Ok("Task deleted successfully.");
                }
                return NotFound("Task not found.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest("Error deleting task.");
            }
        }

        [Authorize]
        [HttpPut("UpdateTask/{id}")]
        public async Task<IActionResult> UpdateTask(int id, string task, string startDate, string deadline, string status)
        {
            try
            {
                await _toDoRepository.UpdateTask(id, task, startDate, deadline, status);
                return Ok("Task updated successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest("Error updating task.");
            }
        }

        [Authorize]
        [HttpGet("Search")]
        public async Task<IActionResult> SearchTasks(string query)
        {
            var tasks = await _toDoRepository.SearchTasksByTaskStringAsync(query);
            return tasks != null && tasks.Count > 0 ? Ok(tasks) : NotFound("No tasks found with the given search string.");
        }
    }
}
