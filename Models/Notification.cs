namespace ConstructApp.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string? SenderId { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }
        public string? Message { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsRead { get; set; }
    }

    
}
