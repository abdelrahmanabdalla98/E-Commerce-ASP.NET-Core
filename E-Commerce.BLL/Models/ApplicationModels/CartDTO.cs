using E_Commerce.DAL.Entity;
using E_Commerce.DAL.Entity_Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.BLL.Models.ApplicationModels
{
    public class CartDTO
    {
        public int Id { get; set; }
        public double? Subtotal { get; set; } = 0;
        public double? ShippingTax { get; set; }
        public double? VAT { get; set; }
        public double? Total { get; set; }= 0;
        public bool? IsDeleted { get; set; }
        public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}
