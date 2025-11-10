using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce.DAL.Entity_Extension;

namespace E_Commerce.DAL.Entity
{
    public class Cart
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity), Required]
        public int Id { get; set; }
        public double Subtotal { get; set; }
        public double ShippingTax { get; set; }
        public double VAT { get; set; }
        public double Total { get; set; }
        public bool IsDeleted { get; set; } = false;
        public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
        public string applicationUserId { get; set; }
        public ApplicationUser applicationUser { get; set; }
    }
}
