using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectManager.Models
{
    [Table("Employees")]
    public class Employee
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [MaxLength(200)]
        public string FullName { get; set; }

        [Required]
        [MaxLength(100)]
        public string Position { get; set; }

        public virtual ICollection<Task> Tasks { get; set; }

        public Employee()
        {
            Tasks = new HashSet<Task>();
        }
    }
}






