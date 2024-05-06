using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ConstructApp.Models.ViewModels
{
    public class ProjectMaterialVM
    {
        public ProjectMaterial ProjectMaterial { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem>? ProjectList { get; set; }

    }
}
