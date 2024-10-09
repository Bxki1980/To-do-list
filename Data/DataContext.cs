using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using To_do_list.Models;

namespace To_do_list.Data
{
    public class DataContext : DbContext
    {
        // Constructor for DataContext with DbContextOptions
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        // Define the DbSets for your models
        public DbSet<ToDoModel> ToDoModel { get; set; }
        public DbSet<UserModel> Users { get; set; }
    }
}
