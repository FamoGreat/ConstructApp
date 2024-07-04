using ConstructApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace ConstructApp.Helpers
{
    public static class ChangeTrackingHelper
    {
        private static readonly Dictionary<Type, PropertyInfo[]> _cachedProperties = new Dictionary<Type, PropertyInfo[]>();

        public static void LogChanges<T>(T existingObject, T updatedObject, EntityState state, string updateReason, DbContext dbContext, string userName) where T : class
        {
            var changes = new List<string>();

            switch (state)
            {
                case EntityState.Added:
                    LogAddedChanges(updatedObject, changes);
                    break;
                case EntityState.Modified:
                    LogModifiedChanges(existingObject, updatedObject, changes);
                    break;
                case EntityState.Deleted:
                    LogDeletedChanges(existingObject, changes);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, "Unsupported EntityState");
            }

            if (changes.Any())
            {
                if (typeof(T) == typeof(ProjectMaterial))
                {
                    var updateLog = new ProjectMaterialUpdateLog
                    {
                        UpdatedBy = userName,
                        UpdatedAt = DateTime.UtcNow,
                        Changes = string.Join("; ", changes),
                        Reason = updateReason,
                        ProjectMaterialId = (updatedObject as ProjectMaterial)?.Id ?? 0
                    };

                    dbContext.Set<ProjectMaterialUpdateLog>().Add(updateLog);
                }
                else if (typeof(T) == typeof(ProjectTool))
                {
                    var updateLog = new ProjectToolUpdateLog
                    {
                        UpdatedBy = userName,
                        UpdatedAt = DateTime.UtcNow,
                        Changes = string.Join("; ", changes),
                        Reason = updateReason,
                        ProjectToolId = (updatedObject as ProjectTool)?.Id ?? 0
                    };

                    dbContext.Set<ProjectToolUpdateLog>().Add(updateLog);
                }

                else if (typeof(T) == typeof(Project))
                {
                    var updateLog = new ProjectLog
                    {
                        UpdatedBy = userName,
                        UpdatedAt = DateTime.UtcNow,
                        Changes = string.Join("; ", changes),
                        Reason = updateReason,
                        ProjectId = (updatedObject as Project)?.Id ?? 0
                    };
                    dbContext.Set<ProjectLog>().Add(updateLog);

                }
                else if (typeof(T) == typeof(Expense))
                {
                    var updateLog = new ExpenseLog
                    {
                        UpdatedBy = userName,
                        UpdatedAt = DateTime.UtcNow,
                        Changes = string.Join("; ", changes),
                        Reason = updateReason,
                        ExpenseId = (updatedObject as Expense)?.Id ?? 0
                    };
                    dbContext.Set<ExpenseLog>().Add(updateLog);

                }
                else if (typeof(T) == typeof(ExpenseType))
                {
                    var updateLog = new ExpenseTypeLog
                    {
                        UpdatedBy = userName,
                        UpdatedAt = DateTime.UtcNow,
                        Changes = string.Join("; ", changes),
                        Reason = updateReason,
                        ExpenseTypeId = (updatedObject as ExpenseType)?.Id ?? 0
                    };
                    dbContext.Set<ExpenseTypeLog>().Add(updateLog);

                }
                else if (typeof(T) == typeof(Approval))
                {
                    var updateLog = new ApprovalLog
                    {
                        UpdatedBy = userName,
                        UpdatedAt = DateTime.UtcNow,
                        Changes = string.Join("; ", changes),
                        Reason = updateReason,
                        ApprovalId = (updatedObject as Approval)?.Id ?? 0
                    };
                    dbContext.Set<ApprovalLog>().Add(updateLog);

                }
                else if (typeof(T) == typeof(Material))
                {
                    var updateLog = new MaterialLog
                    {
                        UpdatedBy = userName,
                        UpdatedAt = DateTime.UtcNow,
                        Changes = string.Join("; ", changes),
                        Reason = updateReason,
                        MaterialId = (updatedObject as Material)?.Id ?? 0
                    };
                    dbContext.Set<MaterialLog>().Add(updateLog);

                }

                dbContext.SaveChanges();
            }
        }

        private static void LogAddedChanges<T>(T updatedObject, List<string> changes)
        {
            var properties = GetProperties<T>();
            foreach (var property in properties)
            {
                var updatedValue = GetValueAsString(property.GetValue(updatedObject));
                changes.Add($"{property.Name}: {updatedValue}");
            }
        }

        private static void LogModifiedChanges<T>(T existingObject, T updatedObject, List<string> changes)
        {
            var properties = GetProperties<T>();
            foreach (var property in properties)
            {
                var existingValue = GetValueAsString(property.GetValue(existingObject));
                var updatedValue = GetValueAsString(property.GetValue(updatedObject));

                if (existingValue != updatedValue)
                {
                    changes.Add($"{property.Name}: {existingValue} -> {updatedValue}");
                }
            }
        }

        private static void LogDeletedChanges<T>(T existingObject, List<string> changes)
        {
            var properties = GetProperties<T>();
            foreach (var property in properties)
            {
                var existingValue = GetValueAsString(property.GetValue(existingObject));
                changes.Add($"{property.Name}: {existingValue}");
            }
        }

        private static PropertyInfo[] GetProperties<T>()
        {
            if (!_cachedProperties.ContainsKey(typeof(T)))
            {
                _cachedProperties[typeof(T)] = typeof(T).GetProperties();
            }

            return _cachedProperties[typeof(T)];
        }

        private static string GetValueAsString(object? value)
        {
            if (value == null)
            {
                return "null";
            }

            if (value is Enum)
            {
                return value.ToString();
            }

            return value.ToString() ?? "null";
        }
    }
}
