namespace ConstructApp.Models.ViewModels
{
    public class NotificationViewModel
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsRead { get; set; }
        public SenderViewModel Sender { get; set; }
    }

    public class SenderViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public byte[] ProfileImage { get; set; }
    }
}
