using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public DateTime? StartDate { get; set; }

        [MaxLength(200)]
        public string Client { get; set; }

        [MaxLength(50)]
        public string ProjectStatus { get; set; }

        [MaxLength(50)]
        public string ProjectPriority { get; set; }

        public decimal? Budget { get; set; }

        [MaxLength(500)]
        public string Tags { get; set; }

        public int? ResponsibleEmployeeID { get; set; }

        [ForeignKey("ResponsibleEmployeeID")]
        public virtual Employee ResponsibleEmployee { get; set; }

        /// <summary>Личные заметки и важная информация по проекту.</summary>
        public string Notes { get; set; }

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

        /// <summary>Есть ли хотя бы одна задача и все задачи в статусе «Завершена».</summary>
        [NotMapped]
        public bool IsCompleted
        {
            get
            {
                if (Tasks == null || Tasks.Count == 0) return false;
                return Tasks.All(t => t.Status == "Завершена");
            }
        }

        [NotMapped]
        public int ActiveTasksCount
        {
            get
            {
                if (Tasks == null) return 0;
                return Tasks.Count(t => t.Status != "Завершена");
            }
        }

        [NotMapped]
        public string UiShortName
        {
            get
            {
                if (string.IsNullOrEmpty(Name)) return string.Empty;
                if (Name.IndexOf("портал", StringComparison.OrdinalIgnoreCase) >= 0) return "Портал";
                if (Name.IndexOf("CRM", StringComparison.OrdinalIgnoreCase) >= 0) return "CRM";
                if (Name.IndexOf("HR", StringComparison.OrdinalIgnoreCase) >= 0) return "HR";
                return Name.Length > 20 ? Name.Substring(0, 17) + "…" : Name;
            }
        }

        public Project()
        {
            Tasks = new HashSet<Task>();
        }
    }
}


