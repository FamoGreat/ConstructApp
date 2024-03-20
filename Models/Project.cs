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
        public DateTime StartDate { get; set; }
        [Required]
        [DisplayName("End Date")]
        public DateTime EndDate { get; set; }
        [Required]
        [DisplayName("Created By")]
        public string? CreatedBy { get; set; }
        [Required]
        [DisplayName("Created Date")]
        public DateTime CreatedDate { get; set; }
        [Required]
        [DisplayName("Total Budget")]

        public decimal TotalBudget { get; set; }
        public virtual List<ProjectMaterial> ProjectMaterials { get; set; } = new List<ProjectMaterial>();
    }
}

