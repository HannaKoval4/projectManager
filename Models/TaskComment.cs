using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectManager.Models
{
    [Table("TaskComments")]
    public class TaskComment
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public int TaskID { get; set; }

        [Required]
        public string Text { get; set; }

        public DateTime CreatedAt { get; set; }

        [MaxLength(100)]
        public string Author { get; set; }

        [ForeignKey("TaskID")]
        public virtual Task Task { get; set; }
    }
}
