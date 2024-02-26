using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentManagement.Domain.Models.Database
{
    [Table("teachers")]
    public class Teacher
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
    }
}