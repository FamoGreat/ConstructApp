namespace ConstructApp.Models
{
    public class ProjectToolUpdateLog
    {
        public int Id { get; set; }
        public int ProjectToolId { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Changes { get; set; }
        public string Reason { get; set; }
        public virtual ProjectTool ProjectTool { get; set; }
    }
}
