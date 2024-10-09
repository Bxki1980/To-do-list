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

        // Create a new task
        [Authorize]
        [HttpPost("CreateNewTask")]
        public IActionResult CreateTask(string task, string startDate, string deadline, string status)
        {
            try
            {
                // Retrieve the User ID from JWT claims
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

        // Get all tasks
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
                return BadRequest("Error retrieving tasks.");
            }
        }

        // Get task by ID
        [Authorize]
        [HttpGet("GetByID/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var task = await _toDoRepository.GetById(id);
                return task != null ? Ok(task) : NotFound("Task not found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // Get task by title
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
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // Delete a task
        [Authorize]
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            try
            {
                var task = await _toDoRepository.GetById(id);
                if (task != null)
                {
                    await _toDoRepository.DeleteTask(id);
                    return Ok("Task deleted successfully.");
                }
                return NotFound("Task not found.");
            }
            catch (Exception ex)
            {
                return BadRequest("Error deleting task.");
            }
        }

        // Update a task
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
                return BadRequest("Error updating task.");
            }
        }

        // Search tasks by query
        [Authorize]
        [HttpGet("Search")]
        public async Task<IActionResult> SearchTasks(string query)
        {
            var tasks = await _toDoRepository.SearchTasksByTaskStringAsync(query);
            return tasks != null && tasks.Count > 0 ? Ok(tasks) : NotFound("No tasks found with the given search string.");
        }
    }
}
