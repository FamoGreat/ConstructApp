using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ConstructApp.Models
{
    public class ProjectTool
    {
        public int Id { get; set; }
        [Required]
        [DisplayName("Tool Name")]
        public string? ToolName { get; set; }
        public int ProjectId { get; set; }

        public virtual Project? Project { get; set; }
    }
}
