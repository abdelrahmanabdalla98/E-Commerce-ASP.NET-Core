using E_Commerce.DAL.Entity_Extension;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.DAL.Entity
{
    public class PaymentCard
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity), Required]
        public int Id { get; set; }
        public string Type { get; set; }
        [ProtectedPersonalData]
        public string CardNum { get; set; }
        public DateOnly ExpireDate { get; set; }
        public string SecurityCode { get; set; }
        public string NameOnCard { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;

        public ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();


    }
}
