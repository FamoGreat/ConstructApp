namespace ConstructApp.Models
{
    public class ApprovalLog
    {
        public int Id { get; set; }
        public int ApprovalId { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Changes { get; set; }
        public string Reason { get; set; }
        public virtual Approval Approval { get; set; }
    }
}
