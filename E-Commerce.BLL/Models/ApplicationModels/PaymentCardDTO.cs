using E_Commerce.DAL.Entity;
using E_Commerce.DAL.Entity_Extension;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.BLL.Models.ApplicationModels
{
    public class PaymentCardDTO
    {
        public int Id { get; set; }
        [Required]
        public string Type { get; set; }
        [Required, RegularExpression(@"^\d+$", ErrorMessage = "Only numbers are allowed.")]
        public string CardNum { get; set; }
        public DateOnly ExpireDate { get; set; }
        [Required, RegularExpression(@"^\d+$", ErrorMessage = "Only numbers are allowed.")]
        public string SecurityCode { get; set; }
        public string NameOnCard { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedOn { get; set; }
        public ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
