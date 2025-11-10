using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.DAL.Entity
{
    public class Invoice
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity), Required]
        public int Id { get; set; }
        public DateTime IssuedTime { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public bool IsDeleted { get; set; }

    }
}
