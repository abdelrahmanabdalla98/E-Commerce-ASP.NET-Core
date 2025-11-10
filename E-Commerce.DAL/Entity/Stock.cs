using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace E_Commerce.DAL.Entity
{
    public class Stock
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity), Required]
        public int Id { get; set; }
        public string InventoryName { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public DateTime UpdatedOn { get; set; }
        public bool IsDeleted { get; set; } = false;

        public List<StockItem> Items { get; set; } = new();


    }
}
