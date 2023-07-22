using DEMO_Task_Management_System.Domain.Entities.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DEMO_Task_Management_System.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
        }

        // DbSet for the Tasks table, which represents the tasks in the system
        public DbSet<Tasks> Tasks { get; set; }

        // DbSet for the User table, which represents the application users
        public DbSet<User> User { get; set; }

        // DbSet for the Projects table, which represents the projects in the system
        public DbSet<Project> Projects { get; set; }

        // DbSet for the TaskAssignments table, which represents task assignments to users
        public DbSet<TaskAssignment> TaskAssignments { get; set; }

        // DbSet for the Teams table, which represents teams in the system
        public DbSet<Team> Teams { get; set; }
    }
}
