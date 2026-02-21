using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectManager.Models
{
    [Table("Tasks")]
    public class Task
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public int ProjectID { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; }

        [Required]
        [MaxLength(50)]
        public string Priority { get; set; }

        public int? EmployeeID { get; set; }

        [ForeignKey("ProjectID")]
        public virtual Project Project { get; set; }

        [ForeignKey("EmployeeID")]
        public virtual Employee Employee { get; set; }
    }
}






