using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Task_Management_System.Data;

namespace Task_Management_System.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(150)]
        [EmailAddress]
        public string Email { get; set; }

        public ICollection<TaskItem> Tasks { get; set; }
    }
}