using System.Collections.Generic;
using System.Threading.Tasks;
using To_do_list.Models;

namespace To_do_list.Repositories
{
    public interface IToDoRepository
    {
        // Create
        ToDoModel CreateTask(string task, string startDate, string deadline, string status, int userId);

        // Read
        Task<IEnumerable<ToDoModel>> GetAllTasks();
        Task<IEnumerable<ToDoModel>> GetByTitle(string name);
        Task<IEnumerable<ToDoModel>> GetById(int id);

        // Delete
        void DeleteTask(int id);

        // Update
        Task UpdateTask(int id, string task, string startDate, string deadline, string status);

        // Search
        Task<List<ToDoModel>> SearchTasksByTaskStringAsync(string taskString);
    }
}
