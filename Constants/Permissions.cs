using System.Collections.Generic;


namespace ConstructApp.Constants
{
    public static class Permissions
    {
        public static List<string> GeneratePermissionsForModule(string module)
        {
            return new List<string>()
            {
                $"Permissions.{module}.Veiw",
                $"Permissions.{module}.Create",
                $"Permissions.{module}.Edit",
                $"Permissions.{module}.Delete",
            };
        }

        public static class UserPermissions
        {
            public const string View = "Permissions.Users.View";
            public const string Create = "Permissions.Users.Create";
            public const string Edit = "Permissions.Users.Edit";
            public const string Delete = "Permissions.Users.Delete";
        }

        public static class ExpensePermissions
        {
            public const string View = "Permissions.Expenses.View";
            public const string Create = "Permissions.Expenses.Create";
            public const string Edit = "Permissions.Expenses.Edit";
            public const string Delete = "Permissions.Expenses.Delete";
        }

        public static class ExpenseTypePermissions
        {
            public const string View = "Permissions.ExpenseTypes.View";
            public const string Create = "Permissions.ExpenseTypes.Create";
            public const string Edit = "Permissions.ExpenseTypes.Edit";
            public const string Delete = "Permissions.ExpenseTypes.Delete";
        }

        public static class ApprovalPermissions
        {
            public const string View = "Permissions.Approvals.View";
            public const string Create = "Permissions.Approvals.Create";
            public const string Edit = "Permissions.Approvals.Edit";
            public const string Delete = "Permissions.Approvals.Delete";
            public const string Approve = "Permissions.Approvals.Approve";

        }
        public static class ProjectPermissions
        {
            public const string View = "Permissions.Projects.View";
            public const string Create = "Permissions.Projects.Create";
            public const string Edit = "Permissions.Projects.Edit";
            public const string Delete = "Permissions.Projects.Delete";
        }
        public static class ProjectMaterialPermissions
        {
            public const string View = "Permissions.ProjectMaterials.View";
            public const string Create = "Permissions.ProjectMaterials.Create";
            public const string Edit = "Permissions.ProjectMaterials.Edit";
            public const string Delete = "Permissions.ProjectMaterials.Delete";
        }
        public static class ProjectToolPermissions
        {
            public const string View = "Permissions.ProjectTools.View";
            public const string Create = "Permissions.ProjectTools.Create";
            public const string Edit = "Permissions.ProjectTools.Edit";
            public const string Delete = "Permissions.ProjectTools.Delete";
        }

        public static class ToolPermissions
        {
            public const string View = "Permissions.Tools.View";
            public const string Create = "Permissions.Tools.Create";
            public const string Edit = "Permissions.Tools.Edit";
            public const string Delete = "Permissions.Tools.Delete";
        }

        public static class MaterialPermissions
        {
            public const string View = "Permissions.Materials.View";
            public const string Create = "Permissions.Materials.Create";
            public const string Edit = "Permissions.Materials.Edit";
            public const string Delete = "Permissions.Materials.Delete";
        }
    }
}
