using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectManager.Models
{
    [Table("Projects")]
    public class Project
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime? Deadline { get; set; }

        public virtual ICollection<Task> Tasks { get; set; }

        [NotMapped]
        public double CompletionPercentage
        {
            get
            {
                if (Tasks == null || Tasks.Count == 0) return 0;
                var completedTasks = Tasks.Count(t => t.Status == "Завершена");
                return (double)completedTasks / Tasks.Count * 100;
            }
        }

        public Project()
        {
            Tasks = new HashSet<Task>();
        }
    }
}


