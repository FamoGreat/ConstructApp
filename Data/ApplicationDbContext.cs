using ConstructApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ConstructApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectMaterial> ProjectMaterials { get; set; }
        public DbSet<Material> Materials { get; set; }
        public DbSet<ProjectTool> ProjectTools { get; set; }
        public DbSet<ExpenseType> ExpenseTypes { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Approval> Approvals { get; set; }
        public DbSet<ProjectMaterialUpdateLog> ProjectMaterialUpdateLogs { get; set; }
        public DbSet<ProjectToolUpdateLog> ProjectToolUpdateLogs { get; set; }
        public DbSet<ProjectLog> ProjectLogs { get; set; }
        public DbSet<MaterialLog> MaterialLogs { get; set; }
        public DbSet<ExpenseTypeLog> ExpenseTypeLogs { get; set; }
        public DbSet<ExpenseLog> ExpenseLogs { get; set; }
        public DbSet<ApprovalLog> ApprovalLogs { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

          

        }

    }
}
