using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

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

        [MaxLength(300)]
        public string Skills { get; set; }

        [NotMapped]
        public string ShortFirstName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(FullName)) return string.Empty;
                var parts = FullName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                return parts.Length > 0 ? parts[0] : FullName;
            }
        }

        public virtual ICollection<Task> Tasks { get; set; }

        public Employee()
        {
            Tasks = new HashSet<Task>();
        }
    }
}







