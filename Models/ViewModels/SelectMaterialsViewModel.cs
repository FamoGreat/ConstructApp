using Microsoft.AspNetCore.Mvc.Rendering;

namespace ConstructApp.Models.ViewModels
{
    public class SelectMaterialsViewModel
    {
        public int ProjectId { get; set; }
        public string SelectedType { get; set; }
        public List<Material> Materials { get; set; }
        public List<int> SelectedMaterialIds { get; set; } = new List<int>();

        public IEnumerable<SelectListItem> ProjectList { get; set; }
    }
}
