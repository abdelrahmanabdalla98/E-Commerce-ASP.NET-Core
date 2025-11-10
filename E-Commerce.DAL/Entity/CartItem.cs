using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.DAL.Entity
{
    public class CartItem
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity), Required]

        public int Id { get; set; }
        public int Quantity { get; set; }
        public string? Size { get; set; }
        public string? Color { get; set; }
        public double Subtotal { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int CartId { get; set; }
        public Cart Cart { get; set; }
    }
}
