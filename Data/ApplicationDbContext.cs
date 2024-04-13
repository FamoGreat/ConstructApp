﻿using ConstructApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ConstructApp.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectMaterial> ProjectMaterials { get; set; }
        public DbSet<ProjectTool> ProjectTools { get; set; }
        public DbSet<ExpenseType> ExpenseTypes { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Approval> Approvals { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Specify ON DELETE NO ACTION for the ProjectId foreign key constraint in the Approvals table
            modelBuilder.Entity<Approval>()
                        .HasOne(a => a.Project)
                        .WithMany()
                        .HasForeignKey(a => a.ProjectId)
                        .OnDelete(DeleteBehavior.NoAction);

        }

    }
}
