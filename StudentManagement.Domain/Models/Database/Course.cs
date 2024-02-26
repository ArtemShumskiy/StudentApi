using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentManagement.Domain.Models.Database
{
    [Table("courses")]
    public class Course
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Column("title")]
        [Required]
        [MaxLength(255)]
        public string Title { get; set; }

        [Column("description")]
        [Required]
        public string Description { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [Column("status")]
        public int Status { get; set; }

        [Column("assignee_id")]
        public long? AssigneeId { get; set; }

        [ForeignKey(nameof(AssigneeId))]
        public virtual Student Assignee { get; set; }
    }
}