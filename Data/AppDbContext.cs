using Microsoft.EntityFrameworkCore;
using Task_Management_System.Models;

namespace Task_Management_System.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<TaskItem> Tasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // تطبيق قاعدة: منع حذف الموظف لو عنده مهام
            modelBuilder.Entity<TaskItem>()
                .HasOne(t => t.Employee)
                .WithMany(e => e.Tasks)
                .HasForeignKey(t => t.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}