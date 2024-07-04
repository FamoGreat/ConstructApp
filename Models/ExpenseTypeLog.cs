namespace ConstructApp.Models
{
    public class ExpenseTypeLog
    {
        public int Id { get; set; }
        public int ExpenseTypeId { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Changes { get; set; }
        public string Reason { get; set; }
        public virtual ExpenseType ExpenseType { get; set; }
    }
}
