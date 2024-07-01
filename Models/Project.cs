using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ConstructApp.Models
{
    public class Project
    {
        public int Id { get; set; }
        [Required]
        [DisplayName("Project Name")]
        public string? ProjectName { get; set; }
        [Required]
        public string? Location { get; set; }
        [Required]
        [DisplayName("Start Date")]
        public DateTime StartDate { get; set; } = DateTime.Today;
        [Required]
        [DisplayName("End Date")]
        public DateTime EndDate { get; set; } = DateTime.Today;
        [Required]
        [DisplayName("Created By")]
        public string? CreatedBy { get; set; }
        [Required]
        [DisplayName("Created Date")]
        public DateTime CreatedDate { get; set; } = DateTime.Today;
        [Required]
        [DisplayName("Total Budget")]

        public decimal TotalBudget { get; set; }
        [DisplayName("Project Description")]
        public string? ProjectDescription { get; set; }
        public decimal TotalMaterialExpense { get; set; }
        public decimal TotalToolExpense { get; set; }
        public virtual List<ProjectMaterial> ProjectMaterials { get; set; } = new List<ProjectMaterial>();
        public virtual List<ProjectTool> ProjectTools { get; set; } = new List<ProjectTool>();
    }
}

