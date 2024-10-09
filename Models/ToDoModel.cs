using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace To_do_list.Models
{
    public class ToDoModel
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public string Task { get; set; }

        [Required]
        public string StartDate { get; set; }

        public string Deadline { get; set; }

        [Required]
        public string Status { get; set; }


        // Foreign key to link tasks to a specific user
        [ForeignKey("UserModel")]
        public int UserID { get; set; }

        // Navigation property
        public UserModel User { get; set; }
    }
}
