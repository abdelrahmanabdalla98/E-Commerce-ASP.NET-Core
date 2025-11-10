using E_Commerce.DAL.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.BLL.Models.ApplicationModels
{
    public class CartItemDTO
    {
        public int Id { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public string Size { get; set; }
        [Required]
        public string Color { get; set; }
        public double Subtotal { get; set; }
        public int productId { get; set; }
        public Product product { get; set; }
        public int? CartId { get; set; }
        public Cart? Cart { get; set; }
    }
}
