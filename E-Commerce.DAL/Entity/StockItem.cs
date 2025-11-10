using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace E_Commerce.DAL.Entity
{
    public class StockItem
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity), Required]
        public int Id { get; set; }
        public int Quantity { get; set; }
        public int Available { get; set; }
        public DateOnly ExpireDate { get; set; }
        public string? Color { get; set; }
        public string? Size { get; set; }
        public int productId { get; set; }
        public Product product { get; set; }
        public int stockId { get; set; }
        public Stock stock { get; set; }
    }
}
