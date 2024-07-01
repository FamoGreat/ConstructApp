using ConstructApp.Constants;
using System.ComponentModel;

namespace ConstructApp.Models
{
    public class Material
    {
        public int Id { get; set; }

        [DisplayName("Material Code")]
        public string Code { get; set; }

        [DisplayName("Material Name")]
        public string Name { get; set; }

        [DisplayName("Material Type")]
        public string Type { get; set; }

        [DisplayName("Unit Of Measurement")]
        public UnitOfMeasurement UnitOfMeasurement { get; set; }
    }

}
