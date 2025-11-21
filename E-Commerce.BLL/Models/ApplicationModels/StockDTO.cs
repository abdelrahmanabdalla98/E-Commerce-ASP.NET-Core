using E_Commerce.DAL.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.BLL.Models.ApplicationModels
{
    public class StockDTO
    {
        public int Id { get; set; }
        public string InventoryName { get; set; } = "";
        public string PhoneNumber { get; set; } = "";

        public bool IsDeleted { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public List<StockItemDTO> Items { get; set; } = new();

    }
}
