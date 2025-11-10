using E_Commerce.DAL.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.BLL.Models.ApplicationModels
{
    public class StockItemDTO
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="Please enter quantity")]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter a valid number.")]
        public int? Quantity { get; set; }
        [Required(ErrorMessage = "Please enter available")]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter a valid number.")]
        public int? Available { get; set; }
        [Required(ErrorMessage ="Please select expire date")]
        public DateOnly? ExpireDate { get; set; }
        public string? Color { get; set; }
        public string? Size { get; set; }
        public int productId { get; set; }

        [Required(ErrorMessage = "Please select stock place")]
        public int? stockId { get; set; }
    }
}
