using ConstructApp.Data;
using ConstructApp.Models;
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
                .OrderByDescending(n => n.Timestamp)
                .ToList();
            return View(notifications);
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
                var currentUserId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var user = _userManager.FindByIdAsync(currentUserId).Result;
                if (user != null)
                {
                    string message = $"Hell! expense request (Expese type: {expense.ExpenseType?.Name}) is pending approval. Please review and take necessary action.";

                    // Create a new notification
                    var notification = new Notification
                    {
                        UserId = currentUserId,
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
                .Include(n => n.ApplicationUser)
                .Where(n => !n.IsRead)
                .OrderByDescending(n => n.Timestamp)
                .ToList();

            var formattedNotifications = notifications.Select(n => new
            {
                n.Id,
                n.Message,
                Timestamp = n.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"),
                ImageUrl = n.ApplicationUser?.ProfileImage != null
                    ? $"data:image/*;base64,{Convert.ToBase64String(n.ApplicationUser.ProfileImage)}"
                    : "~/img/undraw_profile.svg",
                n.ApplicationUser?.UserName 
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
