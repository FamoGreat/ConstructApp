using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

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
        public double EstimatedQuantity { get; set; }

        [Required]
        [DisplayName("Estimated Cost")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal EstimatedCost { get; set; }

        [Required]
        [DisplayName("Unit Of Measurement")]
        public string? MaterialUOM { get; set; } // UnitOfMeasurement

        // Navigation property to link to the Project entity
        public int ProjectId { get; set; }

        public virtual Project? Project { get; set; }
    }
}
