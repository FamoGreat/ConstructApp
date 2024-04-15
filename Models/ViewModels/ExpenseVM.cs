using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ConstructApp.Models.ViewModels
{
    public class ExpenseVM
    {
    
        public Expense Expense { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem>? ProjectList { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem>? ExpenseTypeList { get; set; }

    }
}