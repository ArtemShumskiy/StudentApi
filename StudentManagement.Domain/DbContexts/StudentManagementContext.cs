using Microsoft.EntityFrameworkCore;

using StudentManagement.Domain.Models.Database;

namespace StudentManagement.Domain.DbContexts
{
    public class StudentManagementContext : DbContext
    {
        public DbSet<Course> Courses { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teacherss { get; set; }



        public StudentManagementContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                _ = optionsBuilder.UseNpgsql("Host=localhost;Database=student_management;User ID=postgres;Password=sloshy;Port=5432;");
            }
        }

        public StudentManagementContext(DbContextOptions<StudentManagementContext> options) : base(options)
        {
        }

    }
}