using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using To_do_list.Data;
using To_do_list.Models;

namespace To_do_list.Repositories
{
    public class ToDoRepository : IToDoRepository
    {
        private readonly DataContext _dataContext;

        public ToDoRepository(DataContext context)
        {
            _dataContext = context;
        }

        // Create a new task
        public ToDoModel CreateTask(string task, string startDate, string deadline, string status, int userId)
        {
            var newTask = new ToDoModel
            {
                Task = task,
                StartDate = startDate,
                Deadline = deadline,
                Status = status,
                UserID = userId
            };

            _dataContext.ToDoModel.Add(newTask);
            _dataContext.SaveChanges();
            return newTask;
        }

        // Delete a task
        public async Task DeleteTask(int id)
        {
            var task = await _dataContext.ToDoModel.FindAsync(id);
            if (task != null)
            {
                _dataContext.ToDoModel.Remove(task);
                await _dataContext.SaveChangesAsync();
            }
        }

        // Get all tasks
        public async Task<IEnumerable<ToDoModel>> GetAllTasks()
        {
            return await _dataContext.ToDoModel.OrderBy(x => x.ID).ToListAsync();
        }

        // Get tasks by title
        public async Task<IEnumerable<ToDoModel>> GetByTitle(string task)
        {
            return await _dataContext.ToDoModel.Where(x => x.Task == task).ToListAsync();
        }

        // Get tasks by ID
        public async Task<ToDoModel> GetById(int id)
        {
            return await _dataContext.ToDoModel.FindAsync(id);
        }

        // Update task details
        public async Task UpdateTask(int id, string task, string startDate, string deadline, string status)
        {
            var existingTask = await _dataContext.ToDoModel.FindAsync(id);
            if (existingTask != null)
            {
                existingTask.Task = task;
                existingTask.StartDate = startDate;
                existingTask.Deadline = deadline;
                existingTask.Status = status;

                _dataContext.ToDoModel.Update(existingTask);
                await _dataContext.SaveChangesAsync();
            }
        }

        // Search for tasks by string
        public async Task<List<ToDoModel>> SearchTasksByTaskStringAsync(string taskString)
        {
            return await _dataContext.ToDoModel
                .Where(t => t.Task.ToLower().Contains(taskString.ToLower()))
                .ToListAsync();
        }
    }
}
