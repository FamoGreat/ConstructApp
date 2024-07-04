using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ConstructApp.Models.ViewModels
{
    public class ProjectToolVM
    {
        public ProjectTool ProjectTool { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem>? ProjectList { get; set; }

        public string? UpdateReason { get; set; }
    }
}
