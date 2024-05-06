using ConstructApp.Helpers;
using ConstructApp.Models.ViewModels;
using ConstructApp.Seeds;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace ConstructApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PermissionController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IAuthorizationService _authorizationService;
        public PermissionController(RoleManager<IdentityRole> roleManager, IAuthorizationService authorizationService)
        {
            _roleManager = roleManager;
            _authorizationService = authorizationService;
        }
        public async Task<ActionResult> Index(string roleId)
        {
            if ((_authorizationService.AuthorizeAsync(User, Constants.Permissions.UserPermissions.View)).Result.Succeeded)
            {

                var model = new PermissionVM();
                var allPermissions = new List<RoleClaimsVM>();
                allPermissions.GetPermissions(typeof(Constants.Permissions.UserPermissions), roleId);
                allPermissions.GetPermissions(typeof(Constants.Permissions.ExpensePermissions), roleId);
                allPermissions.GetPermissions(typeof(Constants.Permissions.ApprovalPermissions), roleId);
                allPermissions.GetPermissions(typeof(Constants.Permissions.ProjectPermissions), roleId);
                allPermissions.GetPermissions(typeof(Constants.Permissions.ProjectMaterialPermissions), roleId);

                var role = await _roleManager.FindByIdAsync(roleId);
                model.RoleId = roleId;
                var claims = await _roleManager.GetClaimsAsync(role);
                var allClaimValues = allPermissions.Select(a => a.Value).ToList();
                var roleClaimValues = claims.Select(a => a.Value).ToList();
                var authorizedClaims = allClaimValues.Intersect(roleClaimValues).ToList();
                foreach (var permission in allPermissions)
                {
                    if (authorizedClaims.Any(a => a == permission.Value))
                    {
                        permission.Selected = true;
                    }
                }
                model.RoleClaims = allPermissions;
                TempData["success"] = "Permission(s) added successfully";

                return View(model);
            }
            return RedirectToAction("Index", "ErrorPermission");
        }
        public async Task<IActionResult> Update(PermissionVM model)
        {
            var role = await _roleManager.FindByIdAsync(model.RoleId);
            var claims = await _roleManager.GetClaimsAsync(role);
            foreach (var claim in claims)
            {
                await _roleManager.RemoveClaimAsync(role, claim);
            }
            var selectedClaims = model.RoleClaims.Where(a => a.Selected).ToList();
            foreach (var claim in selectedClaims)
            {
                await _roleManager.AddPermissionClaims(role, claim.Value);
            }
            return RedirectToAction("Index", new { roleId = model.RoleId });
        }
    }

}
