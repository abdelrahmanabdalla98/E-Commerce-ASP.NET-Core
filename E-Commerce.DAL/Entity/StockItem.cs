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
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int StockId { get; set; }
        public Stock Stock { get; set; }
        public bool IsDeleted { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
