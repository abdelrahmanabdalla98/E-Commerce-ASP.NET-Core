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
        public DateOnly? ExpireDate { get; set; } = null;
        [Required(ErrorMessage = "Please write no if there is no color")]
        public string Color { get; set; }
        [Required(ErrorMessage = "Please write no if there is no size")]
        public string Size { get; set; }
        [Required(ErrorMessage = "Please select product")]
        public int ProductId { get; set; }
        public string? ProductName { get; set; }


        [Required(ErrorMessage = "Please select stock place")]
        public int StockId { get; set; }
        public StockDTO? Stock { get; set; }
        public bool IsDeleted { get; set; } = false;
        public string CreatedBy { get; set; } = "";
        public DateTime CreatedOn { get; set; } = DateTime.Now.Date;

    }
}
