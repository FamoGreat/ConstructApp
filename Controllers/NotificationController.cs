using ConstructApp.Data;
using ConstructApp.Models;
using ConstructApp.Models.ViewModels;
using ConstructApp.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ConstructApp.Controllers
{

    public class NotificationController : Controller, INotificationService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public NotificationController(ApplicationDbContext _dbContext, UserManager<ApplicationUser> userManager)
        {
            dbContext = _dbContext;
            _userManager = userManager;
        }

        public IActionResult ViewAll()
        {
            var notifications = dbContext.Notifications
                .Include(n => n.ApplicationUser)
                .Where(n => !n.IsRead)
                .OrderByDescending(n => n.Timestamp)
                .ToList();

            var senderIds = notifications.Select(n => n.SenderId).Distinct().ToList();
            var senders = dbContext.Users
                .Where(u => senderIds.Contains(u.Id))
                .ToDictionary(u => u.Id);

            var notificationViewModels = notifications.Select(n => new NotificationViewModel
            {
                Id = n.Id,
                Message = n.Message,
                Timestamp = n.Timestamp,
                IsRead = n.IsRead,

                Sender = senders.ContainsKey(n.SenderId) ? new SenderViewModel
                {
                    FirstName = senders[n.SenderId].FirstName,
                    LastName = senders[n.SenderId].LastName,
                    ProfileImage = senders[n.SenderId].ProfileImage
                } : null,
            }).ToList();

            return View(notificationViewModels);
        }

        public IActionResult Single(int id) 
        { 
            var notification = dbContext.Notifications
                .Include (n => n.ApplicationUser)
                .FirstOrDefault(n => n.Id == id);
            if (notification == null)
            {
                return NotFound();
            }
            return View(notification);
        }    

        public void SendNotification(int expenseId, IHttpContextAccessor httpContextAccessor)
        {
            var expense = dbContext.Expenses.FirstOrDefault(e => e.Id == expenseId);
            if (expense != null && expense.ApprovalStatus == ApprovalStatus.Pending)
            {
                var senderUserId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var user = _userManager.FindByIdAsync(senderUserId).Result;
                if (user != null)
                {
                    string message = $"Hello! expense request (Expese type: {expense.ExpenseType?.Name}) is pending approval. Please review and take necessary action.";

                    var notification = new Notification
                    {
                        SenderId = senderUserId,
                        Message = message,
                        Timestamp = DateTime.Now,
                        IsRead = false
                    };

                    dbContext.Notifications.Add(notification);
                    dbContext.SaveChanges();
                }
                
            }
        }

        [HttpGet]
        public IActionResult FetchNotifications()
        {
            var notifications = dbContext.Notifications
                .Where(n => !n.IsRead)
                .OrderByDescending(n => n.Timestamp)
                .ToList();

            var senderIds = notifications.Select(n => n.SenderId).Distinct().ToList();
            var senders = dbContext.Users
                .Where(u => senderIds.Contains(u.Id))
                .ToDictionary(u => u.Id);

            var formattedNotifications = notifications.Select(n => new
            {
                n.Id,
                Sender = senders.ContainsKey(n.SenderId) ? new SenderViewModel
                {
                    FirstName = senders[n.SenderId].FirstName,
                    LastName = senders[n.SenderId].LastName,
                    ProfileImage = senders[n.SenderId].ProfileImage
                } : null,
                n.Message,
                Timestamp = n.Timestamp.ToString("yyyy-MM-dd HH:mm:ss")
            }).ToList();

          
            return Json(formattedNotifications);
        }

        [HttpPost]
        public IActionResult MarkAsRead(int notificationId)
        {
            var notification = dbContext.Notifications.FirstOrDefault(n => n.Id == notificationId);
            if (notification != null)
            {
                notification.IsRead = true;
                dbContext.SaveChanges();
                return Ok();
            }
            return NotFound();
        }



    }
}
