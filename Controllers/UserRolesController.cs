using ConstructApp.Data;
using ConstructApp.Helpers;
using ConstructApp.Models;
using ConstructApp.Models.ViewModels;
using ConstructApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ConstructApp.Controllers
{
    [Authorize(Roles = "Admin")]

    public class UserRolesController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMailService mailService;
        public UserRolesController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IMailService mailService)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            this.mailService = mailService;
        }
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var userRolesVM = new List<UserRolesVM>();
            foreach (ApplicationUser user in users)
            {
                var thisViewModel = new UserRolesVM();
                thisViewModel.UserId = user.Id;
                thisViewModel.Email = user.Email;
                thisViewModel.FirstName = user.FirstName;
                thisViewModel.LastName = user.LastName;
                thisViewModel.Roles = await GetUserRoles(user);
                userRolesVM.Add(thisViewModel);
            }
            return View(userRolesVM);
        }

        private async Task<List<string>> GetUserRoles(ApplicationUser user)
        {
            return new List<string>(await _userManager.GetRolesAsync(user));
        }
        public async Task<IActionResult> Manage(string userId)
        {
            ViewBag.userId = userId;
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
                return View("NotFound");
            }

            ViewBag.UserName = user.UserName;
            var roles = await _roleManager.Roles.Select(role => new
            {
                RoleId = role.Id,
                RoleName = role.Name
            }).ToListAsync();

            var model = new List<ManageUserRolesVM>();
            foreach (var role in roles)
            {
                var userRoleViewModel = new ManageUserRolesVM
                {
                    RoleId = role.RoleId,
                    RoleName = role.RoleName,
                    Selected = await _userManager.IsInRoleAsync(user, role.RoleName)
                };
                model.Add(userRoleViewModel);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Manage(List<ManageUserRolesVM> model, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View();
            }
            var roles = await _userManager.GetRolesAsync(user);
            var result = await _userManager.RemoveFromRolesAsync(user, roles);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove user existing roles");
                return View(model);
            }
            result = await _userManager.AddToRolesAsync(user, model.Where(x => x.Selected).Select(y => y.RoleName));
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add selected roles to user");
                return View(model);
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Register()
        {
            var viewModel = new RegisterVM
            {
                Roles = new SelectList(_roleManager.Roles, "Name", "Name")
            };
            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = new ApplicationUser
                    {
                        UserName = model.Email,
                        Email = model.Email,
                        EmailConfirmed = true,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Signature = model.Signature,
                        PhoneNumber = model.PhoneNumber,
                        CanApproved = model.CanApproved,
                        CanRequestForSomeone = model.CanRequestForSomeone
                    };

                    user.IsApproved = false;
                    string password = PasswordHelper.GenerateRandomPassword();
                    model.Password = password;
                    var result = await _userManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, model.Role);
                        await mailService.SendEmailAsync(new MailRequest
                        {
                            ToEmail = user.Email,
                            Subject = "Welcome to Aidan Construction: Your Generated Password",
                            Body = $@"<!DOCTYPE html>
                                        <html lang=""en"">
                                        <head>
                                            <meta charset=""UTF-8"">
                                            <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                                            <title>Welcome Email</title>
                                            <style>
                                                body {{
                                                    font-family: Arial, sans-serif;
                                                    line-height: 1.6;
                                                    margin: 0;
                                                    padding: 0;
                                                }}
                                                .container {{
                                                    max-width: 600px;
                                                    margin: 20px auto;
                                                    padding: 20px;
                                                    border: 1px solid #ccc;
                                                    border-radius: 5px;
                                                    background-color: #f9f9f9;
                                                }}
                                                h1 {{
                                                    color: #333;
                                                }}
                                                p {{
                                                    margin-bottom: 20px;
                                                }}
                                                .button {{
                                                    display: inline-block;
                                                    padding: 10px 20px;
                                                    background-color: #007bff;
                                                    color: #fff;
                                                    text-decoration: none;
                                                    border-radius: 5px;
                                                }}
                                                .footer {{
                                                    margin-top: 20px;
                                                    padding-top: 20px;
                                                    border-top: 1px solid #ccc;
                                                }}
                                            </style>
                                        </head>
                                        <body>
                                            <div class=""container"">
                                                <h1>Welcome to Aidan Construction!</h1>
                                                <p>Hi {user.FirstName},</p>
                                                <p>We're thrilled to have you as a new member of our community. As part of the registration process, we have generated a password for you to access your account.</p>
                                                <p><strong>Here is your generated password:</strong> {model.Password}</p>
                                                <p>Please remember to keep this password secure and confidential. We recommend not sharing it with anyone. For added security, we encourage you to change your password after your first login.</p>
                                                <p>If you have any questions or need assistance, please don't hesitate to reach out to our support team at <a href=""[Contact Information]"">Contact Information</a>. We're here to help!</p>
                                                <p>Thank you once again for choosing Aidan Construction. We look forward to serving you and helping you achieve your construction goals.</p>
                                                <div class=""footer"">
                                                    <p>Best regards,<br/>[Your Name]<br/>Aidan Construction Team</p>
                                                </div>
                                            </div>
                                        </body>
                                        </html>"
                        });


                        return RedirectToAction("Index");

                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.Message);
            }
            model.Roles = new SelectList(_roleManager.Roles, "Name", "Name");
            return View(model);

        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string userId)
        {
            if (userId == null)
            {
                return NotFound();
            }
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                NotFound();
            }

            var viewModel = new RegisterVM
            {
                Id = userId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Roles = new SelectList(_roleManager.Roles, "Name", "Name"),
                Signature = user.Signature,
                CanApproved = user.CanApproved,
                CanRequestForSomeone = user.CanRequestForSomeone
            };
            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> EditUser(RegisterVM viewModel)
        {
            if (ModelState.IsValid)
            {
                var user = _userManager.Users.FirstOrDefault(u => u.Id == viewModel.Id);
                if (user == null)
                {
                    return NotFound();
                }
                user.FirstName = viewModel.FirstName;
                user.LastName = viewModel.LastName;
                user.Email = viewModel.Email;
                user.PhoneNumber = viewModel.PhoneNumber;
                user.Signature = viewModel.Signature;
                user.CanApproved = viewModel.CanApproved;
                user.CanRequestForSomeone = viewModel.CanRequestForSomeone;

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    TempData["success"] = "User Info Updated successfully";
                    return RedirectToAction("Index", "UserRoles");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }

            
            viewModel.Roles = new SelectList(_roleManager.Roles, "Name", "Name");
            TempData["error"] = "Error in Updating user";
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }
            return RedirectToAction("Index");
        }
    }
}
