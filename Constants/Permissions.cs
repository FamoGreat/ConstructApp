namespace ConstructApp.Constants
{
    public static class Permissions
    {
        public static List<string> GeneratePermissionForModule(string module)
        {
            return new List<string>()
            {
                $"Permissions.{module}.Veiw",
                $"Permissions.{module}.Create",
                $"Permissions.{module}.Edit",
                $"Permissions.{module}.Delete",
            };
        }

    }

    public static class UserPermissions
    {
        public static string View = "Permissions.Users.View";
        public static string Create = "Permissions.Users.Create";
        public static string Edit = "Permissions.Users.Edit";
        public static string Delete = "Permissions.Users.Delete";
    }

    public static class ExpensePermissions
    {
        public const string View = "Permissions.Expenses.View";
        public const string Create = "Permissions.Expenses.Create";
        public const string Edit = "Permissions.Expenses.Edit";
        public const string Delete = "Permissions.Expenses.Delete";
    }

    public static class ApprovalPermissions
    {
        public const string View = "Permissions.Approvals.View";
        public const string Create = "Permissions.Approvals.Create";
        public const string Edit = "Permissions.Approvals.Edit";
        public const string Delete = "Permissions.Approvals.Delete";
    }
    public static class ProjectPermissions
    {
        public const string View = "Permissions.Projects.View";
        public const string Create = "Permissions.Projects.Create";
        public const string Edit = "Permissions.Projects.Edit";
        public const string Delete = "Permissions.Projects.Delete";
    }
}
