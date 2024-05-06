using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using ConstructApp.Constants;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ConstructApp.Models
{
    public class ProjectMaterial
    {
        public int Id { get; set; }

        [Required]
        [DisplayName("Material Code")]
        public string? MaterialCode { get; set; }

        [Required]
        [DisplayName("Material Name")]
        public string? MaterialName { get; set; }

        [Required]
        [DisplayName("Estimated Quantity")]
        public decimal EstimatedQuantity { get; set; }

        [Required]
        [DisplayName("Estimated Cost")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal EstimatedCost { get; set; }

        [Required]
        [DisplayName("Unit Of Measurement")]
        public UnitOfMeasurement MaterialUOM { get; set; }
        [NotMapped]
        public string? MaterialUOMString
        {
            get => Enum.GetName(typeof(UnitOfMeasurement), MaterialUOM);
            set => MaterialUOM = (UnitOfMeasurement)Enum.Parse(typeof(UnitOfMeasurement), value);
        }
        [DisplayName("Project")]
        [ValidateNever]
        [ForeignKey("ProjectId")]
        public int ProjectId { get; set; }
        public virtual Project? Project { get; set; }
    }
}
