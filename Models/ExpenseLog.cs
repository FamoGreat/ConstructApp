namespace ConstructApp.Models
{
    public class ExpenseLog
    {
        public int Id { get; set; }
        public int ExpenseId { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Changes { get; set; }
        public string Reason { get; set; }
        public virtual Expense Expense { get; set; }
    }
}
