namespace ConstructApp.Models
{
    public class ProjectLog
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Changes { get; set; }
        public string Reason { get; set; }
        public virtual Project Project { get; set; }
    }
}
