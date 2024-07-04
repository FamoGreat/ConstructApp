namespace ConstructApp.Models
{
    public class ProjectMaterialUpdateLog
    {
        public int Id { get; set; }
        public int ProjectMaterialId { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Changes { get; set; }
        public string Reason { get; set; }
        public virtual ProjectMaterial ProjectMaterial { get; set; }
    }
}
