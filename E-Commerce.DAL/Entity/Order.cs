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
    public class Order
    {
        // go back to check for intances errors 
        public Order()
        {
            CreatedOn = DateTime.Now;
        }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity), Required]
        public int Id { get; set; }
        public DateTime DeliveryDate { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public int ReturnStatusId { get; set; }
        public bool RefundIssued { get; set; }
        public int PayTypeId { get; set; }
        public bool IsDeleted { get; set; }
        public ICollection<PaymentCard> Cards { get; set; } = new List<PaymentCard>();
        public ICollection<Address> Addresses { get; set; } = new List<Address>();
        public int cartId { get; set; }
        public Cart cart { get; set; }



    }
}
