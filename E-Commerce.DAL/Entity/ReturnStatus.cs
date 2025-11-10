using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.DAL.Entity
{
    public class ReturnStatus
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity), Required]

        public int Id { get; set; }
        public string? Status { get; set; }

    }
}
