using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConstructApp.Models
{
    public class ProjectTool
    {
        public int Id { get; set; }
        [Required]
        [DisplayName("Tool Name")]
        public string? ToolName { get; set; }
        [DisplayName("Tool Description")]
        public string? ToolDescription { get; set; }

        [Required]
        [DisplayName("Numbers of Tools")]
        public decimal ToolsQuantity { get; set; }

        [Required]
        [DisplayName("Tool Cost")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal ToolCost { get; set; }
        public int ProjectId { get; set; }

        public virtual Project? Project { get; set; }
    }
}
