using ConstructApp.Data;
using ConstructApp.Models;
using ConstructApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConstructApp.Controllers
{
    public class NotificationController : Controller, INotificationService
    {
        private readonly ApplicationDbContext dbContext;
        public NotificationController(ApplicationDbContext _dbContext)
        {
            dbContext = _dbContext;
        }


      
        public void SendNotification(int expenseId)
        {
            var expense = dbContext.Expenses.FirstOrDefault(e => e.Id == expenseId);
            if (expense != null && expense.ApprovalStatus == ApprovalStatus.Pending)
            {
                string message = $"Hell! expense request (Expese type: {expense.ExpenseType?.Name}) is pending approval. Please review and take necessary action.";

                // Create a new notification
                var notification = new Notification
                {
                    UserId = expense.CreatedBy,
                    Message = message,
                    Timestamp = DateTime.Now,
                    IsRead = false
                };

                dbContext.Notifications.Add(notification);
                dbContext.SaveChanges();
            }
        }

        [HttpGet]
        public IActionResult FetchNotifications()
        {
            // Fetch unread notifications from the database
            var notifications = dbContext.Notifications
                .Include(n => n.ApplicationUser) 
                .Where(n => !n.IsRead)
                .OrderByDescending(n => n.Timestamp) 
                .ToList();

            var formattedNotifications = notifications.Select(n => new
            {
                Message = n.Message,
                Timestamp = n.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"),
                ImageUrl = n.ApplicationUser?.ProfileImage 
            });

            return Json(formattedNotifications);
        }

    }
}
