using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


using Microsoft.EntityFrameworkCore;

namespace StudentManagement.Domain.Models.Database
{
    [Table("students")]
    [Index(nameof(Email), IsUnique = true)]
    public class Student
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Column("email")]
        [Required]
        [MaxLength(255)]
        [EmailAddress]
        public string Email { get; set; }

        [Column("assignee_idst")]
        public long AssigneeIdst { get; set; }

        [ForeignKey(nameof(AssigneeIdst))]
        public virtual Teacher Assigneest { get; set; }
    }
}