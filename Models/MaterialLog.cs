namespace ConstructApp.Models
{
    public class MaterialLog
    {
        public int Id { get; set; }
        public int MaterialId { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Changes { get; set; }
        public string Reason { get; set; }
        public virtual Material Material { get; set; }
    }
}
