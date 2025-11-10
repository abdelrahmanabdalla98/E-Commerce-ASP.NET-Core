using E_Commerce.DAL.Entity;
using E_Commerce.DAL.Entity_Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.BLL.Models.ApplicationModels
{
    public class OrderDTO
    {
        public int Id { get; set; }
        public DateTime DeliveryDate { get; set; }
        public DateTime CreatedOn { get; set; }
        public int ReturnStatusId { get; set; }
        public bool RefundIssued { get; set; }
        public int PayTypeId { get; set; }
        public bool IsDeleted { get; set; }
        public ICollection<PaymentCard> Cards { get; set; } = new List<PaymentCard>();
        public ICollection<Address> Addresses { get; set; } = new List<Address>();
        public string applicationUserId { get; set; }
        public ApplicationUser applicationUser { get; set; }
        public int cartId { get; set; }
        public Cart cart { get; set; }
    }
}
