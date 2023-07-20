using DEMO_Task_Management_System.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DEMO_Task_Management_System.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Tasks> Tasks { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<TaskAssignment> TaskAssignments { get; set; }

    }
}
