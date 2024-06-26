namespace ConstructApp.Models.ViewModels
{
    public class PermissionVM
    {
        public string? RoleId { get; set; }
        public string? RoleName { get; set; }
        public IList<RoleClaimsVM>? RoleClaims { get; set; }
    }
}
