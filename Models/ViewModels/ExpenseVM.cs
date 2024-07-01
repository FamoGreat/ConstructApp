using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ConstructApp.Models.ViewModels
{
    public class ExpenseVM
    {
    
        public Expense Expense { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem>? ProjectList { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem>? ExpenseTypeList { get; set; }
        [ValidateNever]
        public string ChartType { get; set; }
        [DisplayName("Supportive Document")]
        public IFormFile? SupportiveDocument { get; set; }
    }

    public class ExpenseListVM
    {
        public List<Expense> Expenses { get; set; }
    }
}