using ConstructApp.Data;
using ConstructApp.Models;

namespace ConstructApp.Seeds
{
    public static class DefaultExpenseType
    {
        public static void SeedExpenseType(ApplicationDbContext dbContext)
        {
            dbContext.Database.EnsureCreated();
            if (!dbContext.ExpenseTypes.Any())
            {
                var expenseType = new List<ExpenseType>
                {
                    new ExpenseType { Name = "Labor Costs" },
                    new ExpenseType { Name = "Materials Costs" },
                    new ExpenseType { Name = "Equipment Costs" }
                };

                dbContext.ExpenseTypes.AddRange(expenseType);
                dbContext.SaveChanges();
            }
        }
    }
}
