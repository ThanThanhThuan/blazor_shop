using System.ComponentModel.DataAnnotations.Schema;

namespace AccountingApp.Shared.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public string ImageUrl { get; set; } = "https://via.placeholder.com/150";

        public int Stock { get; set; }
    }
}