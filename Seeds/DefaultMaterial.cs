using ConstructApp.Constants;
using ConstructApp.Data;
using ConstructApp.Models;

namespace ConstructApp.Seeds
{
    public static class DefaultMaterial
    {
        public static void SeedMaterials(ApplicationDbContext dbContext)
        {
            if (!dbContext.Materials.Any())
            {
                var materials = GetMaterials();
                dbContext.Materials.AddRange(materials);
                dbContext.SaveChanges();
            }
        }

        private static List<Material> GetMaterials()
        {
            return new List<Material>
        {
            // Structural Materials
            new Material { Code = "001", Name = "Concrete", Type = "Structural", UnitOfMeasurement = UnitOfMeasurement.CubicMeters },
            new Material { Code = "002", Name = "Steel", Type = "Structural", UnitOfMeasurement = UnitOfMeasurement.Tons },
            new Material { Code = "003", Name = "Wood", Type = "Structural", UnitOfMeasurement = UnitOfMeasurement.Pieces },
            new Material { Code = "004", Name = "Masonry (Bricks, Blocks, Stone)", Type = "Structural", UnitOfMeasurement = UnitOfMeasurement.Pieces },
            new Material { Code = "005", Name = "Reinforced Concrete", Type = "Structural", UnitOfMeasurement = UnitOfMeasurement.CubicMeters },
            
            // Finishing Materials
            new Material {  Code = "006", Name = "Drywall", Type = "Finishing", UnitOfMeasurement = UnitOfMeasurement.Sheets },
            new Material { Code = "007", Name = "Plaster", Type = "Finishing", UnitOfMeasurement = UnitOfMeasurement.SquareMeters },
            new Material { Code = "008", Name = "Insulation (Fiberglass, Foam, etc.)", Type = "Finishing", UnitOfMeasurement = UnitOfMeasurement.SquareFeet },
            new Material { Code = "009", Name = "Paint", Type = "Finishing", UnitOfMeasurement = UnitOfMeasurement.Gallons },
            new Material { Code = "010", Name = "Tiles (Ceramic, Porcelain, etc.)", Type = "Finishing", UnitOfMeasurement = UnitOfMeasurement.SquareMeters },
            new Material { Code = "011", Name = "Wallpaper", Type = "Finishing", UnitOfMeasurement = UnitOfMeasurement.SquareYards },
            new Material { Code = "012", Name = "Flooring (Hardwood, Laminate, Vinyl, Carpet, etc.)", Type = "Finishing", UnitOfMeasurement = UnitOfMeasurement.SquareFeet },
            
            // Roofing Materials
            new Material { Code = "013", Name = "Asphalt Shingles", Type = "Roofing", UnitOfMeasurement = UnitOfMeasurement.SquareFeet },
            new Material { Code = "014", Name = "Metal Roofing", Type = "Roofing", UnitOfMeasurement = UnitOfMeasurement.SquareMeters },
            new Material { Code = "015", Name = "Clay or Concrete Tiles", Type = "Roofing", UnitOfMeasurement = UnitOfMeasurement.SquareYards },
            new Material { Code = "016", Name = "Slate", Type = "Roofing", UnitOfMeasurement = UnitOfMeasurement.SquareFeet },
            
            // Siding Materials
            new Material { Code = "017", Name = "Vinyl Siding", Type = "Siding", UnitOfMeasurement = UnitOfMeasurement.SquareMeters },
            new Material {          Code = "018", Name = "Wood Siding", Type = "Siding", UnitOfMeasurement = UnitOfMeasurement.SquareFeet },
            new Material { Code = "019", Name = "Fiber Cement", Type = "Siding", UnitOfMeasurement = UnitOfMeasurement.SquareYards },
            new Material { Code = "020", Name = "Brick Veneer", Type = "Siding", UnitOfMeasurement = UnitOfMeasurement.SquareFeet },
            
            // Plumbing Materials
            new Material { Code = "021", Name = "PVC Pipes", Type = "Plumbing", UnitOfMeasurement = UnitOfMeasurement.Meters },
            new Material { Code = "022", Name = "Copper Pipes", Type = "Plumbing", UnitOfMeasurement = UnitOfMeasurement.Meters },
            new Material { Code = "023", Name = "PEX Pipes", Type = "Plumbing", UnitOfMeasurement = UnitOfMeasurement.Meters },
            
            // Electrical Materials
            new Material { Code = "024", Name = "Wiring (Copper, Aluminum)", Type = "Electrical", UnitOfMeasurement = UnitOfMeasurement.Feet },
            new Material { Code = "025", Name = "Conduits", Type = "Electrical", UnitOfMeasurement = UnitOfMeasurement.Feet },
            new Material {  Code = "026", Name = "Circuit Breakers", Type = "Electrical", UnitOfMeasurement = UnitOfMeasurement.Pieces },
            new Material {  Code = "027", Name = "Switches and Outlets", Type = "Electrical", UnitOfMeasurement = UnitOfMeasurement.Pieces },
            
            // HVAC Materials
            new Material { Code = "028", Name = "Ductwork", Type = "HVAC", UnitOfMeasurement = UnitOfMeasurement.SquareFeet },
            new Material { Code = "029", Name = "Insulation", Type = "HVAC", UnitOfMeasurement = UnitOfMeasurement.SquareMeters },
            new Material { Code = "030", Name = "Thermostats", Type = "HVAC", UnitOfMeasurement = UnitOfMeasurement.Pieces },
            
            // Miscellaneous Materials
            new Material { Code = "031", Name = "Glass", Type = "Miscellaneous", UnitOfMeasurement = UnitOfMeasurement.SquareMeters },
            new Material { Code = "032", Name = "Aluminum", Type = "Miscellaneous", UnitOfMeasurement = UnitOfMeasurement.SquareFeet },
            new Material { Code = "033", Name = "Plastics (for various fixtures and fittings)", Type = "Miscellaneous", UnitOfMeasurement = UnitOfMeasurement.Pieces },
            new Material { Code = "034", Name = "Composite Materials", Type = "Miscellaneous", UnitOfMeasurement = UnitOfMeasurement.Pieces },
            new Material { Code = "035", Name = "Fasteners (Nails, Screws, Bolts)", Type = "Miscellaneous", UnitOfMeasurement = UnitOfMeasurement.Pieces },
            new Material { Code = "036", Name = "Sealants and Adhesives", Type = "Miscellaneous", UnitOfMeasurement = UnitOfMeasurement.Pieces },
            
            // Green and Sustainable Materials
            new Material { Code = "037", Name = "Bamboo", Type = "Green/Sustainable", UnitOfMeasurement = UnitOfMeasurement.Pieces },
            new Material { Code = "038", Name = "Recycled Steel", Type = "Green/Sustainable", UnitOfMeasurement = UnitOfMeasurement.Tons },
            new Material { Code = "039", Name = "Recycled Plastic", Type = "Green/Sustainable", UnitOfMeasurement = UnitOfMeasurement.Tons },
            new Material { Code = "040", Name = "Rammed Earth", Type = "Green/Sustainable", UnitOfMeasurement = UnitOfMeasurement.CubicMeters },
            new Material { Code = "041", Name = "Straw Bale", Type = "Green/Sustainable", UnitOfMeasurement = UnitOfMeasurement.Pieces },
            new Material { Code = "042", Name = "Hempcrete", Type = "Green/Sustainable", UnitOfMeasurement = UnitOfMeasurement.CubicMeters },
        };
        }
    }
}
