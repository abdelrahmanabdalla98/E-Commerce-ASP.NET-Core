using E_Commerce.DAL.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.BLL.Models.ApplicationModels
{
    public class InvoiceDTO
    {
        public int Id { get; set; }
        [Required]
        public DateTime IssuedTime { get; set; }
        public int orderId { get; set; }
        public Order order { get; set; }
        public bool IsDeleted { get; set; }
    }
}
